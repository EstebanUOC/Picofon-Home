using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UserChildren : Panel
{
    public UIManager UIManager;

    [SerializeField]
    private GameObject _loading;

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

    public void Start()
    {
        OnHide += () => gameObject.SetActive(false);

        CustomButtonBase selectButton = _selectButton.GetComponent<CustomButtonBase>();
        CustomButtonBase registerButton = _registerButton.GetComponent<CustomButtonBase>();

        selectButton.OnClick += OnSelectChild;
        registerButton.OnClick += OnRegisterChild;

        _logoutButton.OnClick += OnLogout;
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

        string userId = UIManager.CurrentUser.Id;

        ApiResult<ChildListItemDTO[]> result = await userService.GetUserChildren(userId, token);

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

        MapPathPayload.ChildId = childId;

        _loading.SetActive(true);

        Tween tween = Tween.EulerAngles(
            _loading.transform.GetChild(1),
            startValue: Vector3.zero,
            endValue: Vector3.forward * 360f,
            duration: 1f,
            cycles: -1
        );

        LevelDataStore instance = LevelDataStore.Instance;

        await instance.LoadPlans(childId);

        PerformanceLog.Log("Plans loaded");

        tween.Complete();

        if (instance.HasPlans())
        {
            SceneManager.LoadScene("MapPathScene");
            return;
        }

        ModalData modalData = new()
        {
            Title = "Advertència",
            Message = "Aquest nen encara no té cap pla de teràpia associat.",
        };

        await UIManager.ShowModal(modalData);

        _loading.SetActive(false);
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
