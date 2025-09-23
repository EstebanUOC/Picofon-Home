using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void IrAEscenaJuego()
    {
        SceneManager.LoadScene("EscenaJuego");
    }

    public void MostrarCreditos()
    {
        Debug.Log("Mostrar créditos (puedes usar un panel o cambiar de escena)");
    }

    public void SalirDelJuego()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}

