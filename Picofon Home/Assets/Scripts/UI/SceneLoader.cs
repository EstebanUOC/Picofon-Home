using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class SceneLoader : MonoBehaviour
{
    public string mapSceneName = "Scenes/MapPathScene";  
    
    private const string PROCESSED_PLANS_KEY = "ProcessedPlanIds";
   
    public void LoadMapScene()
    {
        OnLevelCompleted();
        SceneManager.LoadScene(mapSceneName);
    }
    
    private void OnLevelCompleted()
    {
        int currentPlanId = LevelPayload.PlanId;
        int lastCompleted = GamePrefs.LastCompletedLevel;
        
        Debug.Log($"Current PlanId: {currentPlanId}, LastCompleted: {lastCompleted}");
        
        // Get already processed PlanIds from PlayerPrefs
        HashSet<int> processedPlanIds = GetProcessedPlanIds();
        
        // Check if we've already processed this PlanId
        if (!processedPlanIds.Contains(currentPlanId))
        {
            // Increment and save
            GamePrefs.LastCompletedLevel = lastCompleted + 1;
            
            // Mark this PlanId as processed and save
            processedPlanIds.Add(currentPlanId);
            SaveProcessedPlanIds(processedPlanIds);
            
            PlayerPrefs.Save();
            
            Debug.Log($"🎉 Level progress updated! Last completed level: {GamePrefs.LastCompletedLevel} (PlanId: {currentPlanId})");
        }
        else
        {
            Debug.Log($"ℹ️ PlanId {currentPlanId} already processed, no increment needed.");
        }
    }
    
    private HashSet<int> GetProcessedPlanIds()
    {
        string savedIds = PlayerPrefs.GetString(PROCESSED_PLANS_KEY, "");
        if (string.IsNullOrEmpty(savedIds)) return new HashSet<int>();
        
        return new HashSet<int>(savedIds.Split(',').Select(int.Parse));
    }
    
    private void SaveProcessedPlanIds(HashSet<int> planIds)
    {
        string idsString = string.Join(",", planIds);
        PlayerPrefs.SetString(PROCESSED_PLANS_KEY, idsString);
    }
}