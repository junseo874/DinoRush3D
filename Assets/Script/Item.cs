using UnityEngine;

public class Item : MonoBehaviour
{
    public enum ItemType { ScoreBoost, SpeedBoost, Invincibility }

    public ItemType itemType;
    public float effectDuration = 3f;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        GameObject player = other.gameObject;

        switch (itemType)
        {
            case ItemType.ScoreBoost:
                player.GetComponent<ScoreManager>().AddScore(100);
                break;
            case ItemType.SpeedBoost:
                player.GetComponent<PlayerMove>().ActivateSpeedBoost(effectDuration);
                break;
            case ItemType.Invincibility:
                player.GetComponent<PlayerCollision>().ActivateInvincibility(effectDuration);
                break;
        }

        Destroy(gameObject);
    }
}