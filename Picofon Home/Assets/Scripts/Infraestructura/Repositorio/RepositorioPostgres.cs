using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Npgsql;
using System;

public class RepositorioPostgres : IRepositorioUsuario
{
    private string conexion = "Host=54.75.243.210;Port=3306;Username=root;Password=1234Picofon*;Database=PICOFON_TEST";

    public Usuario ObtenerUsuarioPorNombre(string nombre)
    {
        using var conn = new NpgsqlConnection(conexion);
        conn.Open();

        using var cmd = new NpgsqlCommand("SELECT nombre, edad FROM usuarios WHERE nombre = @nombre", conn);
        cmd.Parameters.AddWithValue("nombre", nombre);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            var usuario = new Usuario(reader.GetString(0), reader.GetInt32(1));

            // Aquí puedes leer también sus actividades preferidas si están en otra tabla
            return usuario;
        }

        return null;
    }

    public void GuardarUsuario(Usuario usuario)
    {
        using var conn = new NpgsqlConnection(conexion);
        conn.Open();

        using var cmd = new NpgsqlCommand("INSERT INTO usuarios (nombre, edad) VALUES (@nombre, @edad)", conn);
        cmd.Parameters.AddWithValue("nombre", usuario.Nombre);
        cmd.Parameters.AddWithValue("edad", usuario.Edad);

        cmd.ExecuteNonQuery();

        // También podrías guardar actividades preferidas si es necesario
    }

    public void GuardarProgreso(Usuario usuario, Actividad actividad)
    {
        using var conn = new NpgsqlConnection(conexion);
        conn.Open();

        using var cmd = new NpgsqlCommand("INSERT INTO progreso (nombre_usuario, titulo_actividad, fecha) VALUES (@usuario, @actividad, NOW())", conn);
        cmd.Parameters.AddWithValue("usuario", usuario.Nombre);
        cmd.Parameters.AddWithValue("actividad", actividad.Titulo);

        cmd.ExecuteNonQuery();
    }

    public Usuario ObtenerPalabrasSegmentadas()
    {
        using var conn = new NpgsqlConnection(conexion);
        conn.Open();

        using var cmd = new NpgsqlCommand("SELECT * FROM WORD_SEGMENT ws ", conn);
        cmd.Parameters.AddWithValue("id", 1);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            var usuario = new Usuario(reader.GetString(0), reader.GetInt32(1));

            // Aquí puedes leer también sus actividades preferidas si están en otra tabla
            return usuario;
        }

        return null;
    }

}

