using Picofon.Core.Auth;
using Picofon.Core.Auth.Events;

namespace Picofon.Core.Auth.Inputs
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UI;

    public class ToggleGroupField : FormInput
    {
        [Space(15)]
        public ToggleGroup ToggleGroup;

        [Space(15)]
        public Toggle OtherToggle;
        public FormInput OtherInput;

        private readonly InputChangedEventChannel _inputChangedEventChannel = new();

        public void Start()
        {
            OtherInput.gameObject.SetActive(false);
            OtherInput.SetInputChangedEventChannel(_inputChangedEventChannel);

            foreach (Toggle toggle in ToggleGroup.GetComponentsInChildren<Toggle>())
            {
                toggle.onValueChanged.AddListener(OnToggleChanged);
            }

            OtherToggle.onValueChanged.AddListener(OnOtherToggleChanged);
        }

        public override string GetData()
        {
            if (OtherToggle.isOn)
                return OtherInput.GetData();

            IEnumerable toggles = ToggleGroup.ActiveToggles();

            string disorder = string.Empty;
            foreach (Toggle toggle in toggles)
            {
                disorder = toggle.name;
            }

            return disorder;
        }

        private void OnOtherToggleChanged(bool isOn)
        {
            OtherInput.gameObject.SetActive(isOn);

            if (isOn)
            {
                _inputChangedEventChannel.OnInputChanged += HandleOtherInputValidated;
                Valid = OtherInput.Valid;
            }
            else
            {
                _inputChangedEventChannel.OnInputChanged -= HandleOtherInputValidated;
                Valid = false;
            }
        }

        private void HandleOtherInputValidated(bool isValid)
        {
            Valid = isValid;
        }

        private void OnToggleChanged(bool value)
        {
            Valid = value;
        }
    }
}
