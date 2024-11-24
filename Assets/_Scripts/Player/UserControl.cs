using System.Collections.Generic;
using UnityEngine;

public class UserControl : MonoBehaviour
{
    [SerializeField]
    private World _worldRef;

    [SerializeField]
    private GameObject _spherePrefab;

    private GameObject _activeSphere;
    
    private float _sphereRadius = 1f;

    private float _sphereRadiusSensitivity = 1f;

    private Vector3 _hoverWorldPoint;

    private bool _isAddingMode = false;

    private void Start()
    {
        _activeSphere = Instantiate(_spherePrefab, Vector3.zero, Quaternion.identity);
        _activeSphere.SetActive(false);
    }

    private void Update()
    {
        _drawSphere();

        if (Input.GetKeyDown(KeyCode.V))
            _isAddingMode = !_isAddingMode;

        if (Input.GetMouseButton(1))
            _updateSphereRadious();
        else
            _updateHoverPoint();

        if (Input.GetMouseButton(0))
        {
            Dictionary<Vector3Int, float> verticesActivation = new();

            foreach (var vertex in _worldRef.GetVerticesByCondition(_isVertexInSphere)) 
            {
                if (_isAddingMode)
                    verticesActivation[vertex] = -WorldDataSinglton.Instance.ACTIVATION_THRESHOLD * Time.deltaTime;
                else
                    verticesActivation[vertex] = WorldDataSinglton.Instance.ACTIVATION_THRESHOLD * Time.deltaTime;
            }

            _worldRef.AddVerticesActivation(verticesActivation);
        }
    }

    private bool _isVertexInSphere(Vector3Int vertex)
    {
        float distance = Vector3.Distance(_hoverWorldPoint, vertex);

        return distance < _sphereRadius;
    }

    private void _updateHoverPoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.GetComponent<ChunkMeshRenderer>() != null) 
            {
                _hoverWorldPoint = hit.point;
                _activeSphere.SetActive(true);
            }
        }
        else
        {
            _activeSphere.SetActive(false);
        }
    }

    private void _updateSphereRadious()
    {
        float mouseX = Input.GetAxis("Mouse X");
        _sphereRadius += mouseX * _sphereRadiusSensitivity;

        if (_sphereRadius <= 0.1f) _sphereRadius = 0.1f;
    }

    private void _drawSphere()
    {
        _activeSphere.transform.position = _hoverWorldPoint;
        _activeSphere.transform.localScale = new Vector3(_sphereRadius, _sphereRadius, _sphereRadius);
    }
}
