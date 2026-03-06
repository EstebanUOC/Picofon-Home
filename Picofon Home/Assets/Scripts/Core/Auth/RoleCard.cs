using PrimeTween;
using UnityEngine;
using UnityEngine.EventSystems;

public class RoleCard : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private UserRole roleType;

    public GenericEventChannel<UserRole> EventChannel;

    public void OnPointerClick(PointerEventData eventData)
    {
        AnimateClick();
        EventChannel.Raise(roleType);
    }

    private void AnimateClick()
    {
        Tween scaleIn = Tween.Scale(transform, 0.9f, 0.1f, Ease.OutQuad);
        Tween scaleOut = Tween.Scale(transform, 1f, 0.1f, Ease.OutQuad);

        Sequence.Create().Group(scaleIn).Chain(scaleOut);
    }
}
