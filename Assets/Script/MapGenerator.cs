using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Transform spawnPoint;
    public MapTilePool tilePool;
    public int tileCount = 7;
    public float tileLength = 10f;

    private Queue<GameObject> activeTiles = new Queue<GameObject>();

    void Start()
    {
        // 타일들을 초기 위치부터 지정된 개수만큼 생성
        for (int i = 0; i < tileCount; i++)
        {
            float zPos = spawnPoint.position.z + i * tileLength;
            GameObject tile = tilePool.GetTile();
            tile.transform.position = new Vector3(0, 0, zPos);
            tile.GetComponent<MapMover>().SetGenerator(this);
            activeTiles.Enqueue(tile);
            tile.GetComponent<MapTile>().SpawnObstacle();
        }
    }

    public void RecycleTile(GameObject tile)
    {
        // 마지막 타일 위치 기준으로 새로운 위치 지정
        GameObject lastTile = GetLastTile();
        float newZ = lastTile.transform.position.z + tileLength;
        tile.transform.position = new Vector3(0, 0, newZ);

        activeTiles.Dequeue();
        activeTiles.Enqueue(tile);
    }

    private GameObject GetLastTile()
    {
        GameObject[] activeArray = activeTiles.ToArray();
        return activeArray[activeArray.Length - 1];
    }
}