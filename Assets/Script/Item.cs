using UnityEngine;

public class Item : MonoBehaviour
{
    public int scoreValue = 5;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
            if (scoreManager != null)
            {
                scoreManager.AddScore(scoreValue);
            }
            Destroy(gameObject);
        }
    }
}