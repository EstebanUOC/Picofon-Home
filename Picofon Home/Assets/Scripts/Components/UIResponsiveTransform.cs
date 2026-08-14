using System;
using UnityEngine;

namespace Picofon.Components
{
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
}
