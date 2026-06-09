using UnityEngine;

public class FG_Checkpoint : MonoBehaviour
{
    public Transform respawnPoint;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (FG_GameManager.Instance != null)
            {
                FG_GameManager.Instance.UpdateCheckpoint(respawnPoint != null ? respawnPoint.position : transform.position);
            }
        }
    }
}
