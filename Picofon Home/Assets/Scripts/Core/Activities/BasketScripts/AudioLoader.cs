using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public enum AudioID
{
    Intro,
    Positive,
    Negative,
}

public readonly struct ActivityLabels
{
    public readonly LanguageID Language { get; init; }
    public readonly ActivitySkill Skill { get; init; }
    public readonly string Activity { get; init; }
}

public class AudioLoader
{
    private AsyncOperationHandle<AudioClip>[] _audioHandles;

    private AsyncOperationHandle<IList<AudioClip>> _introHandle;

    private AudioClip _clip;

    public async UniTask LoadAudios(string[] audioPaths, ActivityLabels labels)
    {
        _audioHandles = new AsyncOperationHandle<AudioClip>[audioPaths.Length];

        string skillLabel = labels.Skill switch
        {
            ActivitySkill.Initial => "skill-initial",
            ActivitySkill.Medial => "skill-medial",
            ActivitySkill.Final => "skill-final",
            _ => string.Empty,
        };

        string languageLabel = labels.Language switch
        {
            LanguageID.Catalan => "lang-ca",
            LanguageID.Spanish => "lang-es",
            _ => string.Empty,
        };

        IEnumerable<string> keys = new string[] { languageLabel, skillLabel, labels.Activity };

        _introHandle = Addressables.LoadAssetsAsync<AudioClip>(
            keys,
            null,
            Addressables.MergeMode.Intersection
        );

        await _introHandle.Task.AsUniTask();

        foreach (var clip in _introHandle.Result)
        {
            clip.LoadAudioData();

            await LoadAudio(clip);
        }

        string prefix = labels.Language switch
        {
            LanguageID.Catalan => "CA-",
            LanguageID.Spanish => "SP-",
            _ => string.Empty,
        };

        for (int i = 0; i < audioPaths.Length; i++)
        {
            string path = TextUtils.RemoveAccentsAndPrepend(input: audioPaths[i], prefix: prefix);

            AsyncOperationHandle<AudioClip> handle = Addressables.LoadAssetAsync<AudioClip>(path);
            await handle.Task.AsUniTask();

            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Addressables.Release(handle);
                PerformanceLog.LogError($"Failed to load audio at path: {path}");
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

        if (_introHandle.IsValid())
        {
            foreach (var clip in _introHandle.Result)
            {
                clip.UnloadAudioData();
            }

            Addressables.Release(_introHandle);
        }

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

    public void GetIntroAudios(AudioClip[] clips)
    {
        IList<AudioClip> introClips = _introHandle.Result;

        for (int i = 0; i < introClips.Count; i++)
        {
            char lastChar = introClips[i].name[^1];

            if (lastChar == 'I')
            {
                clips[0] = introClips[i];
                continue;
            }

            char secondLastChar = introClips[i].name[^2];

            int variant = 0;

            if (secondLastChar == 'N')
            {
                variant = 1;
            }

            switch (lastChar)
            {
                case 'P':
                    clips[1 + variant] = introClips[i];
                    break;
                case 'N':
                    if (secondLastChar != '-')
                    {
                        variant++;
                    }

                    clips[2 + variant] = introClips[i];
                    break;
            }
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
