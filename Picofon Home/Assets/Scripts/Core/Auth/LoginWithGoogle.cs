using System;
using System.Collections;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Google;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginWithGoogle : MonoBehaviour
{
    [Header("Firebase")]
    public string GoogleAPI =
        "1068789468608-otkna5ad1hgh9qqn0vt67630k67ri69r.apps.googleusercontent.com";
    private FirebaseAuth auth;
    private FirebaseUser user;
    private bool isFirebaseReady = false;

    [Header("Panels")]
    public GameObject LoginPage;
    public GameObject ChildDataPage;

    [Header("Buttons")]
    public Button LoginButton;
    public Button ContinueButton;
    public Button DebugSignInButton;

    [Header("Child Data UI")]
    public TMP_InputField ChildNameField;
    public TMP_InputField ChildLastNameField;
    public TMP_InputField ChildIDField;
    public TMP_Dropdown AgeField;

    [Header("Disorder Toggles")]
    public ToggleGroup DisorderToggleGroup;
    public Toggle NoToggle;
    public Toggle TELToggle;
    public Toggle TEAToggle;
    public Toggle TDAHToggle;
    public Toggle OtherToggle;
    public TMP_InputField OtherInput; // Assign in inspector if using "Others" text field

    [Header("Modal")]
    public Modal modal;

    private bool isSigningIn = false;

    private bool inputGroupValid = false;
    private bool toogleGroupValid = false;

    private void Start()
    {
        Debug.Log("Msg::::: Start()");

        InitFirebase();

        // Configure Google Sign-In
        GoogleSignIn.Configuration = new GoogleSignInConfiguration
        {
            RequestIdToken = true,
            WebClientId = GoogleAPI,
            RequestEmail = true,
        };

        InitComponents();
    }

    private void InitComponents()
    {
        // Login components
        if (LoginButton != null)
        {
            LoginButton.onClick.RemoveAllListeners();
            LoginButton.onClick.AddListener(Login);
        }

        if (DebugSignInButton != null)
        {
            DebugSignInButton.onClick.RemoveAllListeners();
            DebugSignInButton.onClick.AddListener(DebugLogin);
        }

        LoginPage.SetActive(true);

        // Child data components

        if (ContinueButton != null)
        {
            ContinueButton.onClick.RemoveAllListeners();
            ContinueButton.onClick.AddListener(OnContinue);
        }

        ChildDataPage.SetActive(false);

        // Listeners for input fields to update button state
        UnityAction<string> textChange = OnInputChange;

        ChildNameField.onValueChanged.AddListener(textChange);
        ChildLastNameField.onValueChanged.AddListener(textChange);
        ChildIDField.onValueChanged.AddListener(textChange);

        UnityAction<bool> toggleChange = OnToogleChange;

        NoToggle.onValueChanged.AddListener(toggleChange);
        TELToggle.onValueChanged.AddListener(toggleChange);
        TEAToggle.onValueChanged.AddListener(toggleChange);
        TDAHToggle.onValueChanged.AddListener(toggleChange);
        OtherToggle.onValueChanged.AddListener(toggleChange);
    }

    private void OnInputChange(string value)
    {
        if (value == "")
        {
            inputGroupValid = false;
            ContinueButton.interactable = false;
            return;
        }

        bool nameValid = !string.IsNullOrWhiteSpace(ChildNameField.text);
        bool lastNameValid = !string.IsNullOrWhiteSpace(ChildLastNameField.text);
        bool idValid = !string.IsNullOrWhiteSpace(ChildIDField.text);

        bool valid = nameValid && lastNameValid && idValid;

        inputGroupValid = valid;

        ContinueButton.interactable = toogleGroupValid && inputGroupValid;
    }

    private void OnToogleChange(bool value)
    {
        bool valid = NoToggle.isOn || TELToggle.isOn || TEAToggle.isOn || TDAHToggle.isOn;

        toogleGroupValid = valid;
        ContinueButton.interactable = toogleGroupValid && inputGroupValid;

        if (valid)
            return;

        OtherInput.onValueChanged.AddListener(text =>
        {
            bool otherValid = !string.IsNullOrWhiteSpace(OtherInput.text);
            toogleGroupValid = otherValid;
            ContinueButton.interactable = toogleGroupValid && inputGroupValid;
        });
    }

    private void InitFirebase()
    {
        FirebaseApp
            .CheckAndFixDependenciesAsync()
            .ContinueWithOnMainThread(task =>
            {
                var status = task.Result;
                if (status == DependencyStatus.Available)
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

        GoogleSignIn
            .DefaultInstance.SignIn()
            .ContinueWithOnMainThread(async googleTask =>
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
                    StartCoroutine(
                        new LoginAPI().SendFirebaseToken(
                            firebaseIdToken,
                            success =>
                            {
                                if (success)
                                    Debug.Log("✅ Backend login success.");
                                else
                                    Debug.LogError("❌ Backend login failed.");
                            }
                        )
                    );

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
        // TitleText.enabled = false;
        // WelcomeMessage.text = $"{user.DisplayName}, gràcies per registrar-te";
        // EmailText.text = user.Email;

        LoginPage.SetActive(false);
        ChildDataPage.SetActive(true);
    }

    private void OnContinue()
    {
        Debug.Log("Msg::::: Continue button clicked.");

        // 1. Parse names from ChildNameField
        string firstName = ChildNameField.text.Trim();
        string lastName = ChildLastNameField.text.Trim();

        IEnumerable toggles = DisorderToggleGroup.ActiveToggles();

        // 2. Determine selected disorder
        string disorder = "No";
        foreach (Toggle toggle in toggles)
        {
            if (toggle.name == "Other")
            {
                disorder = OtherInput != null ? OtherInput.text : "";
            }
            disorder = toggle.name;
        }

        // 3. Fill out the model (replace hardcoded values as needed with dynamic ones)
        ChildModel child = new()
        {
            FirstName = firstName,
            LastName = lastName,
            BirthDate = "2020-05-01", // Set this from date input if you have one
            Disorder = disorder,
            School = "Escuela",
            Grade = "5",
            CenterId = 1,
            OwnerId = "AwgdI1xsu5RoU6zgLvTfAZeklbn2", // Change if needed
            Id = "62448460X", // Change if dynamic
        };

        bool valid = ChildModel.Validate(child);

        if (!valid)
        {
            Debug.LogError("Validation failed for ChildModel fields.");
            return;
        }

        StartCoroutine(SendChildData(child));
    }

    IEnumerator SendChildData(ChildModel data)
    {
        string json = data.ToJson();
        Debug.Log("ChildModel JSON: " + json);

        UnityWebRequest request = new ChildService().SendChildData(json);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            modal.Show(
                "Èxit",
                "Les dades del nen s'han enviat correctament.",
                () =>
                {
                    SceneManager.LoadScene("MapPath");
                }
            );
        }
        else
        {
            Debug.LogError("Error sending child data: " + request.error);
            modal.Show(
                "Error",
                "Hi ha hagut un error en enviar les dades del nen. Si us plau, torna-ho a intentar més tard.",
                () => { }
            );
        }
    }

    private void LogChildModel(string context, ChildModel data)
    {
        Debug.Log(
            $"{context}: "
                + $"first_name={data.FirstName}, "
                + $"last_name={data.LastName}, "
                + $"birth_date={data.BirthDate}, "
                + $"disorder={data.Disorder}, "
                + $"school={data.School}, "
                + $"grade={data.Grade}, "
                + $"center_id={data.CenterId}, "
                + $"owner_id={data.OwnerId}, "
                + $"id={data.Id}"
        );
    }

    private void DebugLogin()
    {
        LoginPage.SetActive(false);
        ChildDataPage.SetActive(true);
    }
}
