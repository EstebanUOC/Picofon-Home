using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class InputField : MonoBehaviour
{
    public TMP_InputField Input;

    public bool Valid
    {
        get { return _valid; }
        set
        {
            if (_valid == value)
                return;

            _valid = value;
            if (_valid)
                OnValid();
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

    protected Color _defaultColor;
    protected Color _validColor;
    protected Color _errorColor;

    protected ColorBlock _defaultColorBlock;

    private bool _valid = false;
    private bool _error = false;

    public virtual void Start()
    {
        Input.onValueChanged.AddListener(OnInputChange);

        _defaultColorBlock = Input.colors;
        _defaultColor = ParseColorOrDefault("#AD46FF", Color.white);
        _validColor = ParseColorOrDefault("#00C950", Color.white);
        _errorColor = ParseColorOrDefault("#FB2C36", Color.white);
    }

    public virtual string GetText()
    {
        return Input.text;
    }

    protected virtual void OnInputChange(string input)
    {
        ValidateInput(input);
    }

    protected virtual void ValidateInput(string input) { }

    protected virtual void OnError()
    {
        _defaultColorBlock.selectedColor = _errorColor;
        Input.colors = _defaultColorBlock;
    }

    protected virtual void OnValid()
    {
        _defaultColorBlock.selectedColor = _validColor;
        Input.colors = _defaultColorBlock;
    }

    protected virtual void OnReset()
    {
        _defaultColorBlock.selectedColor = _defaultColor;
        Input.colors = _defaultColorBlock;
    }

    private Color ParseColorOrDefault(string hex, Color fallback)
    {
        return ColorUtility.TryParseHtmlString(hex, out Color c) ? c : fallback;
    }
}
