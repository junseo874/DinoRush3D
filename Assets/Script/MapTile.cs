using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile : MonoBehaviour
{
    public float tileLength = 10f;
    public List<Transform> obstacleSpawnPoints;
    public GameObject[] obstaclePrefabs;
    public List<Transform> itemSpawnPoints;
    public GameObject[] itemPrefabs;
    public float itemSpawnChance = 0.3f; // 30% 확률로 생성

    private bool obstacleSpawned = false;

    void OnEnable()
    {
        obstacleSpawned = false;
        StartCoroutine(DelayedSpawn());
    }

    IEnumerator DelayedSpawn()
    {
        yield return new WaitForSeconds(5f);
        if (!obstacleSpawned)
        {
            SpawnObstacle();
            obstacleSpawned = true;
        }
    }

    public void SpawnObstacle()
    {
        if (obstacleSpawnPoints.Count == 0 || obstaclePrefabs.Length == 0) return;

        int pointIndex = Random.Range(0, obstacleSpawnPoints.Count);
        int prefabIndex = Random.Range(0, obstaclePrefabs.Length);

        Transform spawnPoint = obstacleSpawnPoints[pointIndex];
        GameObject obstacle = Instantiate(
            obstaclePrefabs[prefabIndex],
            spawnPoint.position,
            Quaternion.identity,
            this.transform
        );
    }
    public void SpawnItem()
    {
        if (obstacleSpawnPoints.Count == 0 || itemPrefabs.Length == 0) return;

        int pointIndex = Random.Range(0, obstacleSpawnPoints.Count);
        int itemIndex = Random.Range(0, itemPrefabs.Length);

        Transform spawnPoint = obstacleSpawnPoints[pointIndex];
        GameObject item = Instantiate(
            itemPrefabs[itemIndex],
            spawnPoint.position + Vector3.up * 1.5f, // 약간 떠 있도록
            Quaternion.identity,
            this.transform
        );
    }

    public void ClearObstacles()
    {
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Obstacle"))
            {
                Destroy(child.gameObject);
            }
        }
    }
}