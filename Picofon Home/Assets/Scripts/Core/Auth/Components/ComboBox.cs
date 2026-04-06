using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ComboBox : MonoBehaviour, IPointerClickHandler
{
    private const string SpanishCode = "Espanyol";
    private const string CatalanCode = "Català";

    [SerializeField]
    private RectTransform _arrow;

    [SerializeField]
    private RectTransform _template;

    [Space]
    [SerializeField]
    private Image _flag;

    [SerializeField]
    private TMP_Text _languageName;

    [Space]
    [SerializeField]
    private ComboItem[] _options;

    private bool isOpen = false;

    private LanguageData _selectedLanguage;

    public void Awake()
    {
        GenericEventChannel<LanguageData> channel = new();

        foreach (var option in _options)
            option.EventChannel = channel;

        _selectedLanguage = _options[1].EventData;

        channel.OnRaised += HandleOptionSelected;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        isOpen = !isOpen;

        AnimatedOpenClose();
    }

    public LanguageCode GetSelectedLanguage()
    {
        return _selectedLanguage.Code;
    }

    private void HandleOptionSelected(LanguageData data)
    {
        isOpen = false;
        _selectedLanguage = data;

        string name = data.Code switch
        {
            LanguageCode.ES => SpanishCode,
            LanguageCode.CA => CatalanCode,
            _ => CatalanCode,
        };

        _flag.sprite = data.Flag;
        _languageName.text = name;

        AnimatedOpenClose();
    }

    private void AnimatedOpenClose()
    {
        Vector3 targetRotation = new(0, 0, 90);
        float targetY = -60f;
        float scaleY = 0;

        if (isOpen)
        {
            _template.gameObject.SetActive(true);
            _template.localScale = new Vector3(1, 0, 1);

            targetRotation = _arrow.localEulerAngles + new Vector3(0, 0, 180);
            targetY = -140f;
            scaleY = 1;
        }

        Tween.EulerAngles(
            target: _arrow,
            startValue: _arrow.localEulerAngles,
            endValue: targetRotation,
            duration: 0.5f,
            ease: Ease.OutBack
        );

        Sequence
            .Create()
            .Group(
                Tween.UIAnchoredPositionY(
                    target: _template,
                    endValue: targetY,
                    duration: 0.5f,
                    ease: Ease.OutBack
                )
            )
            .Group(
                Tween.ScaleY(
                    target: _template,
                    endValue: scaleY,
                    duration: 0.5f,
                    ease: Ease.OutBack
                )
            )
            .OnComplete(
                target: _template,
                target =>
                {
                    if (!isOpen)
                    {
                        target.gameObject.SetActive(false);
                    }
                }
            );
    }
}
