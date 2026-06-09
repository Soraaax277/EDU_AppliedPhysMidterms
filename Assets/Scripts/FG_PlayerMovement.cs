using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class FG_PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float jumpForce = 7f;
    private Rigidbody rb;
    private bool isGrounded;
    private float fallTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }

        // Kill player if falling downwards for more than 1.5 seconds (void death)
        if (!isGrounded && rb.linearVelocity.y < -0.1f)
        {
            fallTimer += Time.deltaTime;
            if (fallTimer > 1.5f)
            {
                if (FG_GameManager.Instance != null)
                {
                    FG_GameManager.Instance.PlayerDied();
                }
                fallTimer = 0f; 
            }
        }
        else if (isGrounded || rb.linearVelocity.y > 0f)
        {
            // Reset the timer if we are on the ground or flying upwards
            fallTimer = 0f;
        }
    }

    void FixedUpdate()
    {
        float moveHorizontal = 0f;
        float moveVertical = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) moveVertical += 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) moveVertical -= 1f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveHorizontal -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveHorizontal += 1f;
        }

        Vector3 movement = Vector3.zero;
        Camera cam = Camera.main;
        if (cam != null)
        {
            Vector3 camForward = cam.transform.forward;
            camForward.y = 0;
            camForward.Normalize();

            Vector3 camRight = cam.transform.right;
            camRight.y = 0;
            camRight.Normalize();

            movement = (camForward * moveVertical + camRight * moveHorizontal).normalized;
        }
        else
        {
            movement = new Vector3(moveHorizontal, 0.0f, moveVertical).normalized;
        }

        Vector3 targetVelocity = movement * moveSpeed;
        Vector3 velocityChange = targetVelocity - rb.linearVelocity;
        velocityChange.y = 0f; // Preserve vertical momentum (gravity & jumping)
        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Break the wall if it's tagged "Breakable" or if its name contains "Breakable" or "Wall"
        // Break the wall if it's tagged "Breakable" or if its name contains "Breakable" or "Wall"
        bool hasBreakableTag = false;
        try { hasBreakableTag = collision.gameObject.CompareTag("Breakable"); } catch { }
        
        if (hasBreakableTag || 
            collision.gameObject.name.ToLower().Contains("breakable") ||
            (collision.gameObject.name.ToLower().Contains("wall") && !collision.gameObject.name.ToLower().Contains("floor")))
        {
            Rigidbody wallRb = collision.gameObject.GetComponent<Rigidbody>();
            if (wallRb == null)
            {
                // Give the wall physics so it can be pushed
                wallRb = collision.gameObject.AddComponent<Rigidbody>();
                
                // Give it a satisfying push in the direction the player is moving
                Vector3 pushDirection = collision.relativeVelocity;
                wallRb.AddForce(-pushDirection * 2f, ForceMode.Impulse);

                // Destroy the wall after 4 seconds so it doesn't clutter the stage
                Destroy(collision.gameObject, 4f);
            }
        }
    }

    void OnCollisionStay(Collision collision)
    {
        isGrounded = true;
    }

    void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }
}
