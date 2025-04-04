using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction; // Grabbable
using Oculus.Interaction.Throw;

public class Pen : MonoBehaviour
{
    [Header("Pen Properties")]
    public Transform tip;
    public Material drawingMaterial;
    public Material tipMaterial;
    [Range(0.01f, 0.1f)]
    public float penWidth = 0.01f;
    public Color[] penColors;

    [Header("Grabbable")]
    public Grabbable grabbable;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip paintSound;

    private LineRenderer currentDrawing;
    private int index;
    private int currentColorIndex;

    private bool _drawButtonPressed = false;

    private void Start()
    {
        currentColorIndex = 0;
        tipMaterial.color = penColors[currentColorIndex];
    }

    private void Update()
    {
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
                    if (audioSource.clip != paintSound)
                    {
                        audioSource.clip = paintSound;
                    }
                    audioSource.loop = true;
                    audioSource.Play();
                }
            }
        }
        else
        {
            if (_drawButtonPressed)
            {
                _drawButtonPressed = false;
                if (audioSource != null && audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
            }
            
            if (currentDrawing != null)
            {
                currentDrawing = null;
            }
        }

        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            SwitchColor();
        }
    }

    private void Draw()
    {
        if (currentDrawing == null)
        {
            index = 0;
            currentDrawing = new GameObject("Line").AddComponent<LineRenderer>();
            currentDrawing.material = drawingMaterial;
            currentDrawing.startColor = currentDrawing.endColor = penColors[currentColorIndex];
            currentDrawing.startWidth = currentDrawing.endWidth = penWidth;
            currentDrawing.positionCount = 1;
            currentDrawing.SetPosition(0, tip.position);
        }
        else
        {
            Vector3 currentPos = currentDrawing.GetPosition(index);
            if (Vector3.Distance(currentPos, tip.position) > 0.01f)
            {
                index++;
                currentDrawing.positionCount = index + 1;
                currentDrawing.SetPosition(index, tip.position);
            }
        }
    }

    private void SwitchColor()
    {
        currentColorIndex = (currentColorIndex + 1) % penColors.Length;
        tipMaterial.color = penColors[currentColorIndex];
    }
}