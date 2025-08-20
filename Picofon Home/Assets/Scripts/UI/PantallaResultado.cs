using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PantallaResultado : MonoBehaviour
{
    // ---------- Singleton ----------
    public static PantallaResultado Instance;

    [Header("UI")]
    [SerializeField] private GameObject panelResultado;   // Panel de UI que se muestra/oculta
    [SerializeField] private TextMeshProUGUI textoMensaje; // Texto para mostrar retroalimentación

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        // Oculta el panel al inicio
        if (panelResultado != null)
            panelResultado.SetActive(false);
    }

    /// <summary>
    /// Muestra un mensaje en la pantalla de resultados.
    /// </summary>
    public void MostrarMensaje(string mensaje)
    {
        if (textoMensaje != null)
            textoMensaje.text = mensaje;

        if (panelResultado != null)
            panelResultado.SetActive(true);

        // Opcional: esconder el mensaje después de unos segundos
        CancelInvoke(nameof(OcultarMensaje));
        Invoke(nameof(OcultarMensaje), 2.5f);
    }

    /// <summary>
    /// Oculta el panel de resultados.
    /// </summary>
    public void OcultarMensaje()
    {
        if (panelResultado != null)
            panelResultado.SetActive(false);
    }
}
