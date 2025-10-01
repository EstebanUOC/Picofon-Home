using UnityEngine;
using UnityEngine.UI;

public class PortraitLevelMapGenerator : MonoBehaviour
{
    [Header("Buttons")]
    public RectTransform buttonParent;   // Assign LevelButtonsParent
    public GameObject buttonPrefab;
    public int numberOfLevels = 12;
    public float verticalSpacing = 400f;   // More space for portrait
    public float horizontalOffset = 200f;  // Smaller zigzag for portrait

    [Header("Background (Tiled)")]
    public RectTransform contentParent;      
    public RectTransform backgroundsParent;  
    public GameObject backgroundPrefab;      
    public float backgroundHeight = 1080f;   // Use taller tiles for portrait

    [Header("Scroll")]
    public ScrollRect scrollRect; 

    void Start()
    {
        GenerateMap();

        // Force scroll to bottom at start (child scrolls up)
        if (scrollRect != null)
        {
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f; // Start at bottom
        }
    }

    void GenerateMap()
    {
        // 1) Generate Buttons
        for (int i = 0; i < numberOfLevels; i++)
        {
            GameObject btn = Instantiate(buttonPrefab, buttonParent);
            RectTransform rt = btn.GetComponent<RectTransform>();

            // Zigzag left / right in portrait
            float x = (i % 2 == 0) ? -horizontalOffset : horizontalOffset;
            float y = -(i * verticalSpacing); // start bottom → go upward

            rt.anchoredPosition = new Vector2(x, y);

            // Label
            var text = btn.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (text != null) text.text = "Set " + (i + 1);
        }

        // 2) Resize Content
        float totalHeight = numberOfLevels * verticalSpacing;
        contentParent.sizeDelta = new Vector2(contentParent.sizeDelta.x, totalHeight + 800f);

        // 3) Background Tiles
        int numberOfTiles = Mathf.CeilToInt(contentParent.sizeDelta.y / backgroundHeight);

        for (int i = 0; i < numberOfTiles; i++)
        {
            GameObject bg = Instantiate(backgroundPrefab, backgroundsParent);
            RectTransform bgRt = bg.GetComponent<RectTransform>();

            float y = -i * backgroundHeight;
            bgRt.anchoredPosition = new Vector2(0, y);
            bgRt.sizeDelta = new Vector2(contentParent.rect.width, backgroundHeight);
            bg.name = "BackgroundTile_" + (i + 1);
        }
    }
}
