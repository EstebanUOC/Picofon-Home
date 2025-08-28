using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Globo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textoSilaba;
    private bool esDiferente;

    private void Awake()
    {
        // Si no se asignó desde el Inspector, lo busca automáticamente en los hijos
        if (textoSilaba == null)
        {
            textoSilaba = GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    /// <summary>
    /// Configura el globo con la sílaba y si es el diferente
    /// </summary>
    public void Configurar(string silaba, bool diferente)
    {
        if (textoSilaba != null)
        {
            textoSilaba.text = silaba;
        }
        else
        {
            Debug.LogError(" No se encontró un TextMeshProUGUI en el prefab del globo");
        }

        esDiferente = diferente;
    }

    private void OnMouseDown()
    {
        // Notifica al minijuego si era el globo correcto o no
        MinijuegoGlobos controlador = FindObjectOfType<MinijuegoGlobos>();
        if (controlador != null)
        {
            controlador.ReventarGlobo(esDiferente);
        }

        // Destruye el globo al hacer clic
        Destroy(gameObject);
    }
}
