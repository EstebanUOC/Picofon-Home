using System; // 🔥 ADD THIS for Exception class
using System.Collections;
using System.Collections.Generic;
using Picofon.Games.Judge;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BalloonPopSeaManager : MonoBehaviour
{
    [Header("🫧 Burbujas")]
    [SerializeField]
    private Transform bubbleContainerHorizontal1;

    [SerializeField]
    private Transform bubbleContainerHorizontal2;

    [SerializeField]
    private GameObject bubblePrefab;

    [Header("✅ Botones Sí / No (solo para modo Judge)")]
    [SerializeField]
    private Button buttonYes;

    [SerializeField]
    private Button buttonNo;

    [Header("⭐ Feedback Panel")]
    [SerializeField]
    private FeedbackPanelController feedbackController;

    [Header("🌐 API Service")]
    [SerializeField]
    private GameAPIService balloonPopAPI;

    private ActivityJudge currentActivity;
    private readonly List<GameObject> spawnedBubbles = new();
    private int currentTaskType = 1; // 🔥 1=Judge, 2=Select, 3=Relate (from TherapyPlan)
    private Picofon.Games.Relate.ActivityRelate currentRelateActivity;

    private void Start()
    {
        if (balloonPopAPI == null)
        {
            Debug.LogError("❌ No se asignó GameAPIService en el inspector.");
            return;
        }

        // 🎯 Obtener el tipo de tarea del TherapyPlan actual
        currentTaskType = balloonPopAPI.GetCurrentTaskType();
        Debug.Log($"🎮 Tipo de tarea detectado: {currentTaskType}");

        // ✅ Iniciar automáticamente la actividad
        LoadCurrentActivity();
    }

    // ============================================================
    // 🔥 NEW METHOD - Load activity based on TherapyPlan
    // ============================================================
    private void LoadCurrentActivity()
    {
        Debug.Log($"🔄 Cargando actividad para tipo de tarea: {currentTaskType}");

        StartCoroutine(
            balloonPopAPI.LoadActivity(
                json => ProcessActivityResponse(json),
                err => Debug.LogError(err)
            )
        );
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

            switch (currentTaskType)
            {
                case 1: // Judge
                    var judgeData = JsonUtility.FromJson<ApiResponseJudge>(json); // 🔥 USE FULL NAMESPACE
                    if (judgeData?.data?.activity1 != null)
                    {
                        LoadJudgeMode(judgeData.data.activity1);
                        Debug.Log(
                            $"✅ Successfully loaded Judge activity: {judgeData.data.activity1.word1.word} vs {judgeData.data.activity1.word2.word}"
                        );
                    }
                    else
                    {
                        Debug.LogError("❌ Datos Judge inválidos o nulos");
                        if (judgeData != null)
                        {
                            Debug.LogError(
                                $"🔍 Judge Data Structure - Success: {judgeData.success}, Data: {judgeData.data != null}, Activity1: {judgeData.data?.activity1 != null}"
                            );
                        }
                    }
                    break;

                case 2: // Select
                    var selectData = JsonUtility.FromJson<Picofon.Games.Select.ApiResponseSelect>(json);
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
                    var relateData = JsonUtility.FromJson<Picofon.Games.Relate.ApiResponseRelate>(json);
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

    // ======================
    // 🧠 MODO JUDGE (1)
    // ======================
    private void LoadJudgeMode(ActivityJudge activity)
    {
        currentActivity = activity;

        // ✅ Mostrar botones Sí/No solo para Judge
        buttonYes.gameObject.SetActive(true);
        buttonNo.gameObject.SetActive(true);
        buttonYes.GetComponent<Image>().raycastTarget = true;
        buttonNo.GetComponent<Image>().raycastTarget = true;
        buttonYes.interactable = true;
        buttonNo.interactable = true;

        ClearBubbles();

        Sprite s1 = LoadSprite(activity.word1.PATH);
        Sprite s2 = LoadSprite(activity.word2.PATH);

        CreateBubble(s1);
        CreateBubble(s2);

        buttonYes.onClick.RemoveAllListeners();
        buttonNo.onClick.RemoveAllListeners();

        buttonYes.onClick.AddListener(() => Answer(true));
        buttonNo.onClick.AddListener(() => Answer(false));
    }

    // ======================
    // 🎯 MODO SELECT (2)
    // ======================
    private void LoadSelectMode(Picofon.Games.Select.ActivitySelect activity)
    {
        ClearBubbles();

        // ❌ Ocultar botones Sí/No para Select
        buttonYes.gameObject.SetActive(false);
        buttonNo.gameObject.SetActive(false);
        buttonYes.GetComponent<Image>().raycastTarget = false;
        buttonNo.GetComponent<Image>().raycastTarget = false;
        buttonYes.interactable = false;
        buttonNo.interactable = false;

        // Cargar sprites
        Sprite mainSprite = LoadSprite(activity.main_word.PATH);
        Sprite correctSprite = LoadSprite(activity.correct_option.PATH);
        Sprite wrong1Sprite = LoadSprite(activity.wrong_option1.PATH);
        Sprite wrong2Sprite = LoadSprite(activity.wrong_option2.PATH);

        var options = new List<(Sprite sprite, string word, string syll, bool correct)>
        {
            (mainSprite, activity.main_word.word, activity.main_word.syllabified_word, false),
            (
                correctSprite,
                activity.correct_option.word,
                activity.correct_option.syllabified_word,
                true
            ),
            (
                wrong1Sprite,
                activity.wrong_option1.word,
                activity.wrong_option1.syllabified_word,
                false
            ),
            (
                wrong2Sprite,
                activity.wrong_option2.word,
                activity.wrong_option2.syllabified_word,
                false
            ),
        };

        Shuffle(options);

        // Crear 4 burbujas
        for (int i = 0; i < options.Count; i++)
        {
            Transform container = (i < 2) ? bubbleContainerHorizontal1 : bubbleContainerHorizontal2;
            CreateSelectBubble(options[i], container, activity);
        }
    }

    // ======================
    // 🔗 MODO RELATE (3)
    // ======================
    private void LoadRelateMode(Picofon.Games.Relate.ActivityRelate activity)
    {
        if (activity == null || activity.main_word == null)
        {
            Debug.LogError("❌ RELATE ERROR: activity.main_word es NULL, revisa JSON del backend");
            return;
        }

        ClearBubbles();

        currentRelateActivity = activity;

        // ❌ Ocultar botones Sí/No para Relate
        buttonYes.gameObject.SetActive(false);
        buttonNo.gameObject.SetActive(false);
        buttonYes.GetComponent<Image>().raycastTarget = false;
        buttonNo.GetComponent<Image>().raycastTarget = false;
        buttonYes.interactable = false;
        buttonNo.interactable = false;

        var layout = bubbleContainerHorizontal2.GetComponent<HorizontalLayoutGroup>();
        if (layout != null)
            layout.spacing = 150f;

        Sprite mainSprite = SafeLoadSprite(activity.main_word.PATH);
        CreateRelateBubble(
            mainSprite,
            activity.main_word.syllabified_word,
            false,
            bubbleContainerHorizontal1
        );

        var options = new List<(Sprite sprite, string syll, bool correct)>
        {
            (
                SafeLoadSprite(activity.correct_option?.PATH),
                activity.correct_option?.syllabified_word,
                true
            ),
            (
                SafeLoadSprite(activity.wrong_option1?.PATH),
                activity.wrong_option1?.syllabified_word,
                false
            ),
            (
                SafeLoadSprite(activity.wrong_option2?.PATH),
                activity.wrong_option2?.syllabified_word,
                false
            ),
            (
                SafeLoadSprite(activity.wrong_option3?.PATH),
                activity.wrong_option3?.syllabified_word,
                false
            ),
        };

        options.RemoveAll(o => o.sprite == null);

        Shuffle(options);

        foreach (var op in options)
            CreateRelateBubble(op.sprite, op.syll, op.correct, bubbleContainerHorizontal2);
    }

    private Sprite SafeLoadSprite(string path)
    {
        if (string.IsNullOrEmpty(path))
            return null;

        return LoadSprite(path);
    }

    private void CreateRelateBubble(Sprite sprite, string syll, bool isCorrect, Transform parent)
    {
        GameObject b = Instantiate(bubblePrefab, parent);
        spawnedBubbles.Add(b);

        Image img = b.transform.Find("Image").GetComponent<Image>();
        img.sprite = sprite;
        img.preserveAspect = true;

        Button btn = b.transform.Find("ButtonOp").GetComponent<Button>();

        // ✅ Burbuja principal (sin botón)
        if (parent == bubbleContainerHorizontal1)
        {
            btn.gameObject.SetActive(false);
            return;
        }

        btn.gameObject.SetActive(true);
        btn.interactable = true;

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() =>
        {
            Sprite mainSprite = LoadSprite(currentRelateActivity.main_word.PATH);
            string mainSyll = currentRelateActivity.main_word.syllabified_word;

            // ✅ Siempre comparar seleccionado vs mainWord
            feedbackController.ShowFeedback(
                sprite, // seleccionada
                mainSprite, // palabra base
                isCorrect, // correcto o neutral
                syll,
                mainSyll
            );

            if (isCorrect)
                StartCoroutine(NextActivity());
        });
    }

    private void CreateSelectBubble(
        (Sprite sprite, string word, string syll, bool correct) option,
        Transform parent,
        Picofon.Games.Select.ActivitySelect activity
    )
    {
        GameObject b = Instantiate(bubblePrefab, parent);
        spawnedBubbles.Add(b);

        Image img = b.transform.Find("Image").GetComponent<Image>();
        img.sprite = option.sprite;
        img.preserveAspect = true;

        Button btn = b.transform.Find("ButtonOp").GetComponent<Button>();
        btn.interactable = true;

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() =>
        {
            bool correct = option.correct;

            if (correct)
            {
                // ✅ Feedback positivo: seleccionada vs palabra principal
                feedbackController.ShowFeedback(
                    option.sprite,
                    LoadSprite(activity.main_word.PATH),
                    true,
                    option.syll,
                    activity.main_word.syllabified_word
                );

                StartCoroutine(NextActivity());
            }
            else
            {
                // ✅ Buscar otra opción incorrecta para el feedback
                var neutralList = new List<(Sprite sprite, string syll)>
                {
                    (LoadSprite(activity.main_word.PATH), activity.main_word.syllabified_word),
                    (
                        LoadSprite(activity.wrong_option1.PATH),
                        activity.wrong_option1.syllabified_word
                    ),
                    (
                        LoadSprite(activity.wrong_option2.PATH),
                        activity.wrong_option2.syllabified_word
                    ),
                };

                // ❌ eliminar la seleccionada
                neutralList.RemoveAll(o => o.sprite == option.sprite);

                // ❌ eliminar la correcta
                Sprite correctSprite = LoadSprite(activity.correct_option.PATH);
                neutralList.RemoveAll(o => o.sprite == correctSprite);

                // 🎯 Seleccionar una incorrecta aleatoria
                var randomOther = neutralList[UnityEngine.Random.Range(0, neutralList.Count)];

                // ✅ Feedback Neutral: seleccionada + otra incorrecta
                feedbackController.ShowFeedback(
                    option.sprite,
                    randomOther.sprite,
                    false,
                    option.syll,
                    randomOther.syll
                );
            }
        });
    }

    private void Answer(bool guess)
    {
        bool correct = (guess == currentActivity.answer);

        Sprite s1 = LoadSprite(currentActivity.word1.PATH);
        Sprite s2 = LoadSprite(currentActivity.word2.PATH);

        feedbackController.ShowFeedback(
            s1,
            s2,
            correct,
            currentActivity.word1.syllabified_word,
            currentActivity.word2.syllabified_word
        );

        if (correct)
        {
            StartCoroutine(NextActivity());
        }
    }

    // ============================================================
    // 🔥 UPDATED - Single NextActivity method for all task types
    // ============================================================
    private IEnumerator NextActivity()
    {
        yield return new WaitForSeconds(2.2f);
        LoadCurrentActivity();
    }

    private void CreateBubble(Sprite sprite)
    {
        Transform targetContainer = bubbleContainerHorizontal1;

        // 📌 Para futuros modos (Select/Relate)
        if (currentTaskType != 1) // 🔥 1 = Judge
            targetContainer = bubbleContainerHorizontal2;

        GameObject b = Instantiate(bubblePrefab, targetContainer);
        spawnedBubbles.Add(b);

        Image img = b.transform.Find("Image").GetComponent<Image>();
        if (img == null)
        {
            Debug.LogError("❌ No se encontró el hijo 'Image' dentro del BubblePrefab");
            return;
        }

        img.sprite = sprite;
        img.preserveAspect = true;

        Button btn = b.GetComponentInChildren<Button>();
        if (btn)
            btn.interactable = false;
        b.transform.localScale = Vector3.one;
        b.transform.localRotation = Quaternion.identity;
    }

    private void ClearBubbles()
    {
        foreach (var b in spawnedBubbles)
            Destroy(b);
        spawnedBubbles.Clear();
    }

    private Sprite LoadSprite(string p)
    {
        string file = System.IO.Path.GetFileNameWithoutExtension(p);
        Sprite s = Resources.Load<Sprite>($"Images/ImgButtons/{file}");

        if (!s)
            Debug.LogWarning($"⚠ No se encontró sprite: {file}");

        return s;
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
}
