using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinijuegoGlobos : MonoBehaviour
{
    [SerializeField] private GameObject prefabGlobo;
    [SerializeField] private Transform[] posicionesGlobos; // 4 posiciones
    private int rondasJugadas = 0;
    [SerializeField] private int maxRondas = 5;

    public void ReventarGlobo(bool esCorrecto)
    {
        if (esCorrecto)
            PantallaResultado.Instance.MostrarMensaje(" ¡Correcto!");
        else
            PantallaResultado.Instance.MostrarMensaje(" Intenta otra vez");

        rondasJugadas++;

        if (rondasJugadas < maxRondas)
        {
            LimpiarEscena();
            GenerarGlobos();
        }
        else
        {
            PantallaResultado.Instance.MostrarMensaje(" ¡Juego terminado!");
            // aquí podrías regresar al menú o cargar otra escena
        }
    }

    private void LimpiarEscena()
    {
        foreach (var globo in GameObject.FindGameObjectsWithTag("Globo"))
            Destroy(globo);
    }


    private List<string> silabas = new List<string>() { "ma", "me", "mi", "mo", "mu", "pa", "pe", "pi" };

    private void Start()
    {
        GenerarGlobos();
    }

    private void GenerarGlobos()
    {
        // Elegir sílaba base
        string silabaBase = silabas[Random.Range(0, silabas.Count)];

        // Elegir sílaba diferente
        string silabaDiferente;
        do
        {
            silabaDiferente = silabas[Random.Range(0, silabas.Count)];
        } while (silabaDiferente == silabaBase);

        // Escoger al azar qué globo será el diferente
        int indiceDiferente = Random.Range(0, posicionesGlobos.Length);

        for (int i = 0; i < posicionesGlobos.Length; i++)
        {
            GameObject globo = Instantiate(prefabGlobo, posicionesGlobos[i].position, Quaternion.identity);

            if (i == indiceDiferente)
                globo.GetComponent<Globo>().Configurar(silabaDiferente, true);
            else
                globo.GetComponent<Globo>().Configurar(silabaBase, false);
        }
    }
}
