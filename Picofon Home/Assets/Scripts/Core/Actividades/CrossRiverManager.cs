using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

// ✅ Namespaces por minijuego (reutilizando modelos existentes)
using Picofon.Games.Judge;
using Picofon.Games.Relate;
using Picofon.Games.Select;
using Picofon.Games.Create;

// ✅ Alias global de WordData
using WordData = Picofon.Games.Judge.WordData;

/// <summary>
/// Controlador principal del minijuego CrossRiver.
/// Usa los mismos modelos que BalloonPopSea pero reemplaza el prefab por LifebeltPrefab.
/// Modo 0 → Judge (Sí/No)
/// Modo 1 → Relate (Selecciona el diferente)
/// Modo 2 → Create (Con pista)
/// Modo 3 → Select (Elige el correcto)
/// </summary>
public class CrossRiverManager : ActivityBaseManager
{
    private int currentMode = 0;
    private bool correctAnswered = false;

    private readonly Dictionary<Button, Sprite> buttonToSprite = new();
    private readonly Dictionary<Sprite, string> spriteToWord = new();

    [Header("Prefabs y contenedores")]
    [SerializeField] private GameObject lifebeltPrefab; // 🛟 reemplaza bubblePrefab
    [SerializeField] private Transform containerRow1;
    [SerializeField] private Transform containerRow2;

    private HorizontalLayoutGroup layoutRow1;
    private HorizontalLayoutGroup layoutRow2;

    [Header("Feedback visual")]
    [SerializeField] private GameObject panelFeedback;
    [SerializeField] private TMP_Text feedbackText;
    [SerializeField] private Image feedbackImage1;
    [SerializeField] private Image feedbackImage2;
    [SerializeField] private TMP_Text feedbackName1;
    [SerializeField] private TMP_Text feedbackName2;

    [Header("Imágenes auxiliares")]
    [SerializeField] private Image extraImage;
    [SerializeField] private Sprite extraCorrectSprite;
    [SerializeField] private Sprite extraIncorrectSprite;
    [SerializeField] private Image cloudImage;

    [Header("Botones modo Judge (Sí/No)")]
    [SerializeField] private Button buttonYes;
    [SerializeField] private Button buttonNo;

    [Header("Sprites locales")]
    [SerializeField] private string resourcesFolder = "CrossRiver";
    [SerializeField] private List<Sprite> gameImages = new();

    public bool IsBusyShowingFeedback { get; private set; }

    // ==============================================================
    protected override void Awake()
    {
        base.Awake();
        EnsureSpritesLoaded();

        if (panelFeedback) panelFeedback.SetActive(false);
        if (cloudImage) cloudImage.enabled = false;

        layoutRow1 = containerRow1?.GetComponent<HorizontalLayoutGroup>();
        layoutRow2 = containerRow2?.GetComponent<HorizontalLayoutGroup>();
    }

    private void Start()
    {
        if (api != null)
        {
            Debug.Log($"🌊 Cargando modo inicial {currentMode} desde API...");
            StartCoroutine(api.LoadActivity(currentMode, OnJsonLoaded, OnError));
        }
        else
        {
            Debug.LogWarning("⚠️ No hay API asignada, modo local sin carga remota.");
        }
    }

    // ==============================================================
    // 🔹 Carga desde botón / API / JSON directo
    // ==============================================================
    #region Carga directa
    public new void LoadMode(int mode)
    {
        currentMode = mode;
        Debug.Log($"🎮 Botón → modo {mode}");
        if (api != null)
            StartCoroutine(api.LoadActivity(mode, OnJsonLoaded, OnError));
        else
            Debug.LogWarning("⚠️ No hay API asignada.");
    }

    public void LoadMode(int mode, string json)
    {
        currentMode = mode;
        if (string.IsNullOrEmpty(json))
        {
            Debug.LogError("❌ JSON vacío en LoadMode(mode, json).");
            return;
        }
        Debug.Log($"🧩 Cargando modo {mode} con JSON directo...");
        OnJsonLoaded(json);
    }
    #endregion

    // ==============================================================
    // 🔹 Procesamiento del JSON recibido
    // ==============================================================
    protected override void OnJsonLoaded(string json)
    {
        ClearContainers();
        correctAnswered = false;

        if (string.IsNullOrEmpty(json))
        {
            Debug.LogError("❌ JSON vacío recibido en OnJsonLoaded.");
            return;
        }

        try
        {
            switch (currentMode)
            {
                case 0:
                    var judgeData = JsonUtility.FromJson<ApiResponseJudge>(json);
                    if (judgeData?.data?.activity1 != null)
                        LoadJudgeMode(judgeData.data.activity1);
                    break;

                case 1:
                    var relateData = JsonUtility.FromJson<ApiResponseRelate>(json);
                    if (relateData?.data?.activity1 != null)
                        LoadRelateMode(relateData.data.activity1);
                    break;

                case 2:
                    var createData = JsonUtility.FromJson<ApiResponseCreate>(json);
                    if (createData?.data?.activity1 != null)
                        LoadCreateMode(createData.data.activity1);
                    break;

                case 3:
                    var selectData = JsonUtility.FromJson<ApiResponseSelect>(json);
                    if (selectData?.data?.activity1 != null)
                        LoadSelectMode(selectData.data.activity1);
                    break;

                default:
                    Debug.LogWarning($"⚠️ Modo {currentMode} no implementado.");
                    break;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"❌ Error al procesar JSON del modo {currentMode}: {ex.Message}");
        }
    }

    // ==============================================================
    // 🟢 MODO 0 – Judge
    // ==============================================================
    private void LoadJudgeMode(ActivityJudge activity)
    {
        currentMode = 0;
        Debug.Log($"🟢 Judge → {activity.word1.word} vs {activity.word2.word}");

        ClearContainers();

        buttonYes?.gameObject.SetActive(true);
        buttonNo?.gameObject.SetActive(true);

        Sprite s1 = LoadSprite(activity.word1.PATH);
        Sprite s2 = LoadSprite(activity.word2.PATH);

        CreateLifebelt(containerRow1, s1);
        CreateLifebelt(containerRow1, s2);

        spriteToWord.Clear();
        if (s1) spriteToWord[s1] = activity.word1.word;
        if (s2) spriteToWord[s2] = activity.word2.word;

        bool correctIsYes = activity.answer;
        bool correctIsNo = !activity.answer;

        buttonYes.onClick.RemoveAllListeners();
        buttonNo.onClick.RemoveAllListeners();

        buttonYes.onClick.AddListener(() => EvaluateJudge(activity, true, correctIsYes, s1, s2));
        buttonNo.onClick.AddListener(() => EvaluateJudge(activity, false, correctIsNo, s1, s2));
    }

    // ==============================================================
    // 🟣 MODO 1 – Relate
    // ==============================================================
    private void LoadRelateMode(ActivityRelate activity)
    {
        currentMode = 1;
        Debug.Log($"🟣 Relate cargado → principal: {activity.main_word.word}");

        ClearContainers();
        buttonYes?.gameObject.SetActive(false);
        buttonNo?.gameObject.SetActive(false);

        spriteToWord.Clear();
        buttonToSprite.Clear();

        if (layoutRow2 != null)
        {
            layoutRow2.spacing = 150f;
            layoutRow2.childAlignment = TextAnchor.MiddleCenter;
        }

        Sprite main = LoadSprite(activity.main_word.PATH);
        Sprite correct = LoadSprite(activity.correct_option.PATH);
        Sprite wrong1 = LoadSprite(activity.wrong_option1.PATH);
        Sprite wrong2 = LoadSprite(activity.wrong_option2.PATH);
        Sprite wrong3 = LoadSprite(activity.wrong_option3.PATH);

        CreateLifebelt(containerRow1, main);

        List<(Sprite sprite, bool isCorrect)> options = new()
        {
            (correct, false),
            (wrong1, false),
            (wrong2, false),
            (wrong3, true)
        };

        foreach (var opt in options.Where(o => o.sprite != null))
        {
            GameObject lifebelt = CreateLifebelt(containerRow2, opt.sprite);
            Button btn = lifebelt.transform.Find("ButtonOp")?.GetComponent<Button>();
            if (btn)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => EvaluateRelate(activity, opt.isCorrect, opt.sprite));
            }
        }
    }

    // ==============================================================
    // 🟧 MODO 2 – Create
    // ==============================================================
    private void LoadCreateMode(ActivityCreate activity)
    {
        currentMode = 2;
        Debug.Log($"🟧 Create cargado → palabra: {activity.main_word.word}");

        ClearContainers();
        buttonYes?.gameObject.SetActive(false);
        buttonNo?.gameObject.SetActive(false);

        layoutRow1.spacing = 0;
        layoutRow2.spacing = 0;

        Sprite main = LoadSprite(activity.main_word.PATH);
        Sprite hint = LoadSprite(activity.hint_word.PATH);

        CreateLifebelt(containerRow1, main);
        CreateLifebelt(containerRow2, hint);

        ShowFeedback(true, main, hint, $"Pista: {activity.hint}", activity.main_word.word, activity.hint_word.word);
    }

    // ==============================================================
    // 🔵 MODO 3 – Select
    // ==============================================================
    private void LoadSelectMode(ActivitySelect activity)
    {
        currentMode = 3;
        Debug.Log($"🔵 Select cargado → {activity.main_word.word}");

        ClearContainers();
        buttonYes?.gameObject.SetActive(false);
        buttonNo?.gameObject.SetActive(false);

        Sprite main = LoadSprite(activity.main_word.PATH);
        Sprite correct = LoadSprite(activity.correct_option.PATH);
        Sprite wrong1 = LoadSprite(activity.wrong_option1.PATH);
        Sprite wrong2 = LoadSprite(activity.wrong_option2.PATH);

        List<(Sprite sprite, bool isCorrect)> options = new()
        {
            (correct, true),
            (wrong1, false),
            (wrong2, false)
        };

        options = options.Where(o => o.sprite != null).OrderBy(x => Random.value).ToList();

        foreach (var opt in options)
        {
            GameObject lifebelt = CreateLifebelt(containerRow1, opt.sprite);
            Button btn = lifebelt.transform.Find("ButtonOp")?.GetComponent<Button>();
            if (btn)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => EvaluateSelect(activity, opt.isCorrect, opt.sprite));
            }
        }
    }

    // ==============================================================
    // 🎯 Evaluaciones (idénticas a BalloonPopSea)
    // ==============================================================
    private void EvaluateJudge(ActivityJudge act, bool pressedYes, bool isCorrect, Sprite s1, Sprite s2)
    {
        correctAnswered = isCorrect;
        string msg = isCorrect ? act.feedback_positive : act.feedback_neutral;
        ShowFeedback(isCorrect, s1, s2, msg, act.word1.word, act.word2.word);
    }

    private void EvaluateRelate(ActivityRelate act, bool isCorrect, Sprite chosen)
    {
        correctAnswered = isCorrect;
        string msg = isCorrect ? act.feedback_positive : act.feedback_neutral;
        ShowFeedback(isCorrect, chosen, LoadSprite(act.main_word.PATH), msg, act.main_word.word, spriteToWord.ContainsKey(chosen) ? spriteToWord[chosen] : "?");
    }

    private void EvaluateSelect(ActivitySelect act, bool isCorrect, Sprite chosen)
    {
        correctAnswered = isCorrect;
        string msg = isCorrect ? act.feedback_positive : act.feedback_neutral;
        ShowFeedback(isCorrect, chosen, LoadSprite(act.main_word.PATH), msg, act.main_word.word, spriteToWord.ContainsKey(chosen) ? spriteToWord[chosen] : "?");
    }

    // ==============================================================
    // 🎨 Feedback visual
    // ==============================================================
    private void ShowFeedback(bool correct, Sprite img1, Sprite img2, string msg, string w1, string w2)
    {
        if (!panelFeedback || !feedbackText) return;
        panelFeedback.SetActive(true);
        feedbackText.text = msg;
        feedbackText.color = correct ? Color.green : new Color(1f, 0.5f, 0f);
        feedbackImage1.sprite = img1;
        feedbackImage2.sprite = img2;
        feedbackName1.text = w1;
        feedbackName2.text = w2;

        if (extraImage)
        {
            extraImage.enabled = true;
            extraImage.sprite = correct ? extraCorrectSprite : extraIncorrectSprite;
        }

        if (cloudImage) cloudImage.enabled = !correct;
        StartCoroutine(FeedbackThenNext());
    }

    private IEnumerator FeedbackThenNext()
    {
        yield return new WaitForSeconds(2.5f);
        panelFeedback.SetActive(false);
        if (cloudImage) cloudImage.enabled = false;
        IsBusyShowingFeedback = false;
        if (correctAnswered) StartCoroutine(api.LoadActivity(currentMode, OnJsonLoaded, OnError));
    }

    // ==============================================================
    // 🧩 Utilidades
    // ==============================================================
    private void ClearContainers()
    {
        foreach (Transform c in containerRow1) Destroy(c.gameObject);
        foreach (Transform c in containerRow2) Destroy(c.gameObject);
    }

    private Sprite LoadSprite(string path)
    {
        if (string.IsNullOrEmpty(path)) return null;
        string fileName = System.IO.Path.GetFileNameWithoutExtension(path);
        string fullPath = $"Images/ImgButtons/{fileName}";
        Sprite s = Resources.Load<Sprite>(fullPath);
        if (!s) Debug.LogWarning($"⚠️ Sprite no encontrado: {fullPath}");
        return s;
    }

    private void EnsureSpritesLoaded()
    {
        if (gameImages == null) gameImages = new List<Sprite>();
        if (gameImages.Count == 0 && !string.IsNullOrEmpty(resourcesFolder))
        {
            var loaded = Resources.LoadAll<Sprite>(resourcesFolder);
            if (loaded.Length > 0) gameImages.AddRange(loaded);
        }
    }

    private GameObject CreateLifebelt(Transform parent, Sprite sprite)
    {
        if (lifebeltPrefab == null)
        {
            Debug.LogError("❌ No se asignó el prefab de salvavidas (lifebeltPrefab) en el inspector.");
            return null;
        }

        GameObject lifebelt = Instantiate(lifebeltPrefab, parent);
        Image img = lifebelt.transform.Find("Image")?.GetComponent<Image>();
        if (img != null)
        {
            img.sprite = sprite;
            img.preserveAspect = true;
            Debug.Log($"🛟 Lifebelt creado con sprite: {sprite?.name ?? "NULL"}");
        }

        Button btn = lifebelt.transform.Find("ButtonOp")?.GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.interactable = true;
            btn.gameObject.SetActive(true);
            if (sprite != null && !buttonToSprite.ContainsKey(btn))
                buttonToSprite[btn] = sprite;
        }

        return lifebelt;
    }

    protected override void OnError(string err)
    {
        Debug.LogError($"⚠️ Error modo {currentMode}: {err}");
    }
}
