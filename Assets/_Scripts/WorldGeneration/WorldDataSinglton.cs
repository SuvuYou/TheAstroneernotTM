using UnityEngine;

public class WorldDataSinglton : MonoBehaviour
{
    public static WorldDataSinglton Instance;
    
    [SerializeField]
    [Range(0.0f, 1.0f)]
    [Tooltip("Threshold for noise values; Below this value the vertex will not be activated")]
    private float _acttivationThreshold = 0.5f;

    [SerializeField]
    private int _renderDistance = 2;

    [SerializeField]
    private int _chunkSize = 15;

    public float ACTIVATION_THRESHOLD { get => _acttivationThreshold; }

    public int CHUNK_SIZE { get => _chunkSize; }

    // Add + 1 for intersecting vertices between chunks:
    // x-axis: (0, 15) -> Chunk #1
    // x-axis: (15, 30) -> Chunk #2
    // 15 belongs to both chunks for rendering purposes
    public int CHUNK_SIZE_WITH_INTERSECTIONS { get => _chunkSize + 1; }

    public int WORLD_SIZE { get => _renderDistance; }

    public int RENDER_DISTANCE { get => _renderDistance; }

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
