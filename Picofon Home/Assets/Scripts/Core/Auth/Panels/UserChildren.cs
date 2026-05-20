using System;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static TMPro.TMP_Dropdown;

public class UserChildren : MonoBehaviour
{
    private const int HeightCenter = 630;
    private const int HeightChildren = 955;
    private const int HeightNoChildren = 415;
    private const int HeightChildrenTherapist = 595;

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
    private CustomButton _registerChildButton;

    [SerializeField]
    private CustomButtonLoading _updateChildButton;

    [Space]
    [SerializeField]
    private RectTransform _contentTransform;

    [SerializeField]
    private Image _overlay;

    [SerializeField]
    private RectTransform _overlayTransform;

    private string _userId;
    private UserRole _userRole;

    private string[] _childrenIds;
    private int[] _centerIds;
    private bool _hasChildren;

    private RectTransform _panel;
    private Action _onAlphaComplete;

    public void Start()
    {
        _logoutButton.OnClick += OnLogout;

        _selectCenterButton.OnClick += OnSelectCenter;

        _selectChildButton.OnClick += OnSelectChild;

        _registerChildButton.OnClick += OnRegisterChild;

        _updateChildButton.OnClick += OnUpdateChild;

        _panel = GetComponent<RectTransform>();

        _onAlphaComplete = () => _overlay.gameObject.SetActive(false);
    }

    public void OnEnable()
    {
        _userRole = _authManager.CurrentUser.Role;

        _centerContent.SetActive(false);
        _childContent.SetActive(false);

        _childrenDropdown.gameObject.SetActive(true);
        _selectChildButton.gameObject.SetActive(true);

        _registerChildButton.gameObject.SetActive(true);

        if (_userRole == UserRole.Therapist)
        {
            LoadCenters().Forget();
            return;
        }

        LoadChildren().Forget();
    }

    private async UniTask LoadChildrenAsync(int centerId = -1)
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

            _hasChildren = false;

            return;
        }

        _hasChildren = true;

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

    private async UniTaskVoid LoadChildren(int centerId = -1)
    {
        _overlay.gameObject.SetActive(true);
        _contentTransform.sizeDelta = new Vector2(_contentTransform.sizeDelta.x, HeightCenter);

        if (centerId != -1)
        {
            _ = Tween.Alpha(_overlay, startValue: 0f, endValue: 1f, duration: 0.3f);
        }

        await LoadChildrenAsync(centerId);

        if (_userRole == UserRole.Therapist && !_hasChildren)
        {
            _ = Tween
                .Alpha(_overlay, startValue: 1f, endValue: 0f, duration: 0.3f)
                .OnComplete(_onAlphaComplete);

            ModalData modalData = new()
            {
                Title = "No Children",
                Message =
                    "There are no children associated with the selected center. Please register a child on the web platform or choose another center.",
                Panel = _panel,
            };

            _ = _uiManager.ShowModal(modalData);

            return;
        }

        _centerContent.SetActive(false);

        Vector2 target = new(_contentTransform.sizeDelta.x, HeightChildren);

        if (_userRole == UserRole.Therapist)
        {
            target.y = HeightChildrenTherapist;

            _registerChildButton.gameObject.SetActive(false);
            _updateChildButton.gameObject.SetActive(false);
        }

        if (!_hasChildren)
        {
            target.y = HeightNoChildren;
        }

        _ = Sequence
            .Create()
            .Group(Tween.UISizeDelta(_contentTransform, endValue: target, duration: 0.2f))
            .Group(
                Tween
                    .UISizeDelta(_overlayTransform, endValue: target, duration: 0.2f)
                    .OnComplete(target: _childContent, target => target.SetActive(true))
            )
            .Chain(
                Tween
                    .Alpha(_overlay, startValue: 1f, endValue: 0f, duration: 0.3f)
                    .OnComplete(_onAlphaComplete)
            );
    }

    private async UniTask LoadCentersAsync()
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

            _centerLabel.SetText(result.Data[0].Name);

            _centerIds[0] = result.Data[0].Id;

            return;
        }

        _labelObject.SetActive(false);
        _centerDropdown.gameObject.SetActive(true);

        _centerDropdown.ClearOptions();

        for (int i = 0; i < result.Data.Length; i++)
        {
            CenterDTO center = result.Data[i];

            _centerDropdown.options.Add(new OptionData(center.Name));

            _centerIds[i] = center.Id;
        }

        _centerDropdown.RefreshShownValue();
    }

    private async UniTaskVoid LoadCenters()
    {
        _contentTransform.sizeDelta = new Vector2(_contentTransform.sizeDelta.x, HeightCenter);
        _overlayTransform.sizeDelta = new Vector2(_contentTransform.sizeDelta.x, HeightCenter);

        _centerContent.SetActive(true);
        _overlay.gameObject.SetActive(true);

        await LoadCentersAsync();

        _ = Tween.Alpha(_overlay, endValue: 0f, duration: 0.5f).OnComplete(_onAlphaComplete);
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

    private void OnSelectCenter()
    {
        int selectedIndex = _centerDropdown.value;
        int centerId = _centerIds[selectedIndex];

        LoadChildren(centerId).Forget();
    }

    private void OnRegisterChild()
    {
        RegisterChild.IsUpdate = false;

        _uiManager.ShowPanel(PanelEnum.RegisterChild);
    }

    private async UniTaskVoid OnUpdateChildAsync()
    {
        string childId = _childrenIds[_childrenDropdown.value];

        ChildService childService = new();

        ApiResult<ChildDataDTO> result = await childService.GetChild(childId);

        _updateChildButton.EndLoading();

        if (!result.Success)
        {
            ModalData modalData = new()
            {
                Title = "Error",
                Message = "Could not load child data. Please try again later.",
                Panel = _panel,
            };

            await _uiManager.ShowModal(modalData);

            return;
        }

        _authManager.SetCurrentChild(result.Data);

        RegisterChild.IsUpdate = true;

        _uiManager.ShowPanel(PanelEnum.RegisterChild);
    }

    private void OnUpdateChild()
    {
        OnUpdateChildAsync().Forget();
    }

    private void OnLogout()
    {
        _authManager.Logout();
    }
}
