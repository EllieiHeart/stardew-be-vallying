using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
    public GameObject cloudPrefab;
    public GameObject birdPrefab;
    public GameObject leafPrefab;

    public float cloudSpawnInterval = 5f;
    public float birdSpawnInterval = 10f;
    public float leafSpawnInterval = 2f;

    public float spawnAreaWidth = 10f;
    public float spawnAreaHeight = 6f;

    void Start()
    {
        StartCoroutine(SpawnClouds());
        StartCoroutine(SpawnBirds());
        StartCoroutine(SpawnLeaves());
    }

    IEnumerator SpawnClouds()
    {
        while (true)
        {
            Vector3 spawnPosition = new Vector3(Random.Range(-spawnAreaWidth, spawnAreaWidth), -spawnAreaHeight, 0);
            Instantiate(cloudPrefab, spawnPosition, Quaternion.identity);
            yield return new WaitForSeconds(cloudSpawnInterval);
        }
    }

    IEnumerator SpawnBirds()
    {
        while (true)
        {
            Vector3 spawnPosition = new Vector3(Random.Range(-spawnAreaWidth, spawnAreaWidth), Random.Range(-spawnAreaHeight, spawnAreaHeight), 0);
            Instantiate(birdPrefab, spawnPosition, Quaternion.identity);
            yield return new WaitForSeconds(birdSpawnInterval);
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
}