using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class ActividadUI : MonoBehaviour
{
    // Evento que notifica el resultado de la actividad
    public UnityEvent<bool> OnResultadoValidado;

    // Se puede sobrescribir para inicializar la actividad
    public virtual void IniciarActividad()
    {
        Debug.Log("Iniciando actividad...");
    }

    // Método a implementar por cada tipo de actividad
    public abstract void ValidarRespuesta(bool respuestaUsuario);

    // Llama cuando se completa la actividad (correcta o incorrecta)
    protected void FinalizarActividad(bool resultadoCorrecto)
    {
        if (resultadoCorrecto)
            Debug.Log("O Respuesta correcta.");
        else
            Debug.Log("X Respuesta incorrecta.");

        OnResultadoValidado?.Invoke(resultadoCorrecto);
    }
}

