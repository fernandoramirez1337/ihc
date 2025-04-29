using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction; // Para Grabbable
using Oculus.Interaction.Throw;

public class Pen : MonoBehaviour
{
    [Header("Pen Properties")]
    [Tooltip("Transform que marca la punta del pen.")]
    public Transform tip;

    [Tooltip("Material base para los trazos. Cada trazo clonará este material para preservar su color.")]
    public Material drawingMaterial;

    [Range(0.01f, 0.1f), Tooltip("Ancho del trazo.")]
    public float penWidth = 0.01f;

    [Header("Grabbable")]
    public Grabbable grabbable;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip paintSound;

    [Header("Undo")]
    [Tooltip("Botón OVR para deshacer el último trazo.")]
    public OVRInput.Button undoButton = OVRInput.Button.Two;

    private LineRenderer currentDrawing;
    private int index;
    private bool _drawButtonPressed = false;
    private readonly List<LineRenderer> _strokes = new List<LineRenderer>();

    private void Update()
    {
        // Detectar deshacer
        if (OVRInput.GetDown(undoButton))
        {
            UndoLastStroke();
        }

        // Detección de dibujo
        bool isGrabbed = grabbable.GrabPoints.Count > 0;
        Vector2 primaryAxis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        Vector2 secondaryAxis = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
        bool drawButtonDown = isGrabbed && (primaryAxis.y < -0.7f || secondaryAxis.y < -0.7f);

        if (drawButtonDown)
        {
            Draw();
            if (!_drawButtonPressed)
            {
                _drawButtonPressed = true;
                if (audioSource != null && paintSound != null)
                {
                    audioSource.clip = paintSound;
                    audioSource.loop = true;
                    audioSource.Play();
                }
            }
        }
        else if (_drawButtonPressed)
        {
            _drawButtonPressed = false;
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
            currentDrawing = null;
        }
    }

    private void Draw()
    {
        if (currentDrawing == null)
        {
            BeginStroke();
        }
        else
        {
            ContinueStroke();
        }
    }

    private void BeginStroke()
    {
        index = 0;
        GameObject go = new GameObject("Line");
        LineRenderer lr = go.AddComponent<LineRenderer>();
        // Clonar material para que cada trazo conserve su color
        lr.material = new Material(drawingMaterial);
        lr.startWidth = lr.endWidth = penWidth;
        lr.positionCount = 1;
        lr.SetPosition(0, tip.position);

        currentDrawing = lr;
        _strokes.Add(lr);
    }

    private void ContinueStroke()
    {
        Vector3 currentPos = currentDrawing.GetPosition(index);
        if (Vector3.Distance(currentPos, tip.position) > 0.01f)
        {
            index++;
            currentDrawing.positionCount = index + 1;
            currentDrawing.SetPosition(index, tip.position);
        }
    }

    private void UndoLastStroke()
    {
        if (_strokes.Count == 0) return;
        int lastIndex = _strokes.Count - 1;
        LineRenderer last = _strokes[lastIndex];
        _strokes.RemoveAt(lastIndex);
        if (last != null)
        {
            Destroy(last.gameObject);
        }
    }
}
