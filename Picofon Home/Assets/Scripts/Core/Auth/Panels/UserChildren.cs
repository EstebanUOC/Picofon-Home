using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class UserChildren : Panel
{
    public UIManager UIManager;

    [SerializeField]
    private LoadingTransition _loadingTransition;

    [Space]
    [SerializeField]
    private TMP_Dropdown _childrenDropdown;

    [Space]
    [SerializeField]
    private GameObject _selectButton;

    [SerializeField]
    private GameObject _registerButton;

    [Space]
    [SerializeField]
    private SimpleButton _logoutButton;

    private string[] _childrenIds;

    private string _userId;

    public void Start()
    {
        OnHide += () => gameObject.SetActive(false);

        CustomButtonBase selectButton = _selectButton.GetComponent<CustomButtonBase>();
        CustomButtonBase registerButton = _registerButton.GetComponent<CustomButtonBase>();

        selectButton.OnClick += OnSelectChild;
        registerButton.OnClick += OnRegisterChild;

        _logoutButton.OnClick += OnLogout;

        _userId = UIManager.CurrentUser.Id;
    }

    public override void Show()
    {
        base.Show();
        LoadChildren().Forget();
    }

    private async UniTaskVoid LoadChildren()
    {
        UserService userService = UIManager.UserService;
        CancellationToken token = this.GetCancellationTokenOnDestroy();

        _userId = UIManager.CurrentUser.Id;

        ApiResult<ChildListItemDTO[]> result = await userService.GetUserChildren(_userId, token);

        if (!result.Success)
        {
            ModalData modalData = new()
            {
                Title = "Error",
                Message = "Could not load children. Please try again later.",
            };
            await UIManager.ShowModal(modalData);
            return;
        }

        if (result.Data.Length == 0)
        {
            _childrenDropdown.gameObject.SetActive(false);
            _selectButton.gameObject.SetActive(false);
            return;
        }

        _childrenIds = new string[result.Data.Length];
        _childrenDropdown.ClearOptions();

        StringBuilder sb = new();

        for (int i = 0; i < result.Data.Length; i++)
        {
            ChildListItemDTO child = result.Data[i];

            sb.Clear();
            sb.Append(child.FirstName);
            sb.Append(" ");
            sb.Append(child.LastName);

            string fullName = sb.ToString();

            TMP_Dropdown.OptionData option = new(fullName);
            _childrenDropdown.options.Add(option);

            _childrenIds[i] = child.Id;
        }
        _childrenDropdown.RefreshShownValue();
    }

    private void OnSelectChild()
    {
        OnSelectChildAsync().Forget();
    }

    private async UniTaskVoid OnSelectChildAsync()
    {
        int selectedIndex = _childrenDropdown.value;
        string childId = _childrenIds[selectedIndex];

        childId = "99345678A";
        MapPathPayload.ChildId = childId;
        MapPathPayload.ConductedById = UIManager.CurrentUser.Id;

        LevelDataStore instance = LevelDataStore.Instance;

        _loadingTransition.PlayLoadingTransition();

        await instance.LoadPlans(childId);

        if (!instance.HasPlans())
        {
            await instance.CreateDefaultPlans(childId, _userId);
        }

        _loadingTransition.Continue(success: instance.HasPlans(), uiManager: UIManager);
    }

    private void OnRegisterChild()
    {
        UIManager.ShowRegisterChild();
    }

    private void OnLogout()
    {
        UIManager.Logout();
    }
}
