using UnityEngine;
using UnityEngine.UIElements;
using Firebase;
using Firebase.Auth;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class ForgetPasswordUI : MonoBehaviour
{
    private FirebaseAuth auth;
    private bool firebaseReady = false;

    private TextField emailField;
    private Button resetButton;
    private Label messageLabel;

    async void Awake()
    {
        await InitializeFirebase();
    }

    private async Task InitializeFirebase()
    {
        var result = await FirebaseApp.CheckAndFixDependenciesAsync();

        if (result == DependencyStatus.Available)
        {
            auth = FirebaseAuth.DefaultInstance;
            firebaseReady = true;
            Debug.Log("Firebase Ready for password reset");
        }
        else
        {
            Debug.LogError("Firebase init failed: " + result);
        }
    }

    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        emailField = root.Q<TextField>("ForgotEmailField");
        resetButton = root.Q<Button>("ResetPasswordButton");
        messageLabel = root.Q<Label>("MessageLabel");

        resetButton.clicked += OnResetClicked;
    }

    private async void OnResetClicked()
    {
        if (!firebaseReady)
        {
            ShowMessage("Firebase not initialized!");
            return;
        }

        string email = emailField.text.Trim();

        if (string.IsNullOrEmpty(email))
        {
            ShowMessage("Please enter your email.");
            return;
        }

        ShowMessage("Sending reset link...");

        try
        {
            await auth.SendPasswordResetEmailAsync(email);
            ShowMessage("If this email is registered, a reset link has been sent.");

            await Task.Delay(1500);

            SceneManager.LoadScene("authentication"); // <-- your scene name
        }
        catch (FirebaseException ex)
        {
            string error = GetFirebaseError(ex);
            ShowMessage("Error: " + error);
        }
    }

    private string GetFirebaseError(FirebaseException ex)
    {
        AuthError error = (AuthError)ex.ErrorCode;

        switch (error)
        {
            case AuthError.InvalidEmail:
                return "Invalid email format.";
            case AuthError.UserNotFound:
                return "This email is not registered.";
            default:
                return error.ToString();
        }
    }

    private void ShowMessage(string msg)
    {
        messageLabel.text = msg;
        Debug.Log(msg);
    }
}