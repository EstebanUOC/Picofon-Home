using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BalloonPopSeaManager : MonoBehaviour
{
    [Header("UI del reto")]
    [SerializeField] private Image image1;
    [SerializeField] private Image image2;

    [Header("Feedback")]
    [SerializeField] private GameObject panelFeedback;
    [SerializeField] private TMP_Text feedbackText;
    [SerializeField] private Image feedbackImage1;
    [SerializeField] private Image feedbackImage2;

    [SerializeField] private TMP_Text feedbackName1;
    [SerializeField] private TMP_Text feedbackName2;

    [Header("Imagen adicional")]
    [SerializeField] private Image extraImage;
    [SerializeField] private Sprite extraCorrectSprite;   // Correct_Answer015
    [SerializeField] private Sprite extraIncorrectSprite; // Incorrect_Answer007

    [Header("Imagen de nube (solo en incorrecto)")]
    [SerializeField] private Image cloudImage;

    [Header("Botones")]
    [SerializeField] private Button buttonYes;
    [SerializeField] private Button buttonNo;

    [Header("Sprites disponibles (asignar o autoload)")]
    [SerializeField] private List<Sprite> gameImages = new List<Sprite>();
    [SerializeField] private string resourcesFolder = "BalloonPopSea";

    // Estado actual
    private Sprite current1;
    private Sprite current2;
    private bool sameSyllable;
    public bool IsBusyShowingFeedback { get; private set; } = false;
    private bool isInitialized = false;

    private void Awake()
    {
        if (!image1) image1 = GameObject.Find("Image1")?.GetComponent<Image>();
        if (!image2) image2 = GameObject.Find("Image2")?.GetComponent<Image>();

        if (!panelFeedback) panelFeedback = GameObject.Find("PanelFeedBack");
        if (panelFeedback && !feedbackText) feedbackText = panelFeedback.transform.Find("FeedbackText")?.GetComponent<TMP_Text>();
        if (panelFeedback && !feedbackImage1) feedbackImage1 = panelFeedback.transform.Find("Row/ColLeft/FeedbackImage1")?.GetComponent<Image>();
        if (panelFeedback && !feedbackImage2) feedbackImage2 = panelFeedback.transform.Find("Row/ColRight/FeedbackImage2")?.GetComponent<Image>();
        if (panelFeedback && !feedbackName1) feedbackName1 = panelFeedback.transform.Find("Row/ColLeft/FeedbackName1")?.GetComponent<TMP_Text>();
        if (panelFeedback && !feedbackName2) feedbackName2 = panelFeedback.transform.Find("Row/ColRight/FeedbackName2")?.GetComponent<TMP_Text>();
        if (panelFeedback && !extraImage) extraImage = panelFeedback.transform.Find("ExtraImage")?.GetComponent<Image>();

        if (!buttonYes) buttonYes = GameObject.Find("ButtonYes")?.GetComponent<Button>();
        if (!buttonNo) buttonNo = GameObject.Find("ButtonNo")?.GetComponent<Button>();

        EnsureSpritesLoaded();

        if (panelFeedback) panelFeedback.SetActive(false);
        if (cloudImage) cloudImage.enabled = false;
    }

    private void Start()
    {
        if (isInitialized) return;
        isInitialized = true;
        ShowNewPair();
    }

    private void SetButtonsInteractable(bool enable)
    {
        if (buttonYes) buttonYes.interactable = enable;
        if (buttonNo) buttonNo.interactable = enable;
    }

    private void EnsureSpritesLoaded()
    {
        if (gameImages == null) gameImages = new List<Sprite>();

        if (gameImages.Count < 2 && !string.IsNullOrEmpty(resourcesFolder))
        {
            var loaded = Resources.LoadAll<Sprite>(resourcesFolder);
            if (loaded != null && loaded.Length > 0)
            {
                gameImages.Clear();
                gameImages.AddRange(loaded);
            }
        }
    }

    public void ShowNewPair()
    {
        EnsureSpritesLoaded();

        if (gameImages == null || gameImages.Count < 2) return;

        int i1 = Random.Range(0, gameImages.Count);
        int i2; do { i2 = Random.Range(0, gameImages.Count); } while (i2 == i1);

        current1 = gameImages[i1];
        current2 = gameImages[i2];

        image1.sprite = current1;
        image2.sprite = current2;

        SetButtonsInteractable(true);

        sameSyllable = FirstSyllable(current1.name).ToLower() == FirstSyllable(current2.name).ToLower();
    }

    public void OnFirstButton() => ValidateAnswer(true);
    public void OnSecondButton() => ValidateAnswer(false);

    public void ValidateAnswer(bool pressedFirst)
    {
        if (IsBusyShowingFeedback) return;

        bool isCorrect = (sameSyllable && pressedFirst) || (!sameSyllable && !pressedFirst);
        ShowFeedback(isCorrect);
    }

    private void ShowFeedback(bool isCorrect)
    {
        if (!panelFeedback || !feedbackText) return;

        IsBusyShowingFeedback = true;

        Sprite shown1 = current1;
        Sprite shown2 = current2;

        panelFeedback.SetActive(true);
        feedbackText.text = isCorrect ? "¡Correcto!" : "Incorrecto";
        feedbackText.color = isCorrect ? Color.green : new Color(0.6f, 0, 0.6f); // morado

        feedbackImage1.sprite = shown1;
        feedbackImage2.sprite = shown2;

        // 🔹 Colorear primera sílaba en verde/morado dinámicamente
        if (feedbackName1) feedbackName1.text = GetColoredWord(shown1.name, isCorrect);
        if (feedbackName2) feedbackName2.text = GetColoredWord(shown2.name, isCorrect);

        if (extraImage)
        {
            extraImage.enabled = true;
            extraImage.sprite = isCorrect ? extraCorrectSprite : extraIncorrectSprite;
        }

        if (cloudImage) cloudImage.enabled = !isCorrect;

        SetButtonsInteractable(false);
        StartCoroutine(FeedbackThenNext());
    }

    private IEnumerator FeedbackThenNext()
    {
        yield return new WaitForSeconds(5f);

        if (panelFeedback) panelFeedback.SetActive(false);
        if (cloudImage) cloudImage.enabled = false;

        IsBusyShowingFeedback = false;
        ShowNewPair();
    }

    private string GetColoredWord(string raw, bool isCorrect)
    {
        if (string.IsNullOrEmpty(raw)) return "";
        string clean = PrettyName(raw);
        string firstSyl = FirstSyllable(clean);

        if (string.IsNullOrEmpty(firstSyl)) return clean;

        string color = isCorrect ? "#00FF00" : "#800080"; // verde / morado

        // Coloreamos SOLO la primera sílaba y dejamos el resto normal
        return $"<color={color}>{firstSyl}</color>{clean.Substring(firstSyl.Length)}";
    }


    private static readonly HashSet<string> digraphs = new HashSet<string> { "ch", "ll", "rr" };
    private static readonly HashSet<char> vowels = new HashSet<char> { 'a', 'e', 'i', 'o', 'u', 'á', 'é', 'í', 'ó', 'ú' };

    private string FirstSyllable(string word)
    {
        if (string.IsNullOrEmpty(word)) return "";
        word = word.ToLower().Trim();

        word = word.Replace("que", "qe").Replace("qui", "qi")
                   .Replace("gue", "ge").Replace("gui", "gi");

        int start = 0;
        while (start < word.Length && !char.IsLetter(word[start])) start++;
        if (start >= word.Length) return "";

        int i = start;
        string result = "";

        // 1️⃣ Ataque: consonantes iniciales (soportando dígrafos)
        if (i + 1 < word.Length && digraphs.Contains(word.Substring(i, 2)))
        {
            result += word.Substring(i, 2);
            i += 2;
        }
        else if (!vowels.Contains(word[i]))
        {
            result += word[i];
            i++;
            // Si hay una segunda consonante y NO hay vocal después, mantenerla en ataque
            if (i < word.Length && !vowels.Contains(word[i]) && word[i] != 'h')
            {
                // Solo agregar si la siguiente letra después de esta es consonante también
                // (evitamos tomar la c de "dip**l**oma")
                if (i + 1 < word.Length && !vowels.Contains(word[i + 1]))
                {
                    result += word[i];
                    i++;
                }
            }
        }

        // 2️⃣ Núcleo: vocal + posible diptongo
        if (i < word.Length && vowels.Contains(word[i]))
        {
            result += word[i];
            i++;

            if (i < word.Length && (word[i] == 'i' || word[i] == 'u'))
            {
                result += word[i];
                i++;
            }
        }

        // 3️⃣ Coda: añadir una consonante SOLO si NO está seguida de vocal
        if (i < word.Length && !vowels.Contains(word[i]))
        {
            // Si después hay otra consonante o fin de palabra, la coda se mantiene (ej: "pan", "sol")
            if (i + 1 >= word.Length || !vowels.Contains(word[i + 1]))
            {
                result += word[i];
            }
        }

        return result;
    }



    private string PrettyName(string raw)
    {
        if (string.IsNullOrEmpty(raw)) return "";
        raw = raw.Replace('_', ' ').Replace('-', ' ').Trim();
        return char.ToUpper(raw[0]) + (raw.Length > 1 ? raw.Substring(1) : "");
    }
}
