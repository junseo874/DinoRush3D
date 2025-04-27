using System.Collections.Generic;
using UnityEngine;

public class MapTilePool : MonoBehaviour
{
    public GameObject tilePrefab;
    public int poolSize = 10;

    private Queue<GameObject> pool = new Queue<GameObject>();

    void Start()
    {
        // 사전 지정된 개수만큼 미리 타일을 생성해 비활성화
        for (int i = 0; i < poolSize; i++)
        {
            GameObject tile = Instantiate(tilePrefab);
            tile.SetActive(false);
            pool.Enqueue(tile);
        }
    }

    public GameObject GetTile()
    {
        // 풀에서 꺼내 활성화하고, 다시 큐에 넣음
        GameObject tile = pool.Dequeue();
        tile.SetActive(true);
        pool.Enqueue(tile);
        return tile;
    }
}