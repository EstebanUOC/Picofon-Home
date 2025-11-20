using UnityEngine;

public abstract class ActivityBaseManager : MonoBehaviour
{
    [SerializeField] protected GameAPIService api; // 👈 Nuevo nombre de tipo

    protected virtual void Awake()
    {
        if (api == null)
            api = FindAnyObjectByType<GameAPIService>(); // 👈 Buscamos el servicio
    }

    /// <summary>
    /// Solicita los datos del modo indicado al servicio API.
    /// </summary>
    public void LoadMode(int mode)
    {
        if (api == null)
        {
            Debug.LogError("⚠️ Falta referencia al GameAPIService.");
            return;
        }

       // StartCoroutine(api.LoadActivity(mode, OnJsonLoaded, OnError));
    }

    protected abstract void OnJsonLoaded(string json);

    protected virtual void OnError(string error)
    {
        Debug.LogError($"⚠️ Error al cargar modo: {error}");
    }
}
