using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GeneradorDePalabras : MonoBehaviour
{
    // ---------- Singleton ----------
    private static GeneradorDePalabras instancia;
    public static GeneradorDePalabras Instancia
    {
        get
        {
            if (instancia == null)
            {
                instancia = FindObjectOfType<GeneradorDePalabras>();
                if (instancia == null)
                {
                    GameObject go = new GameObject("GeneradorDePalabras");
                    instancia = go.AddComponent<GeneradorDePalabras>();
                }
            }
            return instancia;
        }
    }

    // ---------- Lista de palabras de ejemplo ----------
    private List<string> palabras = new List<string>()
    {
        "mano", "mapa", "casa", "perro", "marco",
        "sol", "silla", "mesa", "mamá", "ratón"
    };

    // ---------- Métodos principales ----------

    /// <summary>
    /// Devuelve una palabra aleatoria de la lista.
    /// </summary>
    public string ObtenerPalabraAleatoria()
    {
        if (palabras.Count == 0) return string.Empty;
        int index = Random.Range(0, palabras.Count);
        return palabras[index];
    }

    /// <summary>
    /// Devuelve todas las palabras que empiezan con una sílaba dada.
    /// </summary>
    public List<string> ObtenerPalabrasPorSilaba(string silaba)
    {
        List<string> resultado = new List<string>();
        foreach (var palabra in palabras)
        {
            if (palabra.StartsWith(silaba, System.StringComparison.OrdinalIgnoreCase))
            {
                resultado.Add(palabra);
            }
        }

        // Si no encuentra ninguna, devuelve aleatorias como distracción
        if (resultado.Count == 0)
        {
            resultado.AddRange(ObtenerDistractoras(3));
        }

        return resultado;
    }

    /// <summary>
    /// Devuelve una lista de palabras aleatorias como distractoras.
    /// </summary>
    private List<string> ObtenerDistractoras(int cantidad)
    {
        List<string> distractoras = new List<string>();
        for (int i = 0; i < cantidad; i++)
        {
            distractoras.Add(ObtenerPalabraAleatoria());
        }
        return distractoras;
    }

    // ---------- Unity ----------
    void Awake()
    {
        // Aseguramos que el Singleton no se destruya al cambiar de escena
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instancia != this)
        {
            Destroy(gameObject);
        }
    }
}
