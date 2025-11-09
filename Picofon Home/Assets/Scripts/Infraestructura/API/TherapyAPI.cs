using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TherapyAPI : MonoBehaviour
{
    private const string BaseURL =
        "https://ehc-picofon2.techlab.uoc.edu/api/v1/unity-proxy/therapy/child";

    public IEnumerator LoadTherapyPlans(string childId, Action<List<TherapyPlan>> onSuccess)
    {
        string url = $"{BaseURL}/{childId}";
        Debug.Log($"Requesting therapy data → {url}");

        using UnityWebRequest req = UnityWebRequest.Get(url);

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            string json = req.downloadHandler.text;

            TherapyResponse response = TherapyResponse.FromJson(json);
            onSuccess?.Invoke(response.Data);
        }
        else
        {
            Debug.LogError($"<DEBUG:ERROR> TherapyAPI error: {req.error}");
            onSuccess?.Invoke(null);
        }
    }
}
