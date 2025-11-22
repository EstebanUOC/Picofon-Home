using UnityEngine;
using UnityEngine.Events;

public abstract class FormInput : MonoBehaviour
{
    [Space(15)]
    public ChildFields Key;

    public bool Valid
    {
        get { return _valid; }
        set
        {
            if (_valid == value)
                return;

            _valid = value;
            if (_valid)
            {
                OnValid();
                onValidated.Invoke();
            }
            else
                OnReset();
        }
    }

    public bool Error
    {
        get { return _error; }
        set
        {
            if (_error == value)
                return;

            _error = value;
            if (_error)
                OnError();
            else
                OnReset();
        }
    }

    private readonly UnityEvent onValidated = new();

    private bool _valid = false;
    private bool _error = false;

    protected static Color _defaultColor = ParseColorOrDefault("#AD46FF", Color.white);
    protected static Color _validColor = ParseColorOrDefault("#00C950", Color.white);
    protected static Color _errorColor = ParseColorOrDefault("#FB2C36", Color.white);

    private static Color ParseColorOrDefault(string hex, Color fallback)
    {
        return ColorUtility.TryParseHtmlString(hex, out Color c) ? c : fallback;
    }

    public void AddOnValidatedListener(UnityAction call)
    {
        onValidated.AddListener(call);
    }

    public abstract string GetData();

    protected abstract void ValidateInput(string input);

    protected abstract void OnError();

    protected abstract void OnValid();

    protected abstract void OnReset();
}
