using Picofon.Games.Judge;
using Picofon.Games.Relate;
using Picofon.Games.Select;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;




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


    private ActivityJudge lastActivityShown;

    private int currentTaskType = 1;


    // 🧍‍♂️ Personaje
    private RectTransform imageCharacter;
    private readonly float moveDuration = 0.8f;
    private readonly float arcHeight = 150f;
    private readonly AnimationCurve moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Vector2 startCharacterPos = new(250, 200);
    private Vector2 finalCharacterPos = new(1700, 800);
    private bool isAnimating = false;

    private List<GameObject> spawnedButtons = new();
    private int currentMode = 0;
    private GameAPIService apiService;


    private List<ActivityJudge> judgeActivities = new();
    private List<ActivitySelect> selectActivities = new();
    private List<ActivityRelate> relateActivities = new();

    private int currentActivityIndex = 0;

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
        {
            imageCharacter.anchoredPosition = startCharacterPos;

            // No detener animación al inicio porque aún no ha empezado
            if (imageCharacter.TryGetComponent(out CharacterAnimator anim))
                anim.SetIdleFrame(); // Nueva función que pondremos

        }
    }


    private void Start()
    {
        apiService = FindObjectOfType<GameAPIService>();
        if (apiService == null)
        {
            Debug.LogError(" GameAPIService no encontrado.");
            return;
        }

  
        currentTaskType = apiService.GetCurrentTaskType();
        Debug.Log($" CrossRiver Start() → task_type_id detectado = {currentTaskType}");

        AssignModeButtons();

        StartCoroutine(LoadModeFromAPI(currentTaskType));
    }

    // ============================================================
    // 🔹 Carga dinámica de modos
    // ============================================================
    private IEnumerator LoadModeFromAPI(int mode)
    {
        currentTaskType = mode;

        if (apiService == null)
        {
            Debug.LogError(" No se encontró GameAPIService.");
            yield break;
        }

        Debug.Log($" CrossRiver: Loading mode = {mode}");

        yield return apiService.LoadActivity(
            json => LoadMode(mode, json),
            err => Debug.LogError($" CrossRiver API error: {err}")
        );

        // Important change because have a new format API.  
        //  yield return apiService.LoadActivity(mode,
        //      json => LoadMode(mode, json),
        //      err => Debug.LogError(err));
    }

    public void LoadMode(int mode, string json)
    {
        ClearButtons();

        if (string.IsNullOrEmpty(json))
        {
            Debug.LogError(" JSON vacío en LoadMode");
            return;
        }

        Debug.Log($" CrossRiver → procesando JSON para modo {mode}");

        switch (mode)
        {
            case 1:
                var judgeData = JsonUtility.FromJson<ApiResponseJudge>(json);

                if (judgeData?.data == null)
                {
                    Debug.LogError(" JSON sin data (Judge)");
                    return;
                }

                // limpiar lista
                judgeActivities.Clear();
                currentActivityIndex = 0;

                // agregar actividades
                if (judgeData.data.activity1 != null) judgeActivities.Add(judgeData.data.activity1);
                if (judgeData.data.activity2 != null) judgeActivities.Add(judgeData.data.activity2);
                if (judgeData.data.activity3 != null) judgeActivities.Add(judgeData.data.activity3);
                if (judgeData.data.activity4 != null) judgeActivities.Add(judgeData.data.activity4);
                if (judgeData.data.activity5 != null) judgeActivities.Add(judgeData.data.activity5);

                LoadJudgeMode(judgeActivities[currentActivityIndex]);
                break;


            case 2:
                var selectData = JsonUtility.FromJson<ApiResponseSelect>(json);

                if (selectData?.data == null)
                {
                    Debug.LogError(" JSON sin data (Select)");
                    return;
                }

                selectActivities.Clear();
                currentActivityIndex = 0;

                if (selectData.data.activity1 != null) selectActivities.Add(selectData.data.activity1);
                if (selectData.data.activity2 != null) selectActivities.Add(selectData.data.activity2);
                if (selectData.data.activity3 != null) selectActivities.Add(selectData.data.activity3);


                LoadSelectMode(selectActivities[currentActivityIndex]);
                break;


            case 3:
                var relateData = JsonUtility.FromJson<ApiResponseRelate>(json);

                if (relateData?.data == null)
                {
                    Debug.LogError("❌ JSON sin data (Relate)");
                    return;
                }

                relateActivities.Clear();
                currentActivityIndex = 0;

                if (relateData.data.activity1 != null) relateActivities.Add(relateData.data.activity1);
                if (relateData.data.activity2 != null) relateActivities.Add(relateData.data.activity2);


                LoadRelateMode(relateActivities[currentActivityIndex]);
                break;


            default:
                Debug.LogError($" task_type_id no soportado: {mode}");
                break;
        }
    }


    // ============================================================
    // 🧩 Normaliza campos PATH del JSON (para RELATE o SELECT)
    // ============================================================
    private void NormalizePaths_Relate(Picofon.Games.Relate.ActivityRelate activity)
    {
        if (activity == null) return;

        void Fix(ref string path)
        {
            if (!string.IsNullOrEmpty(path))
                path = path.Trim();
        }

        Fix(ref activity.main_word.PATH);
        Fix(ref activity.correct_option.PATH);
        Fix(ref activity.wrong_option1.PATH);
        Fix(ref activity.wrong_option2.PATH);
        Fix(ref activity.wrong_option3.PATH);

        // Log completo
        Debug.Log($"📂 PATHs normalizados:\n" +
                  $" main={activity.main_word.PATH}\n" +
                  $" correct={activity.correct_option.PATH}\n" +
                  $" wrong1={activity.wrong_option1.PATH}\n" +
                  $" wrong2={activity.wrong_option2.PATH}\n" +
                  $" wrong3={activity.wrong_option3.PATH}");
    }

    // ============================================================
    // 🟣 MODO 2 – RELATE (una imagen principal + 4 opciones)
    // ============================================================
    private void LoadRelateMode(Picofon.Games.Relate.ActivityRelate activity)
    {
        ClearButtons();

        if (firstImage != null)
            firstImage.gameObject.SetActive(false);
        if (secondImage != null)
            secondImage.gameObject.SetActive(false);

        if (questionText != null)
            questionText.text = activity.question;


        NormalizePaths_Relate(activity);


        if (imageMain != null)
        {
            imageMain.gameObject.SetActive(true);
            imageMain.sprite = LoadLocalSprite(activity.main_word.PATH);
            imageMain.preserveAspect = true;
        }

        // 🖼️ Carga los sprites de las opciones
        Sprite correctSprite = LoadLocalSprite(activity.correct_option.PATH);
        Sprite wrong1Sprite = LoadLocalSprite(activity.wrong_option1.PATH);
        Sprite wrong2Sprite = LoadLocalSprite(activity.wrong_option2.PATH);
        Sprite wrong3Sprite = LoadLocalSprite(activity.wrong_option3.PATH);

        // ✅ Verificación: si alguna imagen no existe
        if (correctSprite == null) Debug.LogWarning("⚠️ No se cargó sprite del correct_option.");
        if (wrong1Sprite == null) Debug.LogWarning("⚠️ No se cargó sprite del wrong_option1.");
        if (wrong2Sprite == null) Debug.LogWarning("⚠️ No se cargó sprite del wrong_option2.");
        if (wrong3Sprite == null) Debug.LogWarning("⚠️ No se cargó sprite del wrong_option3.");

        // 🎲 Lista de opciones
        List<(Sprite sprite, bool isCorrect, string word, string syllWord)> options = new()
    {
        (correctSprite, true,  activity.correct_option.word,  activity.correct_option.syllabified_word),
        (wrong1Sprite,  false, activity.wrong_option1.word,  activity.wrong_option1.syllabified_word),
        (wrong2Sprite,  false, activity.wrong_option2.word,  activity.wrong_option2.syllabified_word),
        (wrong3Sprite,  false, activity.wrong_option3.word,  activity.wrong_option3.syllabified_word)
    };

        // 🔀 Posiciones aleatorias
        List<Vector2> shuffledPositions = mode2Positions.OrderBy(_ => UnityEngine.Random.value).ToList();

        // 🎨 Crear prefabs
        for (int i = 0; i < options.Count; i++)
        {
            var opt = options[i];
            GameObject btnObj = Instantiate(buttonPrefab, buttonContainer);
            btnObj.name = $"Button_Relate_{i + 1}";

            RectTransform rect = btnObj.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = new Vector2(0, 0);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = shuffledPositions[i];

            // Imagen principal del botón
            Transform imgTf = btnObj.transform.Find("Image");
            if (imgTf != null)
            {
                Image img = imgTf.GetComponent<Image>();
                if (img != null && opt.sprite != null)
                {
                    img.sprite = opt.sprite;
                    img.preserveAspect = true;
                    img.color = Color.white;
                }
                else
                {
                    Debug.LogWarning($"⚠️ Imagen no encontrada en prefab {btnObj.name}");
                }
            }

            // Decorativo Lifebelt
            Transform lifebelt = btnObj.transform.Find("BackgroundLifebelt");
            if (lifebelt != null && lifebelt.TryGetComponent(out Image bg))
            {
                Sprite lifebeltSprite = Resources.Load<Sprite>("Images/Images/CrossRiver/lifebelt_violet_Lluni");
                if (lifebeltSprite != null)
                {
                    bg.sprite = lifebeltSprite;
                    bg.preserveAspect = true;
                }
            }

            // Listener del botón
            Transform buttonOp = btnObj.transform.Find("ButtonOp");
            if (buttonOp != null && buttonOp.TryGetComponent(out Button button))
            {
                bool isCorrect = opt.isCorrect;
                Sprite selectedSprite = opt.sprite;
                string selectedWord = opt.syllWord;
                RectTransform targetRect = rect;

                button.onClick.AddListener(() =>
                {
                    EvaluateRelate(activity, isCorrect, selectedSprite, selectedWord, targetRect);
                });
            }

            // Ocultar texto
            TMP_Text txt = btnObj.GetComponentInChildren<TMP_Text>();
            if (txt != null) txt.gameObject.SetActive(false);

            spawnedButtons.Add(btnObj);
        }

        Debug.Log($"🟣 Modo Relate cargado: main={activity.main_word.word}, correct={activity.correct_option.word}");
    }


    // ============================================================
    // 🎯 Evaluar respuesta modo RELATE
    // ============================================================
    private void EvaluateRelate(Picofon.Games.Relate.ActivityRelate activity,
                                bool isCorrect, Sprite chosenSprite, string chosenSyllWord, RectTransform pressedBtnRect)
    {
        if (isAnimating) return;

        // 🔹 Datos del main_word (para feedback)
        Sprite mainSprite = LoadLocalSprite(activity.main_word.PATH);
        string mainSyllWord = activity.main_word.syllabified_word;

        // 🔹 Mensaje de feedback según el botón seleccionado
        string feedbackMessage = isCorrect ? activity.feedback_positive : activity.feedback_neutral;

        // 🔹 Inicia animación + feedback (reutiliza la del modo 1)
        StartCoroutine(HandleFeedback_Relate_WithAnim(
            correct: isCorrect,
            targetRect: pressedBtnRect,
            mainSprite: mainSprite,
            chosenSprite: chosenSprite,
            mainWord: mainSyllWord,
            chosenWord: chosenSyllWord,
            message: feedbackMessage
        ));
    }

    // ============================================================
    // 🎬 Feedback + animación para modo RELATE
    // ============================================================
    private IEnumerator HandleFeedback_Relate_WithAnim(
        bool correct,
        RectTransform targetRect,
        Sprite mainSprite,
        Sprite chosenSprite,
        string mainWord,
        string chosenWord,
        string message)
    {
        isAnimating = true;

        // 1️⃣ Movimiento hacia el botón presionado
        if (imageCharacter != null && targetRect != null)
            yield return MoveToCurve(imageCharacter, targetRect.anchoredPosition);

        // 2️⃣ Si es correcto → también ir al punto final
        if (correct && imageCharacter != null)
        {
            yield return new WaitForSeconds(0.2f);
            yield return MoveToCurve(imageCharacter, finalCharacterPos);
            yield return new WaitForSeconds(0.3f);
        }

        // 3️⃣ Mostrar feedback con las dos imágenes (main + elegida)
        if (feedbackController != null)
        {
            feedbackController.ShowFeedback(
                mainSprite,     // Imagen de main_word
                chosenSprite,   // Imagen del prefab seleccionado
                correct,
                mainWord,
                chosenWord
            );
        }

        // 4️⃣ Esperar mientras se muestra el feedback
        yield return new WaitForSeconds(3f);

        // 5️⃣ Volver a la posición inicial
        if (imageCharacter != null)
            yield return MoveToCurve(imageCharacter, startCharacterPos);

        // 6️⃣ Si fue correcto, recargar nueva actividad
        if (correct)
        {
            yield return StartCoroutine(NextActivity());
        }




        isAnimating = false;
    }



    // ============================================================
    // 🔵 MODO 1 (usando modelo SELECT con 4 imágenes del JSON)
    // ============================================================
    private void LoadSelectMode(ActivitySelect activity)
    {
        if (imageMain != null)
            imageMain.gameObject.SetActive(false);

        if (firstImage != null) 
            firstImage.gameObject.SetActive(false);
        if (secondImage != null) 
            secondImage.gameObject.SetActive(false);

        if (questionText != null)
            questionText.text = activity.question;

        ClearButtons();

        // 🖼️ Carga los 4 sprites desde Resources
        Sprite mainSprite = LoadLocalSprite(activity.main_word.PATH);
        Sprite correctSprite = LoadLocalSprite(activity.correct_option.PATH);
        Sprite wrong1Sprite = LoadLocalSprite(activity.wrong_option1.PATH);
        Sprite wrong2Sprite = LoadLocalSprite(activity.wrong_option2.PATH);

        // ============================================================
        // 🧩 Crear lista de opciones y ubicar el correct_option en posición aleatoria
        // ============================================================

        // Posiciones base
        List<Vector2> positions = mode1Positions.ToList();

        // Índice aleatorio donde irá la opción correcta (0–3)
        int randomCorrectIndex = UnityEngine.Random.Range(0, positions.Count);

        // Crea una lista vacía para las opciones finales
        List<(Sprite sprite, bool isCorrect, string word)> options = new();

        // Genera las opciones, pero coloca la correcta en la posición aleatoria
        for (int i = 0; i < 4; i++)
        {
            if (i == randomCorrectIndex)
                options.Add((correctSprite, true, activity.correct_option.word));
            else
            {
                // Selecciona el siguiente incorrecto disponible
                if (options.Count(x => !x.isCorrect) == 0)
                    options.Add((mainSprite, false, activity.main_word.word));
                else if (options.Count(x => !x.isCorrect) == 1)
                    options.Add((wrong1Sprite, false, activity.wrong_option1.word));
                else
                    options.Add((wrong2Sprite, false, activity.wrong_option2.word));
            }
        }


        // 🔹 Si quieres que se muestren siempre en el orden original, comenta la siguiente línea:
        // options = options.OrderBy(x => Random.value).ToList();

        // ============================================================
        // 🎨 Crear los 4 prefabs y colocar las imágenes
        // ============================================================
        for (int i = 0; i < options.Count; i++)
        {
            GameObject btnObj = Instantiate(buttonPrefab, buttonContainer);
            btnObj.name = $"Button_Select_{i + 1}";

            RectTransform rect = btnObj.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = mode1Positions[i]; // usa las 4 posiciones predefinidas

            // =============================
            // 🖼️ Imagen principal del botón
            // =============================
            Transform imageChild = btnObj.transform.Find("Image");
            if (imageChild != null && options[i].sprite != null)
            {
                Image img = imageChild.GetComponent<Image>();
                img.sprite = options[i].sprite;
                img.preserveAspect = true;
                img.color = Color.white;
                imageChild.gameObject.SetActive(true);
            }

            // =============================
            // ⚓ Lifebelt decorativo
            // =============================
            Transform lifebelt = btnObj.transform.Find("BackgroundLifebelt");
            if (lifebelt != null)
            {
                Image bg = lifebelt.GetComponent<Image>();
                if (bg != null)
                {
                    Sprite lifebeltSprite = Resources.Load<Sprite>("Images/Images/CrossRiver/lifebelt_violet_Lluni");
                    if (lifebeltSprite != null)
                    {
                        bg.sprite = lifebeltSprite;
                        bg.preserveAspect = true;
                    }
                }
            }

            // =============================
            // 🔘 Listener de selección
            // =============================
            // 🔘 Listener de selección
            Transform buttonOp = btnObj.transform.Find("ButtonOp");
            if (buttonOp != null && buttonOp.TryGetComponent(out Button button))
            {
                bool isCorrect = options[i].isCorrect;
                Sprite selectedSprite = options[i].sprite;
                RectTransform targetRect = rect; // 👈 capturamos a dónde animar

                button.onClick.AddListener(() =>
                {
                    EvaluateSelect(activity, isCorrect, selectedSprite, targetRect); // 👈 nuevo parámetro
                });
            }


            // ❌ No mostrar texto en los botones del modo Select
            TMP_Text label = btnObj.GetComponentInChildren<TMP_Text>();
            if (label != null)
            {
                label.text = "";                     // Quita el texto
                label.gameObject.SetActive(false);   // Oculta el componente visual
            }


            spawnedButtons.Add(btnObj);
        }

        Debug.Log($"🔵 Modo Select cargado con 4 imágenes: {activity.main_word.word}, {activity.correct_option.word}, {activity.wrong_option1.word}, {activity.wrong_option2.word}");
    }


    // ============================================================
    // 🎯 Evaluación de respuestas modo Select — usa syllabified_word del modelo
    // ============================================================
    // Antes:
    // private void EvaluateSelect(ActivitySelect activity, bool isCorrect, Sprite chosenSprite)

    // Ahora:
    private void EvaluateSelect(ActivitySelect activity, bool isCorrect, Sprite chosenSprite, RectTransform pressedBtnRect)
    {
        if (isAnimating) return;

        string chosenSyllWord = GetSyllabifiedWordBySprite(activity, chosenSprite);
        string otherSyllWord;
        Sprite otherSprite;

        if (isCorrect)
        {
            // ✅ correcto → mostrar opción correcta + otra aleatoria
            otherSyllWord = PickRandomWord(new[]
            {
            activity.main_word.syllabified_word,
            activity.wrong_option1.syllabified_word,
            activity.wrong_option2.syllabified_word
        });

            otherSprite = PickRandomSprite(new[]
            {
            LoadLocalSprite(activity.main_word.PATH),
            LoadLocalSprite(activity.wrong_option1.PATH),
            LoadLocalSprite(activity.wrong_option2.PATH)
        });

            StartCoroutine(HandleFeedback_Select_WithAnim(
                correct: true,
                targetRect: pressedBtnRect,
                sprite1: LoadLocalSprite(activity.correct_option.PATH),
                sprite2: otherSprite,
                word1: activity.correct_option.syllabified_word,
                word2: otherSyllWord
            ));
        }
        else
        {
            // ❌ incorrecto → palabra elegida + otra aleatoria
            otherSyllWord = PickRandomWord(new[]
            {
            activity.main_word.syllabified_word,
            activity.wrong_option1.syllabified_word,
            activity.wrong_option2.syllabified_word
        }, exclude: chosenSyllWord);

            otherSprite = PickRandomSprite(new[]
            {
            LoadLocalSprite(activity.main_word.PATH),
            LoadLocalSprite(activity.wrong_option1.PATH),
            LoadLocalSprite(activity.wrong_option2.PATH)
        }, excludeSprite: chosenSprite);

            StartCoroutine(HandleFeedback_Select_WithAnim(
                correct: false,
                targetRect: pressedBtnRect,
                sprite1: chosenSprite,
                sprite2: otherSprite,
                word1: chosenSyllWord,
                word2: otherSyllWord
            ));
        }
    }

    // 🎨 Feedback + animación (modo Select) replicando el patrón del modo Judge
    private IEnumerator HandleFeedback_Select_WithAnim(
        bool correct,
        RectTransform targetRect,
        Sprite sprite1,
        Sprite sprite2,
        string word1,
        string word2)
    {
        isAnimating = true;

        // 1) Ir al botón pulsado
        if (imageCharacter != null && targetRect != null)
            yield return MoveToCurve(imageCharacter, targetRect.anchoredPosition);

        // 2) Si es correcto, ir también al punto final (como en HandleCorrectFeedback de modo 0)
        if (correct && imageCharacter != null)
        {
            yield return new WaitForSeconds(0.2f);
            yield return MoveToCurve(imageCharacter, finalCharacterPos);
            yield return new WaitForSeconds(0.3f);
        }

        // 3) Mostrar feedback (imágenes + palabras silábicas)
        if (feedbackController != null)
        {
            feedbackController.ShowFeedback(
                sprite1,
                sprite2,
                correct,
                word1,
                word2
            );
        }

        // 4) Esperar a que el panel de feedback se muestre
        yield return new WaitForSeconds(3f);

        // 5) Volver al punto inicial
        if (imageCharacter != null)
            yield return MoveToCurve(imageCharacter, startCharacterPos);

        // 6) Recargar la actividad del modo actual si fue correcta (igual que en modo 0)
        if (correct)
            yield return StartCoroutine(LoadModeFromAPI(currentTaskType));


        isAnimating = false;
    }




    // ============================================================
    // 🎨 Feedback visual — solo imágenes (sin texto) y recarga segura del modo
    // ============================================================
    private IEnumerator HandleFeedback_Select(bool correct, Sprite sprite1, Sprite sprite2, string word1, string word2)
    {
        isAnimating = true;

        if (feedbackController != null)
        {
            // Muestra las dos imágenes y las palabras asociadas
            feedbackController.ShowFeedback(
                sprite1,
                sprite2,
                correct,
                word1,
                word2
            );
        }

        // Espera antes de cambiar
        yield return new WaitForSeconds(2.5f);

        // 🔹 Si la respuesta es correcta, se recarga el modo Select
        if (correct)
        {
            Debug.Log("🔁 Recargando actividad del modo Select...");

            // Aseguramos que el servicio esté disponible
            if (apiService == null)
            {
                apiService = FindObjectOfType<GameAPIService>();
                if (apiService == null)
                {
                    Debug.LogError("❌ No se encontró GameAPIService para recargar la actividad.");
                    yield break;
                }
            }

            // Nueva petición segura al backend
            yield return StartCoroutine(LoadModeFromAPI(currentTaskType));

        }

        isAnimating = false;
    }



    // ============================================================
    // 🧠 Funciones auxiliares
    // ============================================================
    private string PickRandomWord(IEnumerable<string> words, string exclude = null)
    {
        var list = words.Where(w => w != exclude && !string.IsNullOrEmpty(w)).ToList();
        if (list.Count == 0) return "";
        return list[UnityEngine.Random.Range(0, list.Count)];
    }

    private Sprite PickRandomSprite(IEnumerable<Sprite> sprites, Sprite excludeSprite = null)
    {
        var list = sprites.Where(s => s != null && s != excludeSprite).ToList();
        if (list.Count == 0) return null;
        return list[UnityEngine.Random.Range(0, list.Count)];
    }
    // ============================================================
    // 🔡 Obtiene la palabra silábica (syllabified_word) según el sprite
    // ============================================================
    private string GetSyllabifiedWordBySprite(ActivitySelect activity, Sprite sprite)
    {
        if (sprite == null) return "";

        string spriteName = sprite.name.ToLower();
        if (spriteName.Contains(System.IO.Path.GetFileNameWithoutExtension(activity.main_word.PATH).ToLower()))
            return activity.main_word.syllabified_word;
        if (spriteName.Contains(System.IO.Path.GetFileNameWithoutExtension(activity.correct_option.PATH).ToLower()))
            return activity.correct_option.syllabified_word;
        if (spriteName.Contains(System.IO.Path.GetFileNameWithoutExtension(activity.wrong_option1.PATH).ToLower()))
            return activity.wrong_option1.syllabified_word;
        if (spriteName.Contains(System.IO.Path.GetFileNameWithoutExtension(activity.wrong_option2.PATH).ToLower()))
            return activity.wrong_option2.syllabified_word;

        return "?";
    }




    // ============================================================
    // ✅ Feedback correcto (modo Select)
    // ============================================================
    private IEnumerator HandleCorrectFeedback_Select(ActivitySelect activity, string message)
    {
        isAnimating = true;

        ShowFeedback(message, true);
        yield return new WaitForSeconds(2.5f);

        // Recargar siguiente actividad del mismo modo
        StartCoroutine(LoadModeFromAPI(currentTaskType));


        isAnimating = false;
    }

    // ============================================================
    // ❌ Feedback incorrecto (modo Select)
    // ============================================================
    private IEnumerator HandleIncorrectFeedback_Select(ActivitySelect activity, string message)
    {
        ShowFeedback(message, false);
        yield return new WaitForSeconds(2.5f);
    }

    // ============================================================
    // 🟣 Modo Judge (Sí / No)
    // ============================================================
    private void LoadJudgeMode(ActivityJudge activity)
    {
        if (imageMain != null)
            imageMain.gameObject.SetActive(false);
        if (firstImage != null) 
            firstImage.gameObject.SetActive(false);
        if (secondImage != null) 
            secondImage.gameObject.SetActive(false);
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
                        spritePath = "Images/CrossRiver/lifebelt_pink_Lluni";
                    else
                        spritePath = "Images/CrossRiver/lifebelt_violet_Lluni";

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
        StartCoroutine(LoadModeFromAPI(currentTaskType));


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

    // ============================================================
    // 🧭 Carga sprite desde ImgButtons usando el PATH del JSON
    // ============================================================
    private Sprite LoadLocalSprite(string imageName)
    {
        if (string.IsNullOrEmpty(imageName))
        {
            Debug.LogWarning($"⚠️ LoadLocalSprite: nombre vacío. PATH no recibido (modo={currentMode}).");
            return null;
        }


        // 🧩 Limpieza del nombre y armado del path
        string fileName = System.IO.Path.GetFileNameWithoutExtension(imageName).Trim();
        string path = $"Images/ImgButtons/{fileName}";

        // 🔍 Mostrar el path que se intentará cargar
        Debug.Log($"🔎 Intentando cargar sprite desde: Resources/{path} (original PATH='{imageName}')");

        // Cargar el sprite desde Resources
        Sprite sprite = Resources.Load<Sprite>(path);

        if (sprite != null)
        {
            Debug.Log($"✅ Sprite encontrado correctamente en: Resources/{path}");
            return sprite;
        }
        else
        {
            Debug.LogWarning($"❌ Sprite NO encontrado. PATH original del JSON: '{imageName}' → Ruta buscada: Resources/{path}");
            return null;
        }
    }


    private void AssignModeButtons()
    {
        if (topButtonsContainer == null)
            topButtonsContainer = GameObject.Find("TopButtons")?.transform;

        if (topButtonsContainer == null)
        {
            Debug.LogWarning(" No se encontró TopButtons.");
            return;
        }

        for (int i = 0; i < 3; i++)
        {
            Transform button = topButtonsContainer.Find($"Button{i}");
            if (button != null && button.TryGetComponent(out Button btn))
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() =>
                {
                    currentTaskType = apiService.GetCurrentTaskType();
                    Debug.Log($" Recargando modo real: {currentTaskType}");
                    StartCoroutine(LoadModeFromAPI(currentTaskType));
                });

                Debug.Log($" Botón Button{i} asignado (recarga task_type).");
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
        CharacterAnimator animController = null;
        if (element != null && element.TryGetComponent(out animController))
        {
            // ⏯️ Reproducir animación una vez durante el salto
            StartCoroutine(animController.PlayOnce());
        }

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

        // ✅ asegurar idle al final
        if (animController != null)
            animController.SetIdleFrame();
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

    // ============================================================
    // 🧩 Deserialización segura para RELATE (usando JsonUtility nativo)
    // ============================================================
    private Picofon.Games.Relate.ActivityRelate ParseRelateActivity(string json)
    {
        if (string.IsNullOrEmpty(json))
        {
            Debug.LogError("❌ JSON vacío, no se puede parsear modo Relate.");
            return null;
        }

        try
        {
            // 🔹 Usa el modelo ApiResponseRelate directamente
            var response = JsonUtility.FromJson<Picofon.Games.Relate.ApiResponseRelate>(json);

            if (response == null)
            {
                Debug.LogError("❌ No se pudo deserializar el JSON en ApiResponseRelate.");
                return null;
            }

            if (response.data == null)
            {
                Debug.LogError("❌ JSON no contiene 'data'.");
                return null;
            }

            // Puedes elegir qué actividad cargar (por defecto la primera)
            var activity = response.data.activity1;

            if (activity == null)
            {
                Debug.LogError("❌ JSON no contiene 'activity1' en 'data'.");
                return null;
            }

            return activity;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"❌ Error al parsear modo Relate: {ex.Message}");
            return null;
        }
    }
    private IEnumerator NextActivity()
    {
        yield return new WaitForSeconds(0.5f);

        currentActivityIndex++;

        // Judge
        if (currentTaskType == 1)
        {
            if (currentActivityIndex < judgeActivities.Count)
            {
                LoadJudgeMode(judgeActivities[currentActivityIndex]);
                yield break;
            }
        }

        // Select
        if (currentTaskType == 2)
        {
            if (currentActivityIndex < selectActivities.Count)
            {
                LoadSelectMode(selectActivities[currentActivityIndex]);
                yield break;
            }
        }

        // Relate
        if (currentTaskType == 3)
        {
            if (currentActivityIndex < relateActivities.Count)
            {
                LoadRelateMode(relateActivities[currentActivityIndex]);
                yield break;
            }
        }

        // Si ya no quedan actividades → pedir nuevo set
        Debug.Log(" FIN DEL GRUPO → Cargar nuevo set");
        currentActivityIndex = 0; 
        StartCoroutine(LoadModeFromAPI(currentTaskType));
    }




}