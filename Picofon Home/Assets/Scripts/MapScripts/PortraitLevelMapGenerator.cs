using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PortraitLevelMapGenerator : MonoBehaviour
{
    [Header("Buttons")]
    public RectTransform buttonParent;
    public GameObject buttonPrefab;
    public int numberOfLevels = 12;

    [Header("Button Placement")]
    public float startY = 1600f;
    public float stepY = 80f;
    public float leftX = -50f;
    public float rightX = 50f;

    [Header("Background (Tiled)")]
    public RectTransform contentParent;
    public RectTransform backgroundsParent;
    public GameObject backgroundPrefab;
    public float backgroundHeight = 1536f;

    [Header("Scroll")]
    public ScrollRect scrollRect;

    [Header("Button Overlays")]
    public Sprite seaSprite;
    public Sprite partySprite;
    public Sprite basketSprite;
    public Sprite riverSprite;
    public Sprite padlockSprite;  // 🆕 assign your padlock icon in Inspector

    private string[] scenes = new string[]
    {
        "BalloonPopSeaScene",
        "BalloonPopParty",
        "BasketScene",
        "CrossTheRiverScene"
    };

    private Button[] levelButtons;
    private GameObject[] padlockIcons;

    void Start()
    {
        GenerateMap();
        UpdateLevelLocks();

        if (scrollRect != null)
        {
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
        }
    }

    void GenerateMap()
    {
        levelButtons = new Button[numberOfLevels];
        padlockIcons = new GameObject[numberOfLevels];

        for (int i = 0; i < numberOfLevels; i++)
        {
            GameObject btn = Instantiate(buttonPrefab, buttonParent);
            RectTransform rt = btn.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(70f, 70f);

            float x = (i % 2 == 0) ? leftX : rightX;
            float y = startY - (i * stepY);
            rt.anchoredPosition = new Vector2(x, y);

            string sceneName = scenes[i % scenes.Length];

            // --- Scene icon ---
            Image moonImage = btn.GetComponent<Image>();
            if (moonImage != null)
            {
                GameObject sceneOverlay = new GameObject("SceneOverlay", typeof(RectTransform), typeof(Image));
                sceneOverlay.transform.SetParent(moonImage.transform, false);

                RectTransform sceneRT = sceneOverlay.GetComponent<RectTransform>();
                sceneRT.anchorMin = new Vector2(0.5f, 0.5f);
                sceneRT.anchorMax = new Vector2(0.5f, 0.5f);
                sceneRT.pivot = new Vector2(0.5f, 0.5f);
                sceneRT.sizeDelta = new Vector2(40f, 40f);
                sceneRT.anchoredPosition = Vector2.zero;

                Image sceneImage = sceneOverlay.GetComponent<Image>();
                switch (sceneName)
                {
                    case "BalloonPopSeaScene": sceneImage.sprite = seaSprite; break;
                    case "BalloonPopParty": sceneImage.sprite = partySprite; break;
                    case "BasketScene": sceneImage.sprite = basketSprite; break;
                    case "CrossTheRiverScene": sceneImage.sprite = riverSprite; break;
                }
                sceneImage.preserveAspect = true;
            }

            // --- Create padlock overlay ---
            GameObject padlock = new GameObject("PadlockIcon", typeof(RectTransform), typeof(Image));
            padlock.transform.SetParent(btn.transform, false);
            RectTransform lockRT = padlock.GetComponent<RectTransform>();
            lockRT.anchorMin = new Vector2(0.5f, 0.5f);
            lockRT.anchorMax = new Vector2(0.5f, 0.5f);
            lockRT.pivot = new Vector2(0.5f, 0.5f);
            lockRT.sizeDelta = new Vector2(30f, 30f);
            lockRT.anchoredPosition = Vector2.zero;

            Image lockImage = padlock.GetComponent<Image>();
            lockImage.sprite = padlockSprite;
            lockImage.preserveAspect = true;

            padlockIcons[i] = padlock;

            // --- Button click listener ---
            Button buttonComp = btn.GetComponent<Button>();
            int levelIndex = i;
            if (buttonComp != null)
            {
                buttonComp.onClick.AddListener(() =>
                {
                    if (buttonComp.interactable)
                    {
                        Debug.Log("Loading: " + sceneName);
                        PlayerPrefs.SetInt("LastCompletedLevel", levelIndex);
                        PlayerPrefs.Save();
                        SceneManager.LoadScene(sceneName);
                    }
                });

                levelButtons[i] = buttonComp;
            }
        }

        // --- Resize Content + Backgrounds ---
        float totalHeight = startY + (numberOfLevels * stepY);
        contentParent.sizeDelta = new Vector2(contentParent.sizeDelta.x, totalHeight + 500f);

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

    void UpdateLevelLocks()
    {
        int lastCompleted = PlayerPrefs.GetInt("LastCompletedLevel", -1);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            bool unlocked = (i <= lastCompleted + 1);
            levelButtons[i].interactable = unlocked;

            Image img = levelButtons[i].GetComponent<Image>();
            if (img != null)
                img.color = unlocked ? Color.white : new Color(0.6f, 0.6f, 0.6f, 1f);

            if (padlockIcons[i] != null)
                padlockIcons[i].SetActive(!unlocked); // hide if unlocked
        }
    }
}
