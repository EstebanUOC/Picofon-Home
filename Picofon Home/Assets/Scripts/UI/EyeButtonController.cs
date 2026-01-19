using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class EyeButtonController : MonoBehaviour
{
    [Header("👁️ Eye Button Settings")]
    [SerializeField] private Button eyeButton;
    [SerializeField] private GameObject wordDisplayPrefab; // 🔥 CHANGE: Prefab for individual word displays
    [SerializeField] private Transform wordDisplayContainer; // 🔥 NEW: Parent for all word displays
    [SerializeField] private float displayDuration = 10f;
    
    [Header("🎨 Visual Feedback")]
    [SerializeField] private Image eyeButtonImage;
    [SerializeField] private Color activeColor = Color.gray;
    [SerializeField] private Color inactiveColor = Color.white;

    private readonly List<GameObject> currentWordDisplays = new();

    private void Start()
    {
        Debug.Log("👁️ EyeButtonController - Initializing eye button");
        DebugCanvasInfo(); // 🔥 ADD THIS LINE
        // 🔥 FIX: Check if eyeButton is null before using it
        if (eyeButton != null)
        {
            // Set up button click listener
            eyeButton.onClick.AddListener(ToggleWordDisplay);
        }
        else
        {
            Debug.LogError("👁️ EyeButtonController: eyeButton is null! Check inspector reference.");
        }
        
        // Ensure container is clear initially
        if (wordDisplayContainer != null)
        {
            ClearAllWordDisplays();
        }
        else
        {
            Debug.LogError("👁️ EyeButtonController: wordDisplayContainer is null! Check inspector reference.");
        }
            
        // Set initial button color
        if (eyeButtonImage != null)
        {
            eyeButtonImage.color = inactiveColor;
        }
        else
        {
            Debug.LogWarning("👁️ EyeButtonController: eyeButtonImage is null - color feedback disabled");
        }
            
        Debug.Log("👁️ EyeButtonController - Initialization complete");
    }

    // 🔥 NEW METHOD: Show all words at once near their bubbles
    public void ShowAllWords(List<(string word, Vector3 bubblePosition)> wordPositions)
    {
        if (wordDisplayPrefab == null)
        {
            Debug.LogError("👁️ wordDisplayPrefab is null! Check inspector reference.");
            return;
        }

        if (wordDisplayContainer == null)
        {
            Debug.LogError("👁️ wordDisplayContainer is null! Check inspector reference.");
            return;
        }

        ClearAllWordDisplays();

        Debug.Log($"👁️ Showing {wordPositions.Count} words simultaneously");

        foreach (var wp in wordPositions)
        {
            CreateWordDisplay(wp.word, wp.bubblePosition);
        }

        // Visual feedback
        if (eyeButtonImage != null)
            eyeButtonImage.color = activeColor;

        // Auto-hide after duration
        StartCoroutine(HideAllAfterDelay());
    }

    // UPDATE the CreateWordDisplay method:
    private void CreateWordDisplay(string word, Vector3 bubblePosition)
    {
        GameObject wordDisplay = Instantiate(wordDisplayPrefab, wordDisplayContainer);
        currentWordDisplays.Add(wordDisplay);

        TextMeshProUGUI textComponent = wordDisplay.GetComponentInChildren<TextMeshProUGUI>();
        if (textComponent != null)
        {
            textComponent.text = word.ToUpper();
        }

        // Position the word display near the bubble
        RectTransform rectTransform = wordDisplay.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            // Convert bubble world position to canvas position
            Vector2 canvasPos = WorldToCanvasPosition(bubblePosition);
        
            // 🔥 FIX: Use smaller offset and ensure it's within screen bounds
            canvasPos += new Vector2(0f, -150f); // Smaller offset
        
            // 🔥 NEW: Clamp position to stay within screen bounds
            canvasPos = ClampToCanvas(canvasPos, rectTransform.sizeDelta);
        
            rectTransform.anchoredPosition = canvasPos;
        
            Debug.Log($"👁️ Created word display: '{word}' at canvas position: {canvasPos} (bubble at: {bubblePosition})");
        }

        wordDisplay.SetActive(true);
    }

    // 🔥 NEW METHOD: Keep word displays within canvas bounds
    private Vector2 ClampToCanvas(Vector2 position, Vector2 displaySize)
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            Vector2 canvasHalfSize = canvasRect.sizeDelta * 0.5f;
        
            // Clamp X position
            float maxX = canvasHalfSize.x - displaySize.x * 0.5f - 10f;
            float minX = -canvasHalfSize.x + displaySize.x * 0.5f + 10f;
            position.x = Mathf.Clamp(position.x, minX, maxX);
        
            // Clamp Y position  
            float maxY = canvasHalfSize.y - displaySize.y * 0.5f - 10f;
            float minY = -canvasHalfSize.y + displaySize.y * 0.5f + 10f;
            position.y = Mathf.Clamp(position.y, minY, maxY);
        }
    
        return position;
    }

    // REPLACE the WorldToCanvasPosition method in EyeButtonController:
    private Vector2 WorldToCanvasPosition(Vector3 worldPosition)
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            // For Screen Space - Overlay mode, world position is already in screen coordinates
            Vector2 screenPosition = worldPosition;
        
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        
            // Convert screen position to canvas local position
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, 
                screenPosition, 
                null, // No camera needed for Overlay mode
                out localPoint
            );
        
            Debug.Log($"👁️ Position Conversion - World: {worldPosition}, Screen: {screenPosition}, Canvas Local: {localPoint}");
            return localPoint;
        }
        else if (canvas != null)
        {
            // For other render modes (Screen Space - Camera, World Space)
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            Camera canvasCamera = canvas.worldCamera ?? Camera.main;
        
            Vector2 screenPosition = canvasCamera.WorldToScreenPoint(worldPosition);
        
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, 
                screenPosition, 
                canvasCamera, 
                out localPoint
            );
        
            Debug.Log($"👁️ Position Conversion - World: {worldPosition}, Screen: {screenPosition}, Canvas Local: {localPoint}");
            return localPoint;
        }
    
        Debug.LogError("👁️ No canvas found!");
        return Vector2.zero;
    }

    private void ToggleWordDisplay()
    {
        Debug.Log("👁️ Eye button clicked");
        // The actual display is now handled by ShowAllWords method
    }

    private IEnumerator HideAllAfterDelay()
    {
        Debug.Log("👁️ Word displays shown - waiting " + displayDuration + " seconds");
        
        yield return new WaitForSeconds(displayDuration);
        
        HideAllWordDisplays();
    }

    private void HideAllWordDisplays()
    {
        ClearAllWordDisplays();
        
        if (eyeButtonImage != null)
            eyeButtonImage.color = inactiveColor;
            
        Debug.Log("👁️ All word displays hidden");
    }

    private void ClearAllWordDisplays()
    {
        foreach (var display in currentWordDisplays)
        {
            if (display != null)
                Destroy(display);
        }
        currentWordDisplays.Clear();
    }

    public bool IsDisplaying()
    {
        return currentWordDisplays.Count > 0;
    }

        // ADD this method to EyeButtonController:
    private void DebugCanvasInfo()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            Debug.Log($"👁️ Canvas Info:");
            Debug.Log($"   - Render Mode: {canvas.renderMode}");
            Debug.Log($"   - Size: {canvas.GetComponent<RectTransform>().sizeDelta}");
            Debug.Log($"   - Scale Factor: {canvas.scaleFactor}");
            Debug.Log($"   - Reference Resolution: {canvas.GetComponent<CanvasScaler>()?.referenceResolution}");
        }
    }

    
}