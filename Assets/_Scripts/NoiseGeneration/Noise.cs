using UnityEngine;

public static class Noise
{
    public static float GenerateNoiseAt(Vector3 positon)
    {
        float xy = Mathf.PerlinNoise(positon.x, positon.y);
        float xz = Mathf.PerlinNoise(positon.x, positon.z);
        float yz = Mathf.PerlinNoise(positon.y, positon.z);
        float yx = Mathf.PerlinNoise(positon.y, positon.x);
        float zx = Mathf.PerlinNoise(positon.z, positon.x);
        float zy = Mathf.PerlinNoise(positon.z, positon.y);

        return (xy + xz + yz + yx + zx + zy) / 6;
    }
}
