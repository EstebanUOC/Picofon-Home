using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestorDeUsuarios
{
    private static GestorDeUsuarios instancia;
    private IRepositorioUsuario repositorio;

    private GestorDeUsuarios(IRepositorioUsuario repo)
    {
        repositorio = repo;
    }

    public static GestorDeUsuarios GetInstancia(IRepositorioUsuario repo = null)
    {
        if (instancia == null && repo != null)
            instancia = new GestorDeUsuarios(repo);

        return instancia;
    }

    public Usuario ObtenerUsuario(string nombre)
    {
        return repositorio.ObtenerUsuarioPorNombre(nombre);
    }

    public void RegistrarUsuario(Usuario usuario)
    {
        repositorio.GuardarUsuario(usuario);
    }
}

