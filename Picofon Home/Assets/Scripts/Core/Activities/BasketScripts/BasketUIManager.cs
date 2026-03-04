using BasketResponses;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BasketUIManager : MonoBehaviour
{
    [Space(15)]
    [SerializeField]
    private ItemManager _itemManager;

    [SerializeField]
    private ItemClueManager _itemClueManager;

    [SerializeField]
    private GameMenu _gameMenu;

    private AudioClip _introAudio;

    public void Awake()
    {
        _gameMenu.OnMenuOptionSelected += HandleMenuOptionSelected;
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

    private void BackToMap()
    {
        SceneManager.LoadScene("MapPathScene");
        AudioManager.Instance.StopVoice();
    }
}
