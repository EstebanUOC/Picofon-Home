using System.Threading;
using Cysharp.Threading.Tasks;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using static TMPro.TMP_Dropdown;

public class RegisterChild : MonoBehaviour
{
    public static bool IsUpdate { get; set; }

    [SerializeField]
    private UIManager _uiManager;

    [SerializeField]
    private AuthManager _authManager;

    [SerializeField]
    private RectTransform _contentTransform;

    [SerializeField]
    private TMP_Text _titleText;

    [SerializeField]
    private TMP_Text _subtitleText;

    [Header("Academic Information")]
    [SerializeField]
    private RectTransform _academicInfoGroup;

    [SerializeField]
    private CustomToggle _noCenterToggle;

    [SerializeField]
    private TMP_Dropdown _countryDropdown;

    [SerializeField]
    private CustomButtonLoading _countryButton;

    [Space]
    [SerializeField]
    private TMP_Dropdown _centerDropdown;

    [SerializeField]
    private TMP_Dropdown _gradeDropdown;

    [SerializeField]
    private CustomButton _academicSubmitButton;

    [Space]
    [SerializeField]
    private Image _overlayCountryImage;

    [SerializeField]
    private Image _overlayCenterImage;

    [Header("Personal Information")]
    [SerializeField]
    private RectTransform _personalInfoGroup;

    [SerializeField]
    private PersonalContent _personalContent;

    [Header("Communitacion Information")]
    [SerializeField]
    private RectTransform _communicationInfoGroup;

    [SerializeField]
    private CommunicationContent _communicationContent;

    private RectTransform _panel;

    private AcademicService _academicService;

    private CreateChildDTO _childData;

    private byte[] _countryIndexes;
    private byte[] _centerIndexes;
    private byte[] _gradeIndexes;

    public void Start()
    {
        _panel = GetComponent<RectTransform>();

        _countryButton.OnClick += HandleCountrySelect;

        _academicSubmitButton.OnClick += HandleCenterSelect;

        _personalContent.OnValidationSuccess += HandlePersonalInfoSubmit;
        _communicationContent.OnValidationSuccess += HandleSubmit;
    }

    public void OnEnable()
    {
        LoadCountries().Forget();

        _overlayCountryImage.gameObject.SetActive(false);
        _overlayCenterImage.gameObject.SetActive(true);

        _contentTransform.sizeDelta = _academicInfoGroup.sizeDelta;

        _academicInfoGroup.gameObject.SetActive(true);
        _personalInfoGroup.gameObject.SetActive(false);
        _communicationInfoGroup.gameObject.SetActive(false);

        if (IsUpdate)
        {
            _titleText.SetText(
                LocalizationSettings.StringDatabase.GetLocalizedString("UI", "REG-TITLE-UP")
            );

            _subtitleText.SetText(
                LocalizationSettings.StringDatabase.GetLocalizedString("UI", "REG-SUB-UP")
            );

            _personalContent.SetUpdateData(_authManager.CurrentChild);

            return;
        }

        _titleText.SetText(
            LocalizationSettings.StringDatabase.GetLocalizedString("UI", "REG-TITLE")
        );

        _subtitleText.SetText(
            LocalizationSettings.StringDatabase.GetLocalizedString("UI", "REG-SUB")
        );
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

        _countryIndexes = new byte[result.Data.Records.Length];

        int i = 0;
        foreach (CountryDTO country in result.Data.Records)
        {
            OptionData option = new(country.Name);
            _countryDropdown.options.Add(option);

            _countryIndexes[i] = country.Id;

            i++;
        }

        _countryDropdown.RefreshShownValue();
    }

    private async UniTaskVoid LoadCenters()
    {
        int countryId = _countryIndexes[_countryDropdown.value];

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

        _centerIndexes = new byte[centerResult.Data.Records.Length];
        _gradeIndexes = new byte[gradeResult.Data.Length];

        int i = 0;
        foreach (CenterRegisterDTO center in centerResult.Data.Records)
        {
            OptionData option = new(center.Name);
            _centerDropdown.options.Add(option);

            _centerIndexes[i] = center.Id;

            i++;
        }

        i = 0;
        foreach (GradeDTO grade in gradeResult.Data)
        {
            OptionData option = new(grade.LocalName);
            _gradeDropdown.options.Add(option);

            _gradeIndexes[i] = grade.Id;

            i++;
        }

        _centerDropdown.RefreshShownValue();
        _gradeDropdown.RefreshShownValue();

        _overlayCountryImage.gameObject.SetActive(true);

        _countryButton.EndLoading();
        _countryButton.Interactable = false;

        _ = Sequence
            .Create()
            .Group(Tween.Alpha(_overlayCountryImage, startValue: 0, endValue: 0.7f, duration: 0.3f))
            .Group(Tween.Alpha(_overlayCenterImage, startValue: 0.7f, endValue: 0f, duration: 0.3f))
            .OnComplete(
                target: _overlayCenterImage.gameObject,
                target =>
                {
                    target.SetActive(false);
                }
            );
    }

    private void HandleCountrySelect()
    {
        LoadCenters().Forget();
    }

    private void HandleCenterSelect()
    {
        int centerId = _centerIndexes[_centerDropdown.value];
        int gradeId = _gradeIndexes[_gradeDropdown.value];

        _childData = new CreateChildDTO()
        {
            Grade = gradeId,
            Relationship = UserRole.Parent,
            UserId = _authManager.CurrentUser.Id,
        };

        if (!_noCenterToggle.IsSelected)
        {
            _childData.CenterId = centerId;
        }

        if (_countryDropdown.options[_countryDropdown.value].text == "España")
        {
            _personalContent.SetIsSpain(true);
        }

        Tween.UISizeDelta(
            _contentTransform,
            endValue: _personalInfoGroup.sizeDelta,
            duration: 0.2f
        );

        _academicInfoGroup.gameObject.SetActive(false);
        _personalInfoGroup.gameObject.SetActive(true);
    }

    private void HandlePersonalInfoSubmit()
    {
        _personalContent.SetData(_childData);

        Tween.UISizeDelta(
            _contentTransform,
            endValue: _communicationInfoGroup.sizeDelta,
            duration: 0.2f
        );

        _personalInfoGroup.gameObject.SetActive(false);
        _communicationInfoGroup.gameObject.SetActive(true);
    }

    private async UniTaskVoid HandleSubmitAsync()
    {
        UserService userService = _authManager.UserService;
        CancellationToken token = this.GetCancellationTokenOnDestroy();

        ApiResult result;

        if (IsUpdate)
        {
            ChildService childService = new();
            result = await childService.UpdateChild(_childData.Id, _childData, token);
        }
        else
        {
            result = await userService.RegisterChild(_childData, token);
        }

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
            Title = "Detalls del registre",
            Message = message,
            Panel = _panel,
        };

        _communicationContent.EndLoading();

        await _uiManager.ShowModal(modalData);

        if (result.Success)
        {
            _uiManager.ShowPanel(PanelEnum.Children);
        }
    }

    private void HandleSubmit()
    {
        _communicationContent.SetData(_childData);

        // PerformanceLog.Log(
        //     $"Final Child Data: Center ID: {_childData.CenterId}, Grade ID: {_childData.Grade}, Name: {_childData.FirstName}, Last Name: {_childData.LastName}, Birth Date: {_childData.BirthDate}, Language Preference: {_childData.LanguagePreference}, Disorder: {_childData.Disorder}"
        // );

        HandleSubmitAsync().Forget();
    }

    private void HandleReturn()
    {
        _uiManager.ShowPanel(PanelEnum.Children);
    }
}
