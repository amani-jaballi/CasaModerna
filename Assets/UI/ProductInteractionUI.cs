using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Product
{
    public Transform productTransform;
    public Color tempColor;
    public Material tempMaterial;

    [HideInInspector] public Renderer[] renderers;
    [HideInInspector] public Color[] originalColors;
    [HideInInspector] public Material[] originalMaterials;
}

public class ProductInteractionUI : MonoBehaviour
{
    [Header("Products")]
    [SerializeField] private List<Product> products = new List<Product>();

    [Header("Zoom Settings")]
    [SerializeField] private float zoomSpeed = 2f;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 200f;
    [SerializeField] private float minVerticalAngle = -60f;
    [SerializeField] private float maxVerticalAngle = 60f;

    [Header("Temporary Change Duration")]
    [SerializeField] private float changeDuration = 15f;

    private UIDocument uiDocument;
    private Product currentProduct;

    private bool zoomMode;
    private bool rotateMode;

    private Coroutine activeCoroutine;

    // Rotation values
    private float targetX;
    private float targetY;

    private void OnEnable()
    {
        uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;

        root.Q<Button>("ZoomButton").clicked += () => SetMode(true, false);
        root.Q<Button>("RotateButton").clicked += () => SetMode(false, true);
        root.Q<Button>("ColorButton").clicked += ChangeColorTemporarily;
        root.Q<Button>("MaterialButton").clicked += ChangeMaterialTemporarily;

        foreach (var product in products)
        {
            if (product.productTransform == null) continue;

            product.renderers = product.productTransform.GetComponentsInChildren<Renderer>(true);

            int count = product.renderers.Length;
            product.originalColors = new Color[count];
            product.originalMaterials = new Material[count];

            for (int i = 0; i < count; i++)
            {
                product.originalColors[i] = product.renderers[i].material.color;
                product.originalMaterials[i] = product.renderers[i].material;
            }
        }

        if (products.Count > 0)
            currentProduct = products[0];
    }

    private void Update()
    {
        if (currentProduct == null) return;

        // -------- ZOOM (UNCHANGED) --------
        if (zoomMode)
        {
            float scroll = Input.mouseScrollDelta.y;
            Camera.main.transform.position += Camera.main.transform.forward * scroll * zoomSpeed;
        }

        // -------- ROTATION (UPDATED X & Y) --------
        if (rotateMode && Input.GetMouseButton(0))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            targetY += mouseX * rotationSpeed * Time.deltaTime;
            targetX -= mouseY * rotationSpeed * Time.deltaTime;

            targetX = Mathf.Clamp(targetX, minVerticalAngle, maxVerticalAngle);

            currentProduct.productTransform.rotation = Quaternion.Euler(targetX, targetY, 0f);
        }
    }

    private void SetMode(bool zoom, bool rotate)
    {
        zoomMode = zoom;
        rotateMode = rotate;
    }

    // -------- TEMP COLOR --------
    private void ChangeColorTemporarily()
    {
        if (currentProduct == null) return;

        StopActiveCoroutine();
        activeCoroutine = StartCoroutine(TempColor(currentProduct));
    }

    private IEnumerator TempColor(Product product)
    {
        foreach (var r in product.renderers)
            r.material.color = product.tempColor;

        yield return new WaitForSeconds(changeDuration);

        for (int i = 0; i < product.renderers.Length; i++)
            product.renderers[i].material.color = product.originalColors[i];
    }

    // -------- TEMP MATERIAL --------
    private void ChangeMaterialTemporarily()
    {
        if (currentProduct == null || currentProduct.tempMaterial == null) return;

        StopActiveCoroutine();
        activeCoroutine = StartCoroutine(TempMaterial(currentProduct));
    }

    private IEnumerator TempMaterial(Product product)
    {
        foreach (var r in product.renderers)
            r.material = product.tempMaterial;

        yield return new WaitForSeconds(changeDuration);

        for (int i = 0; i < product.renderers.Length; i++)
            product.renderers[i].material = product.originalMaterials[i];
    }

    private void StopActiveCoroutine()
    {
        if (activeCoroutine != null)
            StopCoroutine(activeCoroutine);
    }

    // -------- SELECT PRODUCT --------
    public void SetCurrentProduct(int index)
    {
        if (index < 0 || index >= products.Count) return;
        currentProduct = products[index];

        targetX = 0;
        targetY = 0;
    }
}