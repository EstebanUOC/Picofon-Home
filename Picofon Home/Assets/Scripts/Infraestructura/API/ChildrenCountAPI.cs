using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ChildrenCountAPI : MonoBehaviour
{
    public IEnumerator SendFirebaseToken(string id, Action<bool, List<ChildModel>> onComplete)
    {
        UserChildrenCountRequest payload = new(id);

        using UnityWebRequest req = new(payload.Url, "GET");

        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Children count request sent successfully!");
            string jsonResponse = req.downloadHandler.text;

            UserChildrenCountResponse response = UserChildrenCountResponse.FromJson(jsonResponse);
            onComplete?.Invoke(true, response.Data);
        }
        else
        {
            Debug.LogError(
                "Children count API error: " + req.error + "\nResponse: " + req.downloadHandler.text
            );
            Debug.LogError($"❌ Login API error ({req.responseCode}): {req.downloadHandler.text}");
            onComplete?.Invoke(false, null);
        }
    }
}
