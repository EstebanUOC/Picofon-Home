using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class GameAPIService : MonoBehaviour
{
    private const string BASE_URL = "https://ehc-picofon2.techlab.uoc.edu/api/v1/unity-proxy/questions/";

    public IEnumerator LoadActivity(Action<string> onSuccess, Action<string> onError = null)
    {
        // 🔍 Obtener el TherapyPlan actual desde LevelDataStore
        int currentPlanId = LevelPayload.PlanId;
        TherapyPlan currentPlan = LevelDataStore.Instance.GetLevelPlan(currentPlanId);

        if (currentPlan == null)
        {
            string errorMsg = "❌ No se pudo obtener el TherapyPlan actual";
            Debug.LogError(errorMsg);
            onError?.Invoke(errorMsg);
            yield break;
        }

        // 🎯 CORREGIDO: Usar el ID del plan de terapia (32) no el template ID (10)
        string therapyPlanId = currentPlan.Id.ToString(); // 🔥 CAMBIADO: currentPlan.Id en lugar de currentPlan.TherapyTemplateId
        string childId = currentPlan.ChildId;
        string url = BASE_URL + therapyPlanId + "/" + childId;

        Debug.Log($"🌐 Solicitando datos del plan {currentPlanId} → {url}");
        Debug.Log($"📋 Modo de juego: {currentPlan.TherapyTemplate?.TaskTypeId} - {currentPlan.TherapyTemplate?.TaskTypeName}");

        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            req.certificateHandler = new BypassCertificate();
            req.timeout = 15;

            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success && req.responseCode == 200)
            {
                string json = req.downloadHandler.text;
                Debug.Log($"✅ Respuesta recibida (plan {currentPlanId}) — longitud: {json.Length}");
                
                // Log the actual JSON to see what we're getting
                Debug.Log($"📄 JSON Response: {json}");
                
                onSuccess?.Invoke(json);
            }
            else
            {
                string errorMsg = $"❌ Error al solicitar plan {currentPlanId}: {req.error} (HTTP {req.responseCode})";
                Debug.LogError(errorMsg);
                
                // Log the response body even for errors
                if (req.downloadHandler != null && !string.IsNullOrEmpty(req.downloadHandler.text))
                {
                    Debug.LogError($"📄 Error Response: {req.downloadHandler.text}");
                }
                
                onError?.Invoke(errorMsg);
            }
        }
    }

    public int GetCurrentTaskType()
    {
        int currentPlanId = LevelPayload.PlanId;
        TherapyPlan currentPlan = LevelDataStore.Instance.GetLevelPlan(currentPlanId);
        
        if (currentPlan?.TherapyTemplate != null)
        {
            return currentPlan.TherapyTemplate.TaskTypeId;
        }
        
        return 1; // Default to Judge if no plan found
    }

    private class BypassCertificate : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true;
        }
    }
}
