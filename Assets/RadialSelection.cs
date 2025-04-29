using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(Canvas))]
public class RadialSelection : MonoBehaviour
{
    [Tooltip("Botón de OVRInput para abrir/usar el menú radial.")]
    public OVRInput.Button spawnButton;

    [Tooltip("Componente que provee la lista de colores y determina el número de segmentos.")]
    public SetColorFromList colorProvider;

    [Tooltip("Prefab con Image para cada segmento radial.")]
    public GameObject radialPartPrefab;

    [Tooltip("Canvas donde se instanciarán los segmentos.")]
    public Transform radialPartCanvas;

    [Tooltip("Distancia desde la mano/controlador hasta el centro del menú.")]
    public float menuSpawnDistance = 0.2f;

    [Tooltip("Espacio angular (en grados) entre cada segmento.")]
    public float angleBetweenPart = 10f;

    [Tooltip("Transform de la mano/controlador para posicionar el menú.")]
    public Transform handTransform;

    [Tooltip("Evento que recibe el índice del segmento seleccionado.")]
    public UnityEvent<int> OnPartSelected;

    private List<GameObject> spawnedParts = new List<GameObject>();
    private int currentSelectedRadialPart = -1;

    private int NumberOfRadialParts =>
        (colorProvider != null && colorProvider.colors != null)
        ? colorProvider.colors.Count : 0;

    void Update()
    {
        if (NumberOfRadialParts == 0) return;

        if (OVRInput.GetDown(spawnButton))
            SpawnRadialPart();

        if (OVRInput.Get(spawnButton))
            GetSelectedRadialPart();

        if (OVRInput.GetUp(spawnButton))
            HideAndTriggerSelected();
    }

    public void HideAndTriggerSelected()
    {
        OnPartSelected.Invoke(currentSelectedRadialPart);
        radialPartCanvas.gameObject.SetActive(false);
    }

    public void GetSelectedRadialPart()
    {
        Vector3 centerToHand = handTransform.position - radialPartCanvas.position;
        Vector3 projected = Vector3.ProjectOnPlane(centerToHand, radialPartCanvas.forward);
        float angle = Vector3.SignedAngle(radialPartCanvas.up, projected, -radialPartCanvas.forward);
        if (angle < 0) angle += 360f;

        currentSelectedRadialPart = Mathf.FloorToInt(angle * NumberOfRadialParts / 360f);

        for (int i = 0; i < spawnedParts.Count; i++)
        {
            var img = spawnedParts[i].GetComponent<Image>();
            if (i == currentSelectedRadialPart)
            {
                //img.color = Color.yellow;
                spawnedParts[i].transform.localScale = 1.1f * Vector3.one;
            }
            else
            {
                img.color = colorProvider.colors[i];
                spawnedParts[i].transform.localScale = Vector3.one;
            }
        }
    }

    public void SpawnRadialPart()
    {
        if (colorProvider == null)
        {
            Debug.LogError("Debe asignar un SetColorFromList en colorProvider.");
            return;
        }

        // Posiciona el menú un poco alejado de la mano
        Vector3 spawnPosition = handTransform.position + handTransform.forward * menuSpawnDistance;
        radialPartCanvas.position = spawnPosition;
        radialPartCanvas.rotation = handTransform.rotation;
        radialPartCanvas.gameObject.SetActive(true);

        // Limpia instancias previas
        foreach (var part in spawnedParts)
            Destroy(part);
        spawnedParts.Clear();

        int count = NumberOfRadialParts;
        for (int i = 0; i < count; i++)
        {
            float angle = -i * 360f / count - angleBetweenPart / 2f;
            GameObject part = Instantiate(radialPartPrefab, radialPartCanvas);

            // Alinea el segmento en el centro del canvas y rota para crear el slice
            part.transform.localPosition = Vector3.zero;
            part.transform.localEulerAngles = new Vector3(0, 0, angle);

            var img = part.GetComponent<Image>();
            img.fillAmount = (1f / count) - (angleBetweenPart / 360f);
            img.color = colorProvider.colors[i];

            spawnedParts.Add(part);
        }
    }
}
