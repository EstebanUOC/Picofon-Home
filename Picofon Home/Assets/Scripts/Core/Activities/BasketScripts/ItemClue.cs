using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public sealed class ItemClue : MonoBehaviour
{
    [Space(15)]
    [SerializeField]
    private float _moveOffsetY = 28;

    [SerializeField]
    private float _moveDuration = 0.2f;

    private GameObject _image;
    private GameObject _text;

    private Tween _moveTween;

    public void Awake()
    {
        WordItemView _item = GetComponent<WordItemView>();

        _text = _item.Text;
        _image = _item.Icon;

        _text.SetActive(false);

        Image _imageComponent = _image.GetComponent<Image>();
        RectTransform _imageRect = _image.GetComponent<RectTransform>();

        _moveTween = _imageRect
            .DOLocalMoveY(_moveOffsetY, _moveDuration)
            .SetAutoKill(false)
            .Pause();

        _moveTween.OnComplete(() => _text.SetActive(true));
    }

    public void ShowClue()
    {
        _moveTween.Restart();
    }

    public void HideClue()
    {
        _text.SetActive(false);

        _moveTween.PlayBackwards();
    }

    public void OnDestroy()
    {
        _moveTween.Kill();
    }
}
