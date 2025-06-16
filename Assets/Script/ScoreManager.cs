using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public int CurrentScore { get; private set; }

    void Start()
    {
        CurrentScore = 0;
        InvokeRepeating("IncreaseScore", 1f, 1f);
    }

    void IncreaseScore()
    {
        AddScore(1);
    }

    public void AddScore(int value)
    {
        CurrentScore += value;
        scoreText.text = $"점수: {CurrentScore}";
    }
}