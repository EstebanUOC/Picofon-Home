using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Google;
using UnityEngine.SceneManagement;
using System;


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
    public TextMeshProUGUI TitleText;
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
            Debug.LogWarning("⚠️ Firebase not ready yet. Please wait...");
            return;
        }

        if (isSigningIn) 
        {
            Debug.LogWarning("⚠️ Already signing in. Please wait.");
            return;
        }

        isSigningIn = true;
        Debug.Log("🚀 Starting Google Sign-In...");

        // Make sure there’s no existing session
        GoogleSignIn.DefaultInstance.Disconnect();
        GoogleSignIn.DefaultInstance.SignOut();

        GoogleSignIn.DefaultInstance.SignIn().ContinueWithOnMainThread(async googleTask =>
        {
            isSigningIn = false;

            if (googleTask.IsCanceled)
            {
                Debug.LogWarning("❌ Google Sign-In canceled by user.");
                return;
            }

            if (googleTask.IsFaulted)
            {
                Debug.LogError($"❌ Google Sign-In failed: {googleTask.Exception?.Message}");
                return;
            }

            Debug.Log("✅ Google Sign-In success. Exchanging with Firebase...");

            // Get the Google Sign-In IdToken (OAuth token)
            string googleIdToken = googleTask.Result.IdToken;

            // Use that token to sign in with Firebase
            Credential credential = GoogleAuthProvider.GetCredential(googleIdToken, null);

            try
            {
                FirebaseUser newUser = await auth.SignInWithCredentialAsync(credential);
                user = newUser;
                Debug.Log($"✅ Firebase Auth success. Logged in as: {user.DisplayName}");

                // ✅ Get Firebase's ID token (NOT the Google OAuth token)
                string firebaseIdToken = await user.TokenAsync(true);

                Debug.Log($"📤 Sending Firebase ID Token to backend for {user.Email}");

                // Send the ID token to your backend
                StartCoroutine(new LoginAPI().SendFirebaseToken(firebaseIdToken, success =>
                {
                    if (success)
                        Debug.Log("✅ Backend login success.");
                    else
                        Debug.LogError("❌ Backend login failed.");
                }));

                // Optional: trigger UI updates
                OnLoginSuccess();
            }
            catch (Exception ex)
            {
                Debug.LogError($"🔥 Firebase Auth exception: {ex.Message}");
            }
        });
    }


    private void OnLoginSuccess()
    {
        TitleText.enabled = false;
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
