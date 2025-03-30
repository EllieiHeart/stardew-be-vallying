using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Spawner : MonoBehaviour
{
    public GameObject cloudPrefab;
    public GameObject birdPrefab;
    public float birdSpawnDelay = 5f; // Delay before a new bird spawns
    public GameObject leafPrefab;

    public float cloudSpawnInterval = 5f;
    public float leafSpawnInterval = 2f;

    public float spawnAreaWidth = 10f;
    public float spawnAreaHeight = 6f;

    public Transform[] birdPerches; // Assign the perch points in the Inspector

    private List<Transform> availablePerches;
    private bool canSpawnBird = true;
    private CloudPool cloudPool;
    private GameObject currentBird; // To track the currently active bird

    void Start()
    {
        cloudPool = Object.FindFirstObjectByType<CloudPool>();
        if (cloudPool == null)
        {
            Debug.LogError("CloudPool not found in the scene! Clouds will not spawn.");
        }
        availablePerches = new List<Transform>(birdPerches);
        StartCoroutine(SpawnClouds());
        StartCoroutine(SpawnLeaves());
        StartCoroutine(ManageSingleBird()); // Start the coroutine to manage the single bird
    }

    IEnumerator SpawnClouds()
    {
        while (true)
        {
            if (cloudPool != null)
            {
                GameObject cloud = cloudPool.GetPooledCloud();
                if (cloud != null)
                {
                    cloud.transform.position = new Vector3(Random.Range(-spawnAreaWidth, spawnAreaWidth), -spawnAreaHeight, 0);
                    cloud.SetActive(true);
                }
            }
            yield return new WaitForSeconds(cloudSpawnInterval);
        }
    }

    IEnumerator SpawnLeaves()
    {
        while (true)
        {
            Vector3 spawnPosition = new Vector3(Random.Range(-spawnAreaWidth, spawnAreaWidth), spawnAreaHeight, 0);
            Instantiate(leafPrefab, spawnPosition, Quaternion.identity);
            yield return new WaitForSeconds(leafSpawnInterval);
        }
    }

    IEnumerator ManageSingleBird()
    {
        while (true)
        {
            if (currentBird == null && canSpawnBird && availablePerches.Count > 0)
            {
                canSpawnBird = false; // Prevent immediate spawning

                // Choose a random available perch
                int randomIndex = Random.Range(0, availablePerches.Count);
                Transform targetPerch = availablePerches[randomIndex];
                availablePerches.RemoveAt(randomIndex); // Mark perch as occupied

                // Spawn the bird
                GameObject newBirdObj = Instantiate(birdPrefab, GetBirdSpawnPosition(), Quaternion.identity);
                currentBird = newBirdObj;
                Bird birdScript = currentBird.GetComponent<Bird>();
                if (birdScript != null)
                {
                    birdScript.SetTargetPerchAndSpawner(targetPerch, this);
                }
            }
            yield return null; // Wait for the next frame
        }
    }

    public void BirdDespawned()
    {
        currentBird = null; // The bird has despawned
        StartCoroutine(EnableBirdSpawn()); // Start the delay before a new bird can spawn
    }

    IEnumerator EnableBirdSpawn()
    {
        yield return new WaitForSeconds(birdSpawnDelay);
        canSpawnBird = true;
        // When a new bird can spawn, also make the perch available again
        if (birdPerches.Length > 0)
        {
            // Since we don't know which perch the bird went to, we'll just add one back.
            // A more robust system might track which perch was used.
            // For simplicity, we assume a perch becomes free after a bird cycle.
            Transform lastUsedPerch = birdPerches.Except(availablePerches).FirstOrDefault();
            if (lastUsedPerch != null && !availablePerches.Contains(lastUsedPerch))
            {
                availablePerches.Add(lastUsedPerch);
            }
            // To prevent the availablePerches from growing indefinitely if birds keep using the same few,
            // we can reset the available perches each time a bird despawns.
            availablePerches = new List<Transform>(birdPerches);
        }
    }

    private Vector3 GetBirdSpawnPosition()
    {
        // Define a specific spawn area for birds
        return new Vector3(Random.Range(-spawnAreaWidth, spawnAreaWidth), spawnAreaHeight + 2f, 0); // Spawn slightly above the view
    }
}