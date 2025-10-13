using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NumberInputManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_InputField numberInput;
    [SerializeField] private Button submitButton;
    [SerializeField] private TMP_Text resultText;

    private void Start()
    {
        if (submitButton)
            submitButton.onClick.AddListener(OnSubmit);
    }

    private void OnSubmit()
    {
        if (numberInput == null || resultText == null) return;

        string inputText = numberInput.text;

        if (int.TryParse(inputText, out int number))
        {
            resultText.text = $"Ingresaste el número: {number}";
            resultText.color = Color.green;
        }
        else
        {
            resultText.text = "Por favor, ingresa un número válido.";
            resultText.color = Color.red;
        }
    }
}
