using UnityEngine;
using UnityEngine.UIElements;

public class Navigation : MonoBehaviour
{
    [System.Serializable]
    public class ProductData
    {
        public GameObject productObject;
        public string productName;
        public string price;
        public string color;
        public string material;
    }

    [Header("Products")]
    public ProductData[] products = new ProductData[5]; // 5 products

    // UI Labels
    private Label nameLabel;
    private Label priceLabel;
    private Label colorLabel;
    private Label materialLabel;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Buttons - assuming buttons are named btn-product-1, btn-product-2, ..., btn-product-5
        for (int i = 0; i < products.Length; i++)
        {
            int index = i; // local copy for closure
            Button btn = root.Q<Button>($"Button{i + 1}");
            if (btn != null)
                btn.clicked += () => ShowProduct(products[index]);
        }

        // Labels
        nameLabel = root.Q<Label>("name");
        priceLabel = root.Q<Label>("price");
        colorLabel = root.Q<Label>("color");
        materialLabel = root.Q<Label>("material");

        // Check if any label is missing
        if (nameLabel == null || priceLabel == null || colorLabel == null || materialLabel == null)
        {
            Debug.LogError("UI labels not found. Check UXML names.");
            return;
        }
    }

    private void ShowProduct(ProductData product)
    {
        // Deactivate all products
        foreach (var p in products)
        {
            if (p.productObject != null)
                p.productObject.SetActive(false);
        }

        // Activate selected product
        if (product.productObject != null)
            product.productObject.SetActive(true);

        // Update UI
        nameLabel.text = product.productName;
        priceLabel.text = product.price;
        colorLabel.text = product.color;
        materialLabel.text = product.material;
    }
}
