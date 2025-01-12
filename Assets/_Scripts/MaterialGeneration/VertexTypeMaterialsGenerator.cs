using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VertexTypeMaterialsGenerator : MonoBehaviour
{
    private const int TEXTURE_SIZE = 8;

    [SerializeField]
    private VertexTypeMaterialsManagerSO _vertexTypeMaterialsManager;

    [SerializeField]
    private CustomMaterialSO _customMaterial;

    private void Awake()
    {
        _generateVertexTypesTexturesAtlasMaterial();

        _vertexTypeMaterialsManager.OnValidateConfig += _generateVertexTypesTexturesAtlasMaterial;
        _customMaterial.OnValidateConfig += _generateVertexTypesTexturesAtlasMaterial;
    }

    private void _generateVertexTypesTexturesAtlasMaterial() 
    {
        Dictionary<VertexType, Rect> uvRectsByVertexType = new();

        var vertexTypes = _vertexTypeMaterialsManager.ColorsByVertexType.Keys.ToList();
        var vertexColors = vertexTypes.Select(vertexType => _vertexTypeMaterialsManager.ColorsByVertexType[vertexType]).ToList();

        var textures = _createVertexTypeTextures(vertexColors);
        var textureAtlas = _createTextureAtlas(textures, out Rect[] uvRects);

        for (int i = 0; i < vertexTypes.Count; i++)
        {
            uvRectsByVertexType[vertexTypes[i]] = uvRects[i];
        }

        var atlasMaterial = _customMaterial.GetMaterialWithTexture(textureAtlas);

        _vertexTypeMaterialsManager.SetUpTextures(atlasMaterial, textureAtlas, textures, uvRectsByVertexType);
    }

    private Texture2D _createTextureAtlas(List<Texture2D> textures, out Rect[] uvRects)
    {
        var atlasSize = textures.Count * TEXTURE_SIZE;
        var textureAtlas = new Texture2D(atlasSize, atlasSize);
        uvRects = textureAtlas.PackTextures(textures.ToArray(), 2, atlasSize * 2);

        return textureAtlas;
    }

    private List<Texture2D> _createVertexTypeTextures(List<Color> colors)
    {
        List<Texture2D> textures = new();

        foreach (var color in colors)
        {
            var texture = _createSingleColorTexture(color);
            textures.Add(texture);
        }

        return textures;
    }

    private Texture2D _createSingleColorTexture(Color color)
    {
        Texture2D texture = new (TEXTURE_SIZE, TEXTURE_SIZE); 

        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();

        return texture;
    }
}

