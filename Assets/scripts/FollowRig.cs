using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class FollowRig : MonoBehaviour
{
    [Tooltip("Referencia a tu XR Origin (o al transform de la cámara)")]
    public Transform rigTransform;

    [Tooltip("Posición local deseada respecto al rig")]
    public Vector3 localOffset = new Vector3(0f, 0f, 2f);

    void Update()
    {
        // Calcula la posición mundial a partir del offset en espacio local del rig
        transform.position = rigTransform.TransformPoint(localOffset);
        // (Opcional) para que también rote con el rig:
        transform.rotation = rigTransform.rotation;
    }
}