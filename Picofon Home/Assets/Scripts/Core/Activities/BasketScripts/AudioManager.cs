using Cysharp.Threading.Tasks;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Space(15)]
    [SerializeField]
    private AudioSource _sfxSource;

    [SerializeField]
    private AudioSource _uiSource;

    [SerializeField]
    private AudioSource _voiceSource;

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayVoice(AudioClip clip, float volume = 1f)
    {
        _voiceSource.Stop();
        _voiceSource.PlayOneShot(clip, volume);
    }

    public UniTask WaitVoiceToEnd()
    {
        return UniTask.WaitWhile(() => _voiceSource.isPlaying);
    }

    public void StopVoice()
    {
        _voiceSource.Stop();
    }

    public void StopUI()
    {
        _uiSource.Stop();
    }

    public void PlayUI(AudioClip clip, float volume = 1f)
    {
        _uiSource.PlayOneShot(clip, volume);
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        _sfxSource.PlayOneShot(clip, volume);
    }
}
