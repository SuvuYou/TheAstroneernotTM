
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VertexTypeMaterialsManagerSO", menuName = "ScriptableObjects/VertexTypeMaterialsManagerSO")]
class VertexTypeMaterialsManagerSO : ValidatableScriptableObject
{
    public event Action<Material> OnMaterialChanged;

    [SerializeField]
    private SerializableDictionary<VertexType, Color> _colorsByVertexType;
    public Dictionary<VertexType, Color> ColorsByVertexType { get =>  _colorsByVertexType.ToDictionary(); }

    public List<Texture2D> Textures { get; private set; }
    public Texture2D TextureAtlas { get; private set; }
    public Material TextureAtlasMaterial { get; private set; }

    private Dictionary<VertexType, Rect> _uvRectsByVertexType = new();

    public Rect GetMaterialUVsByVertexType(VertexType vertexType) => _uvRectsByVertexType[vertexType];

    public void SetUpTextures(Material atlasMaterial, Texture2D textureAtlas, List<Texture2D> textures, Dictionary<VertexType, Rect> uvRectsByVertexType) 
    {
        TextureAtlasMaterial = atlasMaterial;
        TextureAtlas = textureAtlas;
        Textures = textures;
        _uvRectsByVertexType = uvRectsByVertexType;

        OnMaterialChanged?.Invoke(atlasMaterial);
    }
}

