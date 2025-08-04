using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ValidarSilabaInicial : IEstrategiaValidacion
{
    public bool Validar(string entradaUsuario, string[] datos)
    {
        return datos[0].StartsWith(entradaUsuario);
    }
}
