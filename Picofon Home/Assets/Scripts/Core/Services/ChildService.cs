using UnityEngine.Networking;

public class ChildService
{
    private const string url = "https://ehc-picofon2.techlab.uoc.edu/api/v1/unity-proxy/children/";
    private readonly UnityWebRequest postRequest = new(url, "POST");

    public UnityWebRequest SendChildData(string jsonData)
    {
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        postRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
        postRequest.downloadHandler = new DownloadHandlerBuffer();
        postRequest.SetRequestHeader("Content-Type", "application/json");

        return postRequest;
    }
}
