using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class FG_FakeTile : MonoBehaviour
{
    public float fallDelay = 0.2f;
    private Rigidbody rb;
    private bool hasTriggered = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        // Ensure the tile stays completely frozen until stepped on!
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        // If the player touches it, trigger the fall!
        if (!hasTriggered && collision.gameObject.GetComponent<FG_PlayerMovement>() != null)
        {
            hasTriggered = true;
            StartCoroutine(DropTile());
        }
    }

    IEnumerator DropTile()
    {
        // Wait just a tiny bit so the player has a split second of "uh oh"
        yield return new WaitForSeconds(fallDelay);

        // Turn on physics so it falls!
        rb.isKinematic = false;
        rb.useGravity = true;

        // Give it a tiny push downwards
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        // Clean it up after it falls out of view
        Destroy(gameObject, 4f);
    }
}
