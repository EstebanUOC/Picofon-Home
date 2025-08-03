using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestorDeProgreso : IObservadorActividad
{
    private IRepositorioUsuario repositorio;

    public GestorDeProgreso(IRepositorioUsuario repo)
    {
        repositorio = repo;
    }

    public void OnActividadCompletada(Actividad actividad, Usuario usuario)
    {
        repositorio.GuardarProgreso(usuario, actividad);
        ActualizarEstadisticas();
    }

    public void ActualizarEstadisticas()
    {
        // Implementa lógica de estadísticas
    }
}

