using UnityEngine;
using System.Collections;

/// <summary>
/// 🌊 Controlador global del minijuego CrossRiver.
/// Se encarga de solicitar las actividades desde la API y pasarlas al CrossRiverManager.
/// </summary>
public class CrossRiverGameManager : MonoBehaviour
{
    [Header("Referencias principales")]
    [SerializeField] private GameAPIService api;              // Servicio HTTP
    [SerializeField] private CrossRiverManager riverManager;  // Manager visual del minijuego

    [Header("Modo de juego")]
    [SerializeField, Range(0, 3)]
    private int currentMode = 0; // 0=Judge, 1=Relate, 2=Create, 3=Select

    private void Start()
    {
        if (api == null)
        {
            Debug.LogError("❌ Falta referencia a GameAPIService.");
            return;
        }

        if (riverManager == null)
        {
            Debug.LogError("❌ Falta referencia a CrossRiverManager.");
            return;
        }

        Debug.Log($"🌐 Solicitando actividades del modo {currentMode} (Judge)...");
        StartCoroutine(api.LoadActivity(currentMode, OnJsonLoaded, OnError));
    }

    /// <summary>
    /// ✅ Callback al recibir la respuesta JSON desde la API.
    /// </summary>
    private void OnJsonLoaded(string json)
    {
        if (string.IsNullOrEmpty(json))
        {
            Debug.LogError("❌ JSON vacío o inválido recibido desde la API.");
            return;
        }

        Debug.Log($"✅ JSON recibido correctamente para modo {currentMode}.");
        riverManager.LoadMode(currentMode, json);
    }

    /// <summary>
    /// 🔴 Callback si ocurre un error HTTP o de red.
    /// </summary>
    private void OnError(string error)
    {
        Debug.LogError($"🚨 Error al cargar actividades del modo {currentMode}: {error}");
    }
}
