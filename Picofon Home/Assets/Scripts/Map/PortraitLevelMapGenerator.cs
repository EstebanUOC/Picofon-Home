using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PortraitLevelMapGenerator : MonoBehaviour
{
    [Header("Buttons")]
    public RectTransform buttonParent;   // Assign LevelButtonsParent
    public GameObject buttonPrefab;
    public int numberOfLevels = 12;

    [Header("Button Placement")]
    public float startY = 1190;       // First button Y position
    public float stepY = 110f;         // Distance in Y between buttons
    public float leftX = -70f;         // X for left side
    public float rightX = 80f;         // X for right side

    [Header("Background (Tiled)")]
    public RectTransform contentParent;      
    public RectTransform backgroundsParent;  
    public GameObject backgroundPrefab;      
    public float backgroundHeight = 1536f;   

    [Header("Scroll")]
    public ScrollRect scrollRect;

    // 🎯 Add your scene cycle here
    private string[] scenes = new string[]
    {
        "PopSeaScene",
        "PopPartyScene",
        "BasketScene",
        "CrossTheRiverScene"
    };

    void Start()
    {
        GenerateMap();

        // Start scroll at bottom so first button visible
        if (scrollRect != null)
        {
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
        }
    }

    void GenerateMap()
    {
        // 1) Generate Buttons
        for (int i = 0; i < numberOfLevels; i++)
        {
            GameObject btn = Instantiate(buttonPrefab, buttonParent);
            RectTransform rt = btn.GetComponent<RectTransform>();

            // Alternate X positions
            float x = (i % 2 == 0) ? leftX : rightX;
            float y = startY - (i * stepY);
            rt.anchoredPosition = new Vector2(x, y);

            // Pick scene from cycle
            string sceneName = scenes[i % scenes.Length];

            // Add label (optional)
            var text = btn.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (text != null) text.text = "Level " + (i + 1) + "\n" + sceneName;

            // Add click listener
            Button buttonComp = btn.GetComponent<Button>();
            if (buttonComp != null)
            {
                int levelIndex = i; // capture loop var
                buttonComp.onClick.AddListener(() =>
                {
                    Debug.Log("Loading: " + sceneName + " (Level " + (levelIndex + 1) + ")");
                    SceneManager.LoadScene(sceneName);
                });
            }
        }

        // 2) Resize Content
        float totalHeight = startY + (numberOfLevels * stepY);
        contentParent.sizeDelta = new Vector2(contentParent.sizeDelta.x, totalHeight + 500f);

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
