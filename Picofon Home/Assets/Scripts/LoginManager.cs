using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    public TMP_InputField inputUsuario;
    public TMP_InputField inputContrasena;
    public Button botonLogin;
    public GameObject textoCarga; // Asigna el texto de "Cargando..."

    private void Start()
    {
        botonLogin.onClick.AddListener(ValidarLogin);
        textoCarga.SetActive(false); // Asegúrate de que esté oculto al inicio
    }

    void ValidarLogin()
    {
        string usuario = inputUsuario.text;
        string contrasena = inputContrasena.text;

        if (usuario == "admin" && contrasena == "1234")
        {
            Debug.Log("¡Login exitoso!");
            StartCoroutine(CargarEscenaConEspera());
        }
        else
        {
            Debug.Log("Usuario o contraseña incorrectos.");
        }
    }

    System.Collections.IEnumerator CargarEscenaConEspera()
    {
        textoCarga.SetActive(true); // Muestra "Cargando..."
        yield return new WaitForSeconds(2f); // Espera 2 segundos
        SceneManager.LoadScene("EscenaPrincipal");
    }
}

