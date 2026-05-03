using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class UserChildren : MonoBehaviour
{
    [SerializeField]
    private AuthManager _authManager;

    [SerializeField]
    private UIManager _uiManager;

    [Space]
    [SerializeField]
    private LoadingTransition _loadingTransition;

    [Space]
    [SerializeField]
    private TMP_Dropdown _childrenDropdown;

    [Space]
    [SerializeField]
    private CustomButton _selectButton;

    [SerializeField]
    private CustomButton _registerButton;

    [Space]
    [SerializeField]
    private SimpleButton _logoutButton;

    private string[] _childrenIds;

    private string _userId;

    private RectTransform _panel;

    public void Start()
    {
        _selectButton.OnClick += OnSelectChild;
        _registerButton.OnClick += OnRegisterChild;

        _logoutButton.OnClick += OnLogout;

        _userId = _authManager.CurrentUser.Id;

        _panel = GetComponent<RectTransform>();
    }

    public void OnEnable()
    {
        LoadChildren().Forget();
    }

    private async UniTaskVoid LoadChildren()
    {
        UserService userService = _authManager.UserService;
        CancellationToken token = this.GetCancellationTokenOnDestroy();

        _userId = _authManager.CurrentUser.Id;

        ApiResult<ChildListItemDTO[]> result = await userService.GetUserChildren(_userId, token);

        if (!result.Success)
        {
            ModalData modalData = new()
            {
                Title = "Error",
                Message = "Could not load children. Please try again later.",
                Panel = _panel,
            };
            await _uiManager.ShowModal(modalData);
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

        MapPathPayload.ChildId = childId;
        MapPathPayload.ConductedById = _authManager.CurrentUser.Id;

        LevelDataStore instance = LevelDataStore.Instance;

        _loadingTransition.PlayLoadingTransition();

        await instance.LoadPlans(childId);

        if (!instance.HasPlans())
        {
            await instance.CreateDefaultPlans(childId, _userId);
        }

        _loadingTransition.Continue(success: instance.HasPlans() && instance.HasActivePlans());

        if (!instance.HasActivePlans())
        {
            await UniTask.WaitForSeconds(2.5f);
            await _uiManager.ShowModal(
                new ModalData
                {
                    Title = "No Active Plans",
                    Message =
                        "There are no active therapy plans for the selected child. Choose another child",
                    Panel = _panel,
                }
            );
            return;
        }
    }

    private void OnRegisterChild()
    {
        _uiManager.Show(PanelEnum.RegisterChild);
    }

    private void OnLogout()
    {
        _authManager.Logout();
    }
}
