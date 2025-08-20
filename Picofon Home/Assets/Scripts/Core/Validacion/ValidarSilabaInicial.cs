using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ValidarSilabaInicial : IEstrategiaValidacion
{
    //public bool Validar(string entradaUsuario, string[] datos)
    //{
    //    return datos[0].StartsWith(entradaUsuario);
    //}
    public bool Validar(string entradaUsuario, string[] datos)
    {
        if (datos.Length == 0) return false;
        string silaba = datos[0];
        return entradaUsuario.StartsWith(silaba, StringComparison.OrdinalIgnoreCase);
    }
}
