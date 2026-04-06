using UnityEngine;
using UnityEngine.EventSystems;

public class SimpleEventButton<T> : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private bool _interactable = true;

    public bool Interactable
    {
        get => _interactable;
        set => _interactable = value;
    }

    public T EventData;

    public GenericEventChannel<T> EventChannel
    {
        get => _eventChannel;
        set => _eventChannel = value;
    }

    private GenericEventChannel<T> _eventChannel;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!_interactable)
            return;

        _eventChannel?.Raise(EventData);
    }
}
