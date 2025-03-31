using UnityEngine;
using Random = UnityEngine.Random;

public class Star : MonoBehaviour
{
    public float fadeInTime = 1.0f;
    public float minFallSpeed = 0.2f;
    public float maxFallSpeed = 0.5f;
    public float minSize = 0.5f;
    public float maxSize = 1.5f;
    public float lifeTime = 5f; // Lifetime of the star before it dies

    private SpriteRenderer spriteRenderer;
    private float alpha = 0f;
    private float timer = 0f;
    private float fallSpeed;
    private float size;

    // Define the states for the star
    private enum StarState { Spawning, Falling, Dying }
    private StarState currentState;

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
        currentState = StarState.Spawning; // Start in the Spawning state
    }

    void Update()
    {
        timer += Time.deltaTime;

        // Handle state transitions and behaviors
        switch (currentState)
        {
            case StarState.Spawning:
                SpawningBehavior();
                break;
            case StarState.Falling:
                FallingBehavior();
                break;
            case StarState.Dying:
                DyingBehavior();
                break;
        }

        // Despawn condition
        if (transform.position.y < -Camera.main.orthographicSize - maxSize ||
            transform.position.x > Camera.main.orthographicSize * Camera.main.aspect + maxSize ||
            transform.position.x < -Camera.main.orthographicSize * Camera.main.aspect - maxSize ||
            timer >= lifeTime) // If the star reaches its life span, it dies
        {
            if (currentState != StarState.Dying) // Prevent multiple transitions
            {
                currentState = StarState.Dying;
            }
        }
    }

    // Spawning state behavior
    private void SpawningBehavior()
    {
        // Fade in
        if (alpha < 1f && timer < fadeInTime)
        {
            alpha = timer / fadeInTime;
            Color color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
        }

        // Transition to Falling state once the fade-in is complete
        if (timer >= fadeInTime)
        {
            currentState = StarState.Falling;
        }
    }

    // Falling state behavior
    private void FallingBehavior()
    {
        // The star falls downward
        transform.Translate(0, -fallSpeed * Time.deltaTime, 0);

        // Randomly slow down the falling speed as it gets closer to the bottom
        if (timer > lifeTime * 0.8f)
        {
            fallSpeed = Mathf.Lerp(fallSpeed, 0, Time.deltaTime);
        }
    }

    // Dying state behavior
    private void DyingBehavior()
    {
        // Fade out
        alpha -= 0.05f * Time.deltaTime;
        if (alpha < 0f) alpha = 0f;
        Color color = spriteRenderer.color;
        color.a = alpha;
        spriteRenderer.color = color;

        // After fading out, destroy the star
        if (alpha <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
