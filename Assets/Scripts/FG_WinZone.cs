using UnityEngine;

public class FG_WinZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (FG_GameManager.Instance != null)
            {
                FG_GameManager.Instance.PlayerWon();
            }
        }
    }
}
