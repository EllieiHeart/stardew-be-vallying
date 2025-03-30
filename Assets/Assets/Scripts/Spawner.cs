using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Spawner : MonoBehaviour
{
    public GameObject cloudPrefab;
    public GameObject birdPrefab;
    public float birdSpawnDelay = 5f;
    public GameObject leafPrefab;

    public float cloudSpawnInterval = 2f;
    public float birdSpawnInterval = 10f;
    public float leafSpawnInterval = 2f;

    public float spawnAreaWidth = 12f;

    public Transform[] birdPerches;

    private List<Transform> availablePerches;
    private bool canSpawnBird = true;
    private CloudPool cloudPool;
    private GameObject currentBird;

    void Start()
    {
        cloudPool = Object.FindFirstObjectByType<CloudPool>();
        if (cloudPool == null)
        {
            Debug.LogError("CloudPool not found in the scene! Clouds will not spawn.");
        }
        availablePerches = new List<Transform>(birdPerches);
        StartCoroutine(SpawnClouds());
        StartCoroutine(SpawnBirds());
        StartCoroutine(SpawnLeaves());
        StartCoroutine(ManageSingleBird());
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
                    // Spawn clouds at a random position within the camera's view
                    float spawnY = Random.Range(-Camera.main.orthographicSize, Camera.main.orthographicSize);
                    cloud.transform.position = new Vector3(-Camera.main.orthographicSize * Camera.main.aspect - 1f, spawnY, 0);
                    cloud.SetActive(true);
                }
            }
            yield return new WaitForSeconds(cloudSpawnInterval);
        }
    }

    IEnumerator SpawnBirds()
    {
        // This coroutine is now empty. The bird spawning is handled entirely by ManageSingleBird()
        yield return null;
    }

    IEnumerator SpawnLeaves()
    {
        while (true)
        {
            Vector3 spawnPosition = new Vector3(Random.Range(-spawnAreaWidth, spawnAreaWidth), Camera.main.orthographicSize, 0);
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
                canSpawnBird = false;

                int randomIndex = Random.Range(0, availablePerches.Count);
                Transform targetPerch = availablePerches[randomIndex];
                availablePerches.RemoveAt(randomIndex);

                GameObject newBirdObj = Instantiate(birdPrefab, GetBirdSpawnPosition(), Quaternion.identity);
                currentBird = newBirdObj;
                Bird birdScript = currentBird.GetComponent<Bird>();
                if (birdScript != null)
                {
                    birdScript.SetTargetPerchAndSpawner(targetPerch, this);
                }
            }
            yield return null;
        }
    }

    public void BirdDespawned()
    {
        currentBird = null;
        StartCoroutine(EnableBirdSpawn());
    }

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

    private Vector3 GetBirdSpawnPosition()
    {
        return new Vector3(Random.Range(-spawnAreaWidth, spawnAreaWidth), Camera.main.orthographicSize + 2f, 0);
    }
}