using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI
{
    public class ActividadCompararSilabaUI : MonoBehaviour
    {
        public static ActividadCompararSilabaUI Instance;

        [SerializeField] private TextMeshProUGUI textoSilaba;
        [SerializeField] private List<Button> botonesCanastas;
        [SerializeField] private GameObject prefabBalon;
        [SerializeField] private Transform spawnBalon;
        [SerializeField] private int rondasTotales = 3; // cuántas veces quieres jugar

        private int rondasRestantes;
        private ActividadCompararSilaba actividadActual;
        private GameObject balonActual; // referencia al balón en juego

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            rondasRestantes = rondasTotales;
            string silaba = "ma"; // aquí puedes pedirla del GeneradorDePalabras
            List<string> opciones = new List<string>()
            {
                "mano", "mapa", "casa", "perro"
            };

            MostrarOpciones(silaba, opciones);
        }

        public void MostrarOpciones(string silaba, List<string> opciones)
        {
            textoSilaba.text = silaba;
            actividadActual = new ActividadCompararSilaba("Baloncesto - Comparar Sílabas", silaba);

            int total = Mathf.Min(botonesCanastas.Count, opciones.Count);

            for (int i = 0; i < total; i++)
            {
                botonesCanastas[i].GetComponentInChildren<TextMeshProUGUI>().text = opciones[i];
                string palabra = opciones[i];
                Transform canasta = botonesCanastas[i].transform;

                botonesCanastas[i].onClick.RemoveAllListeners();
                botonesCanastas[i].onClick.AddListener(() => SeleccionarCanasta(palabra, canasta));
            }


            // Genera un balón y lo guardamos en balonActual
            balonActual = Instantiate(prefabBalon, spawnBalon.position, Quaternion.identity);
        }

        private void NuevaRonda()
        {
            string silaba = "pa";
            List<string> opciones = new List<string>() { "pato", "pala", "gato", "sol" };

            MostrarOpciones(silaba, opciones);
        }


        private void SeleccionarCanasta(string palabraSeleccionada, Transform canastaTransform)
        {
            // Mueve el balón hacia la canasta
            if (balonActual != null)
            {
                balonActual.GetComponent<Balon>().LanzarHacia(canastaTransform.position);
            }

            bool resultado = actividadActual.ValidarRespuesta(palabraSeleccionada);

            if (resultado)
                PantallaResultado.Instance.MostrarMensaje(" ¡Correcto!");
            else
                PantallaResultado.Instance.MostrarMensaje(" Intenta otra vez");

            // Reducir rondas
            rondasRestantes--;

            if (rondasRestantes > 0)
            {
                //  iniciar nueva ronda después de 1 segundo
                Invoke(nameof(NuevaRonda), 1f);
            }
            else
            {
                //  fin del juego
                PantallaResultado.Instance.MostrarMensaje("¡Juego terminado!");
            }
        }

    }
}
