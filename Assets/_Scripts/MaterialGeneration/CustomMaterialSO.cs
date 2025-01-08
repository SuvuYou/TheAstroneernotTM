

using System;
using UnityEngine;

[CreateAssetMenu(fileName = "CustomMaterial", menuName = "ScriptableObjects/CustomMaterial")]
class CustomMaterialSO : ValidatableScriptableObject
{
    public Shader _shaderGraph;

    [Range(0f, 1f)]
    public float Metalic;

    [Range(0f, 1f)]
    public float Smoothness;

    public Material GetMaterial()
    {
        Material material = new (_shaderGraph);
        material.SetFloat("_Metalic", Metalic);
        material.SetFloat("_Smoothness", Smoothness);
        
        return material;
    }

    public Material GetMaterialWithTexture(Texture2D texture)
    {
        var material = GetMaterial();
        material.SetTexture("_Texture", texture);

        return material;
    }
}