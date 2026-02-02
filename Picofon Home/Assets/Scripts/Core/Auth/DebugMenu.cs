using System;
using UnityEngine;

public enum DebugMenuResult : byte
{
    Map,
    Children,
    Close,
}

public class DebugMenu : MonoBehaviour
{
    public event Action<DebugMenuResult> OnClose;

    [SerializeField]
    private CustomButtonBase _mapButton;

    [SerializeField]
    private CustomButtonBase _childrenButton;

    [SerializeField]
    private CustomButtonBase _closeButton;

    public void Awake()
    {
        _mapButton.OnClick += () => OnClose.Invoke(DebugMenuResult.Map);
        _childrenButton.OnClick += () => OnClose.Invoke(DebugMenuResult.Children);
        _closeButton.OnClick += () => OnClose.Invoke(DebugMenuResult.Close);
    }
}
