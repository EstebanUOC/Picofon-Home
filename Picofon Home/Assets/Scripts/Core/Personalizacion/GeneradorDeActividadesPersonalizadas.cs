using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneradorDeActividadesPersonalizadas
{
    private ActividadFactory factory = new ActividadFactory();

    public List<Actividad> GenerarParaUsuario(Usuario usuario)
    {
        var lista = new List<Actividad>();

        foreach (var tipo in usuario.ActividadesPreferidas)
        {
            lista.Add(factory.CrearActividad(tipo));
        }

        return lista;
    }
}
