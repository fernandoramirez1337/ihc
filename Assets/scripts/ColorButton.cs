using UnityEngine;

public class ToggleColorButton : MonoBehaviour
{
    // Índice del color que se aplicará al material al activar este toggle.
    public int colorIndex;

    // Referencia al script que cambia el color del material.
    public MaterialColorChanger colorChanger;

    /// <summary>
    /// Esta función debe llamarse desde el evento OnValueChanged del toggle.
    /// Se ejecuta cada vez que se cambia el estado del botón.
    /// </summary>
    /// <param name="isOn">Valor que indica si el toggle está activo (true) o desactivado (false).</param>
    public void OnToggleChanged(bool isOn)
    {
        // Al activarse el botón se cambia el color; en caso de desactivarse, podrías no hacer nada.
        if (isOn)
        {
            if (colorChanger != null)
            {
                colorChanger.ChangeColor(colorIndex);
            }
            else
            {
                Debug.LogError("No se ha asignado el script MaterialColorChanger en el botón.");
            }
        }
    }
}