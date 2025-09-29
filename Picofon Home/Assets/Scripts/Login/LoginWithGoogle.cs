using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using Google;

public class LoginWithGoogle : MonoBehaviour
{
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

    [Header("User Info UI")]   
    public TextMeshProUGUI WelcomeMessage;
    public TextMeshProUGUI EmailText;

    [Header("Child Data UI")]
    public TMP_InputField ChildNameField;
    public TMP_Dropdown AgeDropdown;
    public Toggle TDAH_Toggle;
    public Toggle Down_Toggle;
    public Toggle TEL_Toggle;
    public Toggle TEA_Toggle;
    public Toggle Other_Toggle;
    public TMP_InputField OtherTextField;

    private bool isGoogleSignInInitialized = false;

    private void Start()
    {
        Debug.Log("Msg::::: Start()");
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
        Debug.Log("Msg::::: OnLoginSuccess()");
        user = loggedUser;

        WelcomeMessage.text = $"{user.DisplayName}, gràcies per registrar-te";        
        EmailText.text = user.Email; 

        LoginPanel.SetActive(false);
        ChildDataPanel.SetActive(true);
    }

    private void OnContinue()
    {
        if (user == null)
        {
            Debug.LogError("Msg:::::No user logged in.");
            return;
        }

        string childName = ChildNameField.text;
        string age = AgeDropdown.options[AgeDropdown.value].text;
        bool hasTDAH = TDAH_Toggle.isOn;
        bool hasDown = Down_Toggle.isOn;
        bool hasTEL = TEL_Toggle.isOn;
        bool hasTEA = TEA_Toggle.isOn;
        bool hasOther = Other_Toggle.isOn;
        string otherDetails = OtherTextField.text;

        DocumentReference docRef = firestore.Collection("users").Document(user.UserId);
        var childData = new
        {
            parentName = user.DisplayName,
            parentEmail = user.Email,
            childName = childName,
            age = age,
            TDAH = hasTDAH,
            Down = hasDown,
            TEL = hasTEL,
            TEA = hasTEA,
            Other = hasOther,
            OtherDetails = otherDetails
        };

        docRef.SetAsync(childData).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Msg:::::Child data saved successfully!");
            }
            else
            {
                Debug.LogError("Msg::::: Error saving child data: " + task.Exception);
            }
        });
    }
}
