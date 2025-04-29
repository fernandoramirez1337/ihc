using System.Collections.Generic;
using UnityEngine;

public class SetColorFromList : MonoBehaviour
{
    [Tooltip("Asigna aquí el Material que quieras cambiar.")]
    public Material materialToChange;

    [Tooltip("Lista de colores disponibles.")]
    public List<Color> colors;

    /// <summary>
    /// Cambia el color del material asignado al color de índice i en la lista.
    /// </summary>
    /// <param name="i">Índice del color en la lista.</param>
    public void SetColor(int i)
    {
        if (materialToChange == null)
        {
            Debug.LogError("No hay ningún Material asignado en materialToChange.");
            return;
        }

        if (colors == null || i < 0 || i >= colors.Count)
        {
            Debug.LogError($"Índice fuera de rango: {i}. Debe estar entre 0 y { (colors?.Count ?? 0) - 1 }.");
            return;
        }

        materialToChange.color = colors[i];
    }
}
