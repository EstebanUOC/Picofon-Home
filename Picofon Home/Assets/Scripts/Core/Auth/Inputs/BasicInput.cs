using Picofon.Core.Auth;

namespace Picofon.Core.Auth.Inputs
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class BasicInput : FormInput
    {
        [Space(15)]
        public TMP_InputField Input;

        public int MinLength = 1;

        protected ColorBlock _defaultColorBlock;
        protected Color _colorImage;

        public virtual void Start()
        {
            Input.onValueChanged.AddListener(OnInputChange);
            _defaultColorBlock = Input.colors;
            _colorImage = Input.image.color;
        }

        protected virtual void OnInputChange(string input)
        {
            ValidateInput(input);
        }

        protected override void OnError()
        {
            _defaultColorBlock.selectedColor = _errorColor;
            Input.colors = _defaultColorBlock;
            Input.image.color = _errorColor;
        }

        protected override void OnValid()
        {
            _defaultColorBlock.selectedColor = _validColor;
            Input.colors = _defaultColorBlock;
            Input.image.color = _validColor;
        }

        protected override void OnReset()
        {
            _defaultColorBlock.selectedColor = _defaultColor;
            Input.colors = _defaultColorBlock;
            Input.image.color = _colorImage;
        }

        protected override void ValidateInput(string input)
        {
            Valid = input.Length >= MinLength;
        }

        public override string GetData()
        {
            return Input.text;
        }
    }
}
