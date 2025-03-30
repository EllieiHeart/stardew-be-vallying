using UnityEngine;
using Random = UnityEngine.Random;

public class Cloud : MonoBehaviour
{
    public float minSpeed = 0.5f;
    public float maxSpeed = 1.5f;
    public float minDriftSpeed = 0.1f;
    public float maxDriftSpeed = 0.5f;
    public float minSize = 1f;
    public float maxSize = 2f;

    private float speed;
    private float drift;
    private float size;
    private float alpha = 1f;
    private SpriteRenderer spriteRenderer;
    private CloudPool cloudPool;
    private float driftDirection = 1f; // Always drift right

    void OnEnable()
    {
        speed = Random.Range(minSpeed, maxSpeed);
        drift = Random.Range(minDriftSpeed, maxDriftSpeed) * driftDirection;
        size = Random.Range(minSize, maxSize);
        transform.localScale = new Vector3(size, size, 1f);
        alpha = 1f;
        Color color = spriteRenderer.color;
        color.a = alpha;
        spriteRenderer.color = color;
    }

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        cloudPool = Object.FindFirstObjectByType<CloudPool>();
        if (cloudPool == null)
        {
            Debug.LogError("CloudPool not found in the scene!");
        }
    }

    void Update()
    {
        transform.Translate(drift * Time.deltaTime, -speed * Time.deltaTime, 0); // Drift on X, Speed on Y

        // Despawn when off-screen to the right
        if (transform.position.x > Camera.main.orthographicSize * Camera.main.aspect + maxSize)
        {
            ReturnToPool();
            return;
        }

        // Despawn when off-screen to the top
        if (transform.position.y > Camera.main.orthographicSize + maxSize)
        {
            ReturnToPool();
            return;
        }

        // Despawn when off-screen to the bottom
        if (transform.position.y < -Camera.main.orthographicSize - maxSize)
        {
            ReturnToPool();
            return;
        }
    }

    void ReturnToPool()
    {
        if (cloudPool != null)
        {
            cloudPool.ReturnCloudToPool(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}