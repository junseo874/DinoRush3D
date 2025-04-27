using System.Collections.Generic;
using UnityEngine;

public class MapTile : MonoBehaviour
{
    // 이 타일의 길이 (MapGenerator에서 타일 간격 계산에 사용)
    public float tileLength = 10f;

    // 장애물이 생성될 위치들 (Empty 오브젝트들)
    public List<Transform> obstacleSpawnPoints;

    // 장애물 프리팹 리스트 (랜덤으로 하나 선택됨)
    public GameObject[] obstaclePrefabs;

    // 타일이 생성될 때 장애물도 함께 생성
    public void SpawnObstacle()
    {
        // 위치나 프리팹이 없으면 아무것도 하지 않음
        if (obstacleSpawnPoints.Count == 0 || obstaclePrefabs.Length == 0) return;

        // 위치 하나 랜덤 선택
        int pointIndex = Random.Range(0, obstacleSpawnPoints.Count);

        // 프리팹 하나 랜덤 선택
        int prefabIndex = Random.Range(0, obstaclePrefabs.Length);

        Transform spawnPoint = obstacleSpawnPoints[pointIndex];
        GameObject obstacle = Instantiate(
            obstaclePrefabs[prefabIndex],
            spawnPoint.position,
            Quaternion.identity,
            this.transform // 타일에 붙여서 같이 움직이고 사라지도록 설정
        );
    }

    // 타일 재활용 전에 기존 장애물을 제거하는 함수
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