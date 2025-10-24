using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class LoginAPI
{
    private const string LOGIN_URL = "https://ehc-picofon2.techlab.uoc.edu/api/v1/unity-proxy/auth/login";

    public IEnumerator SendFirebaseToken(string idToken, System.Action<bool> onComplete)
    {
        var payload = new { firebase_id_token = idToken };
        string json = JsonUtility.ToJson(payload);

        using (UnityWebRequest req = new UnityWebRequest(LOGIN_URL, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");

            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                onComplete?.Invoke(true);
            }
            else
            {
                Debug.LogError("Login API error: " + req.error);
                onComplete?.Invoke(false);
            }
        }
    }
}
