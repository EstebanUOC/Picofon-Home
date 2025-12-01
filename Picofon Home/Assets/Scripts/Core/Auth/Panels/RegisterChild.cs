using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RegisterChild : Panel
{
    public UIManager UIManager;

    [Header("Parent Info")]
    public TMP_Text EmailText;
    public TMP_Text UsernameText;

    [Header("Form")]
    public Form ChildRegistrationForm;
    public Button ContinueButton;

    private CancellationTokenSource cts;

    public void Start()
    {
        ContinueButton.onClick.AddListener(OnContinue);

        OnHide += () => gameObject.SetActive(false);
    }

    public override void Show()
    {
        base.Show();
        UserDataDTO parentData = UIManager.CurrentUser;

        EmailText.text = parentData.Email;
        UsernameText.text = parentData.Username;
        ChildRegistrationForm.ParentId = parentData.Id;
    }

    private async void OnContinue()
    {
        ChildCreateDTO childData = ChildRegistrationForm.GatherChildData();

        Debug.Log(
            "Registering child with data:"
                + $"\nID: {childData.Id}"
                + $"\nFirst Name: {childData.FirstName}"
                + $"\nLast Name: {childData.LastName}"
                + $"\nBirth Date: {childData.BirthDate}"
                + $"\nDisorder: {childData.Disorder}"
                + $"\nSchool: {childData.School}"
                + $"\nGrade: {childData.Grade}"
                + $"\nParent ID: {childData.OwnerId}"
        );

        UserService userService = UIManager.UserService;
        cts = new CancellationTokenSource();

        var response = await userService.RegisterChild(childData, cts);

        string message = string.Empty;

        if (response.Success)
        {
            message = "Les dades del nen s'han enviat correctament.";
        }
        else
        {
            foreach (var error in response.Message.Content)
            {
                Debug.LogError($"Error registering child: {error}");
                message += error + "\n";
            }
        }

        Action callback = response.Success
            ? () => SceneManager.LoadScene("MapPathScene")
            : () => { };

        ModalData modalData = new()
        {
            Title = "Registre de nen",
            Message = message,
            OnClose = callback,
        };

        UIManager.ShowModal(modalData);
    }
}
