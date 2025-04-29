using UnityEngine;

public class VolumeCreator : MonoBehaviour
{
    [Header("Pointer & Input")]
    [Tooltip("Transform desde donde lanzas el rayo (p.ej. punta de tu controlador o dedo)")]
    public Transform pointerOrigin;
    [Tooltip("Botón de OVRInput para definir esquinas (p.ej. PrimaryIndexTrigger)")]
    public OVRInput.Button pinchButton = OVRInput.Button.PrimaryIndexTrigger;
    [Tooltip("Capa donde está el suelo o lo que quieras apuntar")]
    public LayerMask raycastLayerMask;
    [Tooltip("Distancia máxima del raycast")]
    public float rayLength = 10f;

    [Header("Volume Prefab")]
    [Tooltip("Prefab de tu cubo semitransparente con BoxCollider")]
    public GameObject volumePrefab;

    private Vector3 _cornerA;
    private GameObject _volume;
    private bool _cornerASet = false;

    void Update()
    {
        // 1) Al apretar el pinch definimos la esquina A
        if (OVRInput.GetDown(pinchButton))
        {
            if (TryGetPointerHit(out _cornerA))
            {
                _volume = Instantiate(volumePrefab, _cornerA, Quaternion.identity);
                _cornerASet = true;
            }
        }
        // 2) Al soltar el pinch definimos la esquina B y ajustamos el cubo
        else if (OVRInput.GetUp(pinchButton) && _cornerASet && _volume != null)
        {
            if (TryGetPointerHit(out Vector3 cornerB))
            {
                // Calcula centro y tamaño
                Vector3 center = (_cornerA + cornerB) * 0.5f;
                Vector3 size   = new Vector3(
                    Mathf.Abs(cornerB.x - _cornerA.x),
                    Mathf.Abs(cornerB.y - _cornerA.y),
                    Mathf.Abs(cornerB.z - _cornerA.z)
                );

                // Aplica transform y collider
                _volume.transform.position   = center;
                _volume.transform.localScale = size;
                var bc = _volume.GetComponent<BoxCollider>();
                if (bc != null) bc.size = Vector3.one;

                // Ya no necesitamos más este script
                enabled = false;
            }
        }
    }

    /// <summary>
    /// Lanza un raycast desde pointerOrigin.forward y devuelve el punto de impacto.
    /// </summary>
    private bool TryGetPointerHit(out Vector3 hitPoint)
    {
        Ray ray = new Ray(pointerOrigin.position, pointerOrigin.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, rayLength, raycastLayerMask))
        {
            hitPoint = hit.point;
            return true;
        }
        hitPoint = Vector3.zero;
        return false;
    }
}