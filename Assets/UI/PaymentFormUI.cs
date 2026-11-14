using UnityEngine;
using UnityEngine.UIElements;
using Firebase;
using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections.Generic;

public class PaymentFormUI : MonoBehaviour
{
    private UIDocument uiDocument;
    private FirebaseFirestore db;

    // UI Fields
    private TextField emailField;
    private TextField firstNameField;
    private TextField addressField;
    private DropdownField countryField;
    private TextField cityField;
    private TextField postalCodeField;
    private TextField phoneField;
    private TextField cardNumberField;
    private TextField expiryField;
    private TextField cvcField;
    private Button confirmButton;
    private Label messageLabel;

    private void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                db = FirebaseFirestore.DefaultInstance;
                Debug.Log("Firebase Firestore Ready");
            }
            else
            {
                Debug.LogError("Firebase initialization failed: " + task.Result);
            }
        });
    }

    private void OnEnable()
    {
        uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null) return;

        var root = uiDocument.rootVisualElement;

        emailField = root.Q<TextField>("emailField");
        firstNameField = root.Q<TextField>("firstNameField");
        addressField = root.Q<TextField>("addressField");
        countryField = root.Q<DropdownField>("countryField");
        cityField = root.Q<TextField>("cityField");
        postalCodeField = root.Q<TextField>("postalCodeField");
        phoneField = root.Q<TextField>("phoneField");
        cardNumberField = root.Q<TextField>("cardNumberField");
        expiryField = root.Q<TextField>("expiryField");
        cvcField = root.Q<TextField>("cvcField");
        confirmButton = root.Q<Button>("confirmButton");
        messageLabel = root.Q<Label>("messageLabel");

        if (messageLabel != null)
            messageLabel.text = "";

        if (confirmButton != null)
            confirmButton.clicked += OnConfirmOrder;
    }

    private void OnConfirmOrder()
    {
        ShowMessage("", Color.clear);

        if (string.IsNullOrWhiteSpace(emailField?.value) ||
            string.IsNullOrWhiteSpace(firstNameField?.value) ||
            string.IsNullOrWhiteSpace(addressField?.value))
        {
            ShowMessage("Please fill in all required fields.", Color.red);
            return;
        }

        if (string.IsNullOrWhiteSpace(cardNumberField?.value) ||
            string.IsNullOrWhiteSpace(expiryField?.value) ||
            string.IsNullOrWhiteSpace(cvcField?.value))
        {
            ShowMessage("Please enter valid card details.", Color.red);
            return;
        }

        SaveToFirestore();
    }

    private void SaveToFirestore()
    {
        if (db == null)
        {
            ShowMessage("Database not ready.", Color.red);
            Debug.LogError("Firestore instance is null");
            return;
        }

        Dictionary<string, object> order = new Dictionary<string, object>()
        {
            { "email", emailField.value },
            { "firstName", firstNameField.value },
            { "address", addressField.value },
            { "country", countryField?.value ?? "" },
            { "city", cityField?.value ?? "" },
            { "postalCode", postalCodeField?.value ?? "" },
            { "phone", phoneField?.value ?? "" },
            { "cardNumber", cardNumberField.value },
            { "expiry", expiryField.value },
            { "cvc", cvcField.value },
            { "timestamp", System.DateTime.UtcNow }
        };

        db.Collection("orders").AddAsync(order).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                ShowMessage("Error saving order.", Color.red);
                Debug.LogError("Firestore save failed: " + task.Exception);
            }
            else
            {
                ShowMessage("Order saved successfully!", Color.green);
                Debug.Log("Order added to Firestore with ID: " + task.Result.Id);
            }
        });
    }

    private void ShowMessage(string message, Color color)
    {
        if (messageLabel == null) return;
        messageLabel.text = message;
        messageLabel.style.color = new StyleColor(color);
    }
}