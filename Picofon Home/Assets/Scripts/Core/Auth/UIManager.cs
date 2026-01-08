using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Firebase.Auth;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Space(15)]
    public Panel LoginPanel;
    public Panel DisclaimerPanel;
    public Panel UserChildrenPanel;
    public Panel RegisterChildPanel;

    [Space(15)]
    public Panel LoadingPanel;
    public Modal ModalPanel;

    [Space(15)]
    public float VersionNumber = 0.2f;

    public UserDataDTO CurrentUser { get; set; }
    public UserService UserService = new();

    public FirebaseAuth FirebaseAuthInstance { get; private set; }

    public void Start()
    {
        LoadingPanel.Show();
        BootstrapApplicacion().Forget();
        // ShowRegisterChild();
    }

    private async UniTaskVoid BootstrapApplicacion()
    {
        CancellationToken ct = this.GetCancellationTokenOnDestroy();

        TimeoutController timeoutController = new();
        CancellationToken timeoutCt = timeoutController.Timeout(TimeSpan.FromSeconds(30));

        FirebaseService firebaseService = new();

        bool success = await firebaseService.RunAsync(ct, timeoutCt);

        if (!success)
        {
            Debug.LogError("Firebase failed to initialize.");
            return;
        }

        await UniTask.WaitForSeconds(2, cancellationToken: ct);

        timeoutController.Reset();
        timeoutController.Dispose();
        FirebaseAuthInstance = FirebaseAuth.DefaultInstance;

        if (FirebaseAuthInstance.CurrentUser is null)
        {
            ShowLogin();
            LoadingPanel.Hide();
            return;
        }

#if !UNITY_EDITOR
        Debug.Log(
            "User is already logged in, DisplayName: "
                + FirebaseAuthInstance.CurrentUser.DisplayName
        );
#endif

        string firebaseIdToken = await FirebaseAuthInstance
            .CurrentUser.TokenAsync(false)
            .AsUniTask()
            .AttachExternalCancellation(ct);

        UserModel user = await UserService.LoginWithFirebaseToken(firebaseIdToken);

        CurrentUser = new UserDataDTO
        {
            Id = user.Id,
            Email = user.Email,
            Username = user.FirstName,
        };

        if (GamePrefs.HasAcceptedTerms)
        {
            ShowUserChildren();
        }
        else
        {
            ShowDisclaimer();
        }

        LoadingPanel.Hide();
    }

    public void ShowLogin()
    {
        HideAllPanels();
        LoginPanel.Show();
    }

    public void ShowRegisterChild()
    {
        HideAllPanels();
        RegisterChildPanel.Show();
    }

    public void ShowDisclaimer()
    {
        HideAllPanels();
        DisclaimerPanel.Show();
    }

    public void ShowUserChildren()
    {
        HideAllPanels();
        UserChildrenPanel.Show();
    }

    public void ShowModal(ModalData data)
    {
        ModalPanel.Show(data);
    }

    public void Logout()
    {
        FirebaseAuthInstance.SignOut();

        CurrentUser = null;

        GamePrefs.ClearAll();
        ShowLogin();
    }

    private void HideAllPanels()
    {
        LoginPanel.Hide();
        RegisterChildPanel.Hide();
        UserChildrenPanel.Hide();
        DisclaimerPanel.Hide();
    }
}
