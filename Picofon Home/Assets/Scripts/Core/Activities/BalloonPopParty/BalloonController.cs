using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BalloonController : MonoBehaviour
{
    [Header("🎈 Referencias")]
    [SerializeField] private Image background;
    [SerializeField] private Image overlayImage;
    // Después
    [SerializeField] public Button buttonOp;


    [Header("⚙️ Configuración de animación")]
    [SerializeField] private float frameDelay = 0.15f;

    private Sprite[] explosionFrames;
    private bool isAnimating = false;

    public bool IsIdle => !isAnimating;
    public bool IsExploding => isAnimating;

    // 🔹 Evento para notificar al Manager cuando termina la animación
    public Action<BalloonController> OnExplosionFinished;

    private void Awake()
    {
        explosionFrames = new Sprite[4];
        for (int i = 0; i < 4; i++)
        {
            string path = $"Images/Images/PopParty/exploding/balloon_exploding00{i + 1}";
            explosionFrames[i] = Resources.Load<Sprite>(path);
        }

        if (background != null && explosionFrames[0] != null)
            background.sprite = explosionFrames[0];

        if (buttonOp != null)
            buttonOp.onClick.AddListener(OnBalloonPressed);
    }

    private void OnBalloonPressed()
    {
        if (isAnimating) return;
        StartCoroutine(PlayExplosionCoroutine());
    }

    public IEnumerator PlayExplosionCoroutine()
    {
        isAnimating = true;

        if (overlayImage != null)
            overlayImage.gameObject.SetActive(false);

        foreach (var frame in explosionFrames)
        {
            if (background != null && frame != null)
                background.sprite = frame;

            yield return new WaitForSeconds(frameDelay);
        }

        yield return new WaitForSeconds(0.2f);
        isAnimating = false;

        Debug.Log($"💥 Animación terminada en {gameObject.name}");

        // 🔔 Notificar al manager cuando termina
        OnExplosionFinished?.Invoke(this);
    }

    // ============================================================
    // 🔄 Reiniciar animación al frame inicial
    // ============================================================
    public void ResetAnimation()
    {
        StopAllCoroutines();
        isAnimating = false;

        if (background != null && explosionFrames != null && explosionFrames.Length > 0)
            background.sprite = explosionFrames[0]; // volver al frame inicial

        if (overlayImage != null)
            overlayImage.gameObject.SetActive(true);
    }
}
