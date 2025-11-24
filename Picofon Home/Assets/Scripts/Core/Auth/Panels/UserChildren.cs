using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UserChildren : Panel
{
    public UIManager UIManager;

    [Header("Dropdowns")]
    public TMP_Dropdown childrenDropdown;

    [Header("Buttons")]
    public Button SelectChildButton;
    public Button RegisterChildButton;

    private CancellationTokenSource cts;

    private readonly Dictionary<string, string> childrenDict = new();

    public void Start()
    {
        SelectChildButton.onClick.AddListener(OnSelectChild);
        RegisterChildButton.onClick.AddListener(OnRegisterChild);
    }

    public override void Show()
    {
        base.Show();
        LoadChildren();
    }

    private async void LoadChildren()
    {
        UserService userService = UIManager.UserService;
        cts = new CancellationTokenSource();

        string userId = UIManager.User.Id;

        var children = await userService.GetUserChildren(userId, cts);

        childrenDropdown.ClearOptions();
        foreach (var child in children)
        {
            string fullName = child.FirstName + " " + child.LastName;
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
}
