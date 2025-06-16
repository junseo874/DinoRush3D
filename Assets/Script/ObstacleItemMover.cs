using UnityEngine;

public class ObstacleItemMover : MonoBehaviour
{
    public float baseSpeed = 5f;
    public float acceleration = 0.5f;
    public float removeZ = -20f;

    private float currentSpeed;

    void OnEnable()
    {
        currentSpeed = baseSpeed;
    }

    void Update()
    {
        currentSpeed += acceleration * Time.deltaTime;
        transform.Translate(Vector3.back * currentSpeed * Time.deltaTime);

        if (transform.position.z < removeZ)
        {
            gameObject.SetActive(false);
            Debug.Log($"[ObstacleItemMover] 비활성화됨: {gameObject.name} 제거됨");
        }
    }
}