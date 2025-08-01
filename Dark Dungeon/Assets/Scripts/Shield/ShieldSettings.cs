using UnityEngine;

public class ShieldSettings : MonoBehaviour
{
    public Vector3 protectPosition;
    public Vector3 protectRotation;
    [HideInInspector] public Vector3 originalPosition;
    [HideInInspector] public Vector3 originalRotation;
}
