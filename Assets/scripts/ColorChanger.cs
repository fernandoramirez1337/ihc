using UnityEngine;

public class MaterialColorChanger : MonoBehaviour
{
    // Material cuyo color se cambiará.
    public Material targetMaterial;

    // Arreglo de 5 colores. Puedes modificarlos en el Inspector.
    public Color[] colors = new Color[5] {
        Color.red, 
        Color.green, 
        Color.blue, 
        Color.yellow, 
        Color.magenta
    };

    /// <summary>
    /// Cambia el color del material según el índice recibido.
    /// </summary>
    /// <param name="index">Índice del color a utilizar (debe estar entre 0 y 4).</param>
    public void ChangeColor(int index)
    {
        if (targetMaterial == null)
        {
            Debug.LogError("No se ha asignado el material en el Inspector.");
            return;
        }

        if (index >= 0 && index < colors.Length)
        {
            targetMaterial.color = colors[index];
        }
        else
        {
            Debug.LogError("Índice de color inválido.");
        }
    }
}