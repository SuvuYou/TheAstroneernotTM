using UnityEngine;

public class WorldDataSinglton : MonoBehaviour
{
    public static WorldDataSinglton Instance;
    
    [SerializeField]
    [Range(0.0f, 1.0f)]
    [Tooltip("Threshold for noise values; Below this value the vertex will not be activated")]
    private float _acttivationThreshold = 0.5f;

    [SerializeField]
    private int _caveThreshold = 15;

    [SerializeField]
    private int _renderDistance = 4;

    [SerializeField]
    private int _chunkSize = 15;

    [SerializeField]
    private int _chunkHeight = 15;

    [SerializeField]
    private NoiseSettings _terrainNoiseSettings;

    [SerializeField]
    private NoiseSettings _caveNoiseSettings;

    public int CAVE_THRESHOLD { get => _caveThreshold; }

    public float ACTIVATION_THRESHOLD { get => _acttivationThreshold; }

    public int CHUNK_SIZE { get => _chunkSize; }

    public int CHUNK_HEIGHT { get => _chunkHeight; }

    public NoiseSettings TERRAIN_NOISE_SETTINGS { get => _terrainNoiseSettings; }

    public NoiseSettings CAVE_NOISE_SETTINGS { get => _caveNoiseSettings; }

    // Add + 1 for intersecting vertices between chunks:
    // x-axis: (0, 15) -> Chunk #1
    // x-axis: (15, 30) -> Chunk #2
    // 15 belongs to both chunks for rendering purposes
    public int CHUNK_SIZE_WITH_INTERSECTIONS { get => _chunkSize + 1; }

    public int CHUNK_HEIGHT_WITH_INTERSECTIONS { get => _chunkHeight + 1; }

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
