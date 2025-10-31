using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CharacterAnimator : MonoBehaviour
{
    [Header("🎞️ Configuración")]
    public Image imageRenderer;
    public string folderPath = "Images/Images/CrossRiver/LlumiJumping";
    public float totalAnimTime = 0.8f; // duración total de la animación (igual o menor a moveDuration)

    private List<Sprite> frames = new();
    private Sprite idleSprite;

    void Awake()
    {
        Sprite[] loaded = Resources.LoadAll<Sprite>(folderPath);
        if (loaded.Length == 0)
        {
            Debug.LogError($"❌ No se encontraron sprites en Resources/{folderPath}");
            return;
        }

        // ✅ Ordenar nombres: Recurso1, Recurso2...
        frames = loaded.OrderBy(s => s.name.Length)
                       .ThenBy(s => s.name)
                       .ToList();

        idleSprite = frames[0];
        if (imageRenderer != null)
            imageRenderer.sprite = idleSprite;

        Debug.Log($"✅ Cargados {frames.Count} frames desde {folderPath}");
    }

    // 🔹 Reproduce todos los frames una sola vez
    public IEnumerator PlayOnce()
    {
        if (frames.Count == 0 || imageRenderer == null)
            yield break;

        int totalFrames = frames.Count;
        float frameTime = totalAnimTime / totalFrames;

        for (int i = 0; i < totalFrames; i++)
        {
            imageRenderer.sprite = frames[i];
            yield return new WaitForSeconds(frameTime);
        }

        // ✅ volver a idle al terminar
        imageRenderer.sprite = idleSprite;
    }

    public void SetIdleFrame()
    {
        if (imageRenderer != null && idleSprite != null)
            imageRenderer.sprite = idleSprite;
    }
}
