using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using Google;
using UnityEngine.SceneManagement;

public class LoginWithGoogle : MonoBehaviour
{
    [Header("Testing")]
    public bool useFakeLogin = false;
    private bool isFakeUser = false;

    [Header("Firebase")]
    public string GoogleAPI = "1068789468608-otkna5ad1hgh9qqn0vt67630k67ri69r.apps.googleusercontent.com";
    private FirebaseAuth auth;
    private FirebaseUser user;
    private FirebaseFirestore firestore;

    [Header("Panels")]
    public GameObject LoginPanel;
    public GameObject ChildDataPanel;

    [Header("Buttons")]
    public Button SignInButton;   // ✅ assign in Inspector
    public Button ContinueButton;

    [Header("Debug / Testing Only")]
    public Button DebugLoginButton;

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
    public TMP_InputField OtherTextField;



    private bool isGoogleSignInInitialized = false;

    private void Start()
    {
        Debug.Log("Msg::::: Start()");

        // ✅ Debug button (simulates login without Firebase/Google)
        if (DebugLoginButton != null)
        {
            Debug.Log("Msg::::: DebugLoginButton start");
            DebugLoginButton.onClick.RemoveAllListeners();
            DebugLoginButton.onClick.AddListener(SimulateLogin);
        }

        InitFirebase();

        // ✅ Initialize Google Sign-In once
        if (!isGoogleSignInInitialized)
        {
            GoogleSignIn.Configuration = new GoogleSignInConfiguration
            {
                RequestIdToken = true,
                WebClientId = GoogleAPI,
                RequestEmail = true
            };
            isGoogleSignInInitialized = true;
            Debug.Log("Msg::::: Google SignIn configuration initialized in Start()");
        }

        // ✅ Wire buttons safely
        if (SignInButton != null)
        {
            SignInButton.onClick.RemoveAllListeners();
            SignInButton.onClick.AddListener(Login);
            Debug.Log("Msg::::: SignIn button wired programmatically in Start()");
        }

        if (ContinueButton != null)
        {
            ContinueButton.onClick.RemoveAllListeners();
            ContinueButton.onClick.AddListener(OnContinue);
        }

        // Default UI state (safe even if already set in Inspector)
        LoginPanel.SetActive(true);
        ChildDataPanel.SetActive(false);

        // ✅ Skip Firebase login automatically when testing in the Unity Editor
        #if UNITY_EDITOR
        Debug.Log("Msg::::: UNITY_EDITOR detected — using fake login for testing");
        SimulateLogin();
        #endif

        ChildNameField.onValueChanged.AddListener(_ => UpdateContinueButtonState());
        AgeDropdown.onValueChanged.AddListener(_ => UpdateContinueButtonState());
        TDAH_Toggle.onValueChanged.AddListener(_ => UpdateContinueButtonState());
        No_Toggle.onValueChanged.AddListener(_ => UpdateContinueButtonState());
        TEL_Toggle.onValueChanged.AddListener(_ => UpdateContinueButtonState());
        TEA_Toggle.onValueChanged.AddListener(_ => UpdateContinueButtonState());
        Other_Toggle.onValueChanged.AddListener(_ => UpdateContinueButtonState());

        UpdateContinueButtonState(); // initialize state at startup
    }

    void InitFirebase()
    {
        Debug.Log("Msg::::: InitFirebase()");
        auth = FirebaseAuth.DefaultInstance;
        firestore = FirebaseFirestore.DefaultInstance;
    }

    public void Login()
    {
        Debug.Log($">>>> SignIn Button Pressed at: {System.DateTime.Now:HH:mm:ss.fff}");
        Debug.Log("Msg:::::Login() called – SignIn button pressed");

        Task<GoogleSignInUser> signIn = GoogleSignIn.DefaultInstance.SignIn();
        TaskCompletionSource<FirebaseUser> signInCompleted = new TaskCompletionSource<FirebaseUser>();

        signIn.ContinueWith(task =>
        {
            Debug.Log("Msg:::::GoogleSignIn task finished. Status: " + task.Status);

            if (task.IsCanceled)
            {
                Debug.LogWarning("Msg:::::Google Sign-In canceled");
                signInCompleted.SetCanceled();
            }
            else if (task.IsFaulted)
            {
                Debug.LogError("Msg:::::Google Sign-In error: " + task.Exception);
                signInCompleted.SetException(task.Exception);
            }
            else
            {
                Debug.Log("Msg:::::Google Sign-In success. Getting Firebase credential...");

                Credential credential = GoogleAuthProvider.GetCredential(task.Result.IdToken, null);

                auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(authTask =>
                {
                    Debug.Log("Msg:::::Firebase SignIn task finished. Status: " + authTask.Status);

                    if (authTask.IsCanceled)
                    {
                        Debug.LogWarning("Msg:::::Firebase Auth canceled");
                        signInCompleted.SetCanceled();
                    }
                    else if (authTask.IsFaulted)
                    {
                        Debug.LogError("Msg:::::Firebase Auth error: " + authTask.Exception);
                        signInCompleted.SetException(authTask.Exception);
                    }
                    else
                    {
                        Debug.Log("Msg:::::Firebase Auth success. Logged in as: " + authTask.Result.DisplayName);
                        signInCompleted.SetResult(authTask.Result);

                        OnLoginSuccess(auth.CurrentUser);
                    }
                });
            }
        });
    }

    /// <summary>
    /// Called when login is successful
    /// </summary>
    private void OnLoginSuccess(FirebaseUser loggedUser)
    {
        isFakeUser = false;
      
        if (loggedUser == null) return;

        Debug.Log("Msg::::: OnLoginSuccess() [FirebaseUser]");
        user = loggedUser;

        WelcomeMessage.text = $"{user.DisplayName}, gràcies per registrar-te";
        EmailText.text = user.Email;

        LoginPanel.SetActive(false);
        ChildDataPanel.SetActive(true);
    }

    // Overload for fake user (only used in debug mode)
    private void OnLoginSuccess(FakeFirebaseUser fakeUser)
    {
        Debug.Log("Msg::::: OnLoginSuccess() [FakeFirebaseUser]");

        WelcomeMessage.text = $"{fakeUser.DisplayName}, gràcies per registrar-te";
        EmailText.text = fakeUser.Email;

        LoginPanel.SetActive(false);
        ChildDataPanel.SetActive(true);
        UpdateContinueButtonState();
    }



    /// <summary>
    /// Fake login for testing UI flow without Firebase
    /// </summary>
    private void SimulateLogin()
    {
        Debug.Log("Msg::::: SimulateLogin() called – skipping Google/Firebase login");

        isFakeUser = true; // ✅ mark that this is a fake session

        FakeFirebaseUser fakeUser = new FakeFirebaseUser(
            "testUser123",
            "Test User",
            "testuser@example.com"
        );

        OnLoginSuccess(fakeUser);
    }

    private void UpdateContinueButtonState()
    {
        bool hasName = !string.IsNullOrWhiteSpace(ChildNameField.text);
        bool hasAge = AgeDropdown.value > 0; // assuming 0 is "Select age"
        bool hasAnyCondition = TDAH_Toggle.isOn || No_Toggle.isOn ||
                               TEL_Toggle.isOn || TEA_Toggle.isOn || Other_Toggle.isOn;

        ContinueButton.interactable = hasName && hasAge && hasAnyCondition;
    }

    private void OnContinue()
    {
        // ✅ If using real Firebase login
        if (!isFakeUser)
        {
            if (user == null)
            {
                Debug.LogError("Msg:::::No user logged in.");
                return;
            }

            DocumentReference docRef = firestore.Collection("users").Document(user.UserId);

            var childData = new
            {
                parentName = user.DisplayName,
                parentEmail = user.Email,
                childName = ChildNameField.text,
                age = AgeDropdown.options[AgeDropdown.value].text,
                TDAH = TDAH_Toggle.isOn,
                No = No_Toggle.isOn,
                TEL = TEL_Toggle.isOn,
                TEA = TEA_Toggle.isOn,
                Other = Other_Toggle.isOn,
                OtherDetails = OtherTextField.text
            };

            docRef.SetAsync(childData).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    Debug.Log("Msg:::::Child data saved successfully!");
                    SceneManager.LoadScene("MapPath");
                }
                else
                {
                    Debug.LogError("Msg::::: Error saving child data: " + task.Exception);
                    ContinueButton.interactable = true;
                }
            });
        }
        else
        {
            // ✅ Fake login flow (skip Firebase, just go to MapPath)
            Debug.Log("Msg::::: Fake user detected — skipping Firestore save.");
            SceneManager.LoadScene("MapPath");
        }
    }

}


public class FakeFirebaseUser
{
    public string UserId { get; }
    public string DisplayName { get; }
    public string Email { get; }

    public FakeFirebaseUser(string id, string name, string mail)
    {
        UserId = id;
        DisplayName = name;
        Email = mail;
    }
}