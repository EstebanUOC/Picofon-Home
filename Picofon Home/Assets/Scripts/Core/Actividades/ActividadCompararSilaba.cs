using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class ActividadCompararSilaba : Actividad
    {
        private string silabaObjetivo;
        private List<string> opciones;
        private IEstrategiaValidacion estrategia;

        public ActividadCompararSilaba(string titulo, string silaba)
        {
            this.Titulo = titulo;
            this.silabaObjetivo = silaba;
            this.estrategia = new ValidarSilabaInicial();
            this.opciones = GeneradorDePalabras.Instancia.ObtenerPalabrasPorSilaba(silaba);
        }

        public override void Ejecutar()
        {
            // Lógica que notifica a la UI para mostrar las canastas
            Debug.Log("Ejecutando Actividad: " + Titulo);
            UI.ActividadCompararSilabaUI.Instance.MostrarOpciones(silabaObjetivo, opciones);
        }

        public bool ValidarRespuesta(string palabraSeleccionada)
        {
            return estrategia.Validar(palabraSeleccionada, new string[] { silabaObjetivo });
        }
    }

