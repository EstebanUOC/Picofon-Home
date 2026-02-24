using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class ProgressBar : MonoBehaviour
{
    [SerializeField]
    private RectTransform fillRect;

    private float _progress;
    private float _maxWidth;

    public void Awake()
    {
        _maxWidth = transform.GetComponent<RectTransform>().rect.width;

        Prueba().Forget();
    }

    private async UniTaskVoid Prueba()
    {
        await UniTask.WaitForSeconds(1);

        fillRect.DOSizeDelta(new Vector2(400, fillRect.sizeDelta.y), 2f).SetEase(Ease.Linear);
    }
}
