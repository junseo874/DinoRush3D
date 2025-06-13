using System;
using UnityEngine;
using System.Collections;

public class PlayerCollision : MonoBehaviour
{
    private bool isDead = false;
    public ButtonManager buttonManager;


    private void OnTriggerEnter(Collider other)
    {
        if (isDead) return;

        if (other.CompareTag("Obstacle"))
        {
            isDead = true;
            Debug.Log("플레이어 사망: 장애물과 충돌함");
            GameOver();
        }
    }

    public void ActivateInvincibility(float duration)
    {
        StartCoroutine(InvincibilityCoroutine(duration));
    }

    private IEnumerator InvincibilityCoroutine(float duration)
    {
        isDead = true;
        yield return new WaitForSeconds(duration);
        isDead = false;
    }
    
    public void GameOver()
    {
        // 게임 멈추기
        Time.timeScale = 0f;
        buttonManager.ShowGameOver();
        // 추후 UI 연결 가능 (Game Over 화면 띄우기)
    }
}