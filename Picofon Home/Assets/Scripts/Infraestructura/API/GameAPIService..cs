using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// 🌐 Servicio centralizado para cargar actividades desde la API.
/// Compatible con todos los modos (Judge, Relate, Create, Select).
/// </summary>
public class GameAPIService : MonoBehaviour
{
    // ============================================================
    // 🔗 URL base del servidor (HTTPS)
    // ============================================================
    private const string BASE_URL = "https://108.130.147.206/api/v1/unity-proxy/questions/";

    // Endpoints por modo (orden: 1=Judge, 2=Select, 3=Relate, 4=Create)
    private readonly string[] MODE_ENDPOINTS = {
        "1/1805359203",   // 🧠 JUDGE
        "8/1805359203",   // 🔗 RELATE
        "9/1805359203",   // ✍️ CREATE
        "10/1805359203"   // 🎯 SELECT
    };

    // ============================================================
    // 🟢 Cargar actividad según el modo de juego
    // ============================================================
    public IEnumerator LoadActivity(int mode, Action<string> onSuccess, Action<string> onError = null)
    {
        mode = Mathf.Clamp(mode, 0, MODE_ENDPOINTS.Length - 1);
        string url = BASE_URL + MODE_ENDPOINTS[mode];

        Debug.Log($"🌐 Solicitando datos del modo {mode} → {url}");

        // ⚠️ Configurar petición HTTPS
        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            // Desactiva verificación SSL si el servidor usa IP sin certificado válido
            req.certificateHandler = new BypassCertificate();
            req.timeout = 15; // segundos

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

    // ============================================================
    // 🧩 Clase auxiliar — Desactiva verificación SSL para pruebas locales
    // ============================================================
    private class BypassCertificate : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            // ⚠️ SOLO usar para desarrollo (IP sin SSL válido)
            return true;
        }
    }
}
