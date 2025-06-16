using UnityEngine;
using TMPro;
using System.Net.Sockets;
using System.Text;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int score = 0;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;

    private string userId;
    private TcpClient client;
    private NetworkStream stream;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 재시작에도 유지
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Initialize(string id, TcpClient tcpClient, int highScore)
    {
        userId = id;
        client = tcpClient;
        stream = client.GetStream();
        score = 0;
        UpdateScoreText();
        UpdateHighScoreUI(highScore);
    }

    public void AddScore(int value)
    {
        score += value;
        UpdateScoreText();
        Debug.Log($"[ScoreManager] 점수 추가됨: {score}");
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = $"점수: {score}";
        }
        else
        {
            Debug.LogWarning("[ScoreManager] scoreText가 연결되지 않았습니다.");
        }
    }

    public void SendScoreToServer()
    {
        if (client == null || !client.Connected || stream == null)
        {
            Debug.LogWarning("[ScoreManager] 서버에 연결되어 있지 않습니다.");
            return;
        }

        string message = $"SAVE_SCORE/{userId}/{score}\n";
        byte[] data = Encoding.UTF8.GetBytes(message);
        stream.Write(data, 0, data.Length);
        Debug.Log($"[ScoreManager] 서버로 점수 전송: {message.Trim()}");
    }

    public void UpdateHighScoreUI(int highScore)
    {
        if (highScoreText != null)
        {
            highScoreText.text = $"최고 점수: {highScore}";
        }
        else
        {
            Debug.LogWarning("[ScoreManager] highScoreText가 연결되지 않았습니다.");
        }
    }

    public void ResetScore()
    {
        score = 0;
        UpdateScoreText();
        Debug.Log("[ScoreManager] 점수 초기화 완료");
    }

    public int GetCurrentScore()
    {
        return score;
    }
}