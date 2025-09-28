using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BalloonPopPartyManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private List<Button> balloonButtons; // Asigna los 4 botones desde el inspector
    [SerializeField] private GameObject panelFeedback;
    [SerializeField] private TMP_Text feedbackText;

    [Header("Feedback Words Row")]
    [SerializeField] private List<Image> feedbackWordImages; // Asigna 4 imágenes desde el inspector
    [SerializeField] private List<TMP_Text> feedbackWordLabels; // Asigna 4 textos desde el inspector

    private WordData wordData;
    private string correctAnswer;
    private List<string> currentOrder = new List<string>(); // guarda el orden real de palabras en esta ronda

    void Start()
    {
        titleText.text = "¿Cuál no empieza igual?";
        LoadWords();
        SetupBalloons();

        if (panelFeedback) panelFeedback.SetActive(false);
    }

    void LoadWords()
    {
        wordData = WordLoader.LoadFromTextAsset("WordsExampleJson");
        if (wordData == null) return;
        correctAnswer = wordData.correct_option;
    }

    void SetupBalloons()
    {
        if (wordData == null) return;

        // Lista de opciones
        List<string> options = new List<string> {
            wordData.main_word,
            wordData.wrong_option1,
            wordData.wrong_option2,
            wordData.wrong_option3
        };

        // Mezclar aleatoriamente
        for (int i = 0; i < options.Count; i++)
        {
            string temp = options[i];
            int randomIndex = Random.Range(i, options.Count);
            options[i] = options[randomIndex];
            options[randomIndex] = temp;
        }

        // Guardar orden actual
        currentOrder.Clear();
        currentOrder.AddRange(options);

        // Asignar a los botones
        for (int i = 0; i < balloonButtons.Count && i < options.Count; i++)
        {
            string displayWord = options[i].Replace("#", ""); // quitar '#'
            balloonButtons[i].GetComponentInChildren<TMP_Text>().text = displayWord;

            string originalWord = options[i];
            balloonButtons[i].onClick.RemoveAllListeners(); // limpiar anteriores
            balloonButtons[i].onClick.AddListener(() => CheckAnswer(originalWord));
        }
    }

    void CheckAnswer(string chosenWord)
    {
        bool isCorrect = chosenWord == correctAnswer;

        panelFeedback.SetActive(true);
        feedbackText.text = isCorrect ? "¡Correcto!" : "Incorrecto";
        feedbackText.color = isCorrect ? Color.green : Color.red;

        if (isCorrect)
        {
            // Mostrar las 4 palabras con su orden
            for (int i = 0; i < currentOrder.Count && i < feedbackWordLabels.Count; i++)
            {
                string word = currentOrder[i].Replace("#", "");
                feedbackWordLabels[i].text = word;

                if (i < feedbackWordImages.Count)
                {
                    feedbackWordImages[i].sprite = balloonButtons[i].GetComponent<Image>().sprite;
                    feedbackWordImages[i].enabled = true;
                }
            }
        }
        else
        {
            // Si es incorrecto, ocultar los labels y sprites
            for (int i = 2; i < feedbackWordLabels.Count; i++)
            {
                feedbackWordLabels[i].text = "";
                if (i < feedbackWordImages.Count)
                {
                    feedbackWordImages[i].sprite = null;
                    feedbackWordImages[i].enabled = false;
                }
            }
        }

        // Esperar 5 segundos y reiniciar
        StartCoroutine(HideFeedbackAndNext());
    }

    private IEnumerator HideFeedbackAndNext()
    {
        yield return new WaitForSeconds(5f);

        if (panelFeedback) panelFeedback.SetActive(false);

        // Reset y nuevo intento
        SetupBalloons();
    }
}
