using UnityEngine;
using System.Collections.Generic;

public class ObstacleItemSpawner : MonoBehaviour
{
    public GameObject[] obstaclePrefabs;
    public GameObject[] itemPrefabs;

    public Transform[] obstacleSpawnPoints;
    public Transform[] itemSpawnPoints;

    public float spawnInterval = 2.0f;
    private float spawnTimer;

    void Start()
    {
        spawnTimer = spawnInterval;
    }

    void Update()
    {
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f)
        {
            SpawnObstacle();
            SpawnItem();
            spawnTimer = spawnInterval;
        }
    }

    void SpawnObstacle()
    {
        if (obstaclePrefabs.Length == 0 || obstacleSpawnPoints.Length == 0) return;

        int prefabIndex = Random.Range(0, obstaclePrefabs.Length);
        int spawnIndex = Random.Range(0, obstacleSpawnPoints.Length);

        GameObject obj = Instantiate(obstaclePrefabs[prefabIndex], obstacleSpawnPoints[spawnIndex].position, Quaternion.identity);
        obj.tag = "Obstacle";
        Debug.Log($"[Spawner] 장애물 스폰됨: {obj.name}");
    }

    void SpawnItem()
    {
        if (itemPrefabs.Length == 0 || itemSpawnPoints.Length == 0) return;

        int prefabIndex = Random.Range(0, itemPrefabs.Length);
        int spawnIndex = Random.Range(0, itemSpawnPoints.Length);

        GameObject obj = Instantiate(itemPrefabs[prefabIndex], itemSpawnPoints[spawnIndex].position, Quaternion.identity);
        obj.tag = "Item";
        Debug.Log($"[Spawner] 아이템 스폰됨: {obj.name}");
    }
}