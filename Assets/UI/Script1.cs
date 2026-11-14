using UnityEngine;
using UnityEngine.UIElements;

public class ProductSwitcher : MonoBehaviour
{
    [Header("Products in Scene")]
    public GameObject[] products; // Size = 5

    private Button[] buttons;

    void OnEnable()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();
        VisualElement root = uiDocument.rootVisualElement;

        buttons = new Button[5];
        buttons[0] = root.Q<Button>("Button1");
        buttons[1] = root.Q<Button>("Button2");
        buttons[2] = root.Q<Button>("Button3");
        buttons[3] = root.Q<Button>("Button4");
        buttons[4] = root.Q<Button>("Button5");

        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i; // important
            buttons[i].clicked += () => ActivateProduct(index);
        }
    }

    void ActivateProduct(int activeIndex)
    {
        for (int i = 0; i < products.Length; i++)
        {
            products[i].SetActive(i == activeIndex);
        }
    }
}
