using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class AudioLoader
{
    private AudioClip[] _audioClips;
    private AudioClip _clip;

    public async UniTask LoadAudios(string[] audioPaths)
    {
        _audioClips = new AudioClip[audioPaths.Length];

        for (int i = 0; i < audioPaths.Length; i++)
        {
            // TODO: Use localized paths when localization is implemented
            string path = TextUtils.RemoveAccentsAndPrepend(audioPaths[i], "CA-");

            UniTask<AudioClip> handle = Addressables.LoadAssetAsync<AudioClip>(path).ToUniTask();

            AudioClip audio = await handle;

            audio.LoadAudioData();

            await LoadAudio(audio);

            _audioClips[i] = audio;
        }
    }

    public void UnloadAudios()
    {
        if (_audioClips == null)
            return;

        foreach (AudioClip clip in _audioClips)
        {
            if (clip != null)
                Addressables.Release(clip);
        }
    }

    public void GetAudios(int index, int quantity, AudioClip[] clips)
    {
        for (int i = 0; i < quantity; i++)
        {
            clips[i] = _audioClips[index * quantity + i];
        }
    }

    private bool IsAudioLoaded()
    {
        return _clip.loadState == AudioDataLoadState.Loaded;
    }

    private async UniTask LoadAudio(AudioClip clip)
    {
        _clip = clip;

        await UniTask.WaitUntil(IsAudioLoaded);
    }
}
