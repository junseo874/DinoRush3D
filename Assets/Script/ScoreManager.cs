// ============================================================================
// ScoreManager.cs - 자동 점수 증가 및 UI 갱신 매니저
// 목적: 일정 시간마다 점수를 자동 증가시키고, TextMeshPro 텍스트에 점수를 갱신
// 작성자: 이준서
// 작성일: 2025년 06월 17일
// 구조: 싱글톤 기반 ScoreManager → 매 프레임 Update 및 자동 누적 처리
// ============================================================================
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public TextMeshProUGUI scoreText;
    private int score = 0;
    private float timer = 0f;
    public float scoreInterval = 1.0f; // 1초마다 점수 증가
    public int scorePerInterval = 1;   // 증가할 점수 양

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        score = 0;
        UpdateScoreText();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= scoreInterval)
        {
            AddScore(scorePerInterval);
            timer = 0f;
            Debug.Log($"[ScoreManager] 점수 증가: {score}");
        }
    }

    public void AddScore(int value)
    {
        score += value;
        UpdateScoreText();
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
    }

    public int GetScore()
    {
        return score;
    }

    public void ResetScore()
    {
        score = 0;
        UpdateScoreText();
    }
}