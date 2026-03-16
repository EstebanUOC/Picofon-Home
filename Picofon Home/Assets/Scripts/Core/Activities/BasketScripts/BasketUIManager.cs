using System;
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

    [SerializeField]
    private ResponsiveTransform[] _responsiveTransforms;

    [SerializeField]
    private UIResponsiveTransform[] _responsiveUITransforms;

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
        foreach (ResponsiveTransform responsiveRect in _responsiveTransforms)
        {
            Transform target = responsiveRect.Target;

            target.localPosition = responsiveRect.Position;
            target.localRotation = responsiveRect.Rotation;
            target.localScale = responsiveRect.Scale;

            if (!responsiveRect.Mirror)
                continue;

            Transform mirrorTarget = responsiveRect.TargetMirror;

            mirrorTarget.localPosition = responsiveRect.Position * new Vector2(-1f, 1f);
        }

        foreach (UIResponsiveTransform responsiveRect in _responsiveUITransforms)
        {
            RectTransform target = responsiveRect.Target;

            target.anchorMin = responsiveRect.AnchorMin;
            target.anchorMax = responsiveRect.AnchorMax;
            target.pivot = responsiveRect.Pivot;
            target.anchoredPosition = responsiveRect.Position;
            target.localRotation = responsiveRect.Rotation;
            target.localScale = responsiveRect.Scale;

            if (responsiveRect.Size != Vector2.zero)
                target.sizeDelta = responsiveRect.Size;

            if (!responsiveRect.Mirror)
                continue;

            RectTransform mirrorTarget = responsiveRect.TargetMirror;

            mirrorTarget.anchoredPosition = responsiveRect.Position * new Vector2(-1f, 1f);
        }
    }

    private void BackToMap()
    {
        SceneManager.LoadScene("MapPathScene");
        AudioManager.Instance.StopVoice();
    }
}

[Serializable]
public struct ResponsiveTransform
{
    public bool Mirror;
    public Transform Target;
    public Transform TargetMirror;
    public Vector2 Position;
    public Quaternion Rotation;
    public Vector3 Scale;
}

[Serializable]
public struct UIResponsiveTransform
{
    public bool Mirror;
    public RectTransform Target;
    public RectTransform TargetMirror;
    public Vector2 Position;
    public Vector2 AnchorMin;
    public Vector2 AnchorMax;
    public Vector2 Pivot;
    public Vector2 Size;
    public Quaternion Rotation;
    public Vector3 Scale;
}
