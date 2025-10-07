using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class BalloonPopSeaAPI : MonoBehaviour
{
    // URL del modo 1
    private const string URL_MODE1 = "http://108.130.147.206/api/unity-proxy/questions/36881/1805359203";
    // URL del modo 0
    private const string URL_MODE0 = "http://108.130.147.206/api/unity-proxy/questions/36295/1805359203";

    // ==============================================================
    // 🔹 Cargar actividades del modo 1
    // ==============================================================
    public IEnumerator LoadActivities(System.Action<Data> onSuccess)
    {
        using (UnityWebRequest req = UnityWebRequest.Get(URL_MODE1))
        {
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                string json = req.downloadHandler.text;
                Debug.Log($"📩 JSON modo 1 recibido: {json.Substring(0, Mathf.Min(200, json.Length))}");
                ApiResponse response = JsonUtility.FromJson<ApiResponse>(json);
                onSuccess?.Invoke(response.data);
            }
            else
            {
                Debug.LogError($"❌ Error modo 1: {req.error}");
            }
        }
    }

    // ==============================================================
    // 🔹 Cargar actividades del modo 0 (Sí / No)
    // ==============================================================
    public IEnumerator LoadSimpleActivities(System.Action<DataSimple> onSuccess)
    {
        using (UnityWebRequest req = UnityWebRequest.Get(URL_MODE0))
        {
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                string json = req.downloadHandler.text;
                Debug.Log($"📩 JSON modo 0 recibido: {json.Substring(0, Mathf.Min(200, json.Length))}");
                ApiResponseSimple response = JsonUtility.FromJson<ApiResponseSimple>(json);
                onSuccess?.Invoke(response.data);
            }
            else
            {
                Debug.LogError($"❌ Error modo 0: {req.error}");
            }
        }
    }
}
