using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Servicio centralizado para cargar actividades desde la API.
/// Compatible con todos los modos (Judge, Relate, Create, Select).
/// </summary>
public class GameAPIService : MonoBehaviour
{
    // URL base del servidor
    private const string BASE_URL = "http://108.130.147.206/api/v1/unity-proxy/questions/";

    // Endpoints por modo (orden: 0=Judge, 1=Relate, 2=Create, 3=Select)
    private readonly string[] MODE_ENDPOINTS = {
        "1/1805359203",  // Judge
        "8/1805359203",  // Relate
        "9/1805359203",  // Create
        "10/1805359203"  // Select
    };

    /// <summary>
    /// Llama a la API y devuelve el JSON completo del modo solicitado.
    /// </summary>
    /// <param name="mode">Modo de juego (0–3)</param>
    /// <param name="onSuccess">Callback al recibir datos correctamente</param>
    /// <param name="onError">Callback si ocurre un error HTTP o de red</param>
    public IEnumerator LoadActivity(int mode, Action<string> onSuccess, Action<string> onError = null)
    {
        // Evita valores fuera de rango
        mode = Mathf.Clamp(mode, 0, MODE_ENDPOINTS.Length - 1);
        string url = BASE_URL + MODE_ENDPOINTS[mode];

        Debug.Log($"🌐 Solicitando datos del modo {mode} → {url}");

        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success && req.responseCode == 200)
            {
                string json = req.downloadHandler.text;
                Debug.Log($"✅ Respuesta recibida (modo {mode}) — longitud: {json.Length}");
                onSuccess?.Invoke(json);
            }
            else
            {
                string errorMsg = $"❌ Error al solicitar modo {mode}: {req.error} (HTTP {req.responseCode})";
                Debug.LogError(errorMsg);
                onError?.Invoke(errorMsg);
            }
        }
    }
}
