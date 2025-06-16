using System.IO;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCollision : MonoBehaviour
{
    private bool isGameOver = false;
    private ScoreManager scoreManager;

    void Start()
    {
        scoreManager = FindObjectOfType<ScoreManager>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isGameOver) return;

        if (collision.gameObject.CompareTag("Obstacle"))
        {
            isGameOver = true;
            GameOver();
        }
    }

    void GameOver()
    {
        int finalScore = scoreManager.CurrentScore;

        if (GameSession.Instance != null)
        {
            TcpClient client = GameSession.Instance.Client;
            if (client != null && client.Connected)
            {
                try
                {
                    using (var stream = client.GetStream())
                    using (var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true })
                    {
                        writer.WriteLine($"SAVE_SCORE:{GameSession.Instance.Username}:{finalScore}");
                    }
                }
                catch
                {
                    Debug.LogWarning("점수 전송 실패");
                }
            }
        }

        Debug.Log("[GameOver] 점수 저장 후 LoginScene으로 복귀");
        SceneManager.LoadScene("LoginScene");
    }
}