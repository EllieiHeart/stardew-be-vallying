using UnityEngine;

public class Cloud : MonoBehaviour
{
    public float minSpeed = 0.5f;
    public float maxSpeed = 1.5f;
    public float minDrift = -0.2f;
    public float maxDrift = 0.2f;
    public float minSize = 50f;
    public float maxSize = 100f;
    public float gatherTime = 10f;
    public float gatherGrowthRate = 0.2f;
    public float gatherFadeRate = 1f;
    public float dissipateFadeRate = 2f;

    private float speed;
    private float drift;
    private float size;
    private float alpha = 1f;
    private string state = "drifting";
    private float stateTimer = 0f;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        speed = Random.Range(minSpeed, maxSpeed);
        drift = Random.Range(minDrift, maxDrift);
        size = Random.Range(minSize, maxSize);
        transform.localScale = new Vector3(size, size, 1f);
        spriteRenderer = GetComponent<SpriteRenderer>();
        Color color = spriteRenderer.color;
        color.a = alpha;
        spriteRenderer.color = color;
    }

    void Update()
    {
        transform.Translate(0, -speed * Time.deltaTime, 0);
        transform.Translate(drift * Time.deltaTime, 0, 0);

        if (state == "drifting")
        {
            if (Random.value < 0.001f) // 0.1% chance to gather
            {
                state = "gathering";
                stateTimer = gatherTime;
            }
        }
        else if (state == "gathering")
        {
            size += gatherGrowthRate * Time.deltaTime;
            alpha -= gatherFadeRate * Time.deltaTime;
            stateTimer -= Time.deltaTime;
            if (stateTimer <= 0)
            {
                state = "drifting";
            }
        }

        if (transform.position.y < Camera.main.orthographicSize * -1) // Reached top
        {
            state = "dissipating";
            alpha -= dissipateFadeRate * Time.deltaTime;
        }

        transform.localScale = new Vector3(size, size, 1f);
        Color currentColor = spriteRenderer.color;
        currentColor.a = alpha;
        spriteRenderer.color = currentColor;

        if (alpha <= 0)
        {
            Destroy(gameObject);
        }
    }
}