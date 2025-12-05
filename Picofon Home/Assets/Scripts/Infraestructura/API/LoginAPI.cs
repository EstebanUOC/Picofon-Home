using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class LoginAPI
{
    private const string LOGIN_URL =
        "https://ehc-picofon2.techlab.uoc.edu/api/v1/unity-proxy/auth/login";

    // public IEnumerator SendFirebaseToken(string idToken, System.Action<bool, UserModel> onComplete)
    // {
    //     LoginRequest payload = new() { FirebaseIdToken = idToken };
    //     string jsonData = payload.ToJson();
    //
    //     using UnityWebRequest req = new(LOGIN_URL, "POST");
    //
    //     byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
    //
    //     req.uploadHandler = new UploadHandlerRaw(bodyRaw);
    //     req.downloadHandler = new DownloadHandlerBuffer();
    //     req.SetRequestHeader("Content-Type", "application/json");
    //
    //     yield return req.SendWebRequest();
    //
    //     if (req.result == UnityWebRequest.Result.Success)
    //     {
    //         Debug.Log("✅ Login token sent successfully!");
    //         string jsonResponse = req.downloadHandler.text;
    //
    //         LoginResponse response = LoginResponse.FromJson(jsonResponse);
    //         onComplete?.Invoke(true, response.Data);
    //     }
    //     else
    //     {
    //         Debug.LogError(
    //             "Login API error: " + req.error + "\nResponse: " + req.downloadHandler.text
    //         );
    //         Debug.LogError($"❌ Login API error ({req.responseCode}): {req.downloadHandler.text}");
    //         onComplete?.Invoke(false, null);
    //     }
    // }
}
