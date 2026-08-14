using Picofon.Core.Auth;

namespace Picofon.Core.Auth.Inputs
{
    using UnityEngine;
    using UnityEngine.UI;

    public class CenterInput : FormInput
    {
        [Space(15)]
        public Toggle Toggle;

        public void Start()
        {
            Toggle.onValueChanged.AddListener(OnToggleChange);
        }

        public override string GetData()
        {
            // TODO: Replace with actual center ID retrieval logic
            int center = 1;
            return center.ToString();
        }

        private void OnToggleChange(bool isOn)
        {
            Valid = isOn;
        }
    }
}
