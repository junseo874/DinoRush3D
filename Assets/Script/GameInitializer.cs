using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInitializer : MonoBehaviour
{
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI highScoreText;

    void Start()
    {
        if (GameSession.Instance == null || GameSession.Instance.Client == null)
        {
            Debug.LogWarning("[GameScene] 클라이언트 정보 없음, LoginScene으로 되돌립니다.");
            SceneManager.LoadScene("LoginScene");
            return;
        }

        Time.timeScale = 1f; // 🔥 게임 시간 재시작

        playerNameText.text = GameSession.Instance.Username;
        highScoreText.text = $"최고 점수: {GameSession.Instance.HighScore}";
    }
}