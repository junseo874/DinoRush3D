// ============================================================================
// ObstacleItemMover.cs - 장애물 및 아이템 이동 및 자동 제거 처리
// 목적: 장애물과 아이템을 점점 가속하며 뒤로 이동시키고, 일정 위치에 도달하면 비활성화
// 작성자: 이준서
// 작성일: 2025년 05월 05일
// 주요 기능:
// - 활성화 시 초기 속도로 시작
// - 매 프레임 속도 증가 및 이동
// - Z축 기준 특정 위치에 도달하면 객체 비활성화
// ============================================================================

using UnityEngine;

public class ObstacleItemMover : MonoBehaviour
{
    public float baseSpeed = 5f;        // 초기 이동 속도
    public float acceleration = 0.5f;   // 초당 가속도
    public float removeZ = -20f;        // 제거 기준 Z 위치

    private float currentSpeed;         // 현재 속도

    // 오브젝트가 활성화될 때 호출됨
    void OnEnable()
    {
        currentSpeed = baseSpeed; // 속도를 초기화
    }

    void Update()
    {
        // 매 프레임 가속도를 적용하여 속도 증가
        currentSpeed += acceleration * Time.deltaTime;

        // 뒤쪽(Z-) 방향으로 오브젝트 이동
        transform.Translate(Vector3.back * currentSpeed * Time.deltaTime);

        // 특정 Z 위치보다 뒤로 가면 비활성화 (객체 풀링 용도)
        if (transform.position.z < removeZ)
        {
            gameObject.SetActive(false);
            Debug.Log($"[ObstacleItemMover] 비활성화됨: {gameObject.name} 제거됨");
        }
    }
}