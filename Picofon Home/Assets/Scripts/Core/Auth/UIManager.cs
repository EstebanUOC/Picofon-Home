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

    public UserDataDTO CurrentUser { get; set; }
    public UserService UserService = new();

    public FirebaseAuth FirebaseAuthInstance { get; private set; }

    public void Start()
    {
        LoadingPanel.Show();
        LoadThins().Forget();
    }

    private async UniTaskVoid LoadThins()
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
        LoadingPanel.Hide();
        timeoutController.Reset();
        timeoutController.Dispose();
        FirebaseAuthInstance = FirebaseAuth.DefaultInstance;
    }

    private void HideAllPanels()
    {
        LoginPanel.Hide();
        RegisterChildPanel.Hide();
        UserChildrenPanel.Hide();
        DisclaimerPanel.Hide();
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
}
