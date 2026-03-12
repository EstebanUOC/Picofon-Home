using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField]
    private GameObject _starPrefab;

    [SerializeField]
    private Transform _starContainer;

    [SerializeField]
    private RectTransform _rocket;

    [SerializeField]
    private RectTransform _fill;

    private readonly Color32 _starColor = new(130, 208, 210, 255);

    const float referenceWidth = 1920f;
    const float referenceHeight = 1080f;
    const float baseOrthoSize = 5f;

    public void Awake()
    {
        float refAspect = referenceWidth / referenceHeight;
        float curAspect = (float)Screen.width / Screen.height;

        Debug.Log($"Reference Aspect Ratio: {refAspect}, Current Aspect Ratio: {curAspect}");
        Debug.Log($"Orto: {baseOrthoSize * (refAspect / curAspect)}");
    }

    public void Initialize(int parts)
    {
        if (parts <= 0)
            return;

        int starCount = _starContainer.childCount;

        if (starCount < parts)
        {
            for (int i = starCount; i < parts; i++)
            {
                Instantiate(_starPrefab, _starContainer);
            }
        }

        if (starCount > parts)
        {
            for (int i = parts; i < starCount; i++)
            {
                _starContainer.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    public void SetProgress(int progress)
    {
        RectTransform star = _starContainer.GetChild(progress - 1) as RectTransform;
        Image image = star.GetComponent<Image>();

        Vector2 size = _fill.sizeDelta;
        Vector2 position = Vector2.right * star.anchoredPosition.x;

        size.x = star.anchoredPosition.x;

        Sequence
            .Create()
            .Group(Tween.UIAnchoredPosition(_rocket, endValue: position, duration: 0.5f))
            .Group(Tween.UISizeDelta(_fill, endValue: size, duration: 0.5f))
            .Chain(Tween.Color(image, endValue: _starColor, duration: 0.3f, ease: Ease.OutBack));
    }
}
