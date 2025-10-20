using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class TherapyAPI : MonoBehaviour
{
    private const string BASE_URL = "https://108.130.147.206/api/v1/unity-proxy/therapy/child";

    public IEnumerator LoadTherapyPlans(
        string childId,
        System.Action<List<TherapyPlan>> onSuccess)
    {
        string url = $"{BASE_URL}/{childId}";
        Debug.Log($"🌐 Requesting therapy data → {url}");

        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                string json = req.downloadHandler.text;
                Debug.Log($"📥 Therapy JSON: {json.Substring(0, Mathf.Min(300, json.Length))}");

                TherapyResponse response = JsonUtility.FromJson<TherapyResponse>(json);
                onSuccess?.Invoke(response.data);
            }
            else
            {
                Debug.LogError($"❌ TherapyAPI error: {req.error}");
                onSuccess?.Invoke(null);
            }
        }
    }
}
