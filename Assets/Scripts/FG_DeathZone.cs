using UnityEngine;

public class FG_DeathZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (FG_GameManager.Instance != null)
            {
                FG_GameManager.Instance.PlayerDied();
            }
        }
    }
}
