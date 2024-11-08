using UnityEngine;

public class WorldDataSinglton : MonoBehaviour
{
    public static WorldDataSinglton Instance;
    
    [SerializeField]
    [Range(0.0f, 1.0f)]
    [Tooltip("Threshold for noise values; Below this value the vertex will not be activated")]
    private float _acttivationThreshold = 0.5f;

    [SerializeField]
    private int _worldSize = 16;

    public float ACTIVATION_THRESHOLD { get => _acttivationThreshold; }

    public int WORLD_SIZE { get => _worldSize; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
