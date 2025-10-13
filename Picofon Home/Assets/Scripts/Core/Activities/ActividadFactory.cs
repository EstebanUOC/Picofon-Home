using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActividadFactory
{
    public Actividad CrearActividad(string tipo)
    {
        switch (tipo)
        {
            case "comparar":
                return new ActividadCompararSilaba("Test", "ma");
            case "crear":
                return new ActividadCrearPalabra();
            default:
                throw new ArgumentException("Tipo de actividad no válida");
        }
    }
}
