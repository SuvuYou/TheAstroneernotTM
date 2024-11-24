using UnityEngine;

[CreateAssetMenu(fileName = "NoiseSettings", menuName = "ScriptableObjects/NoiseSettings")]
public class NoiseSettings : ScriptableObject
{
    public int OcavesCount = 8;
    public float InitialAmplitude = 1f;
    public float InitialFrequency = 1f;
    public float AmplitudeMultiplier = 1f;
    public float FrequencyMultiplier = 1f;
}
