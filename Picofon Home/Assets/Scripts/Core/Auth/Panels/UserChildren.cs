using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using PrimeTween;
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
    private GameObject _centerContent;

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
    private GameObject _childContent;

    [SerializeField]
    private TMP_Dropdown _childrenDropdown;

    [SerializeField]
    private CustomButton _selectChildButton;

    [SerializeField]
    private CustomButton _registerButton;

    [Space]
    [SerializeField]
    private RectTransform _prueba;

    private string _userId;

    private string[] _childrenIds;

    private int[] _centerIds;

    private RectTransform _panel;

    public void Start()
    {
        _logoutButton.OnClick += OnLogout;

        _selectCenterButton.OnClick += OnSelectCenter;

        _selectChildButton.OnClick += OnSelectChild;

        _registerButton.OnClick += OnRegisterChild;

        _panel = GetComponent<RectTransform>();
    }

    public void OnEnable()
    {
        UserRole role = _authManager.CurrentUser.Role;

        bool isTherapist = role == UserRole.Therapist;

        _centerContent.SetActive(isTherapist);
        _childContent.SetActive(!isTherapist);

        if (role == UserRole.Therapist)
        {
            _prueba.sizeDelta = new Vector2(_prueba.sizeDelta.x, 634);

            LoadCenters().Forget();
            return;
        }

        _prueba.sizeDelta = new Vector2(_prueba.sizeDelta.x, 795);
        LoadChildren().Forget();
    }

    private async UniTaskVoid LoadChildren(int centerId = -1)
    {
        UserService userService = _authManager.UserService;
        CancellationToken token = this.GetCancellationTokenOnDestroy();

        _userId = _authManager.CurrentUser.Id;

        ApiResult<ChildListItemDTO[]> result = await userService.GetUserChildren(
            userId: _userId,
            token: token,
            centerId: centerId
        );

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
            _selectChildButton.gameObject.SetActive(false);

            _ = Tween.UISizeDelta(_prueba, new Vector2(_prueba.sizeDelta.x, 415), 0.5f);

            return;
        }

        _ = Tween.UISizeDelta(_prueba, new Vector2(_prueba.sizeDelta.x, 795), 0.5f);

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

        _centerIds = new int[result.Data.Length];

        if (result.Data.Length == 1)
        {
            _centerDropdown.gameObject.SetActive(false);
            _labelObject.SetActive(true);

            _centerLabel.SetText(result.Data[0].Center);

            _centerIds[0] = 1;

            return;
        }

        _labelObject.SetActive(false);
        _centerDropdown.gameObject.SetActive(true);

        _centerDropdown.ClearOptions();

        for (int i = 0; i < result.Data.Length; i++)
        {
            _centerDropdown.options.Add(new OptionData(result.Data[i].Center));

            _centerIds[i] = i + 1;
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

    private async UniTaskVoid OnSelectCenterAsync()
    {
        int selectedIndex = _centerDropdown.value;
        int centerId = _centerIds[selectedIndex];

        LoadChildren(centerId).Forget();

        _centerContent.SetActive(false);
        _childContent.SetActive(true);
    }

    private void OnSelectChild()
    {
        OnSelectChildAsync().Forget();
    }

    private void OnSelectCenter()
    {
        OnSelectCenterAsync().Forget();
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
