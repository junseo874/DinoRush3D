// ============================================================================
// GameInitializer.cs - 게임 시작 시 사용자 정보와 점수 UI 초기화
// 목적: GameScene에서 GameSession 정보 불러오기, ScoreManager 점수 리셋 및 UI 연결
// 작성자: 이준서
// 작성일: 2025년 06월 10일
// 주요 기능:
// - 로그인 정보 기반으로 사용자 이름과 최고 점수 표시
// - ScoreManager 점수 리셋 및 scoreText 바인딩 보정
// ============================================================================
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInitializer : MonoBehaviour
{
    // UI 텍스트: 플레이어 이름, 최고 점수
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI highScoreText;

    void Start()
    {
        // GameSession 정보가 없으면 다시 로그인 화면으로 이동
        if (GameSession.Instance == null || GameSession.Instance.Client == null)
        {
            Debug.LogWarning("[GameScene] 클라이언트 정보 없음, StartScene으로 되돌립니다.");
            SceneManager.LoadScene("LoginScene");
            return;
        }

        // 시간 흐름 재개
        Time.timeScale = 1f;

        // 현재 점수 리셋
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetScore();
        }

        // 사용자 이름 및 최고 점수 UI 설정
        playerNameText.text = GameSession.Instance.Username;
        highScoreText.text = $"최고 점수: {GameSession.Instance.HighScore}";
        Debug.Log("[GameInitializer] 최고 점수: " + GameSession.Instance.HighScore);

        // ScoreManager에 scoreText가 연결되어 있지 않다면 재바인딩
        if (ScoreManager.Instance != null && ScoreManager.Instance.scoreText == null)
        {
            var scoreTextObj = GameObject.Find("ScoreText");
            if (scoreTextObj != null)
            {
                ScoreManager.Instance.scoreText = scoreTextObj.GetComponent<TextMeshProUGUI>();
                Debug.Log("[GameInitializer] ScoreText 다시 연결됨");
            }
            else
            {
                Debug.LogWarning("[GameInitializer] ScoreText 오브젝트를 찾을 수 없음");
            }
        }
    }
}