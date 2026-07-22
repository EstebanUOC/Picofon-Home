using Cysharp.Threading.Tasks;
using PrimeTween;
using TMPro;
using UnityEngine;

public class FrameManager : MonoBehaviour
{
    #region References

    [SerializeField]
    private Transform _container;

    [SerializeField]
    private Transform _leftLabel;

    [SerializeField]
    private TMP_Text _leftWordText;

    [SerializeField]
    private SpriteRenderer _leftWordIcon;

    [SerializeField]
    private Transform _rightLabel;

    [SerializeField]
    private TMP_Text _rightWordText;

    [SerializeField]
    private SpriteRenderer _rightWordIcon;

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
        _leftWordText.text = word;
    }

    public void ShowRightFrame(string word, Sprite icon)
    {
        _rightWordIcon.sprite = icon;
        _rightWordText.text = word;
    }

    public void ShowLabels()
    {
        _leftLabel.gameObject.SetActive(true);
        _rightLabel.gameObject.SetActive(true);

        Tween.EulerAngles(
            _leftLabel,
            startValue: new Vector3(-90, 0, 0),
            endValue: new Vector3(0, 0, 0),
            duration: 0.5f,
            ease: Ease.OutBack
        );

        Tween.EulerAngles(
            _rightLabel,
            startValue: new Vector3(-90, 0, 0),
            endValue: new Vector3(0, 0, 0),
            duration: 0.5f,
            ease: Ease.OutBack
        );
    }
}
