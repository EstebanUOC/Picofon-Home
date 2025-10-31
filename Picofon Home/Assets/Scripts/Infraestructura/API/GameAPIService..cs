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
    // 🔗 URL base del nuevo servidor HTTPS
    // ============================================================
    private const string BASE_URL = "https://ehc-picofon2.techlab.uoc.edu/api/v1/unity-proxy/questions/";

    // ============================================================
    // 📚 Endpoints por modo (orden: 0=Judge, 1=Select, 2=Relate, 3=Create)
    // ============================================================
    private readonly string[] MODE_ENDPOINTS = {
        "1/1805359203",   // 🧠 JUDGE
        "8/1805359203",  // 🎯 SELECT
        "9/1805359203",   // 🔗 RELATE
        "10/1805359203"    // ✍️ CREATE
    };

    // ============================================================
    // 🟢 Cargar actividad según el modo de juego
    // ============================================================
    public IEnumerator LoadActivity(int mode, Action<string> onSuccess, Action<string> onError = null)
    {
        // Ajusta el índice para evitar errores fuera de rango
        mode = Mathf.Clamp(mode, 0, MODE_ENDPOINTS.Length - 1);
        string url = BASE_URL + MODE_ENDPOINTS[mode];

        Debug.Log($"🌐 Solicitando datos del modo {mode} → {url}");

        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            // Desactiva validación SSL solo si el servidor tiene certificado autofirmado
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
    // 🧩 Clase auxiliar — Desactiva verificación SSL para entornos de prueba
    // ============================================================
    private class BypassCertificate : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            // ⚠️ IMPORTANTE: usar solo para desarrollo (no en producción)
            return true;
        }
    }
}
