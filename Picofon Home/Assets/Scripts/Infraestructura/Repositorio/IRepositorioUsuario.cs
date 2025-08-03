using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRepositorioUsuario
{
    Usuario ObtenerUsuarioPorNombre(string nombre);
    void GuardarUsuario(Usuario usuario);
    void GuardarProgreso(Usuario usuario, Actividad actividad);
}
