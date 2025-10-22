using UnityEngine;
using System.Collections;

/// <summary>
/// Controlador global del minijuego BalloonPopSea.
/// Se encarga de pedir los datos a la API y pasarlos al manager de escena.
/// </summary>
public class BalloonPopSeaGameManager : MonoBehaviour
{
    [Header("Referencias principales")]
    [SerializeField] private GameAPIService api;              // Servicio HTTP
    [SerializeField] private BalloonPopSeaManager seaManager; // Manager visual del minijuego

    [Header("Modo de juego")]
    [SerializeField, Range(0, 3)]
    private int currentMode = 1; // 0=Judge, 1=Relate, 2=Create, 3=Select

    private void Start()
    {
        if (api == null)
        {
            Debug.LogError("❌ Falta referencia al GameAPIService.");
            return;
        }

        if (seaManager == null)
        {
            Debug.LogError("❌ Falta referencia al BalloonPopSeaManager.");
            return;
        }

        Debug.Log($"🌐 Cargando actividades del modo {currentMode}...");
        StartCoroutine(api.LoadActivity(currentMode, OnJsonLoaded, OnError));
    }

    /// <summary>
    /// Callback al recibir la respuesta JSON desde la API.
    /// </summary>
    private void OnJsonLoaded(string json)
    {
        if (string.IsNullOrEmpty(json))
        {
            Debug.LogError("❌ JSON vacío o inválido recibido desde la API.");
            return;
        }

        Debug.Log($"✅ JSON recibido correctamente (modo {currentMode}).");
        seaManager.LoadMode(currentMode, json);
    }

    /// <summary>
    /// Callback en caso de error HTTP.
    /// </summary>
    private void OnError(string error)
    {
        Debug.LogError($"🚨 Error al cargar actividades: {error}");
    }
}

