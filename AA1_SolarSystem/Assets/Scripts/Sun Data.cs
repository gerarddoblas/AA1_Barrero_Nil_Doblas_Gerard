using UnityEngine;

[CreateAssetMenu(fileName = "SunData", menuName = "Scriptable Objects/SunData")]
public class SunData : ScriptableObject
{
    [Header("Physical properties")]
    public float mass;
}
