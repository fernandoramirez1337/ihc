using UnityEngine;
using System.Collections;
using Oculus.Interaction.Locomotion;

[RequireComponent(typeof(TeleportInteractor))]
public class FaceObjectCenterOnTeleport : MonoBehaviour
{
    public Transform rigTransform;
    public Transform lookTarget;
    private TeleportInteractor _teleportInteractor;

    void Awake()
    {
        _teleportInteractor = GetComponent<TeleportInteractor>();
    }

    void OnEnable()
    {
        _teleportInteractor.WhenLocomotionPerformed += _ => StartCoroutine(RotateNextFrame());
    }

    void OnDisable()
    {
        _teleportInteractor.WhenLocomotionPerformed -= _ => StartCoroutine(RotateNextFrame());
    }

    private IEnumerator RotateNextFrame()
    {
        // Espera un frame para que el Teleport haya movido ya el rig
        yield return null;

        Vector3 dir = lookTarget.position - rigTransform.position;
        dir.y = 0;
        if (dir.sqrMagnitude < 0.0001f) yield break;

        rigTransform.rotation = Quaternion.LookRotation(dir.normalized, Vector3.up);
    }
}