using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class InputField : MonoBehaviour
{
    public TMP_InputField Input;

    public bool Valid
    {
        get { return _valid; }
    }

    protected Color _defaultColor;
    protected Color _validColor;
    protected Color _errorColor;

    protected ColorBlock _defaultColorBlock;

    protected bool _valid = false;
    protected bool _error = false;

    private bool _changed = false;

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

    protected virtual void ValidateInput(string input)
    {
        _valid = input.Length > 3;
    }

    protected virtual void OnInputChange(string input)
    {
        ValidateInput(input);
        ChangeColor();
    }

    protected virtual void OnError()
    {
        _defaultColorBlock.selectedColor = _errorColor;
    }

    protected virtual void OnValid()
    {
        _defaultColorBlock.selectedColor = _validColor;
    }

    protected virtual void OnReset()
    {
        _defaultColorBlock.selectedColor = _defaultColor;
    }

    private void ChangeColor()
    {
        if (_changed == _valid && !_error)
            return;

        _changed = _valid;

        if (_error)
            OnError();
        else if (_valid)
            OnValid();
        else
            OnReset();

        Input.colors = _defaultColorBlock;
    }

    private Color ParseColorOrDefault(string hex, Color fallback)
    {
        return ColorUtility.TryParseHtmlString(hex, out Color c) ? c : fallback;
    }
}
