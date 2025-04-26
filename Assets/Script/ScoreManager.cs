using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText; // 점수 표시용 Text
    public float score;
    public float scoreIncreaseRate = 10f; // 초당 점수 증가량

    private bool isGameOver = false;

    void Update()
    {
        if (isGameOver) return;

        // 점수 증가 (시간 기반)
        score += scoreIncreaseRate * Time.deltaTime;
        scoreText.text = Mathf.FloorToInt(score).ToString(); // 정수로 표시
    }

    public void StopScoring()
    {
        isGameOver = true;
    }
}