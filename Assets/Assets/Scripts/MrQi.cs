using UnityEngine;

public class MrQi : MonoBehaviour
{
    public float speed = 2f;
    public float lifeTime = 60f;
    public GameObject mysteryBoxPrefab;
    public float mysteryBoxSpawnIntervalMin = 0.5f;
    public float mysteryBoxSpawnIntervalMax = 1.5f;
    public float minBoxSize = 0.5f;
    public float maxBoxSize = 1.5f;
    public float spawnBoxPosition = 0.5f; // Position of the box in the middle of Mr Qi

    private float timer = 0f;
    private SpriteRenderer spriteRenderer;
    private float mysteryBoxSpawnTimer = 0f;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        timer += Time.deltaTime;
        mysteryBoxSpawnTimer += Time.deltaTime;

        // Move across the screen on the X-axis
        transform.Translate(speed * Time.deltaTime, 0, 0);

        // Spawn Mystery Boxes
        if (mysteryBoxSpawnTimer >= Random.Range(mysteryBoxSpawnIntervalMin, mysteryBoxSpawnIntervalMax))
        {
            SpawnMysteryBox();
            mysteryBoxSpawnTimer = 0f;
        }

        // Despawn after lifeTime
        if (timer >= lifeTime)
        {
            Destroy(gameObject);
        }
    }

    void SpawnMysteryBox()
    {
        if (mysteryBoxPrefab != null && spriteRenderer != null)
        {
            // Calculate spawn position under Mr. Qi
            Vector3 boxPosition = new Vector3(transform.position.x, transform.position.y - spawnBoxPosition, transform.position.z);
            GameObject mysteryBox = Instantiate(mysteryBoxPrefab, boxPosition, Quaternion.identity);

            // Randomize Mystery Box size
            float boxSize = Random.Range(minBoxSize, maxBoxSize);
            mysteryBox.transform.localScale = new Vector3(boxSize, boxSize, 1f);

            // Add a component to make the box fall
            MysteryBoxFall mysteryBoxFall = mysteryBox.AddComponent<MysteryBoxFall>();
        }
    }
}