using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ChildService
{
    private const string url = "https://ehc-picofon2.techlab.uoc.edu/api/children/";

    public IEnumerator SendChildData(ChildModel childData, Action<bool> onComplete)
    {
        string jsonData = childData.ToJson();

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);

        using UnityWebRequest req = new(url, "POST");

        req.uploadHandler = new UploadHandlerRaw(bodyRaw);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Child data sent successfully!");
            Debug.Log(req.downloadHandler.text); // backend response
            onComplete?.Invoke(true);
        }
        else
        {
            Debug.LogError(
                "Child API error: " + req.error + "\nResponse: " + req.downloadHandler.text
            );
        }
    }
}
