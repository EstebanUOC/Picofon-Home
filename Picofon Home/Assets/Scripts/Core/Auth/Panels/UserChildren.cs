using System.Collections.Generic;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UserChildren : Panel
{
    public UIManager UIManager;

    [Space(15)]
    public TMP_Dropdown childrenDropdown;

    [Space(15)]
    public CustomButtonBase SelectChildButton;
    public CustomButtonBase RegisterChildButton;

    [Space(15)]
    public Button LogoutButton;

    private readonly Dictionary<string, string> childrenDict = new();

    private CancellationTokenSource cts;

    public void Start()
    {
        OnHide += () => gameObject.SetActive(false);

        SelectChildButton.OnClick += OnSelectChild;
        RegisterChildButton.OnClick += OnRegisterChild;
        LogoutButton.onClick.AddListener(OnLogout);
    }

    public override void Show()
    {
        base.Show();
        LoadChildren().Forget();
    }

    private async UniTaskVoid LoadChildren()
    {
        UserService userService = UIManager.UserService;
        cts = new CancellationTokenSource();

        string userId = UIManager.CurrentUser.Id;

        List<ChildListItemDTO> children;
        try
        {
            children = await userService.GetUserChildren(userId, cts);
        }
        catch (System.Exception)
        {
            ModalData modalData = new()
            {
                Title = "Error",
                Message = "Could not load children. Please try again later.",
            };
            await UIManager.ShowModal(modalData);
            return;
        }

        childrenDropdown.ClearOptions();

        StringBuilder sb = new();

        foreach (var child in children)
        {
            sb.Clear();
            sb.Append(child.FirstName);
            sb.Append(" ");
            sb.Append(child.LastName);

            string fullName = sb.ToString();

            TMP_Dropdown.OptionData option = new(fullName);
            childrenDropdown.options.Add(option);

            childrenDict[fullName] = child.Id;
        }
        childrenDropdown.RefreshShownValue();
    }

    private void OnSelectChild()
    {
        string childName = childrenDropdown.options[childrenDropdown.value].text;
        string childId = childrenDict[childName];

        MapPathPayload.ChildId = childId;

        SceneManager.LoadScene("MapPathScene");
    }

    private void OnRegisterChild()
    {
        UIManager.ShowRegisterChild();
        cts.Cancel();
    }

    private void OnLogout()
    {
        UIManager.Logout();
        cts.Cancel();
    }
}
