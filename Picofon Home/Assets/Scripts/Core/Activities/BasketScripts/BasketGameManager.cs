using UnityEngine;
using System.Collections.Generic;

public class BasketGameManager : MonoBehaviour
{
    [Header("🌐 API Connection")]
    public BasketAPI api;
    public string childId = "1805359203"; // can be set dynamically from MapScene
    public int therapyTemplateId = 1;    // example from your JSON

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
        if (typeTask == 0)
        {
            StartCoroutine(api.LoadBasketActivity(therapyTemplateId, childId, OnBasketActivityLoaded));
        }
        else
        {
            SetupScene();
        }
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
            //Judge
            case 0:
                SpawnBalls(2);               
                if (judgeController != null)
                    judgeController.ActivateTypeJudge();               
                break;
            //Select
            case 1:
                SpawnBalls(4);
                if (hoopCenter != null) hoopCenter.gameObject.SetActive(true);
                break;
            //Relate
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
            startX = -150f;
            spacing = 330f; // between -170 and 140 → difference = 310
            yPos = 380f; // same height as before, you can tweak
        }
        else
        {
            // Default layout
            startX = -700f;
            spacing = 450f;
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

    private void OnBasketActivityLoaded(BasketData data)
    {
        if (data == null || data.activity1 == null)
        {
            Debug.LogError("❌ Basket API: activity1 is null");
            return;
        }

        BasketActivity activity = data.activity1;

        Debug.Log($"✅ Basket activity loaded: {activity.word1.word} vs {activity.word2.word}");

        // Set your typeTask = 0 config normally
        SetupScene();

        // Load images
        Sprite s1 = LoadSprite(activity.word1.PATH);
        Sprite s2 = LoadSprite(activity.word2.PATH);

        // Assign to balls
        AssignImagesToBalls(s1, s2);

        // Pass to JudgeController if needed
        if (judgeController != null)
            judgeController.imagesHaveSameSyllable = activity.answer;
    }

    private Sprite LoadSprite(string imageFileName)
    {
        if (string.IsNullOrEmpty(imageFileName)) return null;
        string imageName = System.IO.Path.GetFileNameWithoutExtension(imageFileName);
        return Resources.Load<Sprite>($"Images/ImgButtons/{imageName}");
    }

   private void AssignImagesToBalls(Sprite s1, Sprite s2)
   {
        if (activeBalls.Count >= 2)
        {
            BallController ball1 = activeBalls[0].GetComponent<BallController>();
            BallController ball2 = activeBalls[1].GetComponent<BallController>();

            if (ball1 != null && ball1.innerImage != null)
                ball1.innerImage.sprite = s1;

            if (ball2 != null && ball2.innerImage != null)
                ball2.innerImage.sprite = s2;
        }
   }



}
