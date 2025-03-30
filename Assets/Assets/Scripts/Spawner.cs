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

    public float cloudSpawnInterval = 2f;
    public float birdSpawnInterval = 10f;
    public float starSpawnInterval = 1f;
    public float mrQiSpawnDelay = 60f;

    public float cloudSpawnAreaXMin = -8f;
    public float cloudSpawnAreaXMax = -2f;
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
        // 1. Cache the main camera
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found! Spawning may not work correctly.");
            return; // Important: Exit Start() if no camera
        }

        // 2. Find the CloudPool
        cloudPool = Object.FindFirstObjectByType<CloudPool>();
        if (cloudPool == null)
        {
            Debug.LogError("CloudPool not found in the scene! Clouds will not spawn.");
            return; // Important: Exit Start() if no CloudPool
        }

        // 3. Initialize available perches (even if empty)
        availablePerches = new List<Transform>(birdPerches);

        // 4. Start coroutines (in a specific order)
        StartCoroutine(SpawnClouds());
        StartCoroutine(ManageSingleBird());
        StartCoroutine(SpawnStars());
        StartCoroutine(SpawnMrQi());
    }

    IEnumerator SpawnClouds()
    {
        while (true)
        {
            if (cloudPool != null && mainCamera != null)
            {
                GameObject cloud = cloudPool.GetPooledCloud();
                if (cloud != null)
                {
                    float spawnX = Random.Range(cloudSpawnAreaXMin, cloudSpawnAreaXMax);
                    float spawnY = Random.Range(cloudSpawnAreaYMin, cloudSpawnAreaYMax);
                    cloud.transform.position = new Vector3(spawnX, spawnY, 0);
                    cloud.SetActive(true);
                }
            }
            yield return new WaitForSeconds(cloudSpawnInterval);
        }
    }

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

    IEnumerator ManageSingleBird()
    {
        while (true)
        {
            if (currentBird == null && canSpawnBird && availablePerches.Count > 0 && mainCamera != null)
            {
                canSpawnBird = false;

                int randomIndex = Random.Range(0, availablePerches.Count);
                Transform targetPerch = availablePerches[randomIndex];
                availablePerches.RemoveAt(randomIndex);

                GameObject newBirdObj = Instantiate(birdPrefab, GetBirdSpawnPosition(), Quaternion.identity);
                currentBird = newBirdObj;
                Bird birdScript = newBirdObj.GetComponent<Bird>();
                if (birdScript != null)
                {
                    birdScript.SetTargetPerchAndSpawner(targetPerch, this);
                }
            }
            yield return null;
        }
    }

    IEnumerator SpawnMrQi()
    {
        yield return new WaitForSeconds(mrQiSpawnDelay);

        if (mainCamera != null && mrQiPrefab != null)
        {
            float spawnX = -mainCamera.orthographicSize * mainCamera.aspect - 2f;
            float spawnY = 0f;
            Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0);
            GameObject mrQi = Instantiate(mrQiPrefab, spawnPosition, Quaternion.identity);

            MrQi mrQiScript = mrQi.AddComponent<MrQi>();
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