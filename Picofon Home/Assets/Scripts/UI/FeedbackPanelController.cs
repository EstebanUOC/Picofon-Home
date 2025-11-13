using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Text.RegularExpressions;

public class FeedbackPanelController : MonoBehaviour
{
    [Header("🧠 Elementos del Panel")]
    [SerializeField] private GameObject panelFeedback;
    [SerializeField] private Image background;
    [SerializeField] private Image backgroundLumi;
    [SerializeField] private Image cloud;
    [SerializeField] private Image imageLeft;
    [SerializeField] private Image imageRight;
    [SerializeField] private TMP_Text textMain;
    [SerializeField] private TMP_Text textSub;

    private Coroutine hideRoutine;
    private Coroutine animRoutine;
    private Sprite spriteCloud;

    private void Awake()
    {
        if (panelFeedback != null)
            panelFeedback.SetActive(false);

        spriteCloud = Resources.Load<Sprite>("Images/Images/PanelFeedback/Cloud");
    }

    public void ShowFeedback(Sprite left, Sprite right, bool correct, string syllWord1, string syllWord2)
    {
        if (panelFeedback == null) return;

        // ⚠️ Detener animaciones previas
        if (animRoutine != null)
            StopCoroutine(animRoutine);

        // ⚠️ Mostrar panel
        panelFeedback.SetActive(true);
        HidePrefabImageIfModeZero();

        // 🔥 Asegurar render order
        background.transform.SetAsFirstSibling();
        backgroundLumi.transform.SetAsLastSibling();

        background.raycastTarget = false;
        backgroundLumi.raycastTarget = false;

        // Fondo back
        string backPath = correct ? "Images/Images/PanelFeedback/back_correct" : "Images/Images/PanelFeedback/back_neutral";
        Sprite backSprite = Resources.Load<Sprite>(backPath);

        if (background != null && backSprite != null)
        {
            background.enabled = true;
            background.sprite = backSprite;
            background.color = Color.white;
        }

        // Cloud
        cloud.enabled = !correct;
        if (!correct && spriteCloud != null) cloud.sprite = spriteCloud;

        // Images
        imageLeft.enabled = left != null; imageLeft.sprite = left;
        imageRight.enabled = right != null; imageRight.sprite = right;

        textMain.text = ColorizeFirstSyllable(syllWord1, correct);
        textSub.text = ColorizeFirstSyllable(syllWord2, correct);

        // ✅ Si es correcto, correr animación
        if (correct)
        {
            animRoutine = StartCoroutine(PlayCorrectAnimation());
        }
        else
        {
            Sprite neutral = Resources.Load<Sprite>("Images/Images/PanelFeedback/Neutral_Answer");
            if (backgroundLumi != null)
            {
                backgroundLumi.enabled = true;
                backgroundLumi.color = Color.white;
                backgroundLumi.sprite = neutral;
            }
        }

        // ✅ Asegurar cierre tras 4s
        if (hideRoutine != null)
            StopCoroutine(hideRoutine);

        hideRoutine = StartCoroutine(HideAfterDelay(4f));
    }

    private IEnumerator PlayCorrectAnimation()
    {
        Sprite[] frames = Resources.LoadAll<Sprite>("Images/Images/PanelFeedback/Correct_Feedback");

        if (frames.Length == 0)
        {
            Debug.LogWarning("⚠ No se encontraron frames en carpeta Correct_Feedback");
            yield break;
        }

        frames = frames.OrderBy(f =>
        {
            string num = Regex.Replace(f.name, @"[^\d]", "");
            return int.Parse(num);
        }).ToArray();

        float totalDuration = 1.25f;
        float frameTime = totalDuration / frames.Length;

        // ✅ Bucle seguro
        foreach (var frame in frames)
        {
            if (backgroundLumi == null || !backgroundLumi.gameObject.activeInHierarchy)
                yield break; // ⚠️ No intentar dibujar si desapareció

            backgroundLumi.enabled = true;
            backgroundLumi.color = Color.white;
            backgroundLumi.sprite = frame;

            yield return new WaitForSeconds(frameTime);
        }
    }

    private IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // ⚠️ Detener animación antes de cerrar panel
        if (animRoutine != null)
        {
            StopCoroutine(animRoutine);
            animRoutine = null;
        }

        if (panelFeedback != null)
            panelFeedback.SetActive(false);

        if (backgroundLumi != null)
            backgroundLumi.enabled = false;
    }

    private void HidePrefabImageIfModeZero()
    {
        Transform imageObj = transform.Find("Image");
        if (imageObj != null)
        {
            Image img = imageObj.GetComponent<Image>();
            if (img != null) img.gameObject.SetActive(false);
        }
    }

    private string ColorizeFirstSyllable(string syllWord, bool correct)
    {
        if (string.IsNullOrEmpty(syllWord)) return "";

        syllWord = syllWord.ToUpper();
        string[] syllables = syllWord.Split('#');

        string colorHex = correct ? "#00C853" : "#FF9100";
        string coloredWord = $"<color={colorHex}>{syllables[0]}</color>";

        for (int i = 1; i < syllables.Length; i++)
            coloredWord += syllables[i];

        return $"<b>{coloredWord}</b>";
    }
}
