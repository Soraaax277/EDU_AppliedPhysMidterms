using UnityEngine;

public class FG_ObstaclePendulum : MonoBehaviour
{
    public float speed = 1.5f;
    public float angle = 45f;
    private Quaternion startRotation;
    private Rigidbody rb;

    void Start()
    {
        startRotation = transform.rotation;
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }

    void FixedUpdate()
    {
        float swing = Mathf.Sin(Time.time * speed) * angle;
        Quaternion swingRot = Quaternion.Euler(0, 0, swing);
        
        // Direct assignment is most reliable for kinematic pendulums
        transform.rotation = startRotation * swingRot;
    }
}
