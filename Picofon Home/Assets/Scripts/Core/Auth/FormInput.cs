using UnityEngine;

public abstract class FormInput : MonoBehaviour
{
    protected static readonly Color _defaultColor = ParseColorOrDefault("#AD46FF", Color.white);
    protected static readonly Color _validColor = ParseColorOrDefault("#00C950", Color.white);
    protected static readonly Color _errorColor = ParseColorOrDefault("#FB2C36", Color.white);

    [Space(15)]
    public ChildFields Key;

    private InputChangedEventChannel _inputChangedEventChannel;

    private bool _valid = false;
    private bool _error = false;

    public bool Valid
    {
        get { return _valid; }
        set
        {
            if (_valid == value)
                return;

            _error = false;

            _valid = value;
            if (_valid)
            {
                OnValid();
                _inputChangedEventChannel.Raise(true);
            }
            else
            {
                OnReset();
                _inputChangedEventChannel.Raise(false);
            }
        }
    }

    public bool Error
    {
        get { return _error; }
        set
        {
            if (_error == value)
                return;

            _valid = false;

            _error = value;
            if (_error)
                OnError();
            else
                OnReset();
        }
    }

    public void SetInputChangedEventChannel(InputChangedEventChannel channel)
    {
        _inputChangedEventChannel = channel;
    }

    public abstract string GetData();

    protected virtual void ValidateInput() { }

    protected virtual void ValidateInput(string input) { }

    protected virtual void OnError() { }

    protected virtual void OnValid() { }

    protected virtual void OnReset() { }

    private static Color ParseColorOrDefault(string hex, Color fallback) =>
        ColorUtility.TryParseHtmlString(hex, out Color c) ? c : fallback;
}
