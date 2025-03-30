using UnityEngine;

public class Bird : MonoBehaviour
{
    public float speed = 3f;
    public float perchStayTime = 2f;

    private string state = "flyingToPerch";
    private float stateTimer = 0f;
    private Transform targetPerch;
    private Spawner spawner;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetTargetPerchAndSpawner(Transform perch, Spawner spawner)
    {
        targetPerch = perch;
        this.spawner = spawner;
        state = "flyingToPerch";
    }

    void Update()
    {
        if (targetPerch == null)
        {
            Destroy(gameObject);
            return;
        }

        if (state == "flyingToPerch")
        {
            // Determine direction and flip sprite (reversed)
            if (targetPerch.position.x > transform.position.x)
            {
                spriteRenderer.flipX = true;  // Face left when moving right
            }
            else
            {
                spriteRenderer.flipX = false; // Face right when moving left
            }

            transform.position = Vector3.MoveTowards(transform.position, targetPerch.position, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPerch.position) < 0.1f)
            {
                state = "perched";
                stateTimer = perchStayTime;
            }
        }
        else if (state == "perched")
        {
            stateTimer -= Time.deltaTime;
            if (stateTimer <= 0)
            {
                if (spawner != null)
                {
                    spawner.BirdDespawned();
                }
                Destroy(gameObject);
            }
        }
    }
}