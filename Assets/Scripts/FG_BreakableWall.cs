using UnityEngine;

public class FG_BreakableWall : MonoBehaviour
{
    public float breakForceThreshold = 2f; // Optional: Require a certain force to break. Set to 0 to break on any touch.
    public GameObject breakEffect; // Optional: Particle system to instantiate on break

    void OnCollisionEnter(Collision collision)
    {
        // Check if the object colliding is the player
        if (collision.gameObject.GetComponent<FG_PlayerMovement>() != null)
        {
            // You can optionally check the collision impulse/velocity here
            // if (collision.impulse.magnitude > breakForceThreshold) { ... }
            
            BreakWall();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is the player
        if (other.GetComponent<FG_PlayerMovement>() != null)
        {
            BreakWall();
        }
    }

    private void BreakWall()
    {
        // Optional: Spawn a particle effect
        if (breakEffect != null)
        {
            Instantiate(breakEffect, transform.position, transform.rotation);
        }

        // Destroy the wall
        Destroy(gameObject);
    }
}
