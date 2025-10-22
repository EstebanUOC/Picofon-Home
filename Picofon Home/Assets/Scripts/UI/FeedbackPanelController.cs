using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FeedbackPanelController : MonoBehaviour
{
    [Header("🧠 Elementos del Panel")]
    [SerializeField] private GameObject panelFeedback;
    [SerializeField] private Image background;
    [SerializeField] private Image cloud;
    [SerializeField] private Image imageLeft;
    [SerializeField] private Image imageRight;
    [SerializeField] private TMP_Text textMain;
    [SerializeField] private TMP_Text textSub;

    private Coroutine hideRoutine;

    // 🖼️ Sprites de feedback (se cargan desde Resources)
    private Sprite spriteCorrect;
    private Sprite spriteNeutral;
    private Sprite spriteCloud;

    private void Awake()
    {
        if (panelFeedback != null)
            panelFeedback.SetActive(false);

        // 🔹 Cargar las imágenes desde la carpeta Resources
        spriteCorrect = Resources.Load<Sprite>("Images/Images/PanelFeedback/Correct_Answer");
        spriteNeutral = Resources.Load<Sprite>("Images/Images/PanelFeedback/Neutral_Answer");
        spriteCloud = Resources.Load<Sprite>("Images/Images/PanelFeedback/Cloud");
    }

    // ============================================================
    // ✅ Mostrar feedback con imágenes y textos dinámicos
    // ============================================================
    public void ShowFeedback(Sprite left, Sprite right, string mainMsg, string subMsg, bool correct)
    {
        if (panelFeedback == null) return;

        panelFeedback.SetActive(true);

        // 🔹 Fondo principal
        if (background != null)
        {
            background.enabled = true;
            background.sprite = correct ? spriteCorrect : spriteNeutral;
            background.preserveAspect = true;
            background.color = Color.white;
        }

        // 🔹 Nube solo si es incorrecto
        if (cloud != null)
        {
            cloud.enabled = !correct;
            if (!correct)
            {
                cloud.sprite = spriteCloud;
                cloud.preserveAspect = true;
            }
        }

        // 🔹 Imágenes izquierda/derecha (palabras del juego)
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

        // 🔹 Textos
        if (textMain != null)
        {
            textMain.text = mainMsg;
            textMain.color = correct ? Color.green : new Color(1f, 0.4f, 0f);
        }

        if (textSub != null)
            textSub.text = subMsg;

        // 🔹 Cancelar corrutina previa
        if (hideRoutine != null)
            StopCoroutine(hideRoutine);

        // 🔹 Ocultar panel después de 3 segundos
        hideRoutine = StartCoroutine(HideAfterDelay(3f));
    }

    // ============================================================
    // 🔹 Ocultar feedback con retardo
    // ============================================================
    private IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (panelFeedback != null)
            panelFeedback.SetActive(false);
    }
}
