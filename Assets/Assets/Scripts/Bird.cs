using UnityEngine;

public class Bird : MonoBehaviour
{
    public enum BirdState { FlyingToPerch, Perched, Chasing, FlyingAway }

    public BirdState currentBirdState;
    private Transform targetPerch;
    private float stateTimer;
    private float lifeTimer = 10f; // Time before bird flies away
    private float flySpeed = 5f; // Speed of bird flying to perch
    private float perchStayTimeMin = 2f; // Min time to stay perched
    private float perchStayTimeMax = 5f; // Max time to stay perched
    private Transform chaseTarget;
    private Spawner spawner;

    void Update()
    {
        stateTimer -= Time.deltaTime;
        lifeTimer -= Time.deltaTime;

        if (lifeTimer <= 0)
        {
            ChangeState(BirdState.FlyingAway); // All birds eventually fly away
        }

        switch (currentBirdState)
        {
            case BirdState.FlyingToPerch:
                FlyTowardsPerch();
                break;
            case BirdState.Perched:
                DoPerchBehavior();
                break;
            case BirdState.Chasing:
                ChaseTarget();
                break;
            case BirdState.FlyingAway:
                FlyAway();
                break;
        }
    }

    public void ChangeState(BirdState newState)
    {
        currentBirdState = newState;
        stateTimer = 0;

        switch (newState)
        {
            case BirdState.FlyingToPerch:
                if (targetPerch != null)
                {
                    FlyTowardsPerch();
                }
                break;
            case BirdState.Perched:
                if (targetPerch != null)
                {
                    DoPerchBehavior();
                    stateTimer = Random.Range(perchStayTimeMin, perchStayTimeMax);
                }
                break;
            case BirdState.Chasing:
                if (chaseTarget != null)
                {
                    ChaseTarget();
                    stateTimer = 5f; // Example chase duration
                }
                break;
            case BirdState.FlyingAway:
                FlyAway();
                if (spawner != null)
                {
                    spawner.BirdDespawned();
                }
                Destroy(gameObject);
                break;
        }
    }

    // Method to handle bird movement towards the perch
    private void FlyTowardsPerch()
    {
        if (targetPerch != null)
        {
            float step = flySpeed * Time.deltaTime; // Adjust flySpeed based on your desired speed
            transform.position = Vector3.MoveTowards(transform.position, targetPerch.position, step);

            // If the bird reaches the perch
            if (transform.position == targetPerch.position)
            {
                ChangeState(BirdState.Perched); // Once it reaches the perch, change state to Perched
            }
        }
    }

    private void DoPerchBehavior()
    {
        // Implement perch behavior (e.g., idle, wait for a random time, etc.)
    }

    private void ChaseTarget()
    {
        // Implement chasing behavior
    }

    private void FlyAway()
    {
        // Implement flying away behavior
    }

    public void SetTargetPerchAndSpawner(Transform perch, Spawner spwnr)
    {
        targetPerch = perch;
        spawner = spwnr;
    }
}
