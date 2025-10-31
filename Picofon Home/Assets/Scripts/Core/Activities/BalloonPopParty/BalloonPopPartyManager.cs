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

        yield return apiService.LoadActivity(mode,
            json => LoadMode(mode, json),
            err => Debug.LogError(err));
    }

    // ============================================================
    private void LoadMode(int mode, string json)
    {
        ClearBalloons();

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
        Vector2[] pos = { new Vector2(-200, 0), new Vector2(200, 0) };

        for (int i = 0; i < 2; i++)
        {
            GameObject balloon = Instantiate(balloonPrefab, container);
            balloon.name = $"Balloon_Judge_{i + 1}";
            balloon.GetComponent<RectTransform>().anchoredPosition = pos[i];

            // 🏷️ Texto de palabra
            TMP_Text txt = balloon.GetComponentInChildren<TMP_Text>();
            string word = (i == 0) ? activity.word1.word : activity.word2.word;
            txt.text = word.ToUpper();

            // ✅ Marcar cuál es la respuesta correcta
            bool correct = (i == 0 && activity.answer) || (i == 1 && !activity.answer);

            // 🔹 Controlador del globo
            var controller = balloon.GetComponent<BalloonController>();
            if (controller == null)
            {
                Debug.LogError($"❌ El prefab {balloon.name} no tiene BalloonController.");
                continue;
            }

            // 🔹 Quitar listeners anteriores del botón
            if (controller.buttonOp != null)
                controller.buttonOp.onClick.RemoveAllListeners();

            // 🔹 Asignar nuevo evento: animación + feedback
            if (controller.buttonOp != null)
            {
                controller.buttonOp.onClick.AddListener(() =>
                {
                    StartCoroutine(PlayJudgeBalloonSequence(balloon, activity, correct));
                });
            }

            spawnedBalloons.Add(balloon);
        }
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
        Sprite[] sprites =
        {
            LoadLocalSprite(activity.main_word.PATH),
            LoadLocalSprite(activity.correct_option.PATH),
            LoadLocalSprite(activity.wrong_option1.PATH),
            LoadLocalSprite(activity.wrong_option2.PATH)
        };

        string[] words =
        {
            activity.main_word.word,
            activity.correct_option.word,
            activity.wrong_option1.word,
            activity.wrong_option2.word
        };

        bool[] correctFlags = { false, true, false, false };

        Vector2[] pos =
        {
            new Vector2(-300, 150), new Vector2(300, 150),
            new Vector2(-300, -150), new Vector2(300, -150)
        };

        for (int i = 0; i < 4; i++)
        {
            GameObject balloon = Instantiate(balloonPrefab, container);
            balloon.name = $"Balloon_Select_{i + 1}";
            balloon.GetComponent<RectTransform>().anchoredPosition = pos[i];

            TMP_Text txt = balloon.GetComponentInChildren<TMP_Text>();
            txt.text = words[i].ToUpper();

            bool isCorrect = correctFlags[i];

            Button btn = balloon.transform.Find("ButtonOp")?.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(() =>
                    StartCoroutine(OnBalloonClicked(balloon, isCorrect,
                        activity.correct_option, activity.main_word)));
            }

            spawnedBalloons.Add(balloon);
        }
    }

    // ============================================================
    // 🟣 MODO 2 — RELATE
    // ============================================================
    private void LoadRelateMode(ActivityRelate activity)
    {
        Vector2[] pos =
        {
            new Vector2(-350, -100),
            new Vector2(350, -100),
            new Vector2(-350, -300),
            new Vector2(350, -300)
        };

        string[] words =
        {
            activity.correct_option.word,
            activity.wrong_option1.word,
            activity.wrong_option2.word,
            activity.wrong_option3.word
        };

        bool[] correctFlags = { true, false, false, false };

        for (int i = 0; i < 4; i++)
        {
            GameObject balloon = Instantiate(balloonPrefab, container);
            balloon.name = $"Balloon_Relate_{i + 1}";
            balloon.GetComponent<RectTransform>().anchoredPosition = pos[i];

            TMP_Text txt = balloon.GetComponentInChildren<TMP_Text>();
            txt.text = words[i].ToUpper();

            bool isCorrect = correctFlags[i];
            Button btn = balloon.transform.Find("ButtonOp")?.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(() =>
                    StartCoroutine(OnBalloonClicked(balloon, isCorrect,
                        activity.main_word, activity.correct_option)));
            }

            spawnedBalloons.Add(balloon);
        }
    }

    // ============================================================
    // 🎬 CLIC → ANIMACIÓN → FEEDBACK
    // ============================================================
    // ============================================================
    // 🎬 CLIC → ANIMACIÓN → FEEDBACK (genérico para todos los modos)
    // ============================================================
    private IEnumerator OnBalloonClicked<T>(
        GameObject balloon,
        bool correct,
        T wordA,
        T wordB)
        where T : class
    {
        // Oculta imagen del globo
        var img = balloon.transform.Find("Image")?.gameObject;
        if (img != null) img.SetActive(false);

        // Reproduce animación
        var controller = balloon.GetComponent<BalloonController>();
        if (controller != null)
            yield return StartCoroutine(controller.PlayExplosionCoroutine());

        // Extrae propiedades comunes por reflexión
        string pathA = wordA.GetType().GetProperty("PATH")?.GetValue(wordA)?.ToString();
        string pathB = wordB.GetType().GetProperty("PATH")?.GetValue(wordB)?.ToString();
        string syllA = wordA.GetType().GetProperty("syllabified_word")?.GetValue(wordA)?.ToString();
        string syllB = wordB.GetType().GetProperty("syllabified_word")?.GetValue(wordB)?.ToString();

        // Feedback visual
        feedbackPanel.ShowFeedback(
            LoadLocalSprite(pathA),
            LoadLocalSprite(pathB),
            correct,
            syllA,
            syllB
        );

        yield return new WaitForSeconds(3f);

        if (correct)
        {
            foreach (var b in spawnedBalloons)
                if (b != null) b.SetActive(false);

            yield return new WaitForSeconds(0.5f);
            StartCoroutine(LoadModeFromAPI(currentMode));
        }
        else
        {
            Debug.Log("❌ Respuesta incorrecta — se mantienen los mismos globos.");
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
        string path = $"Images/ImgButtons/{fileName}"; // ✅ Nueva ruta

        Sprite sprite = Resources.Load<Sprite>(path);

        if (sprite == null)
            Debug.LogWarning($"⚠️ No se encontró sprite en: {path}");

        return sprite;
    }

}
