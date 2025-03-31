using UnityEngine;

public class Bird : MonoBehaviour
{
    public enum BirdState { FlyingToPerch, Perched, Chasing, FlyingAway, FlyingAround } // Added FlyingAround

    public BirdState currentBirdState;
    private Transform targetPerch;
    private float stateTimer;
    private float lifeTimer = 10f; // Time before bird flies away
    private float flySpeed = 3f; // Default speed of bird flying
    private float increasedFlySpeed = 6f; // Increased speed when flying to perch
    private float perchStayTimeMin = 2f; // Min time to stay perched
    private float perchStayTimeMax = 5f; // Max time to stay perched
    private Transform chaseTarget;
    private Spawner spawner;

    private Vector3 randomFlightTarget; // For the random flying around behavior
    private float flyingAroundTime = 5f; // Time to fly around before heading to the perch

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
            case BirdState.FlyingAround:
                FlyAround();
                break;
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
            case BirdState.FlyingAround:
                StartFlyingAround();
                break;
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

    // Flying around behavior before heading to perch
    private void StartFlyingAround()
    {
        // Set a random position for the bird to fly to within the scene
        randomFlightTarget = new Vector3(
            Random.Range(-10f, 10f), // X random range
            Random.Range(-5f, 5f),   // Y random range
            0
        );

        // Set the state timer to control how long the bird flies around
        stateTimer = flyingAroundTime;
    }

    private void FlyAround()
    {
        if (stateTimer > 0)
        {
            // Move the bird towards the random flight target with its normal fly speed
            float step = flySpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, randomFlightTarget, step);
        }
        else
        {
            // After flying around, transition to flying to perch state
            ChangeState(BirdState.FlyingToPerch);
        }
    }

    // Method to handle bird movement towards the perch
    private void FlyTowardsPerch()
    {
        if (targetPerch != null)
        {
            // Increase speed when flying towards the perch
            float step = increasedFlySpeed * Time.deltaTime; // Increased speed
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
        stateTimer = Random.Range(perchStayTimeMin, perchStayTimeMax); // Random perch stay duration
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
