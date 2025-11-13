using System;
using System.Collections;
using Firebase.Auth;
using Firebase.Extensions;
using Google;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginWithGoogle : MonoBehaviour
{
    [Header("Firebase")]
    public string GoogleAPI =
        "1068789468608-otkna5ad1hgh9qqn0vt67630k67ri69r.apps.googleusercontent.com";

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
    public TMP_InputField ChildSchoolField;
    public TMP_Dropdown ChildGradeField;

    // public TMP_Dropdown AgeField;

    [Header("Child Birthdate")]
    public TMP_InputField BirthDayField;
    public TMP_InputField BirthMonthField;
    public TMP_InputField BirthYearField;

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

    private FirebaseService firebaseService;
    private FirebaseUser user;

    private bool isSigningIn = false;

    private bool inputGroupValid = false;
    private bool toogleGroupValid = false;
    private bool birthdateGroupValid = false;
    private bool schoolGroupValid = false;

    private void Start()
    {
        Debug.Log("Msg::::: Start()");

        firebaseService = new FirebaseService();
        firebaseService.InitFirebase();

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
        OtherInput.gameObject.SetActive(false);

        // Listeners for input fields to update button state
        ChildNameField.onValueChanged.AddListener(OnInputChange);
        ChildLastNameField.onValueChanged.AddListener(OnInputChange);
        ChildIDField.onValueChanged.AddListener(OnInputChange);
        ChildSchoolField.onValueChanged.AddListener(OnInputChange);

        // Birthdate fields
        BirthDayField.onValueChanged.AddListener(OnInputChange);
        BirthMonthField.onValueChanged.AddListener(OnInputChange);
        BirthYearField.onValueChanged.AddListener(OnInputChange);

        // Grade dropdown
        ChildGradeField.onValueChanged.AddListener(OnDropdownChange);

        NoToggle.onValueChanged.AddListener(OnToggleChange);
        TELToggle.onValueChanged.AddListener(OnToggleChange);
        TEAToggle.onValueChanged.AddListener(OnToggleChange);
        TDAHToggle.onValueChanged.AddListener(OnToggleChange);
        OtherToggle.onValueChanged.AddListener(OnToggleChange);
        OtherToggle.onValueChanged.AddListener(OnToggleOtherChanged);
    }

    private void OnInputChange(string value)
    {
        ValidateAllFields();
    }

    private void OnDropdownChange(int value)
    {
        ValidateAllFields();
    }

    private void ValidateAllFields()
    {
        bool nameValid = !string.IsNullOrWhiteSpace(ChildNameField.text);
        bool lastNameValid = !string.IsNullOrWhiteSpace(ChildLastNameField.text);
        bool idValid = !string.IsNullOrWhiteSpace(ChildIDField.text);
        inputGroupValid = nameValid && lastNameValid && idValid;

        schoolGroupValid = !string.IsNullOrWhiteSpace(ChildSchoolField.text);

        bool dayValid = !string.IsNullOrWhiteSpace(BirthDayField.text);
        bool monthValid = !string.IsNullOrWhiteSpace(BirthMonthField.text);
        bool yearValid = !string.IsNullOrWhiteSpace(BirthYearField.text);
        birthdateGroupValid = dayValid && monthValid && yearValid;

        UpdateContinueButton();
    }

    private void OnToggleChange(bool value)
    {
        bool valid = NoToggle.isOn || TELToggle.isOn || TEAToggle.isOn || TDAHToggle.isOn;

        toogleGroupValid = valid;

        if (!valid && OtherToggle.isOn)
        {
            OtherInput.onValueChanged.RemoveAllListeners();
            OtherInput.onValueChanged.AddListener(text =>
            {
                bool otherValid = !string.IsNullOrWhiteSpace(OtherInput.text);
                toogleGroupValid = otherValid;
                UpdateContinueButton();
            });
        }

        UpdateContinueButton();
    }

    private void OnToggleOtherChanged(bool isOn)
    {
        OtherInput.gameObject.SetActive(isOn);
        if (!isOn)
            OtherInput.text = string.Empty;
    }

    private void UpdateContinueButton()
    {
        bool allValid =
            inputGroupValid && toogleGroupValid && birthdateGroupValid && schoolGroupValid;
        ContinueButton.interactable = allValid;
    }

    public void Login()
    {
        if (!firebaseService.IsFirebaseReady)
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

                if (googleTask.IsFaulted)
                {
                    Debug.LogError($"<DEBUG> Google Sign-In failed: {googleTask.Exception}");
                    return;
                }

                if (googleTask.IsCanceled)
                {
                    Debug.LogWarning("<DEBUG> Google Sign-In canceled by user.");
                    return;
                }

                Debug.Log("✅ Google Sign-In success. Exchanging with Firebase...");

                // Get the Google Sign-In IdToken (OAuth token)
                string googleIdToken = googleTask.Result.IdToken;

                // Use that token to sign in with Firebase
                Credential credential = GoogleAuthProvider.GetCredential(googleIdToken, null);

                try
                {
                    user = await firebaseService.SignIn(credential);
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
        LoginPage.SetActive(false);
        ChildDataPage.SetActive(true);

        ChildRegister childRegister = ChildDataPage.GetComponent<ChildRegister>();
        childRegister.SetParentInfo(user.Email, user.DisplayName);
    }

    private void OnContinue()
    {
        Debug.Log("Msg::::: Continue button clicked.");

        string firstName = ChildNameField.text.Trim();
        string lastName = ChildLastNameField.text.Trim();

        string year = BirthYearField.text.Trim();
        string month = BirthMonthField.text.Trim().PadLeft(2, '0');
        string day = BirthDayField.text.Trim().PadLeft(2, '0');
        string birthDate = $"{year}-{month}-{day}";

        string school = ChildSchoolField.text.Trim();

        int grade = ChildGradeField.value + 1;

        IEnumerable toggles = DisorderToggleGroup.ActiveToggles();

        string disorder = "No";
        foreach (Toggle toggle in toggles)
        {
            if (toggle.name == "Other")
            {
                disorder = OtherInput != null ? OtherInput.text : string.Empty;
            }
            disorder = toggle.name;
        }

        ChildModel child = new()
        {
            FirstName = firstName,
            LastName = lastName,
            BirthDate = birthDate,
            Disorder = disorder,
            School = school,
            Grade = grade,
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

        Debug.Log("ChildModel JSON: " + child.ToJson());
        StartCoroutine(SendChildData(child));
    }

    IEnumerator SendChildData(ChildModel data)
    {
        void onComplete(bool success)
        {
            string message = success
                ? "Les dades del nen s'han enviat correctament."
                : "Hi ha hagut un error en enviar les dades del nen. Si us plau, torna-ho a intentar més tard.";

            modal.Show(
                success ? "Èxit" : "Error",
                message,
                success ? () => SceneManager.LoadScene("MapPathScene") : () => { }
            );
        }

        yield return new ChildService().SendChildData(data, onComplete);
    }

    private void DebugLogin()
    {
        // LoginPage.SetActive(false);
        // ChildDataPage.SetActive(true);

        // ChildRegister childRegister = ChildDataPage.GetComponent<ChildRegister>();
        // childRegister.SetParentInfo("test@gmail.com", "Test User");

        SceneManager.LoadScene("BasketScene");
    }
}
