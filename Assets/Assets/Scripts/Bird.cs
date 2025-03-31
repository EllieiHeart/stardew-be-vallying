using UnityEngine;

public class Bird : MonoBehaviour
{
    public enum BirdState { FlyingToPerch, Perched, Chasing, FlyingAway, FlyingAround }

    public BirdState currentBirdState;
    private Transform targetPerch;
    private float stateTimer;
    private float lifeTimer = 15f; // Time before bird flies away
    private float flySpeed = 3f; // Default speed of bird flying
    private float increasedFlySpeed = 6f; // Increased speed when flying to perch
    private float perchStayTimeMin = 2f; // Min time to stay perched
    private float perchStayTimeMax = 5f; // Max time to stay perched
    private Transform chaseTarget;
    private Spawner spawner;

    private Vector3 randomFlightTarget; // For the random flying around behavior
    private float flyingAroundTime = 5f; // Fly around for 5 seconds before heading to perch

    private GameObject mrQi; // Reference to Mr. Qi

    private Vector3 flyAwayTarget = new Vector3(-15f, 10f, 0); // Target position for flying away

    void Start()
    {
        // Try to find Mr. Qi in the scene
        mrQi = GameObject.FindGameObjectWithTag("MrQi");
        if (mrQi != null)
        {
            // If Mr. Qi is already in the scene, start chasing him immediately
            chaseTarget = mrQi.transform;
            ChangeState(BirdState.Chasing);
        }
    }

    void Update()
    {
        stateTimer -= Time.deltaTime;
        lifeTimer -= Time.deltaTime;

        // If Mr. Qi has just spawned or become active, switch to chasing
        if (mrQi != null && mrQi.activeInHierarchy)
        {
            // If the bird is not already chasing Mr. Qi, start chasing him immediately
            if (currentBirdState != BirdState.Chasing)
            {
                chaseTarget = mrQi.transform;
                ChangeState(BirdState.Chasing); // Immediate state change to Chasing
            }
        }
        else
        {
            // If Mr. Qi is no longer in the scene, go back to flying state
            if (chaseTarget != null && !IsTargetVisible(mrQi.transform))
            {
                chaseTarget = null; // Clear the chase target
                ChangeState(BirdState.FlyingAround); // Transition back to flying behavior
            }
        }

        // If life time ends, the bird should fly away
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
        if (currentBirdState == newState) return; // Do not change if the state is already the same

        currentBirdState = newState;
        stateTimer = 0; // Reset the state timer when changing state

        switch (newState)
        {
            case BirdState.FlyingAround:
                StartFlyingAround();
                break;
            case BirdState.FlyingToPerch:
                if (targetPerch != null)
                {
                    FlyTowardsPerch();
                    FlipSpriteForPerch();
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

        // Set the state timer to control how long the bird flies around (5 seconds)
        stateTimer = flyingAroundTime;
    }

    private void FlyAround()
    {
        if (stateTimer > 0)
        {
            // Move the bird towards the random flight target with its normal fly speed
            float step = flySpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, randomFlightTarget, step);

            // Flip the sprite based on the movement direction
            FlipSpriteBasedOnMovement();
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

            // Flip the sprite based on the movement direction
            FlipSpriteForPerch();

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
        if (chaseTarget != null)
        {
            float step = flySpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, chaseTarget.position, step);

            // Flip the sprite based on the movement direction
            FlipSpriteBasedOnMovement();

            // If the bird reaches Mr. Qi (or goes off-screen), stop chasing
            if (!IsTargetVisible(chaseTarget))
            {
                chaseTarget = null; // Clear the chase target when Mr. Qi goes off-screen
                ChangeState(BirdState.FlyingAround); // Return to flying behavior if Mr. Qi is off-screen
            }
        }
    }

    private bool IsTargetVisible(Transform target)
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(target.position);
        return screenPoint.x >= 0 && screenPoint.x <= 1 && screenPoint.y >= 0 && screenPoint.y <= 1;
    }

    private void FlipSpriteBasedOnMovement()
    {
        // Flip the sprite based on the direction of movement
        float horizontalMove = transform.position.x - randomFlightTarget.x;

        if (horizontalMove < 0) // Moving left
        {
            Vector3 scale = transform.localScale;
            scale.x = -Mathf.Abs(scale.x); // Invert the sprite (flip it)
            transform.localScale = scale;
        }
        else if (horizontalMove > 0) // Moving right
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x); // Keep the sprite normal
            transform.localScale = scale;
        }
    }

    // Flip sprite when flying towards the perch (check if the perch is on the left or right)
    private void FlipSpriteForPerch()
    {
        if (targetPerch != null)
        {
            if (targetPerch.position.x > transform.position.x)
            {
                // The perch is to the right, flip the sprite
                Vector3 scale = transform.localScale;
                scale.x = -Mathf.Abs(scale.x); // Flip sprite horizontally
                transform.localScale = scale;
            }
            else
            {
                // The perch is to the left, keep the sprite normal
                Vector3 scale = transform.localScale;
                scale.x = Mathf.Abs(scale.x); // Normal orientation
                transform.localScale = scale;
            }
        }
    }

    private void FlyAway()
    {
        // Implement flying away behavior (e.g., move off-screen or out of bounds)
        float step = flySpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, flyAwayTarget, step); // Example flying away direction

        // Once the bird has reached the fly away target, destroy the bird
        if (transform.position == flyAwayTarget)
        {
            Destroy(gameObject); // The bird leaves the scene after flying away
        }
    }

    public void SetTargetPerchAndSpawner(Transform perch, Spawner spwnr)
    {
        targetPerch = perch;
        spawner = spwnr;
    }
}
