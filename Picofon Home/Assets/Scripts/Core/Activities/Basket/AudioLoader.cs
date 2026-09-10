using Picofon.Core.MapPath;
using Picofon.Utils;

namespace Picofon.Activities.Basket
{
    using System;
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

    public enum MechanicID
    {
        CrossRiver,
        Basket,
        Segmentation,
    }

    public enum SegmentationAudioID
    {
        Finger,
    }

    public readonly struct ActivityLabels
    {
        public readonly MechanicID Mechanic { get; init; }

        public readonly LanguageID Language { get; init; }

        public readonly ActivitySkill Skill { get; init; }

        public readonly string Activity { get; init; }
    }

    public class AudioLoader
    {
        private AsyncOperationHandle<AudioClip>[] _audioHandles;

        private AsyncOperationHandle<AudioClip> _introHandle;

        private AsyncOperationHandle<IList<AudioClip>> _feedbackHandle;

        private AsyncOperationHandle<IList<AudioClip>> _segmentationSoundsHandle;

        private AudioClip _clip;

        public async UniTask LoadAudios(string[] audioPaths, ActivityLabels labels)
        {
            if (labels.Mechanic == MechanicID.Segmentation)
            {
                await LoadAudiosSegmentation(audioPaths, labels);
                return;
            }

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

            string mechanicLabel = labels.Mechanic switch
            {
                MechanicID.CrossRiver => "mecha-cross",
                MechanicID.Basket => "mecha-basket",
                MechanicID.Segmentation => "mecha-segmentation",
                _ => string.Empty,
            };

            string[] keys = new string[]
            {
                languageLabel,
                skillLabel,
                labels.Activity,
                mechanicLabel,
            };

            IEnumerable<string> introEnumerable = keys;

            var introHandle = Addressables.LoadResourceLocationsAsync(
                introEnumerable,
                Addressables.MergeMode.Intersection
            );

            await introHandle.Task.AsUniTask();

            _introHandle = Addressables.LoadAssetAsync<AudioClip>(introHandle.Result[0]);

            await _introHandle.Task.AsUniTask();

            keys[^1] = "feedback";
            IEnumerable<string> feedbackEnumerable = keys;

            _feedbackHandle = Addressables.LoadAssetsAsync<AudioClip>(
                feedbackEnumerable,
                null,
                Addressables.MergeMode.Intersection
            );

            await _feedbackHandle.Task.AsUniTask();

            foreach (var clip in _feedbackHandle.Result)
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

            // Load the image audio clips for the activity

            for (int i = 0; i < audioPaths.Length; i++)
            {
                string path = TextUtils.RemoveAccentsAndPrepend(
                    input: audioPaths[i],
                    prefix: prefix
                );

                AsyncOperationHandle<AudioClip> handle = Addressables.LoadAssetAsync<AudioClip>(
                    path
                );
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

        private async UniTask LoadAudiosSegmentation(string[] audioPaths, ActivityLabels labels)
        {
            _audioHandles = new AsyncOperationHandle<AudioClip>[audioPaths.Length];

            string mechanicLabel = "mecha-segmentation";

            string languageLabel = labels.Language switch
            {
                LanguageID.Catalan => "lang-ca",
                LanguageID.Spanish => "lang-es",
                _ => string.Empty,
            };

            string[] introKeys = new[] { mechanicLabel, languageLabel, "intro" };

            IEnumerable<string> introEnumerable = introKeys;

            var introHandle = Addressables.LoadResourceLocationsAsync(
                introEnumerable,
                Addressables.MergeMode.Intersection
            );

            await introHandle.Task.AsUniTask();

            _introHandle = Addressables.LoadAssetAsync<AudioClip>(introHandle.Result[0]);

            await _introHandle.Task.AsUniTask();

            string[] soundKeys = new[] { mechanicLabel, languageLabel };

            IEnumerable<string> soundEnumerable = soundKeys;

            _segmentationSoundsHandle = Addressables.LoadAssetsAsync<AudioClip>(
                soundEnumerable,
                null,
                Addressables.MergeMode.Intersection
            );

            await _segmentationSoundsHandle.Task.AsUniTask();

            string introName = _introHandle.Result.name;

            foreach (var clip in _segmentationSoundsHandle.Result)
            {
                if (clip.name == introName)
                    continue;

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
                string path = TextUtils.RemoveAccentsAndPrepend(
                    input: audioPaths[i],
                    prefix: prefix
                );

                AsyncOperationHandle<AudioClip> handle = Addressables.LoadAssetAsync<AudioClip>(
                    path
                );

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
            if (_introHandle.IsValid())
            {
                _introHandle.Result.UnloadAudioData();
                Addressables.Release(_introHandle);
            }

            if (_feedbackHandle.IsValid())
            {
                foreach (var clip in _feedbackHandle.Result)
                {
                    clip.UnloadAudioData();
                }

                Addressables.Release(_feedbackHandle);
            }

            if (_segmentationSoundsHandle.IsValid())
            {
                foreach (var clip in _segmentationSoundsHandle.Result)
                {
                    clip.UnloadAudioData();
                }

                Addressables.Release(_segmentationSoundsHandle);
            }

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

        public void GetAudios(int index, int count, AudioClip[] clips)
        {
            for (int i = 0; i < count; i++)
            {
                var handle = _audioHandles[index * count + i];

                if (!handle.IsValid())
                {
                    continue;
                }

                clips[i] = _audioHandles[index * count + i].Result;
            }
        }

        public AudioClip GetAudio(int index)
        {
            var handle = _audioHandles[index];

            if (!handle.IsValid())
            {
                return null;
            }

            return _audioHandles[index].Result;
        }

        public AudioClip GetSegmentationClips(SegmentationAudioID audioID, int syllabesCount)
        {
            char charToSearch = audioID switch
            {
                SegmentationAudioID.Finger => 'S',
                _ => ' ',
            };

            PerformanceLog.Log(
                "Segmentation sound handle count: " + _segmentationSoundsHandle.Result.Count
            );

            for (int i = 0; i < _segmentationSoundsHandle.Result.Count; i++)
            {
                AudioClip clip = _segmentationSoundsHandle.Result[i];

                PerformanceLog.Log(
                    $"Clip name: {clip.name}, Last char: {clip.name[^1]}, Second last char: {clip.name[^2]}"
                );

                if (clip.name[^2] == charToSearch && (clip.name[^1] - '0') == syllabesCount)
                {
                    return clip;
                }
            }

            return null;
        }

        public void GetFeedbackAudios(AudioClip[] clips, MechanicID mechanic = MechanicID.Basket)
        {
            IList<AudioClip> feedbackClips;

            if (mechanic == MechanicID.Segmentation)
            {
                feedbackClips = _segmentationSoundsHandle.Result;
            }
            else
            {
                feedbackClips = _feedbackHandle.Result;
            }

            clips[0] = _introHandle.Result;

            for (int i = 0; i < feedbackClips.Count; i++)
            {
                char lastChar = feedbackClips[i].name[^1];

                char secondLastChar = feedbackClips[i].name[^2];

                int variant = 0;

                if (secondLastChar == 'N')
                {
                    variant = 1;
                }

                switch (lastChar)
                {
                    case 'P':
                        clips[1 + variant] = feedbackClips[i];
                        break;
                    case 'N':
                        if (secondLastChar != '-')
                        {
                            variant++;
                        }

                        clips[2 + variant] = feedbackClips[i];
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
}
