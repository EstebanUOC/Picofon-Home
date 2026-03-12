using BasketResponses;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum DeviceType : byte
{
    Any,
    Mobile,
    Tablet,
}

public class BasketUIManager : MonoBehaviour
{
    [Space]
    [SerializeField]
    private ItemManager _itemManager;

    [SerializeField]
    private ItemClueManager _itemClueManager;

    [SerializeField]
    private GameMenu _gameMenu;

    [Header("Responsive UI")]
    [SerializeField]
    private RectTransform _boardPadding;

    [SerializeField]
    private RectTransform _progressBar;

    private AudioClip _introAudio;

    private DeviceType _deviceType = DeviceType.Any;

    public void Awake()
    {
        _gameMenu.OnMenuOptionSelected += HandleMenuOptionSelected;

        if (IsTablet())
            ApplyResponsiveLayout();
    }

    public void Prueba()
    {
        _itemManager.Prueba();
    }

    public void SetViewContent(in ViewContentDTO content)
    {
        _itemManager.SetItemsContent(in content);
    }

    public void SetAudioClips(AudioClip[] clips)
    {
        _itemManager.SetItemsAudio(clips);
    }

    public void Reset()
    {
        _itemClueManager.SetClueVisibility(false);
    }

    public void SetIntroAudio(AudioClip clip)
    {
        _introAudio = clip;
    }

    public bool IsTablet()
    {
        if (_deviceType != DeviceType.Any)
            return _deviceType == DeviceType.Tablet;

        float dpi = Screen.dpi;

        bool isTablet;

        if (dpi == 0)
        {
            isTablet = Mathf.Min(Screen.width, Screen.height) >= 1200;

            _deviceType = isTablet ? DeviceType.Tablet : DeviceType.Mobile;
            return isTablet;
        }

        float widthInches = Screen.width / dpi;
        float heightInches = Screen.height / dpi;
        float diagonal = Mathf.Sqrt(widthInches * widthInches + heightInches * heightInches);

        isTablet = diagonal >= 6.5f;

        _deviceType = isTablet ? DeviceType.Tablet : DeviceType.Mobile;

        return isTablet;
    }

    private void HandleMenuOptionSelected(GameMenuEvent menuEvent)
    {
        switch (menuEvent)
        {
            case GameMenuEvent.Clue:
                _itemClueManager.ToggleClueVisibility();
                break;
            case GameMenuEvent.Exit:
                BackToMap();
                break;
            case GameMenuEvent.Replay:
                AudioManager.Instance.PlayVoice(_introAudio);
                break;
        }
    }

    private void ApplyResponsiveLayout()
    {
        _boardPadding.sizeDelta = new Vector2(_boardPadding.sizeDelta.x, 215f);

        _progressBar.anchorMin = new Vector2(0.5f, 1f);
        _progressBar.anchorMax = new Vector2(0.5f, 1f);
        _progressBar.pivot = new Vector2(0.5f, 1f);
        _progressBar.anchoredPosition = new Vector2(0f, -75f);
        _progressBar.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    private void BackToMap()
    {
        SceneManager.LoadScene("MapPathScene");
        AudioManager.Instance.StopVoice();
    }
}
