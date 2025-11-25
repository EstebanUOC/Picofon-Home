using System.Collections;
using System.Collections.Generic;
using System; // 🔥 ADD THIS for Exception class
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Picofon.Games.Judge;
using Picofon.Games.Select;
using Picofon.Games.Relate;

public class BalloonPopSeaManager : MonoBehaviour
{
    [Header("🫧 Burbujas")]
    [SerializeField] private Transform bubbleContainerHorizontal1;
    [SerializeField] private Transform bubbleContainerHorizontal2;
    [SerializeField] private GameObject bubblePrefab;

    [Header("✅ Botones Sí / No (solo para modo Judge)")]
    [SerializeField] private Button buttonYes;
    [SerializeField] private Button buttonNo;

    [Header("⭐ Feedback Panel")]
    [SerializeField] private FeedbackPanelController feedbackController;

    [Header("🌐 API Service")]
    [SerializeField] private GameAPIService balloonPopAPI;

    private ActivityJudge currentActivity;
    private readonly List<GameObject> spawnedBubbles = new();
    private int currentTaskType = 1; // 🔥 1=Judge, 2=Select, 3=Relate (from TherapyPlan)
    private Picofon.Games.Relate.ActivityRelate currentRelateActivity;

    [Header("👁️ Eye Button")]
    [SerializeField] private EyeButtonController eyeButtonController;

    private readonly List<(string word, Vector3 position, GameObject bubble)> currentWords = new();

    private void Start()
    {
        if (balloonPopAPI == null)
        {
            Debug.LogError("❌ No se asignó GameAPIService en el inspector.");
            return;
        }

        // 🎯 Obtener el tipo de tarea del TherapyPlan actual
        currentTaskType = balloonPopAPI.GetCurrentTaskType();
        Debug.Log($"🎮 Tipo de tarea detectado: {currentTaskType}");


        // 🔥 FIX: Find eye button controller
        FindEyeButtonController();
    
        // 🧪 TEST: Uncomment this line to test the eye button
        // TestEyeButtonFunctionality();
    
        // ✅ Iniciar automáticamente la actividad
        LoadCurrentActivity();
    }

    private IEnumerator TestEyeButton()
    {
        yield return new WaitForSeconds(1f);
        Debug.Log("👁️ Running eye button test...");
       // eyeButtonController.TestWordDisplay();
    }

    // ============================================================
    // 🔥 NEW METHOD - Load activity based on TherapyPlan
    // ============================================================
    private void LoadCurrentActivity()
    {
        Debug.Log($"🔄 Cargando actividad para tipo de tarea: {currentTaskType}");

        StartCoroutine(balloonPopAPI.LoadActivity(
            json => ProcessActivityResponse(json),
            err => Debug.LogError(err)
        ));
    }

    // ============================================================
    // 🔥 UPDATED METHOD - Process response based on current task type
    // ============================================================
    private void ProcessActivityResponse(string json)
    {
        try
        {
            if (string.IsNullOrEmpty(json))
            {
                Debug.LogError("❌ JSON response is null or empty");
                return;
            }

            Debug.Log($"📄 Processing JSON: {json}");

            switch (currentTaskType)
            {
                case 1: // Judge
                    var judgeData = JsonUtility.FromJson<Picofon.Games.Judge.ApiResponseJudge>(json); // 🔥 USE FULL NAMESPACE
                    if (judgeData?.data?.activity1 != null)
                    {
                        LoadJudgeMode(judgeData.data.activity1);
                        Debug.Log($"✅ Successfully loaded Judge activity: {judgeData.data.activity1.word1.word} vs {judgeData.data.activity1.word2.word}");
                    }
                    else
                    {
                        Debug.LogError("❌ Datos Judge inválidos o nulos");
                        if (judgeData != null)
                        {
                            Debug.LogError($"🔍 Judge Data Structure - Success: {judgeData.success}, Data: {judgeData.data != null}, Activity1: {judgeData.data?.activity1 != null}");
                        }
                    }
                    break;
                    
                case 2: // Select
                    var selectData = JsonUtility.FromJson<Picofon.Games.Select.ApiResponseSelect>(json);
                    if (selectData?.data?.activity1 != null)
                    {
                        LoadSelectMode(selectData.data.activity1);
                    }
                    else
                    {
                        Debug.LogError("❌ Datos Select inválidos o nulos");
                    }
                    break;
                    
                case 3: // Relate
                    var relateData = JsonUtility.FromJson<Picofon.Games.Relate.ApiResponseRelate>(json);
                    if (relateData?.data?.activity1 != null)
                    {
                        LoadRelateMode(relateData.data.activity1);
                    }
                    else
                    {
                        Debug.LogError("❌ Datos Relate inválidos o nulos");
                    }
                    break;
                    
                default:
                    Debug.LogError($"❌ Tipo de tarea no soportado: {currentTaskType}");
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ Error procesando respuesta JSON: {e.Message}");
            Debug.LogError($"🔍 Stack trace: {e.StackTrace}");
            Debug.LogError($"📄 Problematic JSON: {json}");
        }
    }


    // ======================
    // 🧠 MODO JUDGE (1)
    // ======================
    // UPDATE LoadJudgeMode method - Add coroutine for delayed word tracking
    private void LoadJudgeMode(ActivityJudge activity)
    {
        currentActivity = activity;
        currentWords.Clear(); // Clear previous words

        // ✅ Mostrar botones Sí/No solo para Judge
        buttonYes.gameObject.SetActive(true);
        buttonNo.gameObject.SetActive(true);
        buttonYes.GetComponent<Image>().raycastTarget = true;
        buttonNo.GetComponent<Image>().raycastTarget = true;
        buttonYes.interactable = true;
        buttonNo.interactable = true;

        ClearBubbles();

        Sprite s1 = LoadSprite(activity.word1.PATH);
        Sprite s2 = LoadSprite(activity.word2.PATH);

        // Create bubbles first
        GameObject bubble1 = CreateBubble(s1);
        GameObject bubble2 = CreateBubble(s2);

        buttonYes.onClick.RemoveAllListeners();
        buttonNo.onClick.RemoveAllListeners();

        buttonYes.onClick.AddListener(() => Answer(true));
        buttonNo.onClick.AddListener(() => Answer(false));
    
        // 🔥 FIX: Use coroutine to wait for layout to update before tracking positions
        StartCoroutine(DelayedWordTracking(activity, bubble1, bubble2));
    }

    // 🔥 NEW COROUTINE: Wait for layout to update before tracking word positions
    private IEnumerator DelayedWordTracking(ActivityJudge activity, GameObject bubble1, GameObject bubble2)
    {
        // Wait for end of frame to ensure layout is calculated
        yield return new WaitForEndOfFrame();
    
        // Force layout update
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(bubbleContainerHorizontal1 as RectTransform);
    
        // Store words with their positions for eye button
        if (bubble1 != null)
        {
            Vector3 worldPos = bubble1.transform.position;
            currentWords.Add((activity.word1.word, worldPos, bubble1));
            Debug.Log($"🎯 Judge Mode - Word 1: '{activity.word1.word}' at world position: {worldPos}");
        }
    
        if (bubble2 != null)
        {
            Vector3 worldPos = bubble2.transform.position;
            currentWords.Add((activity.word2.word, worldPos, bubble2));
            Debug.Log($"🎯 Judge Mode - Word 2: '{activity.word2.word}' at world position: {worldPos}");
        }
    
        // Setup eye button after positions are calculated
        SetupEyeButtonForCurrentWords();
    }

    // ======================
    // 🎯 MODO SELECT (2)
    // ======================
    private void LoadSelectMode(Picofon.Games.Select.ActivitySelect activity)
    {
        ClearBubbles();
        currentWords.Clear(); // 🔥 ADD: Clear previous words

        // ❌ Ocultar botones Sí/No para Select
        buttonYes.gameObject.SetActive(false);
        buttonNo.gameObject.SetActive(false);
        buttonYes.GetComponent<Image>().raycastTarget = false;
        buttonNo.GetComponent<Image>().raycastTarget = false;
        buttonYes.interactable = false;
        buttonNo.interactable = false;

        // Cargar sprites
        Sprite mainSprite = LoadSprite(activity.main_word.PATH);
        Sprite correctSprite = LoadSprite(activity.correct_option.PATH);
        Sprite wrong1Sprite = LoadSprite(activity.wrong_option1.PATH);
        Sprite wrong2Sprite = LoadSprite(activity.wrong_option2.PATH);

        var options = new List<(Sprite sprite, string word, string syll, bool correct)>
        {
            (mainSprite,   activity.main_word.word,   activity.main_word.syllabified_word,   false),
            (correctSprite,activity.correct_option.word, activity.correct_option.syllabified_word, true),
            (wrong1Sprite, activity.wrong_option1.word, activity.wrong_option1.syllabified_word, false),
            (wrong2Sprite, activity.wrong_option2.word, activity.wrong_option2.syllabified_word, false),
        };

        Shuffle(options);

        // Crear 4 burbujas and track words
        List<GameObject> selectBubbles = new List<GameObject>(); // 🔥 NEW: Store bubbles

        // Crear 4 burbujas
        for (int i = 0; i < options.Count; i++)
        {
            Transform container = (i < 2) ? bubbleContainerHorizontal1 : bubbleContainerHorizontal2;
            GameObject bubble = CreateSelectBubble(options[i], container, activity); // 🔥 FIX: Store the returned bubble
            selectBubbles.Add(bubble); // 🔥 FIX: Add to list
        }

        // 🔥 NEW: Setup word tracking for Select mode
        StartCoroutine(DelayedWordTrackingSelect(activity, selectBubbles, options));
    }

    // 🔥 NEW COROUTINE: Wait for layout to update before tracking word positions for Select mode
    private IEnumerator DelayedWordTrackingSelect(Picofon.Games.Select.ActivitySelect activity, List<GameObject> bubbles, List<(Sprite sprite, string word, string syll, bool correct)> options)
    {
        // Wait for end of frame to ensure layout is calculated
        yield return new WaitForEndOfFrame();
    
        // Force layout update
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(bubbleContainerHorizontal1 as RectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(bubbleContainerHorizontal2 as RectTransform);
    
        Debug.Log($"🎯 Select Mode - Tracking {bubbles.Count} bubbles with {options.Count} options");
    
        // Store words with their positions for eye button
        for (int i = 0; i < bubbles.Count; i++)
        {
            if (bubbles[i] != null)
            {
                Vector3 worldPos = bubbles[i].transform.position;
                currentWords.Add((options[i].word, worldPos, bubbles[i]));
                Debug.Log($"🎯 Select Mode - Word {i}: '{options[i].word}' at world position: {worldPos}");
            }
            else
            {
                Debug.LogError($"🎯 Select Mode - Bubble {i} is null! Cannot track word: '{options[i].word}'");
            }
        }
    
        // Setup eye button after positions are calculated
        SetupEyeButtonForCurrentWords();
    }

    // ======================
    // 🔗 MODO RELATE (3)
    // ======================
    private void LoadRelateMode(Picofon.Games.Relate.ActivityRelate activity)
    {
        if (activity == null || activity.main_word == null)
        {
            Debug.LogError("❌ RELATE ERROR: activity.main_word es NULL, revisa JSON del backend");
            return;
        }

        ClearBubbles();
        currentWords.Clear(); // 🔥 ADD: Clear previous words

        currentRelateActivity = activity;

        // ❌ Ocultar botones Sí/No para Relate
        buttonYes.gameObject.SetActive(false);
        buttonNo.gameObject.SetActive(false);
        buttonYes.GetComponent<Image>().raycastTarget = false;
        buttonNo.GetComponent<Image>().raycastTarget = false;
        buttonYes.interactable = false;
        buttonNo.interactable = false;

        var layout = bubbleContainerHorizontal2.GetComponent<HorizontalLayoutGroup>();
        if (layout != null) layout.spacing = 150f;

        Sprite mainSprite = SafeLoadSprite(activity.main_word.PATH);
        GameObject mainBubble = CreateRelateBubble(mainSprite, activity.main_word.syllabified_word, false, bubbleContainerHorizontal1);
        
        var options = new List<(Sprite sprite, string syll, bool correct, string word)>
        {
            (SafeLoadSprite(activity.correct_option?.PATH), activity.correct_option?.syllabified_word, true, activity.correct_option?.word),
            (SafeLoadSprite(activity.wrong_option1?.PATH), activity.wrong_option1?.syllabified_word, false, activity.wrong_option1?.word),
            (SafeLoadSprite(activity.wrong_option2?.PATH), activity.wrong_option2?.syllabified_word, false, activity.wrong_option2?.word),
            (SafeLoadSprite(activity.wrong_option3?.PATH), activity.wrong_option3?.syllabified_word, false, activity.wrong_option3?.word),
        };

        options.RemoveAll(o => o.sprite == null);

        Shuffle(options);

        List<GameObject> relateBubbles = new List<GameObject>(); // 🔥 NEW: Store bubbles
        if (mainBubble != null) 
        {
            relateBubbles.Add(mainBubble);
            Debug.Log($"🎯 Relate Mode - Created main bubble: '{activity.main_word.word}'");
        }

        foreach (var op in options)
        {
            GameObject bubble = CreateRelateBubble(op.sprite, op.syll, op.correct, bubbleContainerHorizontal2);
            if (bubble != null) 
            {
                relateBubbles.Add(bubble);
                Debug.Log($"🎯 Relate Mode - Created option bubble: '{op.word}'");
            }
        }

        // 🔥 NEW: Setup word tracking for Relate mode
        StartCoroutine(DelayedWordTrackingRelate(activity, relateBubbles, mainBubble, options));
    }

    // 🔥 NEW COROUTINE: Wait for layout to update before tracking word positions for Relate mode
    private IEnumerator DelayedWordTrackingRelate(Picofon.Games.Relate.ActivityRelate activity, List<GameObject> bubbles, GameObject mainBubble, List<(Sprite sprite, string syll, bool correct, string word)> options)
    {
        // Wait for end of frame to ensure layout is calculated
        yield return new WaitForEndOfFrame();
    
        // Force layout update
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(bubbleContainerHorizontal1 as RectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(bubbleContainerHorizontal2 as RectTransform);
    
        Debug.Log($"🎯 Relate Mode - Tracking {bubbles.Count} bubbles with {options.Count + 1} words");

        // Store main word with its position
        if (mainBubble != null && activity.main_word != null)
        {
            Vector3 worldPos = mainBubble.transform.position;
            currentWords.Add((activity.main_word.word, worldPos, mainBubble));
            Debug.Log($"🎯 Relate Mode - Main Word: '{activity.main_word.word}' at world position: {worldPos}");
        }
        else
        {
            Debug.LogError($"🎯 Relate Mode - Main bubble is null! Cannot track word: '{activity.main_word?.word}'");
        }
    
        // Store option words with their positions
        for (int i = 0; i < options.Count; i++)
        {
            // Bubbles index: 0 is main bubble, so options start from index 1
            int bubbleIndex = i + 1;
            if (bubbleIndex < bubbles.Count && bubbles[bubbleIndex] != null && !string.IsNullOrEmpty(options[i].word))
            {
                Vector3 worldPos = bubbles[bubbleIndex].transform.position;
                currentWords.Add((options[i].word, worldPos, bubbles[bubbleIndex]));
                Debug.Log($"🎯 Relate Mode - Option Word {i}: '{options[i].word}' at world position: {worldPos}");
            }
            else
            {
                Debug.LogWarning($"🎯 Relate Mode - Option bubble {i} is null or empty! Cannot track word: '{options[i].word}'");
            }
        }
    
        // Setup eye button after positions are calculated
        SetupEyeButtonForCurrentWords();
    }


    private Sprite SafeLoadSprite(string path)
    {
        if (string.IsNullOrEmpty(path))
            return null;

        return LoadSprite(path);
    }

    // 🔥 UPDATE: Change return type from void to GameObject
    private GameObject CreateRelateBubble(Sprite sprite, string syll, bool isCorrect, Transform parent)
    {
        GameObject b = Instantiate(bubblePrefab, parent);
        spawnedBubbles.Add(b);

        Image img = b.transform.Find("Image").GetComponent<Image>();
        img.sprite = sprite;
        img.preserveAspect = true;

        Button btn = b.transform.Find("ButtonOp").GetComponent<Button>();

        // ✅ Burbuja principal (sin botón)
        if (parent == bubbleContainerHorizontal1)
        {
            btn.gameObject.SetActive(false);
            return b; // 🔥 ADD: Return the bubble
        }

        btn.gameObject.SetActive(true);
        btn.interactable = true;

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() =>
        {
            Sprite mainSprite = LoadSprite(currentRelateActivity.main_word.PATH);
            string mainSyll = currentRelateActivity.main_word.syllabified_word;

            // ✅ Siempre comparar seleccionado vs mainWord
            feedbackController.ShowFeedback(
                sprite,      // seleccionada
                mainSprite,  // palabra base
                isCorrect,   // correcto o neutral
                syll,
                mainSyll
            );

            if (isCorrect)
                StartCoroutine(NextActivity());
        });

        return b; // 🔥 ADD: Return the bubble GameObject
    }

    // 🔥 UPDATE: Change return type from void to GameObject
    private GameObject CreateSelectBubble(
        (Sprite sprite, string word, string syll, bool correct) option,
        Transform parent,
        Picofon.Games.Select.ActivitySelect activity)
    {
        GameObject b = Instantiate(bubblePrefab, parent);
        spawnedBubbles.Add(b);

        Image img = b.transform.Find("Image").GetComponent<Image>();
        img.sprite = option.sprite;
        img.preserveAspect = true;

        Button btn = b.transform.Find("ButtonOp").GetComponent<Button>();
        btn.interactable = true;

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() =>
        {
            bool correct = option.correct;

            if (correct)
            {
                // ✅ Feedback positivo: seleccionada vs palabra principal
                feedbackController.ShowFeedback(
                    option.sprite,
                    LoadSprite(activity.main_word.PATH),
                    true,
                    option.syll,
                    activity.main_word.syllabified_word
                );

                StartCoroutine(NextActivity());
            }
            else
            {
                // ✅ Buscar otra opción incorrecta para el feedback
                var neutralList = new List<(Sprite sprite, string syll)>
                {
                    (LoadSprite(activity.main_word.PATH), activity.main_word.syllabified_word),
                    (LoadSprite(activity.wrong_option1.PATH), activity.wrong_option1.syllabified_word),
                    (LoadSprite(activity.wrong_option2.PATH), activity.wrong_option2.syllabified_word)
                };

                // ❌ eliminar la seleccionada
                neutralList.RemoveAll(o => o.sprite == option.sprite);

                // ❌ eliminar la correcta
                Sprite correctSprite = LoadSprite(activity.correct_option.PATH);
                neutralList.RemoveAll(o => o.sprite == correctSprite);

                // 🎯 Seleccionar una incorrecta aleatoria
                var randomOther = neutralList[UnityEngine.Random.Range(0, neutralList.Count)];

                // ✅ Feedback Neutral: seleccionada + otra incorrecta
                feedbackController.ShowFeedback(
                    option.sprite,
                    randomOther.sprite,
                    false,
                    option.syll,
                    randomOther.syll
                );
            }
        });

        return b; // 🔥 ADD: Return the bubble GameObject
    }

    private void Answer(bool guess)
    {
        bool correct = (guess == currentActivity.answer);

        Sprite s1 = LoadSprite(currentActivity.word1.PATH);
        Sprite s2 = LoadSprite(currentActivity.word2.PATH);

        feedbackController.ShowFeedback(
            s1,
            s2,
            correct,
            currentActivity.word1.syllabified_word,
            currentActivity.word2.syllabified_word
        );

        if (correct)
        {
            StartCoroutine(NextActivity());
        }
    }

    // ============================================================
    // 🔥 UPDATED - Single NextActivity method for all task types
    // ============================================================
    private IEnumerator NextActivity()
    {
        yield return new WaitForSeconds(2.2f);
        LoadCurrentActivity();
    }

    // UPDATE CreateBubble method to return the GameObject
    // REPLACE your CreateBubble method with this:
    private GameObject CreateBubble(Sprite sprite)
    {
        Transform targetContainer = bubbleContainerHorizontal1;

        if (currentTaskType != 1) // 🔥 1 = Judge
            targetContainer = bubbleContainerHorizontal2;

        GameObject b = Instantiate(bubblePrefab, targetContainer);
        spawnedBubbles.Add(b);

        Image img = b.transform.Find("Image").GetComponent<Image>();
        if (img == null)
        {
            Debug.LogError("❌ No se encontró el hijo 'Image' dentro del BubblePrefab");
            return null;
        }

        img.sprite = sprite;
        img.preserveAspect = true;

        Button btn = b.GetComponentInChildren<Button>();
        if (btn) btn.interactable = false;
        b.transform.localScale = Vector3.one;
        b.transform.localRotation = Quaternion.identity;
    
        // 🔥 ADDED: Force layout rebuild to get correct positions
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(targetContainer as RectTransform);
    
        Debug.Log($"🫧 Bubble created at world position: {b.transform.position}, local position: {b.transform.localPosition}");
    
        return b;
    }

    private void ClearBubbles()
    {
        foreach (var b in spawnedBubbles) Destroy(b);
        spawnedBubbles.Clear();
    }

    private Sprite LoadSprite(string p)
    {
        string file = System.IO.Path.GetFileNameWithoutExtension(p);
        Sprite s = Resources.Load<Sprite>($"Images/ImgButtons/{file}");

        if (!s)
            Debug.LogWarning($"⚠ No se encontró sprite: {file}");

        return s;
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = UnityEngine.Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    // 🔥 UPDATE SetupEyeButtonForCurrentWords method:
    private void SetupEyeButtonForCurrentWords()
    {
        Debug.Log($"👁️ SetupEyeButtonForCurrentWords called - Current words count: {currentWords.Count}");
        Debug.Log($"👁️ EyeButtonController reference: {eyeButtonController != null}");

        if (eyeButtonController != null && currentWords.Count > 0)
        {
            Debug.Log($"👁️ Setting up eye button with {currentWords.Count} words:");
            foreach (var wordInfo in currentWords)
            {
                Debug.Log($"   - '{wordInfo.word}' at {wordInfo.position} (Bubble: {wordInfo.bubble != null})");
            }

            // Clear previous listeners
            Button eyeButton = eyeButtonController.GetComponent<Button>();
            if (eyeButton != null)
            {
                eyeButton.onClick.RemoveAllListeners();
                eyeButton.onClick.AddListener(() => 
                {
                    Debug.Log("👁️ Eye button clicked! Showing ALL words simultaneously...");
                
                    // 🔥 FIX: Create list of word-position pairs for ALL words
                    var wordPositions = new List<(string word, Vector3 bubblePosition)>();
                
                    foreach (var wordInfo in currentWords)
                    {
                        if (wordInfo.bubble != null)
                        {
                            // Update position in case bubbles moved
                            Vector3 currentPosition = wordInfo.bubble.transform.position;
                            Debug.Log($"👁️ Adding '{wordInfo.word}' at position: {currentPosition}");
                            wordPositions.Add((wordInfo.word, currentPosition));
                        }
                        else
                        {
                            Debug.LogWarning($"👁️ Bubble for word '{wordInfo.word}' is null!");
                        }
                    }
                
                    // 🔥 FIX: Show ALL words at once
                    if (wordPositions.Count > 0)
                    {
                        eyeButtonController.ShowAllWords(wordPositions);
                    }
                    else
                    {
                        Debug.LogWarning("👁️ No valid word positions to display");
                    }
                });
            
                Debug.Log("👁️ Eye button setup completed successfully");
            }
            else
            {
                Debug.LogError("👁️ No Button component found on eyeButtonController!");
            }
        }
        else
        {
            if (eyeButtonController == null)
                Debug.LogWarning("👁️ EyeButtonController is null - check inspector reference");
            else if (currentWords.Count == 0)
                Debug.LogWarning("👁️ No words available for eye button setup");
        }
    }

    // ADD this method to test the eye button:
    private void TestEyeButtonFunctionality()
    {
        if (eyeButtonController != null)
        {
            Debug.Log("👁️ Testing eye button functionality...");
        
            // Test with some dummy positions
            Vector3 testPos1 = new Vector3(200f, 200f, 0f);
            Vector3 testPos2 = new Vector3(400f, 200f, 0f);
        
            //eyeButtonController.ShowWordAtPosition("TEST WORD 1", testPos1);
        
            // Test second word after delay
            StartCoroutine(TestSecondWord(testPos2));
        }
        else
        {
            Debug.LogError("👁️ Cannot test - eyeButtonController is null");
        }
    }

   private IEnumerator TestSecondWord(Vector3 position)
   {
        yield return new WaitForSeconds(3.5f);
       // eyeButtonController.ShowWordAtPosition("TEST WORD 2", position);
   }

    // ADD this method to BalloonPopSeaManager:
   private void FindEyeButtonController()
   {
        if (eyeButtonController == null)
        {
            // Try to find it in the scene
            eyeButtonController = FindObjectOfType<EyeButtonController>();
        
            if (eyeButtonController != null)
            {
                Debug.Log("👁️ Found EyeButtonController automatically in scene");
            }
            else
            {
                Debug.LogError("👁️ Could not find EyeButtonController in scene! Please assign it in inspector.");
            }
        }
        else
        {
            Debug.Log("👁️ EyeButtonController already assigned in inspector");
        }
   }

    


}


