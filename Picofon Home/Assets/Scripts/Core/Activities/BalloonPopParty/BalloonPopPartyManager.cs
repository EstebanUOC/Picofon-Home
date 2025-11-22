using System.Collections;
using System.Collections.Generic;
using System; // 🔥 ADD THIS for Exception class
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Picofon.Games.Judge;
using Picofon.Games.Relate;
using Picofon.Games.Select;

// ✅ Alias para evitar ambigüedad entre modelos
using JudgeWord = Picofon.Games.Judge.WordData;
using RelateWord = Picofon.Games.Relate.WordData;

public class BalloonPopPartyManager : MonoBehaviour
{
    [Header("🔗 Referencias principales")]
    [SerializeField] private GameAPIService apiService;
    [SerializeField] private FeedbackPanelController feedbackPanel;
    [SerializeField] private GameObject balloonPrefab;
    [SerializeField] private RectTransform container;
    [SerializeField] private Button buttonYes;
    [SerializeField] private Button buttonNo;
    [SerializeField] private Image imageMain; // 🔹 Referencia en el Canvas (asignar en Inspector)
    [SerializeField] private Image imageMainBack; // imagen cupcake

    private ActivityJudge currentJudgeActivity;
    private int currentTaskType = 1; // 🔥 1=Judge, 2=Select, 3=Relate (from TherapyPlan)

    // 🔹 Espaciado solo para modo Judge (en píxeles)
    private const float JudgeContainerSpacing = 100f;

    private readonly List<GameObject> spawnedBalloons = new();

    // ============================================================
    private void Start()
    {
        if (apiService == null)
            apiService = FindObjectOfType<GameAPIService>();

        if (feedbackPanel == null)
            feedbackPanel = FindObjectOfType<FeedbackPanelController>();

        // 🎯 Obtener el tipo de tarea del TherapyPlan actual
        currentTaskType = apiService.GetCurrentTaskType();
        Debug.Log($"🎮 Tipo de tarea detectado: {currentTaskType}");

        // ✅ Iniciar automáticamente la actividad
        StartCoroutine(LoadCurrentActivity());
    }

    // ============================================================
    // 🔥 NEW METHOD - Load activity based on TherapyPlan
    // ============================================================
    private IEnumerator LoadCurrentActivity()
    {
        ClearBalloons();

        if (apiService == null)
        {
            Debug.LogError("❌ GameAPIService no encontrado.");
            yield break;
        }

        yield return apiService.LoadActivity(
            json => ProcessActivityResponse(json),
            err => Debug.LogError(err));
    }

    // ============================================================
    // 🔥 UPDATED METHOD - Process response based on current task type
    // ============================================================
    private void ProcessActivityResponse(string json)
    {
        try
        {
            if (string.IsNullOrEmpty(json))
            {
                Debug.LogError("❌ JSON response is null or empty");
                return;
            }

            Debug.Log($"📄 Processing JSON: {json}");

            ClearBalloons();
            
            // Ocultar imagen background principal fuera del modo Relate
            if (imageMainBack != null)
                imageMainBack.gameObject.SetActive(currentTaskType == 3); // 🔥 3 = Relate
            
            // Ocultar imagen principal fuera del modo Relate
            if (imageMain != null)
                imageMain.gameObject.SetActive(currentTaskType == 3); // 🔥 3 = Relate
            
            // 🔹 Ocultar botones Yes/No por defecto
            buttonYes.gameObject.SetActive(false);
            buttonNo.gameObject.SetActive(false);
            
            // Ajustar el espaciado del contenedor según el modo
            var layout = container.GetComponent<HorizontalLayoutGroup>();
            if (layout != null)
            {
                if (currentTaskType == 1) // 🔥 1 = Judge
                {
                    layout.spacing = JudgeContainerSpacing;
                    layout.childAlignment = TextAnchor.MiddleCenter;
                    layout.childForceExpandWidth = false;
                    layout.childForceExpandHeight = false;
                }
                else // Modo Select o Relate
                {
                    // Restaurar configuración por defecto (sin separación)
                    layout.spacing = 0;
                    layout.childAlignment = TextAnchor.UpperLeft;
                    layout.childForceExpandWidth = true;
                    layout.childForceExpandHeight = true;
                }
            }

            switch (currentTaskType)
            {
                case 1: // Judge
                    var judgeData = JsonUtility.FromJson<Picofon.Games.Judge.ApiResponseJudge>(json);
                    if (judgeData?.data?.activity1 != null)
                    {
                        LoadJudgeMode(judgeData.data.activity1);
                        Debug.Log($"✅ Successfully loaded Judge activity: {judgeData.data.activity1.word1.word} vs {judgeData.data.activity1.word2.word}");
                    }
                    else
                    {
                        Debug.LogError("❌ Datos Judge inválidos o nulos");
                    }
                    break;

                case 2: // Select
                    var selectData = JsonUtility.FromJson<ApiResponseSelect>(json);
                    if (selectData?.data?.activity1 != null)
                    {
                        LoadSelectMode(selectData.data.activity1);
                    }
                    else
                    {
                        Debug.LogError("❌ Datos Select inválidos o nulos");
                    }
                    break;

                case 3: // Relate
                    var relateData = JsonUtility.FromJson<ApiResponseRelate>(json);
                    if (relateData?.data?.activity1 != null)
                    {
                        LoadRelateMode(relateData.data.activity1);
                    }
                    else
                    {
                        Debug.LogError("❌ Datos Relate inválidos o nulos");
                    }
                    break;

                default:
                    Debug.LogError($"❌ Tipo de tarea no soportado: {currentTaskType}");
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ Error procesando respuesta JSON: {e.Message}");
            Debug.LogError($"🔍 Stack trace: {e.StackTrace}");
            Debug.LogError($"📄 Problematic JSON: {json}");
        }
    }

    // ============================================================
    // 🟢 MODO 1 — JUDGE (clic → animación 4 frames → feedback)
    // ============================================================
    private void LoadJudgeMode(ActivityJudge activity)
    {
        currentJudgeActivity = activity;

        ClearBalloons();

        // =========================
        // Crear 2 globos con las imágenes
        // Important: remember is important the width of teh prefab, if this is big,  the positions of the vector did not modify.
        // =========================
        Sprite sprite1 = LoadLocalSprite(activity.word1.PATH);
        Sprite sprite2 = LoadLocalSprite(activity.word2.PATH);

        Vector2[] positions = { new Vector2(-150, 0), new Vector2(150, 0) };
        Sprite[] sprites = { sprite1, sprite2 };

        for (int i = 0; i < 2; i++)
        {
            GameObject balloon = Instantiate(balloonPrefab, container);
            balloon.name = $"BalloonJudge_{i + 1}";
            balloon.GetComponent<RectTransform>().anchoredPosition = positions[i];
            spawnedBalloons.Add(balloon);

            // Imagen dentro del prefab
            Image img = balloon.transform.Find("Image")?.GetComponent<Image>();
            if (img != null)
            {
                img.sprite = sprites[i];
                img.preserveAspect = true;
            }

            // 🧩 🔹 OCULTAR texto del prefab si existe (solo modo Judge)
            TMP_Text txt = balloon.GetComponentInChildren<TMP_Text>();
            if (txt != null)
                txt.gameObject.SetActive(false);

            // Desactivar el botón interno del prefab (solo se usan los botones externos Yes/No)
            Button internalBtn = balloon.GetComponentInChildren<Button>();
            if (internalBtn != null)
                internalBtn.interactable = false;
        }

        // =========================
        // Configurar botones Yes/No
        // =========================
        buttonYes.gameObject.SetActive(true);
        buttonNo.gameObject.SetActive(true);
        buttonYes.interactable = true; 
        buttonNo.interactable = true;   

        buttonYes.onClick.RemoveAllListeners();
        buttonNo.onClick.RemoveAllListeners();

        buttonYes.onClick.AddListener(() =>
        {
            foreach (var b in spawnedBalloons)
            {
                var controller = b.GetComponent<BalloonController>();
                if (controller == null) continue;

                controller.OnExplosionFinished = (_) =>
                {
                    // 🔹 Ocultar globos al terminar animación
                    foreach (var obj in spawnedBalloons)
                        obj.SetActive(false);

                    // Mostrar feedback
                    ShowJudgeFeedback(currentJudgeActivity.answer);

                    // 🔹 Cuando desaparezca el feedback
                    feedbackPanel.OnFeedbackHidden = () =>
                    {
                        foreach (var obj in spawnedBalloons)
                        {
                            var bc = obj.GetComponent<BalloonController>();
                            if (bc != null)
                                bc.ResetAnimation();

                            obj.SetActive(true);
                        }

                        if (currentJudgeActivity.answer)
                            StartCoroutine(LoadCurrentActivity()); // 🔥 CHANGED
                    };
                };

                StartCoroutine(controller.PlayExplosionCoroutine());
            }
        });

        buttonNo.onClick.AddListener(() =>
        {
            foreach (var b in spawnedBalloons)
            {
                var controller = b.GetComponent<BalloonController>();
                if (controller == null) continue;

                controller.OnExplosionFinished = (_) =>
                {
                    foreach (var obj in spawnedBalloons)
                        obj.SetActive(false);

                    ShowJudgeFeedback(!currentJudgeActivity.answer);

                    feedbackPanel.OnFeedbackHidden = () =>
                    {
                        foreach (var obj in spawnedBalloons)
                        {
                            var bc = obj.GetComponent<BalloonController>();
                            if (bc != null)
                                bc.ResetAnimation();

                            obj.SetActive(true);
                        }

                        if (!currentJudgeActivity.answer)
                            StartCoroutine(LoadCurrentActivity()); // 🔥 CHANGED
                    };
                };

                StartCoroutine(controller.PlayExplosionCoroutine());
            }
        });
    }

    private void ShowJudgeFeedback(bool correct)
    {
        if (currentJudgeActivity == null) return;

        Sprite sprite1 = LoadLocalSprite(currentJudgeActivity.word1.PATH);
        Sprite sprite2 = LoadLocalSprite(currentJudgeActivity.word2.PATH);

        feedbackPanel.ShowFeedback(
            sprite1,
            sprite2,
            correct,
            currentJudgeActivity.word1.syllabified_word,
            currentJudgeActivity.word2.syllabified_word
        );
    }

    // ============================================================
    // 🎬 Secuencia Judge: animación → feedback → reinicio
    // ============================================================
    private IEnumerator PlayJudgeBalloonSequence(GameObject balloon, ActivityJudge activity, bool correct)
    {
        // 1️⃣ Ocultar overlay del globo presionado
        var img = balloon.transform.Find("Image")?.gameObject;
        if (img != null) img.SetActive(false);

        // 2️⃣ Ejecutar animación
        var controller = balloon.GetComponent<BalloonController>();
        if (controller != null)
            yield return StartCoroutine(controller.PlayExplosionCoroutine());

        // 3️⃣ Mostrar feedback (con ambas palabras e imágenes del JSON)
        feedbackPanel.ShowFeedback(
            LoadLocalSprite(activity.word1.PATH),
            LoadLocalSprite(activity.word2.PATH),
            correct,
            activity.word1.syllabified_word,
            activity.word2.syllabified_word
        );

        // 4️⃣ Ocultar todos los globos mientras se muestra el feedback
        foreach (var b in spawnedBalloons)
            if (b != null) b.SetActive(false);

        // 5️⃣ Esperar unos segundos mientras se muestra el panel
        yield return new WaitForSeconds(3f);

        // 6️⃣ Reiniciar todos los globos a su estado inicial
        foreach (var b in spawnedBalloons)
        {
            if (b == null) continue;

            var bc = b.GetComponent<BalloonController>();
            if (bc != null)
                bc.ResetAnimation();
            b.SetActive(true);
        }

        // 7️⃣ Si fue correcta la respuesta, recargar el modo
        if (correct)
            StartCoroutine(LoadCurrentActivity()); // 🔥 CHANGED
    }

    // ============================================================
    // 🔵 MODO 2 — SELECT
    // ============================================================
    private void LoadSelectMode(ActivitySelect activity)
    {
        ClearBalloons();

        // ✅ Ocultar botones Yes/No (solo modo Judge los usa)
        buttonYes.gameObject.SetActive(false);
        buttonNo.gameObject.SetActive(false);
        buttonYes.interactable = false;
        buttonNo.interactable = false;

        // ✅ Cargar sprites locales
        Sprite spriteMain = LoadLocalSprite(activity.main_word.PATH);
        Sprite spriteCorrect = LoadLocalSprite(activity.correct_option.PATH);
        Sprite spriteWrong1 = LoadLocalSprite(activity.wrong_option1.PATH);
        Sprite spriteWrong2 = LoadLocalSprite(activity.wrong_option2.PATH);

        // ✅ Crear lista de opciones
        var options = new List<(Sprite sprite, string syll, bool correct)>
    {
        (spriteMain,   activity.main_word.syllabified_word,   false),
        (spriteCorrect,activity.correct_option.syllabified_word, true),
        (spriteWrong1, activity.wrong_option1.syllabified_word, false),
        (spriteWrong2, activity.wrong_option2.syllabified_word, false)
    };

        // ✅ Aleatorizar el orden
        Shuffle(options);

        // ✅ Posiciones fijas (cuatro globos)
        Vector2[] pos =
        {
        new Vector2(-300, 150), new Vector2(300, 150),
        new Vector2(-300, -150), new Vector2(300, -150)
    };

        // ✅ Crear cada globo
        for (int i = 0; i < options.Count; i++)
        {
            GameObject balloon = Instantiate(balloonPrefab, container);
            balloon.name = $"Balloon_Select_{i + 1}";
            balloon.GetComponent<RectTransform>().anchoredPosition = pos[i];
            spawnedBalloons.Add(balloon);

            // Imagen principal
            Image img = balloon.transform.Find("Image")?.GetComponent<Image>();
            if (img != null)
            {
                img.sprite = options[i].sprite;
                img.preserveAspect = true;
            }

            // Ocultar texto
            TMP_Text txt = balloon.GetComponentInChildren<TMP_Text>();
            if (txt != null)
                txt.gameObject.SetActive(false);

            var controller = balloon.GetComponent<BalloonController>();
            if (controller != null)
            {
                ConfigureSelectBalloonClick(controller, options[i], activity);
            }
        }
    }

    private void ConfigureSelectBalloonClick(
    BalloonController controller,
    (Sprite sprite, string syll, bool correct) option,
    ActivitySelect activity)
    {
        controller.buttonOp.onClick.RemoveAllListeners();

        controller.OnExplosionFinished = (_) =>
        {
            // Ocultar todos los globos durante feedback
            foreach (var b in spawnedBalloons)
                if (b != null) b.SetActive(false);

            if (option.correct)
            {
                // ✅ Correcto: seleccionada vs palabra principal
                feedbackPanel.ShowFeedback(
                    option.sprite,
                    LoadLocalSprite(activity.main_word.PATH),
                    true,
                    option.syll,
                    activity.main_word.syllabified_word
                );

                StartCoroutine(NextActivity());
            }
            else
            {
                // ⚪ Neutral: seleccionada + otra incorrecta aleatoria
                var neutralList = new List<(Sprite sprite, string syll)>
            {
                (LoadLocalSprite(activity.main_word.PATH), activity.main_word.syllabified_word),
                (LoadLocalSprite(activity.wrong_option1.PATH), activity.wrong_option1.syllabified_word),
                (LoadLocalSprite(activity.wrong_option2.PATH), activity.wrong_option2.syllabified_word)
            };

                // Eliminar la seleccionada y la correcta
                neutralList.RemoveAll(o => o.sprite == option.sprite);
                Sprite correctSprite = LoadLocalSprite(activity.correct_option.PATH);
                neutralList.RemoveAll(o => o.sprite == correctSprite);

                // Seleccionar una al azar
                var randomOther = neutralList[UnityEngine.Random.Range(0, neutralList.Count)];

                feedbackPanel.ShowFeedback(
                    option.sprite,
                    randomOther.sprite,
                    false,
                    option.syll,
                    randomOther.syll
                );
            }

            feedbackPanel.OnFeedbackHidden = () =>
            {
                foreach (var b in spawnedBalloons)
                {
                    if (b == null) continue;
                    var bc = b.GetComponent<BalloonController>();
                    if (bc != null) bc.ResetAnimation();
                    b.SetActive(true);
                }
            };
        };

        controller.buttonOp.onClick.AddListener(() =>
        {
            if (controller.IsIdle)
                StartCoroutine(controller.PlayExplosionCoroutine());
        });
    }

    // ============================================================
    // 🟣 MODO 3 — RELATE
    // ============================================================
    private void LoadRelateMode(ActivityRelate activity)
    {
        if (activity == null || activity.main_word == null)
        {
            Debug.LogError(" Activity o main_word es nulo en LoadRelateMode.");
            StartCoroutine(LoadCurrentActivity()); // 🔥 CHANGED
            return;
        }

        ClearBalloons();

        // ✅ Mostrar imagen principal (solo en modo Relate)
        if (imageMain != null)
        {
            imageMain.gameObject.SetActive(true);
            imageMain.sprite = LoadLocalSprite(activity.main_word.PATH);
            imageMain.preserveAspect = true;
        }

        // ✅ Ocultar botones Yes/No (solo se usan en Judge)
        buttonYes.gameObject.SetActive(false);
        buttonNo.gameObject.SetActive(false);

        // ✅ Cargar sprites de opciones
        Sprite spriteCorrect = LoadLocalSprite(activity.correct_option.PATH);
        Sprite spriteWrong1 = LoadLocalSprite(activity.wrong_option1.PATH);
        Sprite spriteWrong2 = LoadLocalSprite(activity.wrong_option2.PATH);
        Sprite spriteWrong3 = LoadLocalSprite(activity.wrong_option3.PATH);

        // ✅ Crear lista de opciones con bandera de correcto
        var options = new List<(Sprite sprite, string syll, bool correct)>
    {
        (spriteCorrect, activity.correct_option.syllabified_word, true),
        (spriteWrong1, activity.wrong_option1.syllabified_word, false),
        (spriteWrong2, activity.wrong_option2.syllabified_word, false),
        (spriteWrong3, activity.wrong_option3.syllabified_word, false)
    };

        // ✅ Posiciones fijas de globos
        Vector2[] positions =
        {
        new Vector2(-350, 100),
        new Vector2(350, 100),
        new Vector2(-350, -150),
        new Vector2(350, -150)
    };

        // ✅ Crear los globos
        for (int i = 0; i < options.Count; i++)
        {
            GameObject balloon = Instantiate(balloonPrefab, container);
            balloon.name = $"Balloon_Relate_{i + 1}";
            balloon.GetComponent<RectTransform>().anchoredPosition = positions[i];
            spawnedBalloons.Add(balloon);

            // Asignar imagen
            Image img = balloon.transform.Find("Image")?.GetComponent<Image>();
            if (img != null)
            {
                img.sprite = options[i].sprite;
                img.preserveAspect = true;
            }

            // Ocultar texto si el prefab lo tiene
            TMP_Text txt = balloon.GetComponentInChildren<TMP_Text>();
            if (txt != null)
                txt.gameObject.SetActive(false);

            // Configurar evento de clic
            var controller = balloon.GetComponent<BalloonController>();
            if (controller != null)
            {
                ConfigureRelateBalloonClick(controller, options[i], activity);
            }
        }
    }

    private void ConfigureRelateBalloonClick(
    BalloonController controller,
    (Sprite sprite, string syll, bool correct) option,
    ActivityRelate activity)
    {
        controller.buttonOp.onClick.RemoveAllListeners();

        controller.OnExplosionFinished = (_) =>
        {
            // 🔹 Ocultar todos los globos mientras se muestra el feedback
            foreach (var b in spawnedBalloons)
                if (b != null) b.SetActive(false);

            // 🔹 Mostrar feedback (correcto o neutral)
            bool isCorrect = option.correct;

            feedbackPanel.ShowFeedback(
                option.sprite,
                LoadLocalSprite(activity.main_word.PATH),
                isCorrect,
                option.syll,
                activity.main_word.syllabified_word
            );

            // 🔹 Cuando el feedback se oculta → reiniciar globos
            feedbackPanel.OnFeedbackHidden = () =>
            {
                foreach (var b in spawnedBalloons)
                {
                    if (b == null) continue;
                    var bc = b.GetComponent<BalloonController>();
                    if (bc != null) bc.ResetAnimation();
                    b.SetActive(true);
                }

                if (isCorrect)
                    StartCoroutine(LoadCurrentActivity()); // 🔥 CHANGED
            };
        };

        controller.buttonOp.onClick.AddListener(() =>
        {
            if (controller.IsIdle)
                StartCoroutine(controller.PlayExplosionCoroutine());
        });
    }

    // ============================================================
    // 🔥 UPDATED - Single NextActivity method for all task types
    // ============================================================
    private IEnumerator NextActivity()
    {
        yield return new WaitForSeconds(2.2f);
        StartCoroutine(LoadCurrentActivity());
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = UnityEngine.Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    // ============================================================
    private void ClearBalloons()
    {
        foreach (var b in spawnedBalloons)
            if (b != null) Destroy(b);
        spawnedBalloons.Clear();
    }

    // ============================================================
    // 🎨 Cargar sprite desde carpeta ImgButtons
    // ============================================================
    private Sprite LoadLocalSprite(string imageName)
    {
        if (string.IsNullOrEmpty(imageName))
            return null;

        // ✅ Obtiene el nombre del archivo sin extensión
        string fileName = System.IO.Path.GetFileNameWithoutExtension(imageName).Trim();
        string path = $"Images/ImgButtons/{fileName}";

        Sprite sprite = Resources.Load<Sprite>(path);

        if (sprite == null)
            Debug.LogWarning($"⚠️ No se encontró sprite en: {path}");

        return sprite;
    }
}
