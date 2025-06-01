using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public float score;
    public float scoreIncreaseRate = 10f;

    private bool isGameOver = false;

    void Update()
    {
        if (isGameOver) return;

        AddScore(scoreIncreaseRate * Time.deltaTime);
    }

    public void StopScoring()
    {
        isGameOver = true;
    }

    public void AddScore(float amount)
    {
        score += amount;
        scoreText.text = Mathf.FloorToInt(score).ToString(); // 점수 UI도 함께 갱신
    }
}