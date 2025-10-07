using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class BalloonPopSeaManager : MonoBehaviour
{
    private Dictionary<Button, Sprite> buttonToSprite = new(); // Mapea botón → sprite mostrado
    private Dictionary<Sprite, string> spriteToWord = new();    // Mapea sprite → texto

    private Activity currentActivity;
    private bool correctAnswered = false;

    [Header("API Remota")]
    [SerializeField] private BalloonPopSeaAPI api;
    private Data apiData;

    [Header("Prefab y Contenedores")]
    [SerializeField] private GameObject bubblePrefab;
    [SerializeField] private Transform bubbleContainerHorizontal1;
    [SerializeField] private Transform bubbleContainerHorizontal2;

    private HorizontalLayoutGroup layoutRow1;
    private HorizontalLayoutGroup layoutRow2;

    [Header("Feedback")]
    [SerializeField] private GameObject panelFeedback;
    [SerializeField] private TMP_Text feedbackText;
    [SerializeField] private Image feedbackImage1;
    [SerializeField] private Image feedbackImage2;
    [SerializeField] private TMP_Text feedbackName1;
    [SerializeField] private TMP_Text feedbackName2;

    [Header("Imagen adicional")]
    [SerializeField] private Image extraImage;
    [SerializeField] private Sprite extraCorrectSprite;
    [SerializeField] private Sprite extraIncorrectSprite;

    [Header("Imagen de nube (solo en incorrecto)")]
    [SerializeField] private Image cloudImage;

    [Header("Botones principales (solo modo 0)")]
    [SerializeField] private Button buttonYes;
    [SerializeField] private Button buttonNo;

    [Header("Sprites disponibles")]
    [SerializeField] private List<Sprite> gameImages = new List<Sprite>();
    [SerializeField] private string resourcesFolder = "BalloonPopSea";

    private int currentMode = 1; // por defecto modo 1
    private Sprite current1, current2, oddOneOut;
    public bool IsBusyShowingFeedback { get; private set; } = false;

    // ==============================================================
    private void Awake()
    {
        EnsureSpritesLoaded();

        if (panelFeedback) panelFeedback.SetActive(false);
        if (cloudImage) cloudImage.enabled = false;

        if (bubbleContainerHorizontal1 != null)
            layoutRow1 = bubbleContainerHorizontal1.GetComponent<HorizontalLayoutGroup>();
        if (bubbleContainerHorizontal2 != null)
            layoutRow2 = bubbleContainerHorizontal2.GetComponent<HorizontalLayoutGroup>();
    }

    private void Start()
    {
        // Solo ejecuta si quieres probar localmente sin API
        //ShowNewPair();
    }



    // ==============================================================
    // 🔹 Solicitar actividad 1 al servidor
    // ==============================================================
    public void LoadActivity1FromServer()
    {
        if (api == null)
        {
            Debug.LogError("⚠️ Falta referencia a BalloonPopSeaAPI.");
            return;
        }

        if (buttonYes) buttonYes.gameObject.SetActive(false);
        if (buttonNo) buttonNo.gameObject.SetActive(false);

        Debug.Log("🌐 Solicitando Activity1 al servidor...");
        StartCoroutine(api.LoadActivities(OnActivity1Loaded));
    }
    // ==============================================================
    // 🔹 Solicitar actividad del modo 0 (Sí/No)
    // ==============================================================
    public void LoadActivity0FromServer()
    {
        currentMode = 0;
        if (api == null)
        {
            Debug.LogError("⚠️ Falta referencia a BalloonPopSeaAPI.");
            return;
        }

        // Mostrar los botones del modo 0
        if (buttonYes) buttonYes.gameObject.SetActive(true);
        if (buttonNo) buttonNo.gameObject.SetActive(true);

        Debug.Log("🌐 Solicitando Activity0 al servidor...");
        StartCoroutine(api.LoadSimpleActivities(OnActivity0Loaded));
    }

    private void OnActivity0Loaded(DataSimple data)
    {
        if (data == null || data.activity1 == null)
        {
            Debug.LogError("❌ No se recibió activity0 desde la API.");
            return;
        }

        ActivitySimple activity = data.activity1;
        correctAnswered = false;

        Debug.Log($"✅ Activity0 recibida: {activity.palabra_principal} vs {activity.opcion1}");

        LoadSimpleActivityToBubbles(activity);
    }

    private void LoadSimpleActivityToBubbles(ActivitySimple activity)
    {
        ClearContainers();

        if (layoutRow1) layoutRow1.spacing = 250;

        Sprite main = LoadSprite(activity.palabra_principal_PATH);
        Sprite compare = LoadSprite(activity.opcion1_PATH);

        if (main == null || compare == null)
        {
            Debug.LogError("❌ No se pudieron cargar las imágenes del modo 0.");
            return;
        }

        spriteToWord.Clear();
        spriteToWord[main] = activity.palabra_principal;
        spriteToWord[compare] = activity.opcion1;

        // Crear ambas burbujas
        CreateBubble(bubbleContainerHorizontal1, main);
        CreateBubble(bubbleContainerHorizontal1, compare);

        // Configurar botones de respuesta
        buttonYes.onClick.RemoveAllListeners();
        buttonNo.onClick.RemoveAllListeners();

        buttonYes.onClick.AddListener(() => EvaluateAnswer(activity, true));
        buttonNo.onClick.AddListener(() => EvaluateAnswer(activity, false));
    }

    private void EvaluateAnswer(ActivitySimple activity, bool playerAnswer)
    {
        // El campo "respuesta" del JSON indica si el botón correcto es Sí (true) o No (false)
        bool isCorrect = playerAnswer == activity.respuesta;
        correctAnswered = isCorrect;

        string feedback = isCorrect ? activity.feedback_positiu : activity.feedback_neutre;

        Debug.Log(isCorrect
            ? $"✅ Respuesta correcta → {(activity.respuesta ? "Sí" : "No")} era la opción correcta."
            : $"❌ Respuesta incorrecta → {(activity.respuesta ? "Sí" : "No")} era la opción correcta.");

        // Muestra feedback visual
        ShowFeedback(
            isCorrect,
            LoadSprite(activity.palabra_principal_PATH),
            LoadSprite(activity.opcion1_PATH),
            feedback,
            activity.palabra_principal,
            activity.opcion1
        );
    }




    private void OnActivity1Loaded(Data data)
    {
        if (data == null || data.activity1 == null)
        {
            Debug.LogError("❌ No se recibió activity1 desde la API.");
            return;
        }

        currentActivity = data.activity1;
        correctAnswered = false;

        Debug.Log($"✅ Activity1 recibida: {currentActivity.main_word} | Correcta: {currentActivity.correct_option.text}");
        LoadActivityToBubbles(currentActivity);
    }

    // ==============================================================
    // 🔹 Mostrar las imágenes del JSON (modo 1)
    // ==============================================================
    private void LoadActivityToBubbles(Activity activity)
    {
        ClearContainers();

        if (layoutRow1) layoutRow1.spacing = 250;
        if (layoutRow2) layoutRow2.spacing = 250;

        spriteToWord.Clear();
        buttonToSprite.Clear();

        Sprite main = LoadSprite(activity.main_word_PATH);
        Sprite correct = LoadSprite(activity.correct_option.path);
        Sprite wrong1 = LoadSprite(activity.wrong_option1.path);
        Sprite wrong2 = LoadSprite(activity.wrong_option2.path);

        List<Sprite> sprites = new() { main, correct, wrong1, wrong2 };
        sprites = sprites.Where(s => s != null).OrderBy(x => Random.value).ToList();

        if (sprites.Count < 4)
        {
            Debug.LogWarning("⚠️ No se encontraron las 4 imágenes requeridas.");
            return;
        }

        spriteToWord[main] = activity.main_word;
        spriteToWord[correct] = activity.correct_option.text;
        spriteToWord[wrong1] = activity.wrong_option1.text;
        spriteToWord[wrong2] = activity.wrong_option2.text;

        CreateBubble(bubbleContainerHorizontal1, sprites[0]);
        CreateBubble(bubbleContainerHorizontal1, sprites[1]);
        CreateBubble(bubbleContainerHorizontal2, sprites[2]);
        CreateBubble(bubbleContainerHorizontal2, sprites[3]);
    }

    // ==============================================================
    // 🎲 Modo local (si no hay API)
    // ==============================================================
    // ==============================================================
// 🎲 Generar burbujas según el modo
// ==============================================================
public void ShowNewPair()
{
    ClearContainers();

    if (layoutRow1) layoutRow1.spacing = 250;
    if (layoutRow2) layoutRow2.spacing = 250;

    if (gameImages == null || gameImages.Count < 2)
    {
        Debug.LogWarning("⚠️ No hay suficientes imágenes locales cargadas.");
        return;
    }

    if (currentMode == 0)
    {
        // 🎮 MODO 0 → dos imágenes y botones Sí / No
        if (buttonYes) buttonYes.gameObject.SetActive(true);
        if (buttonNo) buttonNo.gameObject.SetActive(true);

        int i1 = Random.Range(0, gameImages.Count);
        int i2;
        do { i2 = Random.Range(0, gameImages.Count); } while (i2 == i1);

        Sprite s1 = gameImages[i1];
        Sprite s2 = gameImages[i2];

        CreateBubble(bubbleContainerHorizontal1, s1);
        CreateBubble(bubbleContainerHorizontal1, s2);

        // Guarda las actuales (por si quieres comparar sílaba, etc.)
        current1 = s1;
        current2 = s2;
    }
    else if (currentMode == 2)
    {
        // 🎮 MODO 2 → 5 imágenes (1 arriba + 4 abajo)
        if (layoutRow2) layoutRow2.spacing = 150;

        HashSet<int> indices = new();
        while (indices.Count < 5)
            indices.Add(Random.Range(0, gameImages.Count));

        var selected = indices.Select(i => gameImages[i]).ToList();

        CreateBubble(bubbleContainerHorizontal1, selected[0]);
        for (int i = 1; i < 5; i++)
            CreateBubble(bubbleContainerHorizontal2, selected[i]);
    }
    else if (currentMode == 1)
    {
        // 🎮 MODO 1 → usa el servidor, no se genera localmente
        Debug.Log("Modo 1 se maneja con LoadActivity1FromServer().");
    }
}


    // ==============================================================
    // 🧩 Crear burbuja con botón interactivo
    // ==============================================================
    private GameObject CreateBubble(Transform parent, Sprite sprite)
    {
        GameObject bubble = Instantiate(bubblePrefab, parent);

        // Imagen principal de la burbuja
        Image img = bubble.transform.Find("Image")?.GetComponent<Image>();
        Button btn = bubble.transform.Find("ButtonOp")?.GetComponent<Button>();

        if (img == null)
        {
            Debug.LogError("❌ Prefab inválido: falta hijo 'Image'.");
            return bubble;
        }

        img.sprite = sprite;

        // 🧩 Si estamos en el modo 0 (Sí/No)
        if (currentMode == 0)
        {
            // 🔹 Desactiva completamente los botones dentro de la burbuja
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                btn.interactable = false;
                btn.enabled = false;
                btn.gameObject.SetActive(false);
            }

            // 🔹 Desactiva todos los botones hijos (por si el prefab tiene varios)
            foreach (var b in bubble.GetComponentsInChildren<Button>(true))
            {
                b.onClick.RemoveAllListeners();
                b.interactable = false;
                b.enabled = false;
                b.gameObject.SetActive(false);
            }

            // 🔹 Desactiva todos los event triggers
            foreach (var trigger in bubble.GetComponentsInChildren<UnityEngine.EventSystems.EventTrigger>(true))
                trigger.enabled = false;

            // 🔹 Desactiva todos los raycastTargets de imágenes
            foreach (var image in bubble.GetComponentsInChildren<Image>(true))
                image.raycastTarget = false;

            // 🔹 Desactiva colliders si los tuviera
            foreach (var col in bubble.GetComponentsInChildren<Collider>(true))
                col.enabled = false;
            foreach (var col2d in bubble.GetComponentsInChildren<Collider2D>(true))
                col2d.enabled = false;

            // 🔹 Desactiva canvases interactivos
            foreach (var canvas in bubble.GetComponentsInChildren<CanvasGroup>(true))
            {
                canvas.interactable = false;
                canvas.blocksRaycasts = false;
            }

            // 🔹 Limpieza de referencias
            buttonToSprite.Clear();

            return bubble; // ✅ Solo visual, sin interacción posible
        }

        // 🧩 En otros modos (1 o 2) → comportamiento normal
        if (btn == null)
        {
            Debug.LogError("⚠️ Prefab sin botón 'ButtonOp'.");
            return bubble;
        }

        btn.enabled = true;
        btn.interactable = true;
        btn.onClick.RemoveAllListeners();
        btn.gameObject.SetActive(true);
        buttonToSprite[btn] = sprite;
        btn.onClick.AddListener(() => OnBubbleClicked(btn));

        return bubble;
    }




    // ==============================================================
    // 🎯 Clic en burbuja
    // ==============================================================
    private void OnBubbleClicked(Button btn)
    {
        if (!buttonToSprite.ContainsKey(btn)) return;
        Sprite chosen = buttonToSprite[btn];

        if (currentMode == 1 && currentActivity != null)
        {
            Sprite correctSprite = LoadSprite(currentActivity.correct_option.path);
            Sprite wrong1 = LoadSprite(currentActivity.wrong_option1.path);
            Sprite wrong2 = LoadSprite(currentActivity.wrong_option2.path);

            bool isCorrect = chosen.name == correctSprite.name;
            string chosenWord = spriteToWord.ContainsKey(chosen) ? spriteToWord[chosen] : "???";

            if (isCorrect)
            {
                correctAnswered = true;
                Sprite randomWrong = (Random.value > 0.5f) ? wrong1 : wrong2;
                string randomWrongWord = spriteToWord.ContainsKey(randomWrong) ? spriteToWord[randomWrong] : "???";
                ShowFeedback(true, chosen, randomWrong, currentActivity.feedback_positive, chosenWord, randomWrongWord);
            }
            else
            {
                Sprite otherWrong = (chosen.name == wrong1?.name) ? wrong2 : wrong1;
                string otherWrongWord = spriteToWord.ContainsKey(otherWrong) ? spriteToWord[otherWrong] : "???";
                ShowFeedback(false, chosen, otherWrong, currentActivity.feedback_neutral, chosenWord, otherWrongWord);
            }
        }
    }

    // ==============================================================
    // 📊 Feedback visual
    // ==============================================================
    private void ShowFeedback(bool isCorrect, Sprite chosen, Sprite match, string message, string word1, string word2)
    {
        if (!panelFeedback || !feedbackText) return;
        IsBusyShowingFeedback = true;

        panelFeedback.SetActive(true);
        feedbackText.text = message;
        feedbackText.color = isCorrect ? Color.green : Color.red;

        feedbackImage1.sprite = chosen;
        feedbackImage2.sprite = match;
        feedbackName1.text = word1;
        feedbackName2.text = word2;

        if (extraImage)
        {
            extraImage.enabled = true;
            extraImage.sprite = isCorrect ? extraCorrectSprite : extraIncorrectSprite;
        }

        if (cloudImage)
            cloudImage.enabled = !isCorrect;

        StartCoroutine(FeedbackThenNext());
    }

    private IEnumerator FeedbackThenNext()
    {
        yield return new WaitForSeconds(3f);

        if (panelFeedback) panelFeedback.SetActive(false);
        if (cloudImage) cloudImage.enabled = false;
        IsBusyShowingFeedback = false;

        // 🔹 Si el jugador respondió correctamente
        if (correctAnswered)
        {
            if (currentMode == 0)
            {
                Debug.Log("🔁 Respuesta correcta → solicitando nueva actividad modo 0...");
                LoadActivity0FromServer(); // ✅ sigue en modo 0
            }
            else if (currentMode == 1)
            {
                Debug.Log("🔁 Respuesta correcta → solicitando nueva actividad modo 1...");
                LoadActivity1FromServer();
            }
            else if (currentMode == 2)
            {
                Debug.Log("🔁 Respuesta correcta → refrescando modo 2 local...");
                ShowNewPair(); // opcional si el modo 2 es local
            }
        }
        else
        {
            Debug.Log("⏸ Respuesta incorrecta → se mantienen las mismas imágenes (sin nueva petición).");
            // No recarga, solo permite que el jugador intente nuevamente
        }
    }



    // ==============================================================
    // Utilidades
    // ==============================================================
    private void ClearContainers()
    {
        foreach (Transform child in bubbleContainerHorizontal1) Destroy(child.gameObject);
        foreach (Transform child in bubbleContainerHorizontal2) Destroy(child.gameObject);
    }

    private Sprite LoadSprite(string imageFileName)
    {
        if (string.IsNullOrEmpty(imageFileName)) return null;
        string imageName = System.IO.Path.GetFileNameWithoutExtension(imageFileName);
        return Resources.Load<Sprite>($"Images/ImgButtons/{imageName}");
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


}
