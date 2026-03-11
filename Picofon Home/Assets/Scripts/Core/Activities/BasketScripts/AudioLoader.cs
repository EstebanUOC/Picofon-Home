using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AudioLoader
{
    private AsyncOperationHandle<AudioClip>[] _audioHandles;
    private AudioClip _clip;

    public async UniTask LoadAudios(string[] audioPaths)
    {
        _audioHandles = new AsyncOperationHandle<AudioClip>[audioPaths.Length];

        for (int i = 0; i < audioPaths.Length; i++)
        {
            // TODO: Use localized paths when localization is implemented
            string path = TextUtils.RemoveAccentsAndPrepend(audioPaths[i], "CA-");

            AsyncOperationHandle<AudioClip> handle = Addressables.LoadAssetAsync<AudioClip>(path);
            await handle.Task.AsUniTask();

            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Addressables.Release(handle);
                Debug.LogError($"Failed to load audio at path: {path}");
                continue;
            }

            AudioClip audio = handle.Result;

            audio.LoadAudioData();

            await LoadAudio(audio);

            _audioHandles[i] = handle;
        }
    }

    public void UnloadAudios()
    {
        if (_audioHandles == null)
            return;

        for (int i = 0; i < _audioHandles.Length; i++)
        {
            _audioHandles[i].Result.UnloadAudioData();

            if (_audioHandles[i].IsValid())
            {
                Addressables.Release(_audioHandles[i]);
            }
        }
    }

    public void GetAudios(int index, int quantity, AudioClip[] clips)
    {
        for (int i = 0; i < quantity; i++)
        {
            var handle = _audioHandles[index * quantity + i];

            if (!handle.IsValid())
            {
                continue;
            }

            clips[i] = _audioHandles[index * quantity + i].Result;
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
