using UnityEngine;
using System.Collections;

public class BalloonPopSeaGameManager : MonoBehaviour
{
    [SerializeField] private BalloonPopSeaAPI api;
    [SerializeField] private BalloonPopSeaManager seaManager;

    private void Start()
    {
        // 👇 Fíjate que se pasa el nombre del método SIN paréntesis
        StartCoroutine(api.LoadActivities(OnActivitiesLoaded));
    }

    // 👇 Este método debe aceptar un parámetro de tipo Data
    private void OnActivitiesLoaded(Data data)
    {
        if (data == null)
        {
            Debug.LogError("❌ No se recibieron datos desde la API.");
            return;
        }

        Debug.Log($"✅ Actividad 1: {data.activity1.question}");
        Debug.Log($"Palabra principal: {data.activity1.main_word}");
        Debug.Log($"Correcta: {data.activity1.correct_option.text}");

        // Aquí puedes pasar la info al manager del minijuego
        seaManager.ShowNewPair();
    }
}
