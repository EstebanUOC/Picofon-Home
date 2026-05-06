using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using static TMPro.TMP_Dropdown;

public class UserChildren : MonoBehaviour
{
    [SerializeField]
    private AuthManager _authManager;

    [SerializeField]
    private UIManager _uiManager;

    [Space]
    [SerializeField]
    private SimpleButton _logoutButton;

    [Header("Center Selection")]
    [SerializeField]
    private TMP_Dropdown _centerDropdown;

    [SerializeField]
    private GameObject _labelObject;

    [SerializeField]
    private TMP_Text _centerLabel;

    [SerializeField]
    private CustomButton _selectCenterButton;

    [Header("Child Selection")]
    [SerializeField]
    private TMP_Dropdown _childrenDropdown;

    [SerializeField]
    private CustomButton _selectButton;

    [SerializeField]
    private CustomButton _registerButton;

    private string _userId;

    private string[] _childrenIds;

    private int[] _centerIds;

    private RectTransform _panel;

    public void Start()
    {
        _selectButton.OnClick += OnSelectChild;
        _registerButton.OnClick += OnRegisterChild;

        _logoutButton.OnClick += OnLogout;

        _panel = GetComponent<RectTransform>();
    }

    public void OnEnable()
    {
        UserRole role = _authManager.CurrentUser.Role;

        if (role == UserRole.Therapist)
        {
            LoadCenters().Forget();
            return;
        }

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

        if (result.Data.Length == 1)
        {
            _childrenDropdown.gameObject.SetActive(false);
            _selectButton.gameObject.SetActive(false);
            return;
        }

        _childrenIds = new string[result.Data.Length];
        _childrenDropdown.ClearOptions();

        StringBuilder stringBuilder = new();

        for (int i = 0; i < result.Data.Length; i++)
        {
            ChildListItemDTO child = result.Data[i];

            stringBuilder.Clear();
            stringBuilder.Append(child.FirstName);
            stringBuilder.Append(" ");
            stringBuilder.Append(child.LastName);

            string fullName = stringBuilder.ToString();

            OptionData option = new(fullName);
            _childrenDropdown.options.Add(option);

            _childrenIds[i] = child.Id;
        }

        _childrenDropdown.RefreshShownValue();
    }

    private async UniTaskVoid LoadCenters()
    {
        UserService userService = _authManager.UserService;
        CancellationToken token = this.GetCancellationTokenOnDestroy();

        _userId = _authManager.CurrentUser.Id;

        ApiResult<CenterDTO[]> result = await userService.GetCenters(_userId, token);

        if (!result.Success)
        {
            ModalData modalData = new()
            {
                Title = "Error",
                Message = "Could not load centers. Please try again later.",
                Panel = _panel,
            };

            await _uiManager.ShowModal(modalData);

            _selectCenterButton.Interactable = false;

            return;
        }

        if (result.Data.Length == 0)
        {
            PerformanceLog.Log(
                "No centers found for user. This should not happen as the user should be associated with at least one center."
            );
            return;
        }

        if (result.Data.Length == 1)
        {
            _centerDropdown.gameObject.SetActive(false);
            _labelObject.SetActive(true);

            _centerLabel.SetText(result.Data[0].Center);

            return;
        }

        _centerIds = new int[result.Data.Length];
        _centerDropdown.ClearOptions();

        for (int i = 0; i < result.Data.Length; i++)
        {
            _centerDropdown.options.Add(new OptionData(result.Data[i].Center));

            _centerIds[i] = i;
        }

        _centerDropdown.RefreshShownValue();
    }

    private async UniTaskVoid OnSelectChildAsync()
    {
        int selectedIndex = _childrenDropdown.value;
        string childId = _childrenIds[selectedIndex];

        MapPathPayload.ChildId = childId;
        MapPathPayload.ConductedById = _authManager.CurrentUser.Id;

        LevelDataStore instance = LevelDataStore.Instance;

        _uiManager.PlayMapTransition();

        await instance.LoadPlans(childId);

        if (!instance.HasPlans())
        {
            await instance.CreateDefaultPlans(childId, _userId);
        }

        _uiManager.ContinueMapTransition(success: instance.HasPlans() && instance.HasActivePlans());

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

    private void OnSelectChild()
    {
        OnSelectChildAsync().Forget();
    }

    private void OnRegisterChild()
    {
        _uiManager.ShowPanel(PanelEnum.RegisterChild);
    }

    private void OnLogout()
    {
        _authManager.Logout();
    }
}
