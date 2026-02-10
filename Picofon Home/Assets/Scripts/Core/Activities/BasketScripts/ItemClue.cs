using DG.Tweening;
using UnityEngine;

public sealed class ItemClue : MonoBehaviour
{
    [Space(15)]
    [SerializeField]
    private float _moveOffsetY = 28;

    [SerializeField]
    private float _transitionDuration = 0.2f;

    [SerializeField]
    private int _targetSize = 128;

    private GameObject _icon;
    private GameObject _text;

    private Sequence _clueSequence;

    public void Awake()
    {
        ItemView _item = GetComponent<ItemView>();

        _text = _item.Text;
        _icon = _item.Icon;

        _text.SetActive(false);

        RectTransform _imageRect = _icon.GetComponent<RectTransform>();

        Tween _moveTween = _imageRect
            .DOLocalMoveY(_moveOffsetY, _transitionDuration)
            .SetAutoKill(false)
            .Pause();

        Vector2 targetSize = Vector2.one * _targetSize;

        Tween _resizeTween = _imageRect
            .DOSizeDelta(targetSize, _transitionDuration)
            .SetAutoKill(false)
            .Pause();

        _clueSequence = DOTween.Sequence().SetAutoKill(false).Pause();

        _clueSequence.Append(_moveTween).Join(_resizeTween);

        _clueSequence.OnComplete(() => _text.SetActive(true));
    }

    public void ShowClue()
    {
        _clueSequence.Restart();
    }

    public void HideClue()
    {
        _text.SetActive(false);

        _clueSequence.PlayBackwards();
    }

    public void OnDestroy()
    {
        _clueSequence.Kill();
    }
}
