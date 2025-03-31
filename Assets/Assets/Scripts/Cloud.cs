using UnityEngine;

public class Cloud : MonoBehaviour
{
    public float minDriftSpeed = 0.1f;
    public float maxDriftSpeed = 0.8f;
    public float minSize = 1f;
    public float maxSize = 2f;
    public float lifespan = 50f;  // Cloud's lifespan before it despawns
    public float sizeDecaySpeed = 0.05f; // Speed of shrinking in idle state
    private float drift;
    private float size;
    private float alpha = 1f;
    private SpriteRenderer spriteRenderer;
    private CloudPool cloudPool;
    private float driftDirection = 1f; // Default drift direction (rightward)
    private float timer = 0f;
    private bool isNearBird = false; // Flag to check if a bird is near

    private enum CloudState { Idle, Moving, Dying, Disturbed }
    private CloudState currentState;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        cloudPool = Object.FindFirstObjectByType<CloudPool>();
        if (cloudPool == null)
        {
            Debug.LogError("CloudPool not found in the scene!");
        }
    }

    void OnEnable()
    {
        // Initialize cloud's properties when it is enabled
        drift = Random.Range(minDriftSpeed, maxDriftSpeed) * driftDirection;
        size = Random.Range(minSize, maxSize);
        transform.localScale = new Vector3(size, size, 1f);
        alpha = 1f;
        currentState = CloudState.Moving; // Start in moving state
        timer = 0f;  // Reset the timer each time the cloud spawns

        // Set drift direction based on spawn position
        if (transform.position.x < 0)
        {
            driftDirection = 1f; // Move right if spawned on the negative x-axis
        }
        else
        {
            driftDirection = -1f; // Move left if spawned on the positive x-axis
        }

        drift = Random.Range(minDriftSpeed, maxDriftSpeed) * driftDirection; // Update drift based on direction
    }

    void Update()
    {
        timer += Time.deltaTime;

        // Perform behavior based on the current state
        switch (currentState)
        {
            case CloudState.Idle:
                IdleBehavior();
                break;
            case CloudState.Moving:
                MoveBehavior();
                break;
            case CloudState.Dying:
                DyingBehavior();
                break;
            case CloudState.Disturbed:
                DisturbedBehavior();
                break;
        }

        // Despawn if the cloud exceeds its lifespan
        if (timer > lifespan)
        {
            currentState = CloudState.Dying;
        }

        // Check if cloud has gone off-screen and return to pool
        if (transform.position.x > Camera.main.orthographicSize * Camera.main.aspect + maxSize || transform.position.x < -Camera.main.orthographicSize * Camera.main.aspect - maxSize)
        {
            ReturnToPool();
        }
    }

    // Idle state behavior (cloud is still, shrinking)
    void IdleBehavior()
    {
        size -= sizeDecaySpeed * Time.deltaTime;
        if (size < minSize) size = minSize;  // Prevent cloud from shrinking below its minimum size

        transform.localScale = new Vector3(size, size, 1f);

        // Switch to moving state if size is above a certain threshold
        if (size > minSize)
        {
            currentState = CloudState.Moving;
        }
    }

    // Moving state behavior (cloud drifts across the screen)
    void MoveBehavior()
    {
        // If a bird is near, enter disturbed state (triggered by bird proximity)
        if (isNearBird)
        {
            currentState = CloudState.Disturbed;
            return;
        }

        // In moving state, the cloud drifts with a constant speed
        transform.Translate(drift * Time.deltaTime, 0, 0);

        // Switch to idle state if it slows down or stays still for too long (simulate a resting phase)
        if (Random.value < 0.1f && drift < minDriftSpeed)
        {
            currentState = CloudState.Idle;
        }

        // Randomly switch to dying state based on time or a triggering event
        if (timer > lifespan * 0.8f)
        {
            currentState = CloudState.Dying;
        }
    }

    // Dying state behavior (cloud shrinks, fades out)
    void DyingBehavior()
    {
        // In dying state, stop the erratic movement (no shaking)
        // The cloud only fades out without shaking
        alpha -= 0.05f * Time.deltaTime;
        if (alpha < 0f) alpha = 0f;
        Color color = spriteRenderer.color;
        color.a = alpha;
        spriteRenderer.color = color;

        // Once alpha reaches zero, return to the pool or destroy
        if (alpha <= 0f)
        {
            ReturnToPool();
        }
    }



    // Disturbed state behavior (cloud gets shaken, moves erratically due to bird proximity)
    void DisturbedBehavior()
    {
        // In disturbed state, the cloud moves erratically as if disturbed by something (like a bird)
        float erraticMovement = Random.Range(-1f, 1f);
        transform.Translate(erraticMovement * Time.deltaTime, 0, 0);

        // Increase the timer faster while the cloud is disturbed
        timer += Time.deltaTime * 2f;  // Timer increases 2x faster in disturbed state

        // Switch back to moving state after a short duration
        if (timer > lifespan * 0.1f) // After 10% of the lifespan, return to moving state
        {
            currentState = CloudState.Moving;
        }
    }


    // Simulate the presence of a bird near the cloud
    public void BirdNearby(bool isNearby)
    {
        isNearBird = isNearby;

        // If the bird leaves, switch back to normal behavior
        if (!isNearBird && currentState == CloudState.Disturbed)
        {
            currentState = CloudState.Moving;
        }
    }

    // Return cloud to the pool or destroy it if no pool exists
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
