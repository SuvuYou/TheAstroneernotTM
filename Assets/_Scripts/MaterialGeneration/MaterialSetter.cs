using UnityEngine;

class MaterialSetter : MonoBehaviour
{
    [SerializeField]
    private VertexTypeMaterialsManagerSO _vertexTypeMaterialsManager;

    private MeshRenderer _meshRederer;

    private void Awake()
    {
        _meshRederer = GetComponent<MeshRenderer>();

        _setMaterial(_vertexTypeMaterialsManager.TextureAtlasMaterial);

        _vertexTypeMaterialsManager.OnMaterialChanged += _setMaterial;
    }

    private void OnDestroy()
    {
        _vertexTypeMaterialsManager.OnMaterialChanged -= _setMaterial;
    }

    private void _setMaterial(Material material) 
    {
        _meshRederer.material = material;
    }
}
