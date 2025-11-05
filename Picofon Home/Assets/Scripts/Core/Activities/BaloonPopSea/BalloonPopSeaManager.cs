using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Picofon.Games.Judge;

public class BalloonPopSeaManager : MonoBehaviour
{
    [Header("🎮 Botones Top Modes")]
    [SerializeField] private Button buttonMode0;
    [SerializeField] private Button buttonMode1;
    [SerializeField] private Button buttonMode2;



    [Header("🫧 Burbujas")]
    [SerializeField] private Transform bubbleContainerHorizontal1;
    [SerializeField] private Transform bubbleContainerHorizontal2;

    [SerializeField] private GameObject bubblePrefab;

    [Header("✅ Botones Sí / No")]
    [SerializeField] private Button buttonYes;
    [SerializeField] private Button buttonNo;

    [Header("⭐ Feedback Panel")]
    [SerializeField] private FeedbackPanelController feedbackController;

    [Header("🌐 API (BalloonPopSeaAPI)")]
    [SerializeField] private GameAPIService balloonPopAPI;   // 🔥 API asignable desde el inspector

    private ActivityJudge currentActivity;
    private readonly List<GameObject> spawnedBubbles = new();
    private int currentMode = 0; // siempre Judge por ahora
    private Picofon.Games.Relate.ActivityRelate currentRelateActivity; // ✅ AGREGA ESTA VARIABLE ARRIBA

    private void Start()
    {
        if (balloonPopAPI == null)
        {
            Debug.LogError("❌ No se asignó BalloonPopSeaAPI (GameAPIService) en el inspector.");
            return;
        }

        // 🎮 Listeners de modos
        // 🎮 Listeners correctos para los modos
        buttonMode0.onClick.AddListener(() => ChangeMode(0)); // Judge
        buttonMode1.onClick.AddListener(() => ChangeMode(1)); // Select
        buttonMode2.onClick.AddListener(() => ChangeMode(2)); // Relate 


        // ✅ Iniciar automáticamente en modo 0 (Judge)
        ChangeMode(0);
    }

    private void ChangeMode(int mode)
    {
        currentMode = mode;

        Debug.Log($"🔄 Cambiando modo BalloonPop a {mode}");

        StartCoroutine(balloonPopAPI.LoadActivity(
            currentMode,
            json => LoadMode(mode, json),
            err => Debug.LogError(err)
        ));
    }


    public void LoadMode(int mode, string json)
    {
        currentMode = mode;

        if (mode == 0) // Judge
        {
            var data = JsonUtility.FromJson<ApiResponseJudge>(json);
            LoadJudgeMode(data.data.activity1);
        }
        else if (mode == 1) // Select
        {
            var data = JsonUtility.FromJson<Picofon.Games.Select.ApiResponseSelect>(json);
            LoadSelectMode(data.data.activity1);
        }
        else if (mode == 2) // Relate
        {
            var data = JsonUtility.FromJson<Picofon.Games.Relate.ApiResponseRelate>(json);
            LoadRelateMode(data.data.activity1);
        }
    }



    private void LoadJudgeMode(ActivityJudge activity)
    {
        currentActivity = activity;

        // ✅ Volver a mostrar botones Sí/No
        buttonYes.gameObject.SetActive(true);
        buttonNo.gameObject.SetActive(true);

        // ✅ Habilitar raycast otra vez
        buttonYes.GetComponent<Image>().raycastTarget = true;
        buttonNo.GetComponent<Image>().raycastTarget = true;

        // También por seguridad en el botón
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
    // 🎯 MODO SELECT (1)
    // ======================
    private void LoadSelectMode(Picofon.Games.Select.ActivitySelect activity)
    {
        ClearBubbles();

        // ✅ Ocultar botones Sí/No y quitar raycast
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
        (mainSprite,   activity.main_word.word,   activity.main_word.syllabified_word,   false),
        (correctSprite,activity.correct_option.word, activity.correct_option.syllabified_word, true),
        (wrong1Sprite, activity.wrong_option1.word, activity.wrong_option1.syllabified_word, false),
        (wrong2Sprite, activity.wrong_option2.word, activity.wrong_option2.syllabified_word, false),
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
    // 🔗 MODO RELATE (2)
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

        buttonYes.gameObject.SetActive(false);
        buttonNo.gameObject.SetActive(false);

        var layout = bubbleContainerHorizontal2.GetComponent<HorizontalLayoutGroup>();
        if (layout != null) layout.spacing = 150f;

        Sprite mainSprite = SafeLoadSprite(activity.main_word.PATH);
        CreateRelateBubble(mainSprite, activity.main_word.syllabified_word, false, bubbleContainerHorizontal1);

        var options = new List<(Sprite sprite, string syll, bool correct)>
    {
        (SafeLoadSprite(activity.correct_option?.PATH), activity.correct_option?.syllabified_word, true),
        (SafeLoadSprite(activity.wrong_option1?.PATH), activity.wrong_option1?.syllabified_word, false),
        (SafeLoadSprite(activity.wrong_option2?.PATH), activity.wrong_option2?.syllabified_word, false),
        (SafeLoadSprite(activity.wrong_option3?.PATH), activity.wrong_option3?.syllabified_word, false),
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
                sprite,      // seleccionada
                mainSprite,  // palabra base
                isCorrect,   // correcto o neutral
                syll,
                mainSyll
            );

            if (isCorrect)
                StartCoroutine(NextActivity_Relate());
        });
    }


    private IEnumerator NextActivity_Relate()
    {
        yield return new WaitForSeconds(2.2f);

        StartCoroutine(balloonPopAPI.LoadActivity(
            currentMode,
            json =>
            {
                var data = JsonUtility.FromJson<Picofon.Games.Relate.ApiResponseRelate>(json);
                LoadRelateMode(data.data.activity1);
            },
            err => Debug.LogError(err)
        ));
    }



    private void CreateSelectBubble(
    (Sprite sprite, string word, string syll, bool correct) option,
    Transform parent,
    Picofon.Games.Select.ActivitySelect activity)
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

                StartCoroutine(NextActivity_Select());
            }
            else
            {
                // ✅ Buscar otra opción incorrecta para el feedback
                var neutralList = new List<(Sprite sprite, string syll)>
            {
                (LoadSprite(activity.main_word.PATH), activity.main_word.syllabified_word),
                (LoadSprite(activity.wrong_option1.PATH), activity.wrong_option1.syllabified_word),
                (LoadSprite(activity.wrong_option2.PATH), activity.wrong_option2.syllabified_word)
            };

                // ❌ eliminar la seleccionada
                neutralList.RemoveAll(o => o.sprite == option.sprite);

                // ❌ eliminar la correcta
                Sprite correctSprite = LoadSprite(activity.correct_option.PATH);
                neutralList.RemoveAll(o => o.sprite == correctSprite);

                // 🎯 Seleccionar una incorrecta aleatoria
                var randomOther = neutralList[Random.Range(0, neutralList.Count)];

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


    private IEnumerator NextActivity_Select()
    {
        yield return new WaitForSeconds(2.2f);

        StartCoroutine(balloonPopAPI.LoadActivity(
            currentMode,
            json => LoadMode(1, json),
            err => Debug.LogError(err)
        ));
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

        StartCoroutine(NextActivity(correct));

    }

    private IEnumerator NextActivity(bool correct)
    {
        yield return new WaitForSeconds(2.2f);

        if (!correct) yield break; // ❌ No avanzar si estuvo mal

        StartCoroutine(balloonPopAPI.LoadActivity(
            currentMode,
            json => LoadMode(0, json),
            err => Debug.LogError(err)
        ));
    }


    private void CreateBubble(Sprite sprite)
    {
        Transform targetContainer = bubbleContainerHorizontal1;

        // 📌 Para futuros modos (Select/Relate)
        if (currentMode != 0)
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

        if (img) img.sprite = sprite;

        Button btn = b.GetComponentInChildren<Button>();
        if (btn) btn.interactable = false;
        b.transform.localScale = Vector3.one;
        b.transform.localRotation = Quaternion.identity;

    }

    private void ClearBubbles()
    {
        foreach (var b in spawnedBubbles) Destroy(b);
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
}

[System.Serializable] public class ApiResponseJudge { public JudgeData data; }
[System.Serializable] public class JudgeData { public ActivityJudge activity1; }
