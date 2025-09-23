using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Globo : MonoBehaviour
{
    [SerializeField] private TextMeshPro textoSilaba;
    private bool esDiferente;

    private void Awake()
    {
        if (textoSilaba == null)
        {
            textoSilaba = GetComponentInChildren<TextMeshPro>();
        }
    }

    public void Configurar(string silaba, bool diferente)
    {
        if (textoSilaba != null)
        {
            textoSilaba.text = silaba;
        }
        else
        {
            Debug.LogError("No se encontró un TextMeshPro en el prefab del globo");
        }

        esDiferente = diferente;
    }

    private void OnMouseDown()
    {
        MinijuegoGlobos controlador = FindObjectOfType<MinijuegoGlobos>();
        if (controlador != null)
        {
            controlador.ReventarGlobo(esDiferente);
        }

        Destroy(gameObject);
    }
}
