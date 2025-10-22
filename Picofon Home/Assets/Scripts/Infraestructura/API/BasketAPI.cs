using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

// Handles requests for BasketScene (type_task = 0)
public class BasketAPI : MonoBehaviour
{
    // Example endpoint:
    // https://ehc-picofon2.techlab.uoc.edu/api/v1/unity-proxy/questions/{therapyID}/{childID}

    private const string BASE_URL = "https://ehc-picofon2.techlab.uoc.edu/api/v1/unity-proxy/questions";

    // Request activity for given therapy + child
    public IEnumerator LoadBasketActivity(
        int therapyTemplateId,
        string childId,
        System.Action<BasketData> onSuccess)
    {
        string url = $"{BASE_URL}/{therapyTemplateId}/{childId}";
        Debug.Log($"🌐 Requesting Basket activity → {url}");

        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                string json = req.downloadHandler.text;
                Debug.Log($"📥 Basket JSON: {json.Substring(0, Mathf.Min(250, json.Length))}");

                BasketResponse response = JsonUtility.FromJson<BasketResponse>(json);
                onSuccess?.Invoke(response.data);
            }
            else
            {
                Debug.LogError($"❌ BasketAPI error: {req.error}");
            }
        }
    }
}
