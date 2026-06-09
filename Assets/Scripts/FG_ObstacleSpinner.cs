using UnityEngine;

public class FG_ObstacleSpinner : MonoBehaviour
{
    public Vector3 spinSpeed = new Vector3(0, 100f, 0);
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }

    void FixedUpdate()
    {
        // For purely script-driven obstacles, transforming directly is completely reliable
        // when the Rigidbody is kinematic.
        transform.Rotate(spinSpeed * Time.fixedDeltaTime);
    }
}
