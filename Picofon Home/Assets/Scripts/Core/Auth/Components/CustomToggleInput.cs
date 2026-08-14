namespace Picofon.Core.Auth.Components
{
    using TMPro;
    using UnityEngine;

    public class CustomToggleInput : MonoBehaviour
    {
        [SerializeField]
        private CustomToggle _toggle;

        [SerializeField]
        private GameObject _background;

        [SerializeField]
        private TMP_InputField _input;

        public void Awake()
        {
            _toggle.OnToggle += Toggle;
            _background.SetActive(false);
            _input.interactable = false;
        }

        private void Toggle(bool isOn)
        {
            _background.SetActive(isOn);
            _input.interactable = isOn;
        }
    }
}
