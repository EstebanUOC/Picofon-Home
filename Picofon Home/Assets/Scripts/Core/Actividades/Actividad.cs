using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Actividad
{
    public string Titulo { get; protected set; }
    public abstract void Ejecutar();
}
