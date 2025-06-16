// ============================================================================
// ObstacleItemSpawner.cs - 장애물 및 아이템 자동 스폰 관리자
// 목적: 일정 시간 간격으로 장애물과 아이템을 각각 무작위 위치에 생성
// 작성자: 이준서
// 작성일: 2025년 05월 05일
// 주요 기능:
// - 일정 주기로 장애물과 아이템을 각각 지정된 위치에 스폰
// - 프리팹과 위치는 배열로 설정하며 무작위로 선택
// - 각각 "Obstacle", "Item" 태그를 부여하여 후속 처리 용이
// ============================================================================

using UnityEngine;
using System.Collections.Generic;

public class ObstacleItemSpawner : MonoBehaviour
{
    public GameObject[] obstaclePrefabs;         // 생성할 장애물 프리팹들
    public GameObject[] itemPrefabs;             // 생성할 아이템 프리팹들

    public Transform[] obstacleSpawnPoints;      // 장애물 생성 위치들
    public Transform[] itemSpawnPoints;          // 아이템 생성 위치들

    public float spawnInterval = 2.0f;           // 생성 주기 (초)
    private float spawnTimer;                    // 생성 타이머

    void Start()
    {
        spawnTimer = spawnInterval; // 시작 시 타이머 초기화
    }

    void Update()
    {
        spawnTimer -= Time.deltaTime;

        // 타이머가 0 이하일 때 장애물과 아이템 생성
        if (spawnTimer <= 0f)
        {
            SpawnObstacle();    // 장애물 스폰
            SpawnItem();        // 아이템 스폰
            spawnTimer = spawnInterval; // 타이머 재설정
        }
    }

    // 장애물을 무작위 프리팹과 무작위 위치에 생성
    void SpawnObstacle()
    {
        if (obstaclePrefabs.Length == 0 || obstacleSpawnPoints.Length == 0) return;

        int prefabIndex = Random.Range(0, obstaclePrefabs.Length);
        int spawnIndex = Random.Range(0, obstacleSpawnPoints.Length);

        GameObject obj = Instantiate(
            obstaclePrefabs[prefabIndex],
            obstacleSpawnPoints[spawnIndex].position,
            Quaternion.identity
        );

        obj.tag = "Obstacle"; // 태그 설정
        Debug.Log($"[Spawner] 장애물 스폰됨: {obj.name}");
    }

    // 아이템을 무작위 프리팹과 무작위 위치에 생성
    void SpawnItem()
    {
        if (itemPrefabs.Length == 0 || itemSpawnPoints.Length == 0) return;

        int prefabIndex = Random.Range(0, itemPrefabs.Length);
        int spawnIndex = Random.Range(0, itemSpawnPoints.Length);

        GameObject obj = Instantiate(
            itemPrefabs[prefabIndex],
            itemSpawnPoints[spawnIndex].position,
            Quaternion.identity
        );

        obj.tag = "Item"; // 태그 설정
        Debug.Log($"[Spawner] 아이템 스폰됨: {obj.name}");
    }
}