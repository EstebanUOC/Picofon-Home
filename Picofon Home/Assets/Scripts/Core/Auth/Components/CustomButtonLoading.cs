using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomButtonLoading : CustomButtonRaised
{
    [Space(15)]
    public RectTransform LoadingRect;

    public void Start()
    {
        LoadingRect
            .DORotate(new Vector3(0, 0, -360), 1f, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.Linear);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("CustomButtonLoading Clicked - Ignored during loading");
    }
}
