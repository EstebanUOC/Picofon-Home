using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BubbleImageAnimator : MonoBehaviour
{
    private float frameRate = 10f;
    private string resourceFolder = "Images/CrossRiver/btnBubble";

    private List<Sprite> frames = new List<Sprite>();
    private Image imageRenderer;
    private int currentFrame = 0;
    private float timer = 0f;

    private void Awake()
    {
        imageRenderer = GetComponent<Image>();
        if (imageRenderer == null)
        {
            Debug.LogError("BubbleImageAnimator: No Image component found!");
            enabled = false;
            return;
        }

        LoadFrames();
    }

    private void LoadFrames()
    {
        frames.Clear();

        for (int i = 0; i <= 47; i++)
        {
            string fileName = $"img ({i})";
            Sprite sprite = Resources.Load<Sprite>($"{resourceFolder}/{fileName}");

            if (sprite != null)
            {
                frames.Add(sprite);
            }
            else
            {
                Debug.LogWarning($"BubbleImageAnimator: Could not load {fileName}");
            }
        }

        if (frames.Count == 0)
        {
            Debug.LogError("BubbleImageAnimator: No frames loaded!");
            enabled = false;
            return;
        }

        // ⭐ IMPORTANT ⭐
        // Set the first frame immediately to avoid showing default sprite
        imageRenderer.sprite = frames[0];
    }

    private void Update()
    {
        if (frames.Count == 0 || imageRenderer == null) return;

        timer += Time.deltaTime;

        if (timer >= 1f / frameRate)
        {
            timer = 0f;
            currentFrame = (currentFrame + 1) % frames.Count;
            imageRenderer.sprite = frames[currentFrame];
        }
    }
}
