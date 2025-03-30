using UnityEngine;
using System.Collections.Generic;

public class CloudPool : MonoBehaviour
{
    public GameObject cloudPrefab;
    public int poolSize = 20;

    private List<GameObject> pooledClouds;

    void Start()
    {
        pooledClouds = new List<GameObject>();
        if (cloudPrefab == null)
        {
            Debug.LogError("Cloud Prefab is not assigned in the CloudPool!");
            return;
        }
        for (int i = 0; i < poolSize; i++)
        {
            GameObject cloud = Instantiate(cloudPrefab);
            cloud.SetActive(false);
            pooledClouds.Add(cloud);
        }
    }

    public GameObject GetPooledCloud()
    {
        for (int i = 0; i < pooledClouds.Count; i++)
        {
            if (pooledClouds[i] != null && !pooledClouds[i].activeInHierarchy)
            {
                return pooledClouds[i];
            }
        }
        return null; // Pool exhausted
    }

    public void ReturnCloudToPool(GameObject cloud)
    {
        if (cloud != null)
        {
            cloud.SetActive(false);
        }
    }
}