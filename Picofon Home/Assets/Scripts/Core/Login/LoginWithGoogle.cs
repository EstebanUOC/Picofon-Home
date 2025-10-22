using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Google;
using UnityEngine.SceneManagement;

public class LoginWithGoogle : MonoBehaviour
{
    [Header("Firebase")]
    public string GoogleAPI = "1068789468608-otkna5ad1hgh9qqn0vt67630k67ri69r.apps.googleusercontent.com";
    private FirebaseAuth auth;
    private FirebaseUser user;
    private bool isFirebaseReady = false;

    [Header("Panels")]
    public GameObject LoginPanel;
    public GameObject ChildDataPanel;

    [Header("Buttons")]
    public Button SignInButton;
    public Button ContinueButton;

    [Header("User Info UI")]
    public TextMeshProUGUI WelcomeMessage;
    public TextMeshProUGUI EmailText;

    [Header("Child Data UI")]
    public TMP_InputField ChildNameField;
    public TMP_Dropdown AgeDropdown;
    public Toggle No_Toggle;
    public Toggle TEL_Toggle;
    public Toggle TEA_Toggle;
    public Toggle TDAH_Toggle;
    public Toggle Other_Toggle;

    private bool isSigningIn = false;

    private void Start()
    {
        Debug.Log("Msg::::: Start()");

        InitFirebase();

        // Configure Google Sign-In
        GoogleSignIn.Configuration = new GoogleSignInConfiguration
        {
            RequestIdToken = true,
            WebClientId = GoogleAPI,
            RequestEmail = true
            
        };

        if (SignInButton != null)
        {
            SignInButton.onClick.RemoveAllListeners();
            SignInButton.onClick.AddListener(Login);
        }

        if (ContinueButton != null)
        {
            ContinueButton.onClick.RemoveAllListeners();
            ContinueButton.onClick.AddListener(OnContinue);
        }

        LoginPanel.SetActive(true);
        ChildDataPanel.SetActive(false);
    }

    private void InitFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var status = task.Result;
            if (status == Firebase.DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
                isFirebaseReady = true;
                Debug.Log("Firebase ready for authentication.");
            }
            else
            {
                Debug.LogError("Firebase dependencies not available: " + status);
            }
        });
    }

    public void Login()
    {
        if (!isFirebaseReady)
        {
            Debug.LogWarning("Firebase not ready yet. Please wait...");
            return;
        }

        if (isSigningIn) return;
        isSigningIn = true;

        GoogleSignIn.DefaultInstance.Disconnect();
        GoogleSignIn.DefaultInstance.SignOut();

        Debug.Log("Msg::::: Starting Google SignIn flow...");

        GoogleSignIn.DefaultInstance.SignIn().ContinueWithOnMainThread(task =>
        {
            isSigningIn = false;
            if (task.IsCanceled)
            {
                Debug.LogWarning("Msg::::: Google Sign-In canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("Msg::::: Google Sign-In error: " + task.Exception);
                return;
            }

            Debug.Log("Msg::::: Google Sign-In success. Exchanging token with Firebase...");
            Credential credential = GoogleAuthProvider.GetCredential(task.Result.IdToken, null);

            auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(authTask =>
            {
                if (authTask.IsCanceled || authTask.IsFaulted)
                {
                    Debug.LogError("Msg::::: Firebase Auth error: " + authTask.Exception);
                    return;
                }

                user = auth.CurrentUser;
                Debug.Log("Msg::::: Firebase Auth success. Logged in as: " + user.DisplayName);
                OnLoginSuccess();
            });
        });
    }

    private void OnLoginSuccess()
    {
        WelcomeMessage.text = $"{user.DisplayName}, gràcies per registrar-te";
        EmailText.text = user.Email;

        LoginPanel.SetActive(false);
        ChildDataPanel.SetActive(true);
    }

    private void OnContinue()
    {
        Debug.Log("Msg::::: Continue button clicked — skipping Firestore.");
        SceneManager.LoadScene("MapPath"); // directly open scene
    }
}
