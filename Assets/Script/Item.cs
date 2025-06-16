// ============================================================================
// Item.cs - 점수 아이템 획득 처리 스크립트
// 목적: 플레이어가 아이템과 충돌하면 점수를 증가시키고 아이템을 제거함
// 작성자: 이준서
// 작성일: 2025년 05월 20일
// 구조: Unity 콜라이더 이벤트 사용 (OnTriggerEnter)
// ============================================================================

using UnityEngine;

public class Item : MonoBehaviour
{
    // 이 아이템을 획득했을 때 추가할 점수
    public int scoreValue = 5;

    // 플레이어와 충돌했을 때 호출됨
    private void OnTriggerEnter(Collider other)
    {
        // 충돌한 대상이 Player 태그일 경우
        if (other.CompareTag("Player"))
        {
            // ScoreManager 인스턴스를 찾아 점수 추가
            ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
            if (scoreManager != null)
            {
                scoreManager.AddScore(scoreValue);  // 점수 증가
            }

            // 아이템 제거
            Destroy(gameObject);
        }
    }
}