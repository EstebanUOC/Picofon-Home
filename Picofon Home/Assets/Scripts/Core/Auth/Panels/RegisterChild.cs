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

    [SerializeField]
    private RectTransform _contentTransform;

    [Header("Academic Information")]
    [SerializeField]
    private RectTransform _academicInfoGroup;

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

        _overlayTransform.sizeDelta = _centerTransform.sizeDelta;
        _overlayTransform.anchoredPosition = _centerTransform.anchoredPosition;

        _contentTransform.sizeDelta = _academicInfoGroup.sizeDelta;

        _academicInfoGroup.gameObject.SetActive(true);
        _personalInfoGroup.gameObject.SetActive(false);
        _communicationInfoGroup.gameObject.SetActive(false);
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
        int centerId = _centerIndexes[_centerDropdown.value];
        int gradeId = _gradeIndexes[_gradeDropdown.value];

        _childData = new CreateChildDTO()
        {
            CenterId = centerId,
            Grade = gradeId,
            Relationship = UserRole.Parent,
            UserId = _authManager.CurrentUser.Id,
        };

        if (_countryDropdown.options[_countryDropdown.value].text == "España")
        {
            _personalContent.SetIsSpain(true);
        }

        _contentTransform.sizeDelta = _personalInfoGroup.sizeDelta;

        _academicInfoGroup.gameObject.SetActive(false);
        _personalInfoGroup.gameObject.SetActive(true);
    }

    private void HandlePersonalInfoSubmit()
    {
        _personalContent.SetData(_childData);

        _contentTransform.sizeDelta = _communicationInfoGroup.sizeDelta;

        _personalInfoGroup.gameObject.SetActive(false);
        _communicationInfoGroup.gameObject.SetActive(true);
    }

    private async UniTaskVoid HandleSubmitAsync()
    {
        UserService userService = _authManager.UserService;
        CancellationToken token = this.GetCancellationTokenOnDestroy();

        ApiResult result = await userService.RegisterChild(_childData, token);

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
