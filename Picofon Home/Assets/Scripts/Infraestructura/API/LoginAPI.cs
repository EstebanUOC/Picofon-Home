using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class LoginAPI
{
    private const string LOGIN_URL = "https://ehc-picofon2.techlab.uoc.edu/api/v1/unity-proxy/auth/login";

    [System.Serializable]
    public class FirebaseLoginPayload
    {
        public string firebase_id_token;
    }


    public IEnumerator SendFirebaseToken(string idToken, System.Action<bool> onComplete)
    {
        FirebaseLoginPayload payload = new FirebaseLoginPayload { firebase_id_token = idToken };
        string json = JsonUtility.ToJson(payload); // ✅ will correctly serialize now

        using (UnityWebRequest req = new UnityWebRequest(LOGIN_URL, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");

            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("✅ Login token sent successfully!");
                Debug.Log(req.downloadHandler.text); // backend response
                onComplete?.Invoke(true);
            }
            else
            {
                Debug.LogError("Login API error: " + req.error + "\nResponse: " + req.downloadHandler.text);
                Debug.LogError($"❌ Login API error ({req.responseCode}): {req.downloadHandler.text}");
                onComplete?.Invoke(false);
            }
        }
    }



    
}
