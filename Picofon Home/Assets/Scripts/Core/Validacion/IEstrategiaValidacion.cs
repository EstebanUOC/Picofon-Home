using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEstrategiaValidacion
{
    bool Validar(string entradaUsuario, string[] datos);
}
