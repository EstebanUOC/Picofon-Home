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
    private ClueController _clueController;

    [SerializeField]
    private GameMenu _gameMenu;

    public void Prueba()
    {
        _itemManager.Prueba();
    }

    public void EnableClueButton(bool enable)
    {
        _clueController.EnableClue(enable);
    }

    public void OnEnable()
    {
        _clueController.OnClueChanged += HandleClueChanged;
        _gameMenu.OnMenuOptionSelected += HandleMenuOptionSelected;
    }

    public void SetViewContent(in ViewContentDTO content)
    {
        _clueController.Reset();

        _itemManager.SetItemsContent(in content);
    }

    public void SetAudioClips(AudioClip[] clips)
    {
        _itemManager.SetItemsAudio(clips);
    }

    public void Reset()
    {
        _clueController.Reset();
        _itemClueManager.SetClueVisibility(false);
    }

    private void BackToMap()
    {
        SceneManager.LoadScene("MapPathScene");
        AudioManager.Instance.StopVoice();
    }

    private void HandleClueChanged(bool showClue)
    {
        _itemClueManager.SetClueVisibility(showClue);
    }

    private void HandleMenuOptionSelected(GameMenuEvent menuEvent)
    {
        switch (menuEvent)
        {
            case GameMenuEvent.Exit:
                BackToMap();
                break;
        }
    }
}
