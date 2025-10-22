using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Picofon.Games.Judge;
using UnityEngine.SceneManagement;


public class CrossRiverManager : MonoBehaviour
{
    [Header("🧩 Controlador de Feedback")]
    [SerializeField] private FeedbackPanelController feedbackController;

    [SerializeField] private Image imageMain;


    [Header("🎯 Prefabs y objetos principales")]
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private RectTransform buttonContainer;

    [Header("🖼️ Imágenes (modo Judge)")]
    [SerializeField] private Image firstImage;
    [SerializeField] private Image secondImage;

    [Header("🧠 Panel de feedback y pregunta")]
    [SerializeField] private GameObject panelFeedback;
    [SerializeField] private TMP_Text feedbackText;
    [SerializeField] private TMP_Text questionText;

    [Header("📦 Botones de cambio de modo (TopButtons)")]
    [SerializeField] private Transform topButtonsContainer;

    [Header("🧭 Botones de cambio de escena")]
    [SerializeField] private Button buttonMapScene;
    [SerializeField] private Button buttonNextScene;

    private readonly string mapSceneName = "MapPath";
    private readonly string nextSceneName = "BalloonPopSeaScene";
    private ActivityJudge lastActivityShown;




    // 🧍‍♂️ Personaje
    private RectTransform imageCharacter;
    private readonly float moveDuration = 1.2f;
    private readonly float arcHeight = 150f;
    private readonly AnimationCurve moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Vector2 startCharacterPos = new(250, 200);
    private Vector2 finalCharacterPos = new(1700, 800);
    private bool isAnimating = false;

    private List<GameObject> spawnedButtons = new();
    private int currentMode = 0;
    private GameAPIService apiService;

    // ============================================================
    // 🔹 Coordenadas configurables SOLO desde código
    // ============================================================
    private Vector2 yesButtonPosition = new(750, 300);
    private Vector2 noButtonPosition = new(1200, 200);

    private Vector2[] mode1Positions =
    {
        new Vector2(750, 700),
        new Vector2(1200, 600),
        new Vector2(750, 300),
        new Vector2(1200, 200)
    };

    private Vector2[] mode2Positions =
    {
        new Vector2(750, 700),
        new Vector2(1200, 600),
        new Vector2(750, 300),
        new Vector2(1200, 200)
    };

    private void Awake()
    {
        apiService = FindObjectOfType<GameAPIService>();
        if (panelFeedback) panelFeedback.SetActive(false);

        imageCharacter = GameObject.Find("ImageCharacter")?.GetComponent<RectTransform>();
        if (imageCharacter != null)
            imageCharacter.anchoredPosition = startCharacterPos;
    }

    private void Start()
    {
        AssignModeButtons();
        if (buttonMapScene != null)
        {
            buttonMapScene.onClick.RemoveAllListeners();
            buttonMapScene.onClick.AddListener(() => ChangeScene(mapSceneName));
        }

        if (buttonNextScene != null)
        {
            buttonNextScene.onClick.RemoveAllListeners();
            buttonNextScene.onClick.AddListener(() => ChangeScene(nextSceneName));
        }


        StartCoroutine(LoadModeFromAPI(0));
    }

    // ============================================================
    // 🔹 Carga dinámica de modos
    // ============================================================
    private IEnumerator LoadModeFromAPI(int mode)
    {
        currentMode = mode;
        if (apiService == null)
        {
            Debug.LogError("❌ No se encontró GameAPIService en la escena.");
            yield break;
        }

        yield return apiService.LoadActivity(mode,
            json => LoadMode(mode, json),
            err => Debug.LogError(err));
    }

    public void LoadMode(int mode, string json)
    {
        ClearButtons();
        currentMode = mode;

        switch (mode)
        {

            case 0:
                if (imageMain != null)
                    imageMain.gameObject.SetActive(false);

                var judgeData = JsonUtility.FromJson<ApiResponseJudge>(json);
                if (judgeData?.data?.activity1 != null)
                    LoadJudgeMode(judgeData.data.activity1);
                else
                    Debug.LogError("❌ JSON no contiene activity1 válido.");
                break;


            case 1:
                HideWordImages();
                CreateModeButtons(mode1Positions);
                if (imageMain != null)
                    imageMain.gameObject.SetActive(true);
                break;

            case 2:
                HideWordImages();
                CreateModeButtons(mode2Positions);
                if (imageMain != null)
                    imageMain.gameObject.SetActive(true);
                break;

            default:
                HideWordImages();
                Debug.LogWarning($"⚠️ Modo {mode} no implementado.");
                break;
        }
    }

    // ============================================================
    // 🟣 Modo Judge (Sí / No)
    // ============================================================
    private void LoadJudgeMode(ActivityJudge activity)
    {
        lastActivityShown = activity;
        if (questionText != null)
            questionText.text = activity.question;

        if (firstImage && secondImage)
        {
            firstImage.gameObject.SetActive(true);
            secondImage.gameObject.SetActive(true);
            firstImage.sprite = LoadLocalSprite(activity.word1.PATH);
            secondImage.sprite = LoadLocalSprite(activity.word2.PATH);
        }

        CreateJudgeButtons(activity);
    }

    private void CreateJudgeButtons(ActivityJudge activity)
    {
        ClearButtons();

        string[] labels = { "Sí", "No" };
        bool[] answers = { true, false };
        Vector2[] positions = { yesButtonPosition, noButtonPosition };

        for (int i = 0; i < 2; i++)
        {
            GameObject btnObj = Instantiate(buttonPrefab, buttonContainer);
            btnObj.name = $"Button_{labels[i]}";

            // ================================
            // 🎮 CONFIGURACIÓN DE IMÁGENES SEGÚN MODO Y PREFAB
            // ================================
            Transform bgLifebelt = btnObj.transform.Find("BackgroundLifebelt");
            if (bgLifebelt != null)
            {
                Image bgImage = bgLifebelt.GetComponent<Image>();
                if (bgImage != null)
                {
                    string spritePath;

                    // 🔹 Solo en modo 0 y en el primer prefab → usar el rosa
                    if (currentMode == 0 && i == 0)
                        spritePath = "Images/Images/CrossRiver/lifebelt_pink_Lluni";
                    else
                        spritePath = "Images/Images/CrossRiver/lifebelt_violet_Lluni";

                    Sprite lifebeltSprite = Resources.Load<Sprite>(spritePath);
                    if (lifebeltSprite != null)
                    {
                        bgImage.sprite = lifebeltSprite;
                        bgImage.color = Color.white;
                        bgImage.preserveAspect = true;
                    }
                    else
                    {
                        Debug.LogWarning($"⚠️ No se encontró el sprite en {spritePath}");
                    }
                }
            }

            // 🔹 Ocultar la imagen "Image" solo en modo 0
            if (currentMode == 0)
            {
                Transform imageChild = btnObj.transform.Find("Image");
                if (imageChild != null)
                    imageChild.gameObject.SetActive(false);
            }

            // ================================
            // 📍 POSICIÓN Y TEXTO
            // ================================
            RectTransform rect = btnObj.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = positions[i];

            TMP_Text text = btnObj.GetComponentInChildren<TMP_Text>();
            if (text != null) text.text = labels[i];

            // ================================
            // 🧠 ASIGNAR LÓGICA DEL BOTÓN
            // ================================
            Transform opTf = btnObj.transform.Find("ButtonOp");
            Button button = opTf ? opTf.GetComponent<Button>() : null;

            if (button != null)
            {
                bool isYes = answers[i];
                button.onClick.AddListener(() =>
                {
                    if (!isAnimating)
                    {
                        bool isCorrect = activity.answer ? isYes : !isYes;
                        string msg = isCorrect ? activity.feedback_positive : activity.feedback_neutral;

                        if (isCorrect)
                            StartCoroutine(HandleCorrectFeedback(activity, msg));
                        else
                            StartCoroutine(HandleSpecialIncorrectFeedback(activity, msg));
                    }
                });
            }

            spawnedButtons.Add(btnObj);
        }

        Debug.Log($"✅ Botones Judge configurados según answer={activity.answer}");
    }



    // ============================================================
    // 🔹 Crear botones genéricos (modo 1 y 2)
    // ============================================================
    private void CreateModeButtons(Vector2[] positions)
    {
        if (buttonPrefab == null || buttonContainer == null)
        {
            Debug.LogError("❌ Falta prefab o contenedor.");
            return;
        }

        ClearButtons();

        for (int i = 0; i < positions.Length; i++)
        {
            GameObject btnObj = Instantiate(buttonPrefab, buttonContainer);
            btnObj.name = $"Button_{i + 1}";

            RectTransform rect = btnObj.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = positions[i];

            // ❌ sin texto ni label
            TMP_Text text = btnObj.GetComponentInChildren<TMP_Text>();
            if (text != null) text.text = "";

            spawnedButtons.Add(btnObj);
        }

        Debug.Log($"✅ Generados {positions.Length} botones en modo {currentMode}");
    }

    // ============================================================
    // 🎯 Evaluar respuesta modo Judge
    // ============================================================
    // ============================================================
    // 🎯 Evaluar respuesta modo Judge (con animaciones diferenciadas)
    // ============================================================
    private void EvaluateJudge(ActivityJudge activity, bool playerAnswer)
    {
        bool correct = (activity.answer == playerAnswer);
        string msg = correct ? activity.feedback_positive : activity.feedback_neutral;

        // Si es el modo 0 y la respuesta es incorrecta → comportamiento especial
        if (currentMode == 0 && !correct)
        {
            StartCoroutine(HandleSpecialIncorrectFeedback(activity, msg));
            return;
        }

        if (correct)
        {
            // ✅ Caso correcto normal
            StartCoroutine(HandleCorrectFeedback(activity, msg));
        }
        else
        {
            // ❌ Caso incorrecto normal (otros modos)
            StartCoroutine(HandleIncorrectFeedback(activity, msg));
        }
    }
    // ============================================================
    // ⚠️ Caso especial modo 0: sin animación hacia el botón
    // ============================================================
    // ============================================================
    // ⚠️ Caso especial modo 0: animación al prefab → feedback → regreso
    // ============================================================
    private IEnumerator HandleSpecialIncorrectFeedback(ActivityJudge activity, string message)
    {
        isAnimating = true;

        // Buscar el botón presionado (opuesto al correcto)
        RectTransform target = FindClosestButton(!activity.answer);
        if (target == null)
        {
            Debug.LogWarning("⚠️ No se encontró el botón presionado.");
            yield break;
        }

        // 🔹 1. Movimiento inicial: del personaje al botón clicado
        yield return MoveToCurve(imageCharacter, target.anchoredPosition);

        // 🔹 2. Mostrar feedback neutral
        ShowFeedback(message, false);

        // Esperar a que el panel se muestre completamente (3 segundos)
        yield return new WaitForSeconds(3f);

        // 🔹 3. Regresar al punto inicial con animación parabólica
        yield return MoveToCurve(imageCharacter, startCharacterPos);

        isAnimating = false;
    }



    // ============================================================
    // ✅ Secuencia completa de respuesta correcta
    // ============================================================
    private IEnumerator HandleCorrectFeedback(ActivityJudge activity, string message)
    {
        isAnimating = true;

        // Mueve al botón correcto
        RectTransform target = FindClosestButton(activity.answer);
        if (target == null) yield break;

        // 🔹 1. Movimiento al botón correcto
        yield return MoveToCurve(imageCharacter, target.anchoredPosition);
        yield return new WaitForSeconds(0.2f);

        // 🔹 2. Movimiento al punto final
        yield return MoveToCurve(imageCharacter, finalCharacterPos);
        yield return new WaitForSeconds(0.3f);

        // 🔹 3. Mostrar feedback positivo
        ShowFeedback(message, true);

        // Esperar a que el feedback se muestre completamente
        yield return new WaitForSeconds(3f);

        // 🔹 4. Regresar al punto inicial
        yield return MoveToCurve(imageCharacter, startCharacterPos);

        // 🔹 5. Nueva petición para recargar imágenes del modo actual
        StartCoroutine(LoadModeFromAPI(currentMode));

        isAnimating = false;
    }


    // ============================================================
    // ❌ Secuencia de respuesta incorrecta
    // ============================================================
    private IEnumerator HandleIncorrectFeedback(ActivityJudge activity, string message)
    {
        RectTransform target = FindClosestButton(activity.answer);
        if (target == null) yield break;

        // Movimiento hacia el botón incorrecto
        yield return MoveToCurve(imageCharacter, target.anchoredPosition);
        yield return new WaitForSeconds(0.3f);

        // Mostrar feedback neutral
        ShowFeedback(message, false);

        // Esperar hasta que el feedback desaparezca
        yield return new WaitForSeconds(3f);

        // Volver al punto inicial
        imageCharacter.anchoredPosition = startCharacterPos;
    }

    // ============================================================
    // 🔹 Buscar el botón "Sí" o "No"
    // ============================================================
    private RectTransform FindClosestButton(bool isYes)
    {
        foreach (var btn in spawnedButtons)
        {
            if (btn.name.Contains(isYes ? "Sí" : "No"))
                return btn.GetComponent<RectTransform>();
        }
        return null;
    }


    private void ShowFeedback(string message, bool correct)
    {
        if (feedbackController == null) return;

        var currentActivity = lastActivityShown; // referencia que guardaremos al mostrar el modo actual
        if (currentActivity == null)
        {
            Debug.LogWarning("⚠️ No hay actividad cargada para mostrar feedback.");
            return;
        }

        feedbackController.ShowFeedback(
            firstImage.sprite,
            secondImage.sprite,
            correct,
            currentActivity.word1.syllabified_word,
            currentActivity.word2.syllabified_word
        );
    }


    // ============================================================
    // 🧩 Utilidades
    // ============================================================
    private void ClearButtons()
    {
        foreach (var btn in spawnedButtons)
            if (btn != null) Destroy(btn.gameObject);
        spawnedButtons.Clear();
    }

    private void HideWordImages()
    {
        if (firstImage) firstImage.gameObject.SetActive(false);
        if (secondImage) secondImage.gameObject.SetActive(false);
    }

    private Sprite LoadLocalSprite(string imageName)
    {
        if (string.IsNullOrEmpty(imageName)) return null;
        string path = $"Images/ImgButtons/{System.IO.Path.GetFileNameWithoutExtension(imageName)}";
        return Resources.Load<Sprite>(path);
    }

    private void AssignModeButtons()
    {
        if (topButtonsContainer == null)
            topButtonsContainer = GameObject.Find("TopButtons")?.transform;

        if (topButtonsContainer == null)
        {
            Debug.LogWarning("⚠️ No se encontró TopButtons.");
            return;
        }

        for (int i = 0; i < 3; i++)
        {
            int mode = i;
            Transform button = topButtonsContainer.Find($"Button{i}");
            if (button != null && button.TryGetComponent(out Button btn))
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => StartCoroutine(LoadModeFromAPI(mode)));
                Debug.Log($"🎮 Asignado Button{i} → modo {mode}");
            }
        }
    }

    // ============================================================
    // 🌀 Movimiento del personaje
    // ============================================================
    // ============================================================
    // 🌀 Movimiento del personaje con dos fases (curva al botón + curva al final)
    // ============================================================
    // ============================================================
    // 🌀 Movimiento del personaje (parabólico correcto en Overlay)
    // ============================================================
    private IEnumerator MoveCharacterToTarget(RectTransform target)
    {
        if (isAnimating || imageCharacter == null || target == null) yield break;
        isAnimating = true;

        // ✅ Ambos están bajo el mismo Canvas Overlay, por lo que usamos posiciones locales directamente
        Vector2 localTarget = target.anchoredPosition;

        Debug.Log($"🎯 Movimiento: {imageCharacter.anchoredPosition} → {localTarget} → {finalCharacterPos}");

        // 🔹 Movimiento parabólico hasta el botón
        yield return MoveToCurve(imageCharacter, localTarget);

        // 🔹 Espera breve
        yield return new WaitForSeconds(0.3f);

        // 🔹 Movimiento parabólico hacia la posición final
        yield return MoveToCurve(imageCharacter, finalCharacterPos);

        // (Opcional) volver al inicio
        yield return new WaitForSeconds(0.3f);
        imageCharacter.anchoredPosition = startCharacterPos;

        isAnimating = false;
    }



    private IEnumerator MoveTo(RectTransform element, Vector2 targetPos)
    {
        Vector2 start = element.anchoredPosition;
        float time = 0f;
        while (time < moveDuration)
        {
            float t = moveCurve.Evaluate(time / moveDuration);
            Vector2 newPos = Vector2.Lerp(start, targetPos, t);
            newPos.y += Mathf.Sin(t * Mathf.PI) * arcHeight;
            element.anchoredPosition = newPos;
            time += Time.deltaTime;
            yield return null;
        }
        element.anchoredPosition = targetPos;
    }
    // ============================================================
    // 🌀 Movimiento parabólico (trayectoria curva)
    // ============================================================
    private IEnumerator MoveToCurve(RectTransform element, Vector2 targetPos)
    {
        Vector2 start = element.anchoredPosition;
        float time = 0f;

        while (time < moveDuration)
        {
            float t = moveCurve.Evaluate(time / moveDuration);

            // Trayectoria curva (parábola)
            Vector2 newPos = Vector2.Lerp(start, targetPos, t);
            newPos.y += Mathf.Sin(t * Mathf.PI) * arcHeight;

            element.anchoredPosition = newPos;
            time += Time.deltaTime;
            yield return null;
        }

        element.anchoredPosition = targetPos;
    }
    // ============================================================
    // 🚀 Cambiar de escena de forma genérica
    // ============================================================
    private void ChangeScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("⚠️ No se ha asignado el nombre de la escena destino.");
            return;
        }

        Debug.Log($"🌍 Cambiando a la escena: {sceneName}");
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

}
