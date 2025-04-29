using Oculus.Interaction;
using Oculus.Interaction.Input;
using System;
using UnityEngine;

public class PalmOpenDetector : MonoBehaviour, IActiveState
{
    [SerializeField, Interface(typeof(IHand))]
    private UnityEngine.Object _handObject;
    private IHand _hand;

    [Tooltip("Umbral mínimo de dot product para considerar el dedo extendido.")]
    [Range(0.5f, 1f)]
    public float extensionThreshold = 0.8f;

    public bool Active => _isPalmOpen;
    private bool _isPalmOpen;

    public event Action WhenPalmOpened = delegate { };
    public event Action WhenPalmClosed = delegate { };

    private bool _lastState = false;

    void Awake()
    {
        _hand = _handObject as IHand;
    }

    void OnEnable()
    {
        _hand.WhenHandUpdated += OnHandUpdated;
    }

    void OnDisable()
    {
        _hand.WhenHandUpdated -= OnHandUpdated;
    }

    private void OnHandUpdated()
    {
        // Para cada dedo, comprobaremos su “rectitud”
        bool allExtended = true;
        foreach (HandFinger finger in Enum.GetValues(typeof(HandFinger)))
        {
            if (!IsFingerExtended(finger))
            {
                allExtended = false;
                break;
            }
        }

        _isPalmOpen = allExtended;

        // Si cambia el estado, disparamos eventos
        if (_isPalmOpen && !_lastState)
            WhenPalmOpened.Invoke();
        else if (!_isPalmOpen && _lastState)
            WhenPalmClosed.Invoke();

        _lastState = _isPalmOpen;
    }

    private bool IsFingerExtended(HandFinger finger)
    {
        // Obtén tres joints: base, medio y punta
        if (!_hand.GetJointPoseFromWrist(HandJointIdHelpers.FromFinger(finger, 1), out Pose j1) ||
            !_hand.GetJointPoseFromWrist(HandJointIdHelpers.FromFinger(finger, 2), out Pose j2) ||
            !_hand.GetJointPoseFromWrist(HandJointIdHelpers.FromFinger(finger, 3), out Pose j3))
        {
            // Si falta tracking, consideramos no extendido
            return false;
        }

        // Vector desde base→medio y medio→punta
        Vector3 v1 = (j2.position - j1.position).normalized;
        Vector3 v2 = (j3.position - j2.position).normalized;

        // Dot product cercano a 1 indica dedos alineados
        float dot = Vector3.Dot(v1, v2);
        return dot >= extensionThreshold;
    }
}

// Helper para mapear Finger→JointId
static class HandJointIdHelpers
{
    public static HandJointId FromFinger(HandFinger finger, int phalanx)
    {
        // phalanx: 1=proximal, 2=medial, 3=distal/tip
        switch(finger)
        {
            case HandFinger.Thumb:
                return phalanx == 3 ? HandJointId.HandThumbTip :
                       phalanx == 2 ? HandJointId.HandThumb2 :
                                      HandJointId.HandThumb1;
            case HandFinger.Index:
                return phalanx == 3 ? HandJointId.HandIndexTip :
                       phalanx == 2 ? HandJointId.HandIndex2 :
                                      HandJointId.HandIndex1;
            case HandFinger.Middle:
                return phalanx == 3 ? HandJointId.HandMiddleTip :
                       phalanx == 2 ? HandJointId.HandMiddle2 :
                                      HandJointId.HandMiddle1;
            case HandFinger.Ring:
                return phalanx == 3 ? HandJointId.HandRingTip :
                       phalanx == 2 ? HandJointId.HandRing2 :
                                      HandJointId.HandRing1;
            case HandFinger.Pinky:
                return phalanx == 3 ? HandJointId.HandPinkyTip :
                       phalanx == 2 ? HandJointId.HandPinky2 :
                                      HandJointId.HandPinky1;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}