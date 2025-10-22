using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Firebase;
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
    public string GoogleAPI =  "1068789468608-otkna5ad1hgh9qqn0vt67630k67ri69r.apps.googleusercontent.com";
    private FirebaseAuth auth;
    private FirebaseUser user;
    private FirebaseFirestore firestore;
    private bool isFirebaseReady = false;

    [Header("Panels")]
    public GameObject LoginPanel;
    public GameObject ChildDataPanel;

    [Header("Buttons")]
    public Button SignInButton;
    public Button ContinueButton;
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

    private bool isSigningIn = false;
    private bool isGoogleSignInInitialized = false;

    private void Start()
    {
        Debug.Log("Msg::::: Start()");

        if (DebugLoginButton != null)
        {
            DebugLoginButton.onClick.RemoveAllListeners();
            DebugLoginButton.onClick.AddListener(SimulateLogin);
        }

        InitFirebase();

        if (!isGoogleSignInInitialized)
        {
            GoogleSignIn.Configuration = new GoogleSignInConfiguration
            {
                RequestIdToken = true,
                WebClientId = GoogleAPI,
                RequestEmail = true
                //ForceCodeForRefreshToken = true // ensure popup shows
            };
            isGoogleSignInInitialized = true;
            Debug.Log("Msg::::: Google SignIn configuration initialized in Start()");
        }

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

#if UNITY_EDITOR
        Debug.Log("Msg::::: UNITY_EDITOR detected – using fake login");
        SimulateLogin();
#endif

        ChildNameField.onValueChanged.AddListener(_ => UpdateContinueButtonState());
        AgeDropdown.onValueChanged.AddListener(_ => UpdateContinueButtonState());
        No_Toggle.onValueChanged.AddListener(_ => UpdateContinueButtonState());
        TEL_Toggle.onValueChanged.AddListener(_ => UpdateContinueButtonState());
        TEA_Toggle.onValueChanged.AddListener(_ => UpdateContinueButtonState());
        TDAH_Toggle.onValueChanged.AddListener(_ => UpdateContinueButtonState());
        Other_Toggle.onValueChanged.AddListener(_ => UpdateContinueButtonState());

        UpdateContinueButtonState();
    }

    private void InitFirebase()
    {
        Debug.Log("Msg::::: InitFirebase()");
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var status = task.Result;
            if (status == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
                firestore = FirebaseFirestore.DefaultInstance;
                isFirebaseReady = true;
                Debug.Log("Firebase ready and initialized successfully.");
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

        Debug.Log($">>>> SignIn Button Pressed at: {System.DateTime.Now:HH:mm:ss.fff}");
        if (isSigningIn) return;
        isSigningIn = true;

        // Ensure popup chooser
        GoogleSignIn.DefaultInstance.Disconnect();
        GoogleSignIn.DefaultInstance.SignOut();

        Debug.Log("Msg::::: Starting Google SignIn flow...");

        GoogleSignIn.DefaultInstance.SignIn().ContinueWithOnMainThread(task =>
        {
            isSigningIn = false;
            Debug.Log("Msg::::: GoogleSignIn task finished. Status: " + task.Status);

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
                isSigningIn = false;

                if (authTask.IsCanceled)
                {
                    Debug.LogWarning("Msg::::: Firebase Auth canceled.");
                    return;
                }
                if (authTask.IsFaulted)
                {
                    Debug.LogError("Msg::::: Firebase Auth error: " + authTask.Exception);
                    return;
                }

                Debug.Log("Msg::::: Firebase Auth success. Logged in as: " + authTask.Result.DisplayName);
                OnLoginSuccess(auth.CurrentUser);
            });
        });
    }

    private void OnLoginSuccess(FirebaseUser loggedUser)
    {
        if (loggedUser == null) return;
        isFakeUser = false;
        user = loggedUser;

        Debug.Log("Msg::::: OnLoginSuccess() [FirebaseUser]");
        WelcomeMessage.text = $"{user.DisplayName}, gràcies per registrar-te";
        EmailText.text = user.Email;

        LoginPanel.SetActive(false);
        ChildDataPanel.SetActive(true);
    }

    private void SimulateLogin()
    {
        Debug.Log("Msg::::: SimulateLogin() – skipping Google/Firebase login");
        isFakeUser = true;

        FakeFirebaseUser fakeUser = new FakeFirebaseUser(
            "testUser123", "Test User", "testuser@example.com"
        );
        OnLoginSuccess(fakeUser);
    }

    private void OnLoginSuccess(FakeFirebaseUser fakeUser)
    {
        Debug.Log("Msg::::: OnLoginSuccess() [FakeFirebaseUser]");
        WelcomeMessage.text = $"{fakeUser.DisplayName}, gràcies per registrar-te";
        EmailText.text = fakeUser.Email;

        LoginPanel.SetActive(false);
        ChildDataPanel.SetActive(true);
        UpdateContinueButtonState();
    }

    private void UpdateContinueButtonState()
    {
        bool hasName = !string.IsNullOrWhiteSpace(ChildNameField.text);
        bool hasAge = AgeDropdown.value > 0;
        bool hasAnyCondition =
            TDAH_Toggle.isOn || No_Toggle.isOn || TEL_Toggle.isOn ||
            TEA_Toggle.isOn || Other_Toggle.isOn;

        ContinueButton.interactable = hasName && hasAge && hasAnyCondition;
    }

    private void OnContinue()
    {
        if (!isFakeUser)
        {
            if (user == null)
            {
                Debug.LogError("Msg::::: No user logged in.");
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
                    Debug.Log("Msg::::: Child data saved successfully!");
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
            Debug.Log("Msg::::: Fake user detected – skipping Firestore save");
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
