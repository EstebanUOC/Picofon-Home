using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Balon : MonoBehaviour
{
    private Vector3 destino;
    private bool enMovimiento = false;
    private float velocidad = 64f;

    // Llamado desde la UI cuando se selecciona una canasta
    public void LanzarHacia(Vector3 posicionCanasta)
    {
        destino = posicionCanasta;
        enMovimiento = true;
    }

    private void Update()
    {
        if (enMovimiento)
        {
            transform.position = Vector3.MoveTowards(transform.position, destino, velocidad * Time.deltaTime);

            // Cuando llega a destino
            if (Vector3.Distance(transform.position, destino) < 0.1f)
            {
                enMovimiento = false;
                // Opcional: destruir el balón al llegar
                Destroy(gameObject, 0.2f);
            }
        }
    }
}
