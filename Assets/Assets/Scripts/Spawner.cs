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

    public float cloudSpawnInterval = 2f;
    public float birdSpawnInterval = 10f;
    public float starSpawnInterval = 1f;

    public float cloudSpawnAreaXMin = -8f;
    public float cloudSpawnAreaXMax = -2f;
    public float cloudSpawnAreaYMin = -4f;
    public float cloudSpawnAreaYMax = 4f;

    public float starSpawnAreaXMin = -10f; // Adjusted for full width
    public float starSpawnAreaXMax = 10f;  // Adjusted for full width
    public float starSpawnAreaYMin = 0f;    // Stars spawn only on the top half
    public float starSpawnAreaYMax = 8f;    // Stars spawn only on the top half

    public float spawnAreaWidth = 12f;

    public Transform[] birdPerches;

    private List<Transform> availablePerches;
    private bool canSpawnBird = true;
    private CloudPool cloudPool;
    private GameObject currentBird;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found! Spawning may not work correctly.");
        }

        cloudPool = Object.FindFirstObjectByType<CloudPool>();
        if (cloudPool == null)
        {
            Debug.LogError("CloudPool not found in the scene! Clouds will not spawn.");
        }
        availablePerches = new List<Transform>(birdPerches);
        StartCoroutine(SpawnClouds());
        StartCoroutine(ManageSingleBird());
        StartCoroutine(SpawnStars());
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
                // Spawn stars at random positions on the top half of the camera view
                float spawnX = Random.Range(-mainCamera.orthographicSize * mainCamera.aspect, mainCamera.orthographicSize * mainCamera.aspect);
                float spawnY = Random.Range(0f, mainCamera.orthographicSize); // Y range adjusted
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