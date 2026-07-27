using UnityEngine;
using UnityEngine.SceneManagement;

public class CrossRiverUIManager : MonoBehaviour
{
    #region References

    [SerializeField]
    private GameMenu _gameMenu;

    [SerializeField]
    private FrameManager _frameManager;

    #endregion

    // Variables

    private AudioClip _introAudio;

    private AudioClip[] _wordClips;

    private bool _labelsVisible;

    public void Awake()
    {
        _gameMenu.OnMenuOptionSelected += HandleMenuOptionSelected;
        _frameManager.OnFrameClicked += HandleFrameClicked;
    }

    public void SetIntroAudio(AudioClip clip)
    {
        _introAudio = clip;
    }

    public void SetWordClips(AudioClip[] clips)
    {
        _wordClips = clips;
    }

    private void HandleFrameClicked(int wordIndex)
    {
        if (_wordClips == null || wordIndex >= _wordClips.Length || _wordClips[wordIndex] == null)
            return;

        AudioManager.Instance.StopUI();
        AudioManager.Instance.PlayUI(_wordClips[wordIndex], 1.5f);
    }

    private void HandleMenuOptionSelected(GameMenuEvent menuEvent)
    {
        switch (menuEvent)
        {
            case GameMenuEvent.Clue:
                ToggleClue();
                break;

            case GameMenuEvent.Exit:
                BackToMap();
                break;

            case GameMenuEvent.Replay:
                AudioManager.Instance.PlayVoice(_introAudio);
                break;
        }
    }

    private void ToggleClue()
    {
        _labelsVisible = !_labelsVisible;

        if (_labelsVisible)
            _frameManager.ShowLabels();
        else
            _frameManager.HideLabels();
    }

    private void BackToMap()
    {
        SceneManager.LoadScene("MapPathScene");
        AudioManager.Instance.StopVoice();
    }
}
