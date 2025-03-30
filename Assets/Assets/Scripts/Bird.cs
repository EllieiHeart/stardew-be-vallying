using UnityEngine;

public class Bird : MonoBehaviour
{
    public float speed = 3f;
    public float perchStayTime = 2f; // Time the bird stays on the perch

    private string state = "flyingToPerch";
    private float stateTimer = 0f;
    private Transform targetPerch;
    private Spawner spawner; // Reference to the Spawner script

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
            Destroy(gameObject); // Should not happen if Spawner works correctly
            return;
        }

        if (state == "flyingToPerch")
        {
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
                // Bird has stayed long enough, inform the spawner to spawn a new one
                if (spawner != null)
                {
                    spawner.BirdDespawned();
                }
                Destroy(gameObject);
            }
        }
    }
}