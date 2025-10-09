using UnityEngine;
using System.Collections.Generic;

public class BasketGameManager : MonoBehaviour
{
    [Header("⚙️ Task Settings (for testing)")]
    [Range(0, 2)]
    public int typeTask = 1; // 0 = 2 balls + 2 hoops | 1 = 4 balls + 1 hoop | 2 = 4 balls + 1 hoop + target image
    public JudgeController judgeController;
    [Header("🔗 References")]
    public GameObject ballPrefab;
    public Transform ballContainer;
    public Transform hoopLeft;     // Left hoop (only for type 0)
    public Transform hoopCenter;   // Middle hoop    
    public GameObject dashboard;   // Optional: dashboard panel for target image
    public GameObject targetImage; // Image that appears only in typeTask = 2

    private List<GameObject> activeBalls = new List<GameObject>();

    void Start()
    {
        SetupScene();
    }

    void SetupScene()
    {
        // Hide all variable elements by default
        if (hoopCenter != null) hoopCenter.gameObject.SetActive(false);
        if (hoopLeft != null) hoopLeft.gameObject.SetActive(false);
        if (dashboard != null) dashboard.SetActive(false);
        if (targetImage != null) targetImage.SetActive(false);

        // Adjust elements based on typeTask
        switch (typeTask)
        {
            case 0:
                SpawnBalls(2);               
                if (judgeController != null)
                    judgeController.ActivateTypeJudge();               
                break;

            case 1:
                SpawnBalls(4);
                if (hoopCenter != null) hoopCenter.gameObject.SetActive(true);
                break;

            case 2:
                SpawnBalls(4);
                if (hoopCenter != null) hoopCenter.gameObject.SetActive(true);
                if (dashboard != null) dashboard.SetActive(true);
                if (targetImage != null) targetImage.SetActive(true);
                break;
        }
    }

    void SpawnBalls(int count)
    {
        // Clear any old balls
        foreach (var ball in activeBalls)
            Destroy(ball);
        activeBalls.Clear();

        float startX;
        float spacing;
        float yPos;

        if (typeTask == 0)
        {
            // Custom layout for 2 balls
            startX = -170f;
            spacing = 310f; // between -170 and 140 → difference = 310
            yPos = 300f; // same height as before, you can tweak
        }
        else
        {
            // Default layout
            startX = -450f;
            spacing = 280f;
            yPos = 300f;
}

        for (int i = 0; i < count; i++)
        {
            Vector3 position = new Vector3(startX + i * spacing, yPos, 0f);
            GameObject ball = Instantiate(ballPrefab, ballContainer);
            ball.GetComponent<RectTransform>().anchoredPosition = position;

            BallController bc = ball.GetComponent<BallController>();

            // 🟠 choose the correct image for each ball (temporary static)
            Sprite exampleSprite = Resources.Load<Sprite>($"StaticImages/image_{i + 1}");

            // Logic per type_task
            if (typeTask == 0)
            {
                // 2 balls, NOT clickable
                Transform target = (i % 2 == 0) ? hoopLeft : hoopCenter;
                bc.Initialize(target, exampleSprite, false);

                // Register each ball in the JudgeController list so it can move them later
                if (judgeController != null)
                    judgeController.activeBalls.Add(bc);
            }
            else if (typeTask == 1 || typeTask == 2)
            {
                // 4 balls, clickable
                bc.Initialize(hoopCenter, exampleSprite, true);
            }

            activeBalls.Add(ball);
        }
    }

}
