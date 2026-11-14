using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Firestore;
using Firebase.Extensions;
using System.Collections.Generic;

public class ProductUIController : MonoBehaviour
{
    private UIDocument uiDocument;
    private VisualElement root;

    private Button buyButton;
    private Label nameLabel;
    private Label priceLabel;

    private FirebaseFirestore db; 
    private List<Dictionary<string, object>> products = new List<Dictionary<string, object>>();

    private int selectedProductIndex = -1; // produit actuellement sélectionné

    // Container dans l'UI pour les boutons produits
    private VisualElement productButtonContainer;

    // Prefab UXML pour créer un bouton produit
    [SerializeField] private VisualTreeAsset productButtonPrefab;

    private void OnEnable()
    {
        uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null) return;

        root = uiDocument.rootVisualElement;

        buyButton = root.Q<Button>("BuyButton");
        nameLabel = root.Q<Label>("name");
        priceLabel = root.Q<Label>("price");
        productButtonContainer = root.Q<VisualElement>("ProductButtonContainer");

        if (buyButton != null)
            buyButton.clicked += OnBuyButtonClicked;

        // Initialiser Firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                db = FirebaseFirestore.DefaultInstance;
                LoadProductsFromFirebase();
            }
            else
            {
                Debug.LogError("Firebase not ready");
            }
        });
    }

    // Charger les produits depuis Firestore
    private void LoadProductsFromFirebase()
    {
        db.Collection("produits").GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (!task.IsCompleted || task.Result == null)
            {
                Debug.LogError("Failed to load products from Firebase");
                return;
            }

            products.Clear();
            if (productButtonContainer != null)
                productButtonContainer.Clear(); // Supprime tous les anciens boutons

            QuerySnapshot snapshot = task.Result;

            int index = 0;
            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                Dictionary<string, object> data = document.ToDictionary();
                products.Add(data);

                // Créer un bouton depuis le prefab
                if (productButtonPrefab != null && productButtonContainer != null)
                {
                    VisualElement cloned = productButtonPrefab.CloneTree();
                    Button productButton = cloned.Q<Button>("ProductButton");
                    if (productButton != null)
                    {
                        productButton.text = data["name"].ToString();

                        int capturedIndex = index; // Important pour closure
                        productButton.clicked += () => {
                            selectedProductIndex = capturedIndex;
                            UpdateUIForProduct(selectedProductIndex);
                        };
                    }

                    productButtonContainer.Add(cloned);
                }

                index++;
            }

            // Sélectionner automatiquement le premier produit
            if (products.Count > 0)
            {
                selectedProductIndex = 0;
                UpdateUIForProduct(selectedProductIndex);
            }
        });
    }

    // Met à jour l'UI pour le produit sélectionné
    private void UpdateUIForProduct(int index)
    {
        if (index < 0 || index >= products.Count) return;

        string name = products[index]["name"].ToString();
        string price = products[index]["price"].ToString();

        nameLabel.text = name;
        priceLabel.text = price;
    }

    // Quand l'utilisateur clique sur "Buy"
    private void OnBuyButtonClicked()
    {
        // Copy to CartData as strings
        CartData.ProductName = nameLabel.text;
        CartData.ProductPrice = priceLabel.text;

        SceneManager.LoadScene("Cart");
    }

}