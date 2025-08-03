using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Actividad
{
    public string Titulo { get; protected set; }
    public abstract void Ejecutar();
}

public class ActividadCompararSilaba : Actividad
{
    public ActividadCompararSilaba()
    {
        Titulo = "¿Empiezan con la misma sílaba?";
    }

    public override void Ejecutar()
    {
        // Lógica de comparación de sílabas
    }
}

public class ActividadCrearPalabra : Actividad
{
    public ActividadCrearPalabra()
    {
        Titulo = "Crea una palabra con esta sílaba";
    }

    public override void Ejecutar()
    {
        // Lógica de creación de palabras
    }
}

public class ActividadFactory
{
    public Actividad CrearActividad(string tipo)
    {
        switch (tipo)
        {
            case "comparar":
                return new ActividadCompararSilaba();
            case "crear":
                return new ActividadCrearPalabra();
            default:
                throw new ArgumentException("Tipo de actividad no válida");
        }
    }
}
