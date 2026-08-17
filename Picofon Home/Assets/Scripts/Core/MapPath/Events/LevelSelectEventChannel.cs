using Picofon.Core.MapPath;

namespace Picofon.Core.MapPath.Events
{
    using System;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Events/Level Select Event Channel")]
    public class LevelSelectEventChannel : ScriptableObject
    {
        public Action<LevelConfig, int> OnEventRaised;

        public void Raise(LevelConfig config, int index)
        {
            OnEventRaised?.Invoke(config, index);
        }
    }
}
