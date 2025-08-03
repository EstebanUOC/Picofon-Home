using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObservadorActividad
{
    void OnActividadCompletada(Actividad actividad, Usuario usuario);
}

