using UnityEngine;

public class FG_Bouncer : MonoBehaviour
{
    public float bounceForce = 20f;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 dir = (collision.transform.position - transform.position).normalized;
                dir.y = 1f; 
                rb.AddForce(dir.normalized * bounceForce, ForceMode.Impulse);
            }
        }
    }
}
