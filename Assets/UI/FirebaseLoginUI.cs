using UnityEngine;
using UnityEngine.UIElements;
using Firebase;
using Firebase.Auth;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class FirebaseLoginUI : MonoBehaviour
{
    private TextField emailField;
    private TextField passwordField;
    private Button loginButton;
    private Button forgotPasswordButton; // NEW
    private Label messageLabel;

    private FirebaseAuth auth;
    private UIDocument uiDocument;

    async void Awake()
    {
        uiDocument = GetComponent<UIDocument>();

        var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
        if (dependencyStatus != DependencyStatus.Available)
        {
            Debug.LogError("Firebase dependency error: " + dependencyStatus);
            return;
        }

        auth = FirebaseAuth.DefaultInstance;
        Debug.Log("Firebase initialized successfully");

        SetupUI();
        CheckAutoLogin();
    }

    void SetupUI()
    {
        VisualElement root = uiDocument.rootVisualElement;

        emailField = root.Q<TextField>("LoginEmailField");
        passwordField = root.Q<TextField>("LoginPasswordField");
        loginButton = root.Q<Button>("LoginButton");
        forgotPasswordButton = root.Q<Button>("ForgotPasswordButton"); // NEW
        messageLabel = root.Q<Label>("LoginMessageLabel");

        if (loginButton != null)
            loginButton.clicked += OnLoginClicked;
        else
            Debug.LogError("LoginButton not found!");

        if (forgotPasswordButton != null)
            forgotPasswordButton.clicked += OnForgotPasswordClicked; // NEW
        else
            Debug.LogError("ForgotPasswordButton not found!");
    }

    void OnForgotPasswordClicked()
    {
        // Load ResetPasswd scene
        if (Application.CanStreamedLevelBeLoaded("ResetPasswd"))
        {
            SceneManager.LoadScene("ResetPasswd");
        }
        else
        {
            Debug.LogError("Scene 'ResetPasswd' not found in Build Settings!");
        }
    }

    void CheckAutoLogin()
    {
        if (auth.CurrentUser != null)
        {
            ShowSuccess("Welcome back!");
        }
    }

    async void OnLoginClicked()
    {
        messageLabel.text = "";
        messageLabel.style.color = Color.white;

        string email = emailField.value.Trim();
        string password = passwordField.value;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ShowError("Email and password cannot be empty.");
            return;
        }

        loginButton.SetEnabled(false);

        try
        {
            await auth.SignInWithEmailAndPasswordAsync(email, password);
            ShowSuccess("Login successful!");

            if (Application.CanStreamedLevelBeLoaded("product"))
                SceneManager.LoadScene("product");
            else
                Debug.LogError("Scene 'product' not in Build Settings!");
        }
        catch (FirebaseException ex)
        {
            HandleFirebaseError(ex);
        }
        finally
        {
            loginButton.SetEnabled(true);
        }
    }

    void HandleFirebaseError(FirebaseException ex)
    {
        AuthError errorCode = (AuthError)ex.ErrorCode;

        switch (errorCode)
        {
            case AuthError.InvalidEmail:
                ShowError("Invalid email address.");
                break;
            case AuthError.UserNotFound:
                ShowError("User not found.");
                break;
            case AuthError.WrongPassword:
                ShowError("Incorrect password.");
                break;
            default:
                ShowError("Login failed.");
                Debug.LogError(ex);
                break;
        }
    }

    void ShowError(string message)
    {
        messageLabel.text = message;
        messageLabel.style.color = Color.red;
    }

    void ShowSuccess(string message)
    {
        messageLabel.text = message;
        messageLabel.style.color = Color.green;
    }
}