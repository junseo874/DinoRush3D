using UnityEngine;

public class MapMover : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float removeZ = -30f;

    private MapGenerator generator;

    public void SetGenerator(MapGenerator gen)
    {
        generator = gen;
    }

    void Update()
    {
        // 맵 타일을 Z 방향으로 뒤로 이동
        transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);

        // 기준 Z 값보다 뒤로 벗어나면 재활용 요청
        if (transform.position.z < removeZ)
        {
            generator.RecycleTile(gameObject);
        }
    }
}