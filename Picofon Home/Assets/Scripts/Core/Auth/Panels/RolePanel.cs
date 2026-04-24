using System.Threading;
using Cysharp.Threading.Tasks;
using PrimeTween;
using UnityEngine;

public class RolePanel : Panel
{
    [SerializeField]
    private UIManager _uiManager;

    [SerializeField]
    private AuthManager _authManager;

    [Space]
    [SerializeField]
    private RoleCard _therapistCard;

    [SerializeField]
    private RoleCard _parentCard;

    [SerializeField]
    private GameObject _loading;

    [SerializeField]
    private RectTransform _loadingIcon;

    public void Start()
    {
        GenericEventChannel<UserRole> eventChannel = new();

        eventChannel.OnRaised += OnRoleSelected;

        _therapistCard.EventChannel = eventChannel;
        _parentCard.EventChannel = eventChannel;

        OnHide += () => gameObject.SetActive(false);
    }

    private void OnRoleSelected(UserRole roleType)
    {
        OnRoleSelectedAsync(roleType).Forget();
    }

    private async UniTaskVoid OnRoleSelectedAsync(UserRole roleType)
    {
        _loading.SetActive(true);
        Tween rotation = Tween.EulerAngles(
            _loadingIcon,
            startValue: Vector3.zero,
            endValue: Vector3.forward * 360,
            duration: 1,
            Ease.OutCubic,
            cycles: -1
        );

        CancellationToken token = this.GetCancellationTokenOnDestroy();

        ApiResult result = await _authManager.UserService.UpdateUserRole(
            _authManager.CurrentUser.Id,
            roleType,
            token
        );

        rotation.Complete();

        ModalData data;

        if (!result.Success)
        {
            data = new() { Title = "Error", Message = result.Message };
            await _uiManager.ShowModal(data);
            return;
        }

        data = new() { Title = "Success", Message = "Your role has been updated." };

        await _uiManager.ShowModal(data);

        _uiManager.ShowUserChildren();
    }
}
