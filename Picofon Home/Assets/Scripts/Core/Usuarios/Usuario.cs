using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Usuario
{
    public string Nombre { get; private set; }
    public int Edad { get; private set; }
    public List<string> ActividadesPreferidas { get; private set; }

    public Usuario(string nombre, int edad)
    {
        Nombre = nombre;
        Edad = edad;
        ActividadesPreferidas = new List<string>();
    }

    public void AgregarActividadPreferida(string tipo)
    {
        ActividadesPreferidas.Add(tipo);
    }
}

