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

    protected ColorBlock _defaultColorBlock;

    protected bool _valid = false;
    protected bool _changed = false;

    public void Start()
    {
        Input.onValueChanged.AddListener(OnInputChange);

        _defaultColorBlock = Input.colors;
        _defaultColor = ColorUtility.TryParseHtmlString("#AD46FF", out var tempColor)
            ? tempColor
            : Color.white;

        _validColor = ColorUtility.TryParseHtmlString("#00C950", out var tempColorValid)
            ? tempColorValid
            : Color.white;
    }

    protected virtual void ValidateInput(string input)
    {
        _valid = input.Length > 3;
    }

    private void OnInputChange(string input)
    {
        ValidateInput(input);
        ChangeColor();
    }

    private void ChangeColor()
    {
        if (_changed == _valid)
            return;

        _changed = _valid;

        if (_valid)
            _defaultColorBlock.selectedColor = _validColor;
        else
            _defaultColorBlock.selectedColor = _defaultColor;

        Input.colors = _defaultColorBlock;
    }
}
