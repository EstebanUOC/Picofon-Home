using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private Transform topButtonsContainer;
    [SerializeField] private Button buttonYes;
    [SerializeField] private Button buttonNo;
    [SerializeField] private Image imageMain; // 🔹 Referencia en el Canvas (asignar en Inspector)
    [SerializeField] private Image imageMainBack; // imagen cupcake

    private ActivityJudge currentJudgeActivity; // Guardamos la actividad actual

    // 🔹 Espaciado solo para modo Judge (en píxeles)
    private const float JudgeContainerSpacing = 150f;




    private int currentMode = 0; // 0 = Judge, 1 = Select, 2 = Relate
    private readonly List<GameObject> spawnedBalloons = new();

    // ============================================================
    private void Start()
    {
        if (apiService == null)
            apiService = FindObjectOfType<GameAPIService>();

        if (feedbackPanel == null)
            feedbackPanel = FindObjectOfType<FeedbackPanelController>();

        AssignModeButtons();
        StartCoroutine(LoadModeFromAPI(0));
    }

    // ============================================================
    private void AssignModeButtons()
    {
        if (topButtonsContainer == null)
            topButtonsContainer = GameObject.Find("TopButtons")?.transform;

        if (topButtonsContainer == null)
        {
            Debug.LogWarning("⚠️ No se encontró TopButtons en la escena.");
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
            }
        }
    }

    // ============================================================
    private IEnumerator LoadModeFromAPI(int mode)
    {
        currentMode = mode;
        ClearBalloons();

        if (apiService == null)
        {
            Debug.LogError("❌ GameAPIService no encontrado.");
            yield break;
        }
     // Importante revisar el nuevo LoadActivity   
     //   yield return apiService.LoadActivity(mode,
     //       json => LoadMode(mode, json),
     //       err => Debug.LogError(err));
    }

    // ============================================================
    private void LoadMode(int mode, string json)
    {
        ClearBalloons();
        // Ocultar imagen backgorund principal fuera del modo Relate
        if (imageMainBack != null)
            imageMainBack.gameObject.SetActive(mode == 2);
        // Ocultar imagen principal fuera del modo Relate
        if (imageMain != null)
            imageMain.gameObject.SetActive(mode == 2);
        // 🔹 Ocultar botones Yes/No por defecto
        buttonYes.gameObject.SetActive(false);
        buttonNo.gameObject.SetActive(false);
        // Ajustar el espaciado del contenedor según el modo
        var layout = container.GetComponent<HorizontalLayoutGroup>();
        if (layout != null)
        {
            if (mode == 0) // Modo Judge
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

        switch (mode)
        {
            case 0:
                var judgeData = JsonUtility.FromJson<ApiResponseJudge>(json);
                if (judgeData?.data?.activity1 != null)
                    LoadJudgeMode(judgeData.data.activity1);
                break;

            case 1:
                var selectData = JsonUtility.FromJson<ApiResponseSelect>(json);
                if (selectData?.data?.activity1 != null)
                    LoadSelectMode(selectData.data.activity1);
                break;

            case 2:
                var relateData = JsonUtility.FromJson<ApiResponseRelate>(json);
                if (relateData?.data?.activity1 != null)
                    LoadRelateMode(relateData.data.activity1);
                break;
        }
    }

    // ============================================================
    // 🟢 MODO 0 — JUDGE (clic → animación 4 frames → feedback)
    // ============================================================
    private void LoadJudgeMode(ActivityJudge activity)
    {
        currentJudgeActivity = activity;

        ClearBalloons();

        // =========================
        // Crear 2 globos con las imágenes
        // =========================
        Sprite sprite1 = LoadLocalSprite(activity.word1.PATH);
        Sprite sprite2 = LoadLocalSprite(activity.word2.PATH);

        Vector2[] positions = { new Vector2(-250, 0), new Vector2(250, 0) };
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
                            StartCoroutine(LoadModeFromAPI(0));
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
                            StartCoroutine(LoadModeFromAPI(0));
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
                bc.ResetAnimation(); // 🔹 Método nuevo (ver abajo)
            b.SetActive(true);
        }

        // 7️⃣ Si fue correcta la respuesta, recargar el modo
        if (correct)
            StartCoroutine(LoadModeFromAPI(currentMode));
    }






    // ============================================================
    // 🔵 MODO 1 — SELECT
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

                StartCoroutine(NextActivity_Select());
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
                var randomOther = neutralList[Random.Range(0, neutralList.Count)];

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
    private void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
    private IEnumerator NextActivity_Select()
    {
        yield return new WaitForSeconds(2.2f);
        StartCoroutine(LoadModeFromAPI(1));
    }


    // ============================================================
    // 🟣 MODO 2 — RELATE
    // ============================================================
    private void LoadRelateMode(ActivityRelate activity)
    {
        if (activity == null || activity.main_word == null)
        {
            Debug.LogError(" Activity o main_word es nulo en LoadRelateMode.");
            StartCoroutine(LoadModeFromAPI(2)); // 🔁 vuelve a pedir la siguiente
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
                    StartCoroutine(LoadModeFromAPI(2)); // 🔁 Cargar nueva actividad si acierta
            };
        };

        controller.buttonOp.onClick.AddListener(() =>
        {
            if (controller.IsIdle)
                StartCoroutine(controller.PlayExplosionCoroutine());
        });
    }

    // ============================================================
    // 🎬 CLIC → ANIMACIÓN → FEEDBACK
    // ============================================================
    // ============================================================
    // 🎬 CLIC → ANIMACIÓN → FEEDBACK (genérico para todos los modos)
    // ============================================================
    private void ConfigureBalloonClick<T>(
    BalloonController controller,
    bool correct,
    T wordA,
    T wordB)
    where T : class
    {
        // Limpiar listeners previos
        controller.buttonOp.onClick.RemoveAllListeners();

        // Asignar listener para cuando termina la animación
        controller.OnExplosionFinished = (balloon) =>
        {
            // 🔹 1️⃣ Ocultar todos los globos
            foreach (var b in spawnedBalloons)
                if (b != null) b.SetActive(false);

            // 🔹 2️⃣ Mostrar el feedback
            string pathA = wordA.GetType().GetProperty("PATH")?.GetValue(wordA)?.ToString();
            string pathB = wordB.GetType().GetProperty("PATH")?.GetValue(wordB)?.ToString();
            string syllA = wordA.GetType().GetProperty("syllabified_word")?.GetValue(wordA)?.ToString();
            string syllB = wordB.GetType().GetProperty("syllabified_word")?.GetValue(wordB)?.ToString();

            feedbackPanel.ShowFeedback(
                LoadLocalSprite(pathA),
                LoadLocalSprite(pathB),
                correct,
                syllA,
                syllB
            );

            // 🔹 3️⃣ Suscribirse al evento OnFeedbackHidden
            feedbackPanel.OnFeedbackHidden = () =>
            {
                foreach (var b in spawnedBalloons)
                {
                    if (b == null) continue;

                    var bc = b.GetComponent<BalloonController>();
                    if (bc != null)
                        bc.ResetAnimation(); // 🔁 restaura el frame base

                    b.SetActive(true); // los muestra otra vez
                }

                // 🔹 4️⃣ Si fue correcta → cargar nueva actividad
                if (correct)
                    StartCoroutine(LoadModeFromAPI(currentMode));
            };
        };

        // 🔹 Click del botón → iniciar animación
        controller.buttonOp.onClick.AddListener(() =>
        {
            if (controller.IsIdle)
                StartCoroutine(controller.PlayExplosionCoroutine());
        });
    }


    private IEnumerator NextAfterDelay(bool correct)
    {
        yield return new WaitForSeconds(2.5f);
        if (correct)
            StartCoroutine(LoadModeFromAPI(currentMode));
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
        string path = $"Images/ImgButtons/{fileName}"; // ✅ Nueva ruta

        Sprite sprite = Resources.Load<Sprite>(path);

        if (sprite == null)
            Debug.LogWarning($"⚠️ No se encontró sprite en: {path}");

        return sprite;
    }

}