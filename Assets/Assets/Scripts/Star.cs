using UnityEngine;
using Random = UnityEngine.Random;

public class Star : MonoBehaviour
{
    public float fadeInTime = 1.0f;
    public float minFallSpeed = 0.2f;
    public float maxFallSpeed = 0.5f;
    public float minSize = 0.5f;
    public float maxSize = 1.5f;
    public float spawnDistanceInterval = 2f;
    public float lifeTime = 5f; 

    private SpriteRenderer spriteRenderer;
    private float alpha = 0f;
    private float timer = 0f;
    private float fallSpeed;
    private float size;
    private float spawnDistanceTimer = 0f;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Color color = spriteRenderer.color;
        color.a = alpha;
        spriteRenderer.color = color;
    }

    void OnEnable()
    {
        fallSpeed = Random.Range(minFallSpeed, maxFallSpeed);
        size = Random.Range(minSize, maxSize);
        transform.localScale = new Vector3(size, size, 1f);
        timer = 0f;
        alpha = 0f;
    }

    void Update()
    {
        timer += Time.deltaTime;
        spawnDistanceTimer += Time.deltaTime;

        // Fade In
        if (alpha < 1f && timer < fadeInTime)
        {
            alpha = timer / fadeInTime;
            Color color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
        }
        // Falling
        else if (timer >= fadeInTime)
        {
            transform.Translate(0, -fallSpeed * Time.deltaTime, 0);
        }

        // Despawn when off-screen OR when lifeTime is reached
        if (transform.position.y < -Camera.main.orthographicSize - maxSize ||
            transform.position.x > Camera.main.orthographicSize * Camera.main.aspect + maxSize ||
            transform.position.x < -Camera.main.orthographicSize * Camera.main.aspect - maxSize ||
            timer >= lifeTime) // **Here's where we use lifeTime**
        {
            Destroy(gameObject);
        }
    }
}