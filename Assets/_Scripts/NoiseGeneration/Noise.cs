using UnityEngine;
using Unity.Mathematics;
using System;

public static class Noise
{
    public static float GenerateNoiseAtPosition(Vector3 position, NoiseSettings settings)
    {
        return _layerNoiseAtPosition((float frequency) => _generateNoiseAtPosition(position * frequency), settings);
    }

    public static float GenerateNoiseAtPosition(Vector2 position, NoiseSettings settings)
    {
        return _layerNoiseAtPosition((float frequency) => _generateNoiseAtPosition(position * frequency), settings);
    }
    
    private static float _generateNoiseAtPosition(Vector3 positon) => noise.snoise(positon);

    private static float _generateNoiseAtPosition(Vector2 positon) => noise.snoise(positon);
    
    private static float _layerNoiseAtPosition(Func<float, float> getNoiseValue, NoiseSettings settings)
    {
        float totalNoiseValue = 0f;
        float totalAmplitudeApplied = 0f;

        float frequency = settings.InitialFrequency;
        float amplitude = settings.InitialAmplitude;

        for (int i = 0; i < settings.OcavesCount; i++)
        {
            totalNoiseValue += getNoiseValue(frequency) * amplitude;

            totalAmplitudeApplied += amplitude; 

            frequency *= settings.FrequencyMultiplier;
            amplitude *= settings.AmplitudeMultiplier;
        }

        return _normalizeValueToRange(value: totalNoiseValue / totalAmplitudeApplied);
    }

    private static float _normalizeValueToRange(float value)
    {
        // Get value from range [-1, 1] to range [0, 1]
        return (value + 1) / 2;
    } 
}