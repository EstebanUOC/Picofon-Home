using System;
using PrimeTween;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomButtonLoading : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private RectTransform _backgroundRect;

    [SerializeField]
    private RectTransform _contentRect;

    [SerializeField]
    private RectTransform _loadingRect;

    [Space]
    [SerializeField]
    private GameObject _contentInfo;

    [SerializeField]
    private GameObject _contentLoading;

    [Space]
    [SerializeField]
    private GameObject _inactiveOverlay;

    public event Action OnClick;

    public bool Interactable
    {
        get => _interactable;
        set => SetInteractable(value);
    }

    private RectTransform _overlayRect;
    private Tween _loadingTween;

    private float _defaultContentY;
    private bool _interactable = true;
    private bool _isLoading = false;

    public void Awake()
    {
        _defaultContentY = _contentRect.anchoredPosition.y;

        Color32 inactiveColor = _backgroundRect.gameObject.GetComponent<Image>().color;
        inactiveColor.a = 130;

        Image overlayImage = _inactiveOverlay.GetComponent<Image>();
        overlayImage.color = inactiveColor;

        _overlayRect = _inactiveOverlay.GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!_interactable)
            return;

        PerformanceLog.Log("<DEBUG> Button pressed, starting loading animation.");

        Vector2 contentPos = (_defaultContentY - 11f) * Vector2.up;
        Vector2 bgSize = 11f * Vector2.down;
        Vector2 bgMoveY = 5.5f * Vector2.down;

        _contentRect.anchoredPosition = contentPos;
        _backgroundRect.sizeDelta = bgSize;
        _backgroundRect.anchoredPosition = bgMoveY;

        _overlayRect.sizeDelta = bgSize;
        _overlayRect.anchoredPosition = bgMoveY;

        _contentInfo.SetActive(false);
        _contentLoading.SetActive(true);

        AnimateLoading(true);

        Interactable = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_isLoading)
            return;

        Vector2 contentPos = _contentRect.anchoredPosition;
        contentPos.y = _defaultContentY;

        _contentRect.anchoredPosition = contentPos;
        _backgroundRect.sizeDelta = Vector2.zero;
        _backgroundRect.anchoredPosition = Vector2.zero;

        _overlayRect.sizeDelta = Vector2.zero;
        _overlayRect.anchoredPosition = Vector2.zero;

        _isLoading = true;

        OnClick?.Invoke();
    }

    public void EndLoading()
    {
        _contentLoading.SetActive(false);
        _contentInfo.SetActive(true);

        AnimateLoading(false);

        Interactable = true;
        _isLoading = false;
    }

    private void SetInteractable(bool value)
    {
        if (_interactable == value)
            return;

        _interactable = value;

        _inactiveOverlay.SetActive(!_interactable);
    }

    private void AnimateLoading(bool isLoading)
    {
        if (!isLoading)
        {
            _loadingTween.Complete();
            return;
        }

        Vector3 targetRotation = new(0, 0, -360);

        _loadingTween = Tween.EulerAngles(
            _loadingRect,
            startValue: Vector3.zero,
            endValue: targetRotation,
            duration: 1f,
            cycles: -1
        );
    }
}
