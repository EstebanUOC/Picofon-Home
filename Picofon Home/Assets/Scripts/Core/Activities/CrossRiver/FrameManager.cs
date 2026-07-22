using PrimeTween;
using TMPro;
using UnityEngine;

public class FrameManager : MonoBehaviour
{
    #region References

    [SerializeField]
    private Transform _container;

    [SerializeField]
    private TMP_Text _leftWordText;

    [SerializeField]
    private SpriteRenderer _leftWordIcon;

    [SerializeField]
    private TMP_Text _rightWordText;

    [SerializeField]
    private SpriteRenderer _rightWordIcon;

    [SerializeField]
    private Fade _fade;

    #endregion

    public void HideFrames()
    {
        float duration = 0.5f;

        Tween.LocalPositionY(_container, endValue: 5, duration, ease: Ease.InBack);
        Tween.LocalPositionX(_container, endValue: -9.5f, duration: duration);
    }

    public void ShowFrames()
    {
        _container.localPosition = new Vector3(0, 5, 0);

        float duration = 0.5f;

        Tween.LocalPositionY(_container, endValue: 0, duration, ease: Ease.OutBack);
    }

    public void ShowLeftFrame(string word, Sprite icon)
    {
        _leftWordIcon.sprite = icon;
    }

    public void ShowRightFrame(string word, Sprite icon)
    {
        _rightWordIcon.sprite = icon;
    }
}
