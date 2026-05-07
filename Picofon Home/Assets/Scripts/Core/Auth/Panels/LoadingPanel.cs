using UnityEngine;

public class LoadingPanel : MonoBehaviour
{
    [Space]
    [SerializeField]
    private GameObject _bootObject;

    [SerializeField]
    private GameObject _mapObject;

    [SerializeField]
    private GameObject _normal;

    BootLoading _bootLoading;

    NormalLoading _normalLoading;

    MapLoading _mapLoading;

    public void Show(LoadingEnum loading)
    {
        if (loading == LoadingEnum.Boot)
        {
            ShowBoot();
            return;
        }

        ShowNormal();
    }

    public void Hide(LoadingEnum loading)
    {
        switch (loading)
        {
            case LoadingEnum.Boot:
                _bootLoading.Hide();
                break;
            case LoadingEnum.Normal:
                _normalLoading.Hide();
                break;
        }

        if (loading == LoadingEnum.Boot)
        {
            _bootLoading.Hide();
            return;
        }

        _normalLoading.Hide();
    }

    public void ShowMapTransition()
    {
        gameObject.SetActive(true);

        _mapObject.SetActive(true);

        if (_mapLoading == null)
        {
            _mapLoading = _mapObject.GetComponent<MapLoading>();
        }

        _mapLoading.Show();
    }

    public void ContinueMapTransition(bool success)
    {
        _mapLoading.Continue(success);
    }

    private void ShowNormal()
    {
        gameObject.SetActive(true);

        _normal.SetActive(true);

        if (_normalLoading == null)
        {
            _normalLoading = _normal.GetComponent<NormalLoading>();
        }

        _normalLoading.Show();
    }

    private void ShowBoot()
    {
        gameObject.SetActive(true);

        _bootObject.SetActive(true);

        if (_bootLoading == null)
        {
            _bootLoading = _bootObject.GetComponent<BootLoading>();
        }

        _bootLoading.Show();
    }
}
