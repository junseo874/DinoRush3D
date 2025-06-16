using System.Collections;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private bool isDead = false;
    private bool isInvincible = false;

    public LoginClient loginClient;

    private void OnTriggerEnter(Collider other)
    {
        if (isDead || isInvincible) return;

        if (other.CompareTag("Obstacle"))
        {
            isDead = true;
            Debug.Log("[충돌] 플레이어가 장애물에 부딪혔습니다.");
            GameOver();
        }
        else if (other.CompareTag("Item"))
        {
            Debug.Log("[충돌] 아이템 획득!");
            ScoreManager.Instance.AddScore(10);
            other.gameObject.SetActive(false);
        }
    }

    public void ActivateInvincibility(float duration)
    {
        if (!isInvincible)
        {
            StartCoroutine(InvincibilityCoroutine(duration));
        }
    }

    private IEnumerator InvincibilityCoroutine(float duration)
    {
        isInvincible = true;
        Debug.Log("[무적] 시작");
        yield return new WaitForSeconds(duration);
        isInvincible = false;
        Debug.Log("[무적] 종료");
    }

    public void GameOver()
    {
        Debug.Log("[게임 오버] Time.timeScale = 0");

        Time.timeScale = 0f;

        if (loginClient != null && loginClient.startPanel != null)
        {
            loginClient.startPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("[GameOver] loginClient 또는 startPanel이 연결되지 않았습니다.");
        }

        if (ScoreManager.Instance != null)
        {
            int finalScore = ScoreManager.Instance.GetCurrentScore();
            Debug.Log($"[게임 오버] 최종 점수: {finalScore}");

            ScoreManager.Instance.SendScoreToServer();
        }
    }

    public void ResetState()
    {
        isDead = false;
        isInvincible = false;
    }
}