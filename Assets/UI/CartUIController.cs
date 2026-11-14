using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Globalization;

public class CartUIController : MonoBehaviour
{
    private UIDocument uiDocument;
    private Button goBackButton;
    private Button removeButton; 
    private Button confirmButton;
    private Button increaseQtyButton;
    private Button decreaseQtyButton;

    private Label quantityLabel;
    private Label productNameLabel;
    private Label productPriceLabel;
    private Label subtotalLabel;
    private Label shippingLabel;
    private Label totalLabel;

    private int quantity = 1;
    private float shippingCost = 8f; // always 8 dt

    private void OnEnable()
    {
        uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null) return;

        var root = uiDocument.rootVisualElement;

        // Find buttons and labels
        confirmButton = root.Q<Button>("ConfirmButton");
        increaseQtyButton = root.Q<Button>("IncreaseQty");
        decreaseQtyButton = root.Q<Button>("DecreaseQty");
        removeButton = root.Q<Button>("RemoveButton");
        goBackButton = root.Q<Button>("GoBackButton"); 
        quantityLabel = root.Q<Label>("QuantityValue");
        productNameLabel = root.Q<Label>("ProductName");
        productPriceLabel = root.Q<Label>("ProductPrice");
        subtotalLabel = root.Q<Label>("SubtotalText");
        shippingLabel = root.Q<Label>("ShippingText");
        totalLabel = root.Q<Label>("TotalText");

        // Add listeners
        if (confirmButton != null) confirmButton.clicked += OnConfirmOrderClicked;
        if (increaseQtyButton != null) increaseQtyButton.clicked += OnIncreaseQtyClicked;
        if (decreaseQtyButton != null) decreaseQtyButton.clicked += OnDecreaseQtyClicked;
        if (removeButton != null) removeButton.clicked += OnRemoveButtonClicked;
         if (goBackButton != null) goBackButton.clicked += OnGoBackButtonClicked;
        InitializeCart();
    }

    private void OnDisable()
    {
        if (confirmButton != null) confirmButton.clicked -= OnConfirmOrderClicked;
        if (increaseQtyButton != null) increaseQtyButton.clicked -= OnIncreaseQtyClicked;
        if (decreaseQtyButton != null) decreaseQtyButton.clicked -= OnDecreaseQtyClicked;
        if (removeButton != null) removeButton.clicked -= OnRemoveButtonClicked;
        if (goBackButton != null) goBackButton.clicked -= OnGoBackButtonClicked;
    }   
private void OnGoBackButtonClicked()
{
    SceneManager.LoadScene("product");
}
    private void InitializeCart()
    {
        // Set product name
        productNameLabel.text = "  Product Name: " + CartData.ProductName;

        // Extract numeric part of price
        string numericPrice = "0";
        if (!string.IsNullOrEmpty(CartData.ProductPrice))
        {
            numericPrice = "";
            foreach (char c in CartData.ProductPrice)
            {
                if ((c >= '0' && c <= '9') || c == '.')
                    numericPrice += c;
            }
        }

        // Update product price label
        productPriceLabel.text = "  Price: " + numericPrice + " dt";

        // Reset quantity
        quantity = 1;
        quantityLabel.text = quantity.ToString();

        // Shipping
        shippingLabel.text = "  Shipping: " + shippingCost.ToString("F2") + " dt";

        UpdateTotals(numericPrice);
        UpdateConfirmButtonState(numericPrice); 
    }

    private void UpdateTotals(string numericPrice = null)
    {
        float price = 0f;

        // If numericPrice is provided, parse it
        if (!string.IsNullOrEmpty(numericPrice))
        {
            float.TryParse(numericPrice, NumberStyles.Any, CultureInfo.InvariantCulture, out price);
        }
        else if (!string.IsNullOrEmpty(CartData.ProductPrice))
        {
            string np = "";
            foreach (char c in CartData.ProductPrice)
            {
                if ((c >= '0' && c <= '9') || c == '.')
                    np += c;
            }
            float.TryParse(np, NumberStyles.Any, CultureInfo.InvariantCulture, out price);
        }

        float subtotal = price * quantity;
        subtotalLabel.text = "  Subtotal: " + subtotal.ToString("F2") + " dt";

        float total = subtotal + shippingCost;
        totalLabel.text = "  Total: " + total.ToString("F2") + " dt";
        UpdateConfirmButtonState(price.ToString(CultureInfo.InvariantCulture));
    }

    private void OnConfirmOrderClicked()
    {
        SceneManager.LoadScene("Payment");
    }

    private void OnIncreaseQtyClicked()
    {
        quantity++;
        quantityLabel.text = quantity.ToString();
        UpdateTotals();
    }

    private void OnDecreaseQtyClicked()
    {
        quantity = Mathf.Max(1, quantity - 1); // minimum 1
        quantityLabel.text = quantity.ToString();
        UpdateTotals();
    }

    private void OnRemoveButtonClicked()
    {
        // Reset CartData
        CartData.ProductName = "";
        CartData.ProductPrice = "0";

        // Clear UI
        productNameLabel.text = "  Product Name: ";
        productPriceLabel.text = "  Price: 0 dt";
        quantity = 1;
        quantityLabel.text = quantity.ToString();
        subtotalLabel.text = "  Subtotal: 0 dt";
        shippingLabel.text = "  Shipping: 0 dt";
        totalLabel.text = "  Total: 0 dt";

        UpdateConfirmButtonState("0"); // <-- désactive ConfirmOrder
    }

    private void UpdateConfirmButtonState(string numericPrice)
    {
        float price = 0f;
        float.TryParse(numericPrice, NumberStyles.Any, CultureInfo.InvariantCulture, out price);

        if (confirmButton != null)
        {
            confirmButton.SetEnabled(price > 0); // activé seulement si prix > 0
        }
    }
}