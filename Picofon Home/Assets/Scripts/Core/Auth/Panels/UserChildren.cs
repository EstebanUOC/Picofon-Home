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
        CancellationTokenSource cts = UIManager.Cts;

        string userId = UIManager.User.Id;

        var children = await userService.GetUserChildren(userId, cts);

        childrenDropdown.ClearOptions();
        foreach (var child in children)
        {
            string fullName = child.FirstName + " " + child.LastName;
            TMP_Dropdown.OptionData option = new(fullName);
            childrenDropdown.options.Add(option);
        }
        childrenDropdown.RefreshShownValue();
    }

    private void OnSelectChild()
    {
        // string selectedChild = childrenDropdown.options[childrenDropdown.value].text;
        // UIManager.ShowChildDashboard(selectedChild);
        SceneManager.LoadScene("MapPathScene");
    }

    private void OnRegisterChild()
    {
        UIManager.ShowRegisterChild();
    }
}
