using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FeedbackPanelController : MonoBehaviour
{
    [Header("🧠 Elementos del Panel")]
    [SerializeField] private GameObject panelFeedback;
    [SerializeField] private Image background;
    [SerializeField] private Image backgroundLumi;
    [SerializeField] private Image cloud;
    [SerializeField] private Image imageLeft;
    [SerializeField] private Image imageRight;
    [SerializeField] private TMP_Text textMain;   // Primera palabra
    [SerializeField] private TMP_Text textSub;    // Segunda palabra

    private Coroutine hideRoutine;
    private Sprite spriteCloud;

    private void Awake()
    {
        if (panelFeedback != null)
            panelFeedback.SetActive(false);

        spriteCloud = Resources.Load<Sprite>("Images/Images/PanelFeedback/Cloud");
    }

    // ============================================================
    // ✅ Mostrar feedback con dos palabras (una en cada texto)
    // ============================================================
    public void ShowFeedback(
        Sprite left,
        Sprite right,
        bool correct,
        string syllWord1,
        string syllWord2
    )
    {
        if (panelFeedback == null) return;
        panelFeedback.SetActive(true);
        HidePrefabImageIfModeZero(); // 👈


        // ============================================================
        // 🎨 Fondos según tipo de feedback
        // ============================================================
        string backPath = correct
            ? "Images/Images/PanelFeedback/back_correct"
            : "Images/Images/PanelFeedback/back_neutral";

        string lumiPath = correct
            ? "Images/Images/PanelFeedback/Correct_Answer"
            : "Images/Images/PanelFeedback/Neutral_Answer";

        Sprite backSprite = Resources.Load<Sprite>(backPath);
        Sprite lumiSprite = Resources.Load<Sprite>(lumiPath);

        if (background != null)
        {
            background.enabled = (backSprite != null);
            if (backSprite != null) background.sprite = backSprite;
            background.color = Color.white;
        }

        if (backgroundLumi != null)
        {
            backgroundLumi.enabled = (lumiSprite != null);
            if (lumiSprite != null) backgroundLumi.sprite = lumiSprite;
            backgroundLumi.color = Color.white;
        }

        // ============================================================
        // ☁️ Nube (solo si es incorrecto)
        // ============================================================
        if (cloud != null)
        {
            cloud.enabled = !correct;
            if (!correct && spriteCloud != null)
            {
                cloud.sprite = spriteCloud;
                cloud.color = Color.white;
            }
        }

        // ============================================================
        // 🖼️ Imágenes
        // ============================================================
        if (imageLeft != null)
        {
            imageLeft.enabled = (left != null);
            imageLeft.sprite = left;
            imageLeft.preserveAspect = true;
        }

        if (imageRight != null)
        {
            imageRight.enabled = (right != null);
            imageRight.sprite = right;
            imageRight.preserveAspect = true;
        }

        // ============================================================
        // 🟩 Primera palabra → textMain
        // ============================================================
        if (textMain != null)
        {
            textMain.text = ColorizeFirstSyllable(syllWord1, correct);
            textMain.alignment = TextAlignmentOptions.Center;
        }

        // ============================================================
        // 🟧 Segunda palabra → textSub
        // ============================================================
        if (textSub != null)
        {
            textSub.text = ColorizeFirstSyllable(syllWord2, correct);
            textSub.alignment = TextAlignmentOptions.Center;
        }

        // ============================================================
        // 🔹 Ocultar feedback después de un tiempo
        // ============================================================
        if (hideRoutine != null)
            StopCoroutine(hideRoutine);
        hideRoutine = StartCoroutine(HideAfterDelay(3f));
    }
    // ============================================================
    // 🔹 Ocultar el objeto "Image" dentro de los prefabs generados
    // ============================================================
    private void HidePrefabImageIfModeZero()
    {
        Transform imageObj = transform.Find("Image");
        if (imageObj != null)
        {
            Image img = imageObj.GetComponent<Image>();
            if (img != null)
                img.gameObject.SetActive(false);
        }
    }


    // ============================================================
    // 🎨 Colorear la primera sílaba de una palabra (en MAYÚSCULAS)
    // ============================================================
    private string ColorizeFirstSyllable(string syllWord, bool correct)
    {
        if (string.IsNullOrEmpty(syllWord))
            return "";

        // Convertimos todo a MAYÚSCULAS antes de separar las sílabas
        syllWord = syllWord.ToUpper();

        string[] syllables = syllWord.Split('#');
        if (syllables.Length == 0)
            return syllWord;

        string colorHex = correct ? "#00C853" : "#FF9100"; // verde o naranja

        // Colorea solo la primera sílaba
        string coloredWord = $"<color={colorHex}>{syllables[0]}</color>";
        for (int i = 1; i < syllables.Length; i++)
            coloredWord += syllables[i];

        return $"<b>{coloredWord}</b>";
    }


    // ============================================================
    // 🔹 Ocultar feedback con retardo
    // ============================================================
    private IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (panelFeedback != null)
            panelFeedback.SetActive(false);
        if (backgroundLumi != null)
            backgroundLumi.enabled = false;
    }
}
