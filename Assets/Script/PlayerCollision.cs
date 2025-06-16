// ============================================================================
// PlayerCollision.cs - 장애물 충돌 시 게임 종료 처리 및 점수 서버 전송
// 목적: 플레이어가 장애물에 충돌하면 점수를 서버에 저장하고 LoginScene으로 복귀
// 작성자: 이준서, 최경빈
// 작성일: 2025년 05월 15일
// 주요 기능:
// - 충돌 감지 (Trigger)
// - 현재 점수 가져오기
// - 서버에 SAVE_SCORE 메시지 전송
// - GameSession 내 최고 점수 갱신
// - LoginScene으로 씬 전환
// ============================================================================

using System.Net.Sockets;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCollision : MonoBehaviour
{
    // 게임이 끝났는지 여부를 나타냄
    private bool isGameOver = false;

    // 충돌 감지
    private void OnTriggerEnter(Collider other)
    {
        if (isGameOver) return;

        // 장애물과 충돌한 경우
        if (other.CompareTag("Obstacle"))
        {
            isGameOver = true;
            Debug.Log("Game Over");

            // 현재 점수 가져오기
            int score = ScoreManager.Instance.GetScore();
            Debug.Log($"[GameOver] 점수 저장 후 LoginScene으로 복귀");

            // 세션 정보가 있을 경우 서버에 점수 저장 요청
            if (GameSession.Instance != null)
            {
                TcpClient client = GameSession.Instance.Client;
                string username = GameSession.Instance.Username;

                if (client != null && client.Connected)
                {
                    NetworkStream stream = client.GetStream();
                    StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

                    // SAVE_SCORE 메시지 전송 형식: SAVE_SCORE:username:score
                    writer.WriteLine($"SAVE_SCORE:{username}:{score}");
                    Debug.Log("[GameOver] SAVE_SCORE 메시지 전송 완료");

                    // GameSession 내 최고 점수도 갱신
                    GameSession.Instance.HighScore = Mathf.Max(GameSession.Instance.HighScore, score);
                }
            }

            // 로그인 씬으로 전환
            SceneManager.LoadScene("LoginScene");
        }
    }
}