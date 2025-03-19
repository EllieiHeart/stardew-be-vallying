using UnityEngine;

public class Bird : MonoBehaviour
{
    public float speed = 3f;
    public float forageTime = 2f;
    public float perchTime = 3f;
    public float forageDip = 0.5f;
    public Transform[] perchPoints; // Assign these in the Inspector

    private string state = "flying";
    private float stateTimer = 0f;
    private Vector3 targetPosition;
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
        targetPosition = new Vector3(Random.Range(-10f, 10f), Random.Range(-5f, 5f), 0);
    }

    void Update()
    {
        if (state == "flying")
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                if (Random.value < 0.2f) // 20% chance to forage
                {
                    state = "foraging";
                    stateTimer = forageTime;
                }
                else if (perchPoints.Length > 0 && Random.value < 0.1f) // 10% chance to perch
                {
                    state = "perching";
                    stateTimer = perchTime;
                    targetPosition = perchPoints[Random.Range(0, perchPoints.Length)].position;
                }
                else
                {
                    targetPosition = new Vector3(Random.Range(-10f, 10f), Random.Range(-5f, 5f), 0);
                }
            }
        }
        else if (state == "foraging")
        {
            transform.Translate(0, -forageDip * Time.deltaTime, 0);
            stateTimer -= Time.deltaTime;
            if (stateTimer <= 0)
            {
                transform.position = new Vector3(transform.position.x, startPosition.y, 0); // Return to original Y
                state = "flying";
                targetPosition = new Vector3(Random.Range(-10f, 10f), Random.Range(-5f, 5f), 0);
            }
        }
        else if (state == "perching")
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            stateTimer -= Time.deltaTime;
            if (stateTimer <= 0)
            {
                state = "flying";
                targetPosition = new Vector3(Random.Range(-10f, 10f), Random.Range(-5f, 5f), 0);
            }
        }
    }
}