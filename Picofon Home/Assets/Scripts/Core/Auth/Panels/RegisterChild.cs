using System.Threading;
using Cysharp.Threading.Tasks;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static TMPro.TMP_Dropdown;

public class RegisterChild : MonoBehaviour
{
    [SerializeField]
    private UIManager _uiManager;

    [SerializeField]
    private AuthManager _authManager;

    [Header("Academic Information")]
    [SerializeField]
    private RectTransform _countryTransform;

    [SerializeField]
    private TMP_Dropdown _countryDropdown;

    [SerializeField]
    private CustomButtonLoading _countryButton;

    [Space(5)]
    [SerializeField]
    private RectTransform _centerTransform;

    [SerializeField]
    private TMP_Dropdown _centerDropdown;

    [SerializeField]
    private TMP_Dropdown _gradeDropdown;

    [SerializeField]
    private CustomButton _academicSubmitButton;

    [Space]
    [SerializeField]
    private Image _overlayImage;

    [SerializeField]
    private RectTransform _overlayTransform;

    private RectTransform _panel;

    private AcademicService _academicService;

    private CreateChildDTO _childData;

    private int[] _indexes;

    public void Start()
    {
        _panel = GetComponent<RectTransform>();

        _countryButton.OnClick += HandleCountrySelect;
    }

    public void OnEnable()
    {
        LoadCountries().Forget();
    }

    private async UniTaskVoid LoadCountries()
    {
        _academicService = new();

        ApiResult<CountriesData> result = await _academicService.GetCountries();

        if (!result.Success)
        {
            PerformanceLog.Log($"Failed to fetch countries: {result.Message}");
            return;
        }

        _indexes = new int[result.Data.Records.Length];

        int i = 0;
        foreach (CountryDTO country in result.Data.Records)
        {
            OptionData option = new(country.Name);
            _countryDropdown.options.Add(option);

            _indexes[i] = country.Id;

            i++;
        }

        _countryDropdown.RefreshShownValue();
    }

    private async UniTaskVoid LoadCenters()
    {
        int countryId = _indexes[_countryDropdown.value];

        ApiResult<CentersData> centerResult = await _academicService.GetCenters(countryId);

        if (!centerResult.Success)
        {
            PerformanceLog.Log($"Failed to fetch centers: {centerResult.Message}");
            return;
        }

        ApiResult<GradeDTO[]> gradeResult = await _academicService.GetGrades(countryId);

        if (!gradeResult.Success)
        {
            PerformanceLog.Log($"Failed to fetch grades: {gradeResult.Message}");
            return;
        }

        foreach (CenterRegisterDTO center in centerResult.Data.Records)
        {
            OptionData option = new(center.Name);
            _centerDropdown.options.Add(option);
        }

        foreach (GradeDTO grade in gradeResult.Data)
        {
            OptionData option = new(grade.LocalName);
            _gradeDropdown.options.Add(option);
        }

        _centerDropdown.RefreshShownValue();
        _gradeDropdown.RefreshShownValue();

        _countryButton.EndLoading();
        _countryButton.Interactable = false;

        _ = Sequence
            .Create()
            .Group(Tween.Alpha(_overlayImage, endValue: 0f, duration: 0.3f))
            .ChainCallback(
                target: _overlayTransform,
                target =>
                {
                    target.sizeDelta = _countryTransform.sizeDelta;
                    target.anchoredPosition = _countryTransform.anchoredPosition;
                }
            )
            .Chain(Tween.Alpha(_overlayImage, endValue: 0.7f, duration: 0.3f));
    }

    private void HandleCountrySelect()
    {
        LoadCenters().Forget();
    }

    private void HandleCenterSelect()
    {
        _childData = new CreateChildDTO()
        {
            CenterId = _centerDropdown.value,
            Grade = _gradeDropdown.value + 1,
        };
    }

    private async UniTask HandleSubmit(CreateChildDTO data)
    {
        UserService userService = _authManager.UserService;
        CancellationToken token = this.GetCancellationTokenOnDestroy();

        ApiResult result = await userService.RegisterChild(data, token);

        string message;

        if (result.Success)
        {
            message = "Les dades del nen s'han enviat correctament.";
        }
        else
        {
            message = result.Message;
        }

        ModalData modalData = new()
        {
            Title = "Registre de nen",
            Message = message,
            Panel = _panel,
        };

        await _uiManager.ShowModal(modalData);

        if (result.Success)
        {
            _uiManager.ShowPanel(PanelEnum.Children);
        }
    }

    private void HandleReturn()
    {
        _uiManager.ShowPanel(PanelEnum.Children);
    }
}
