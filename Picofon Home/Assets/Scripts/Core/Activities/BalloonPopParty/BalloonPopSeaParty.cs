using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Picofon.Games.Judge;

public class BalloonPopSeaParty : MonoBehaviour
{
    [Header("🔗 Referencias")]
    [SerializeField] private GameAPIService apiService;           // Cargará la actividad desde el servidor
    [SerializeField] private FeedbackPanelController feedbackPanel; // Mostrará el feedback
    [SerializeField] private Button buttonJudge;                   // Botón que ejecutará el modo Judge

    private ActivityJudge currentActivity;

    private void Start()
    {
        if (apiService == null)
            apiService = FindObjectOfType<GameAPIService>();

        if (feedbackPanel == null)
            feedbackPanel = FindObjectOfType<FeedbackPanelController>();

        if (buttonJudge != null)
        {
            buttonJudge.onClick.RemoveAllListeners();
            buttonJudge.onClick.AddListener(OnJudgeButtonPressed);
        }

        // Carga inicial del modo Judge (activity1 del JSON)
        StartCoroutine(LoadJudgeActivity());
    }

    // ============================================================
    // 🌐 Cargar la actividad desde la API (modo Judge)
    // ============================================================
    private IEnumerator LoadJudgeActivity()
    {
        if (apiService == null)
        {
            Debug.LogError("❌ No se encontró GameAPIService en la escena.");
            yield break;
        }

        // Modo 0 = Judge
        yield return apiService.LoadActivity(0,
            json =>
            {
                var data = JsonUtility.FromJson<ApiResponseJudge>(json);
                if (data?.data?.activity1 != null)
                {
                    currentActivity = data.data.activity1;
                    Debug.Log($"✅ Actividad Judge cargada: {currentActivity.word1.word} vs {currentActivity.word2.word}");
                }
                else
                {
                    Debug.LogError("❌ No se pudo obtener activity1 del JSON.");
                }
            },
            err => Debug.LogError(err)
        );
    }

    // ============================================================
    // 🟢 Acción del botón: mostrar feedback correcto
    // ============================================================
    private void OnJudgeButtonPressed()
    {
        if (currentActivity == null)
        {
            Debug.LogWarning("⚠️ No hay actividad cargada aún.");
            return;
        }

        // Cargar sprites desde Resources
        Sprite spriteLeft = LoadLocalSprite(currentActivity.word1.PATH);
        Sprite spriteRight = LoadLocalSprite(currentActivity.word2.PATH);

        // Mostrar feedback "correcto" con las dos palabras
        feedbackPanel.ShowFeedback(
            spriteLeft,
            spriteRight,
            true, // ✅ siempre correcto
            currentActivity.word1.syllabified_word,
            currentActivity.word2.syllabified_word
        );

        Debug.Log($"🧠 Feedback mostrado: {currentActivity.word1.word} - {currentActivity.word2.word}");
    }

    // ============================================================
    // 🖼️ Cargar sprite local
    // ============================================================
    private Sprite LoadLocalSprite(string imageName)
    {
        if (string.IsNullOrEmpty(imageName))
            return null;

        string path = $"Images/ImgButtons/{System.IO.Path.GetFileNameWithoutExtension(imageName)}";
        Sprite sprite = Resources.Load<Sprite>(path);

        if (sprite == null)
            Debug.LogWarning($"⚠️ No se encontró sprite en {path}");

        return sprite;
    }
}
