using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Spawner : MonoBehaviour
{
    public GameObject cloudPrefab;
    public GameObject birdPrefab;
    public float birdSpawnDelay = 5f;
    public GameObject starPrefab;
    public GameObject mrQiPrefab;

    private GameObject currentMrQi; // Declare currentMrQi at the class level
    public float cloudSpawnInterval = 2f;
    public float birdSpawnInterval = 10f;
    public float starSpawnInterval = 1f;
    public float mrQiSpawnDelay = 60f;

    // Cloud spawn area (we will calculate based on the camera's view)
    public float cloudSpawnAreaYMin = -4f;
    public float cloudSpawnAreaYMax = 4f;

    public float starSpawnAreaXMin = -10f;
    public float starSpawnAreaXMax = 10f;
    public float starSpawnAreaYMin = 0f;
    public float starSpawnAreaYMax = 8f;

    public float spawnAreaWidth = 12f;

    public Transform[] birdPerches;

    private List<Transform> availablePerches;
    private bool canSpawnBird = true;
    private CloudPool cloudPool;
    private GameObject currentBird;
    private Camera mainCamera;

    void Start()
    {
        // Cache the main camera
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found! Spawning may not work correctly.");
            return;
        }

        // Find the CloudPool
        cloudPool = Object.FindFirstObjectByType<CloudPool>();
        if (cloudPool == null)
        {
            Debug.LogError("CloudPool not found in the scene! Clouds will not spawn.");
            return;
        }

        // Initialize available perches
        availablePerches = new List<Transform>(birdPerches);

        // Start coroutines
        StartCoroutine(SpawnClouds());
        StartCoroutine(ManageSingleBird());
        StartCoroutine(SpawnStars());
        StartCoroutine(ManageMrQiSpawning());
    }

    // Coroutine to spawn clouds from the cloud pool
    IEnumerator SpawnClouds()
{
    while (true)
    {
        if (cloudPool != null && mainCamera != null)
        {
            // Retrieve a cloud from the pool
            GameObject cloud = cloudPool.GetPooledCloud();
            if (cloud != null)
            {
                // Calculate the camera bounds for spawning the cloud
                float cameraHalfWidth = mainCamera.orthographicSize * mainCamera.aspect;
                float spawnX = 0f;

                // Randomly choose to spawn from the left or right side
                if (Random.value < 0.5f)
                {
                    spawnX = -cameraHalfWidth - 2f; // Spawn from the left side
                }
                else
                {
                    spawnX = cameraHalfWidth + 2f; // Spawn from the right side
                }

                float spawnY = Random.Range(cloudSpawnAreaYMin, cloudSpawnAreaYMax);
                cloud.transform.position = new Vector3(spawnX, spawnY, 0);
                cloud.SetActive(true);
            }
        }
        yield return new WaitForSeconds(cloudSpawnInterval);
    }
}




    // Coroutine to spawn stars
    IEnumerator SpawnStars()
    {
        while (true)
        {
            if (mainCamera != null)
            {
                float spawnX = Random.Range(-mainCamera.orthographicSize * mainCamera.aspect, mainCamera.orthographicSize * mainCamera.aspect);
                float spawnY = Random.Range(starSpawnAreaYMin, starSpawnAreaYMax);
                Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0);
                Instantiate(starPrefab, spawnPosition, Quaternion.identity);
            }
            yield return new WaitForSeconds(starSpawnInterval);
        }
    }

    // Coroutine to manage bird spawning
    IEnumerator ManageSingleBird()
    {
        while (true)
        {
            if (currentBird == null && canSpawnBird && availablePerches.Count > 0 && mainCamera != null)
            {
                canSpawnBird = false;

                // Choose a random perch
                int randomIndex = Random.Range(0, availablePerches.Count);
                Transform targetPerch = availablePerches[randomIndex];
                availablePerches.RemoveAt(randomIndex);

                // Spawn the bird
                GameObject newBirdObj = Instantiate(birdPrefab, GetBirdSpawnPosition(), Quaternion.identity);
                currentBird = newBirdObj;
                Bird birdScript = newBirdObj.GetComponent<Bird>();
                if (birdScript != null)
                {
                    birdScript.SetTargetPerchAndSpawner(targetPerch, this);
                    birdScript.ChangeState(Bird.BirdState.FlyingAround); // First fly around
                }

                yield return new WaitForSeconds(birdSpawnDelay);
                canSpawnBird = true;
            }
            else
            {
                yield return null; // Wait until a perch is free or the delay is over
            }
        }
    }

    // Coroutine to manage Mr. Qi spawning
    IEnumerator ManageMrQiSpawning()
    {
        while (true)
        {
            yield return new WaitForSeconds(mrQiSpawnDelay);

            if (mainCamera != null && mrQiPrefab != null)
            {
                float spawnX = -mainCamera.orthographicSize * mainCamera.aspect - 2f;
                float spawnY = 0f;
                Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0);
                currentMrQi = Instantiate(mrQiPrefab, spawnPosition, Quaternion.identity);

                MrQi mrQiScript = currentMrQi.GetComponent<MrQi>();

                // Despawn Mr. Qi after his lifetime
                Destroy(currentMrQi, mrQiSpawnDelay);
            }
        }
    }

    // Method to handle bird despawn and enable bird spawning again
    public void BirdDespawned()
    {
        currentBird = null;
        StartCoroutine(EnableBirdSpawn());
    }

    // Coroutine to enable bird spawning after a delay
    IEnumerator EnableBirdSpawn()
    {
        yield return new WaitForSeconds(birdSpawnDelay);
        canSpawnBird = true;
        if (birdPerches.Length > 0)
        {
            Transform lastUsedPerch = birdPerches.Except(availablePerches).FirstOrDefault();
            if (lastUsedPerch != null && !availablePerches.Contains(lastUsedPerch))
            {
                availablePerches.Add(lastUsedPerch);
            }
            availablePerches = new List<Transform>(birdPerches);
        }
    }

    // Method to get the bird's spawn position
    private Vector3 GetBirdSpawnPosition()
    {
        if (mainCamera != null)
        {
            return new Vector3(Random.Range(-spawnAreaWidth, spawnAreaWidth), mainCamera.orthographicSize + 2f, 0);
        }
        else
        {
            return Vector3.zero;
        }
    }
}
