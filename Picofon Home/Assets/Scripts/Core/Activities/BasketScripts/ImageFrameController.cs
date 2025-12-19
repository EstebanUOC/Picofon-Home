using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ImageFrameController : MonoBehaviour
{
    [Space(15)]
    public GameObject Image;
    public TextMeshProUGUI Text;

    private Image _imageComponent;
    private RectTransform _imageRect;

    private const float _TextShowYPosition = 28;
    private const float _TextShowDuration = 0.2f;

    private float _originalYPosition;

    private Tween _moveTween;

    public void Awake()
    {
        Text.enabled = false;
        _imageComponent = Image.GetComponent<Image>();
        _imageRect = Image.GetComponent<RectTransform>();

        _originalYPosition = _imageRect.localPosition.y;

        _moveTween = _imageRect
            .DOLocalMoveY(_TextShowYPosition, _TextShowDuration)
            .SetAutoKill(false)
            .Pause();

        _moveTween.OnComplete(() => Text.enabled = true);
    }

    public void UpdateFrame(Sprite sprite, string word)
    {
        _imageComponent.transform.localPosition = Vector3.up * _originalYPosition;
        _imageComponent.sprite = sprite;

        Text.enabled = false;
        Text.text = word;
    }

    public void ShowClue()
    {
        _moveTween.Restart();
    }

    public void OnDestroy()
    {
        _moveTween.Kill();
    }
}
