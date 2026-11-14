using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement; // 🔹 Needed for scene switching
using Firebase;
using Firebase.Auth;
using System.Threading.Tasks;

public class FirebaseRegisterUI : MonoBehaviour
{
    [Header("UI Toolkit")]
    private TextField emailField;
    private TextField passwordField;
    private Button registerButton;
    private Button loginButton;          // 🔹 NEW
    private Label messageLabel;

    private FirebaseAuth auth;

    private async void Awake()
    {
        var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
        if (dependencyStatus == DependencyStatus.Available)
        {
            auth = FirebaseAuth.DefaultInstance;
            Debug.Log("Firebase initialized successfully");
        }
        else
        {
            Debug.LogError($"Firebase dependency error: {dependencyStatus}");
        }
    }

    private void OnEnable()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();
        VisualElement root = uiDocument.rootVisualElement;

        emailField = root.Q<TextField>("RegisterEmailField");
        passwordField = root.Q<TextField>("RegisterPasswordField");
        registerButton = root.Q<Button>("RegisterButton");
        loginButton = root.Q<Button>("LoginButton");   // 🔹 NEW
        messageLabel = root.Q<Label>("RegisterMessageLabel");

        // 🔹 Ensure label is visible
        messageLabel.style.display = DisplayStyle.Flex;
        messageLabel.style.visibility = Visibility.Visible;
        messageLabel.style.opacity = 1f;
        messageLabel.style.height = StyleKeyword.Auto;

        if (registerButton != null)
            registerButton.clicked += OnRegisterClicked;

        if (loginButton != null)
            loginButton.clicked += OnLoginClicked;       // 🔹 NEW
    }

    private void OnDisable()
    {
        if (registerButton != null)
            registerButton.clicked -= OnRegisterClicked;

        if (loginButton != null)
            loginButton.clicked -= OnLoginClicked;       // 🔹 NEW
    }

    private void OnLoginClicked()
    {
        // 🔹 Change scene to "Authentification"
        SceneManager.LoadScene("Authentification");
    }

    private async void OnRegisterClicked()
    {
        messageLabel.text = "";
        messageLabel.style.color = Color.white;

        string email = emailField?.value.Trim();
        string password = passwordField?.value;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ShowError("Email and password cannot be empty.");
            return;
        }

        if (password.Length < 6)
        {
            ShowError("Password must be at least 6 characters.");
            return;
        }

        try
        {
            await auth.CreateUserWithEmailAndPasswordAsync(email, password);
            ShowSuccess("Account created successfully!");
        }
        catch (FirebaseException ex)
        {
            HandleFirebaseError(ex);
        }
    }

    private void HandleFirebaseError(FirebaseException ex)
    {
        AuthError errorCode = (AuthError)ex.ErrorCode;
        switch (errorCode)
        {
            case AuthError.InvalidEmail:
                ShowError("Invalid email address.");
                break;
            case AuthError.EmailAlreadyInUse:
                ShowError("This email is already registered.");
                break;
            case AuthError.WeakPassword:
                ShowError("Password is too weak.");
                break;
            default:
                ShowError("Registration failed: " + ex.Message);
                break;
        }
    }

    private void ShowError(string message)
    {
        messageLabel.text = message;
        messageLabel.style.color = Color.red;
    }

    private void ShowSuccess(string message)
    {
        messageLabel.text = message;
        messageLabel.style.color = Color.green;
    }
}
