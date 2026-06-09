using UnityEngine;
using System.Collections;
using TMPro;

public class FG_GameManager : MonoBehaviour
{
    public static FG_GameManager Instance;
    
    public Transform player;
    public Vector3 currentCheckpoint;
    public TMP_Text statusText;
    
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (player != null)
        {
            currentCheckpoint = player.position;
        }
        if (statusText != null) statusText.gameObject.SetActive(false);
    }

    public void UpdateCheckpoint(Vector3 newPos)
    {
        currentCheckpoint = newPos;
    }

    public void PlayerDied()
    {
        StartCoroutine(RespawnRoutine());
    }

    IEnumerator RespawnRoutine()
    {
        if (statusText != null)
        {
            statusText.text = "You Died!";
            statusText.color = Color.red;
            statusText.gameObject.SetActive(true);
        }

        if (player != null)
        {
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            player.position = currentCheckpoint;
        }

        yield return new WaitForSeconds(1.5f);

        if (statusText != null)
        {
            statusText.gameObject.SetActive(false);
        }
    }

    public void PlayerWon()
    {
        if (statusText != null)
        {
            statusText.text = "You Win!";
            statusText.color = Color.green;
            statusText.gameObject.SetActive(true);
        }
    }
}
