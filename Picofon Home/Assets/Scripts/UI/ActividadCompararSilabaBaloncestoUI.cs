using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI
{
    public class ActividadCompararSilabaUI : MonoBehaviour
    {

        private void Start()
        {
            string silaba = "ma"; // aquí puedes pedirla del GeneradorDePalabras
            List<string> opciones = new List<string>()
    {
        "mano", "mapa", "casa", "perro"
    };

            MostrarOpciones(silaba, opciones);
        }

        public static ActividadCompararSilabaUI Instance;

        [SerializeField] private TextMeshProUGUI textoSilaba;
        [SerializeField] private List<Button> botonesCanastas;
        [SerializeField] private GameObject prefabBalon;
        [SerializeField] private Transform spawnBalon;

        private ActividadCompararSilaba actividadActual;

        private void Awake()
        {
            Instance = this;
        }

        private GameObject balonActual;

        public void MostrarOpciones(string silaba, List<string> opciones)
        {
            textoSilaba.text = silaba;
            actividadActual = new ActividadCompararSilaba("Baloncesto - Comparar Sílabas", silaba);

            for (int i = 0; i < botonesCanastas.Count; i++)
            {
                botonesCanastas[i].GetComponentInChildren<TextMeshProUGUI>().text = opciones[i];
                string palabra = opciones[i];
                botonesCanastas[i].onClick.RemoveAllListeners();
                botonesCanastas[i].onClick.AddListener(() => SeleccionarCanasta(palabra));
            }

            // Genera un balón en el escenario
            Instantiate(prefabBalon, new Vector3(spawnBalon.position.x, spawnBalon.position.y, 0), Quaternion.identity);

        }

        private void SeleccionarCanasta(string palabraSeleccionada)
        {
            bool resultado = actividadActual.ValidarRespuesta(palabraSeleccionada);
            if (resultado)
                PantallaResultado.Instance.MostrarMensaje(" ¡Correcto!");
            else
                PantallaResultado.Instance.MostrarMensaje(" Intenta otra vez");
        }

    }

}
