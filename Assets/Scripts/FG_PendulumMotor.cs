using UnityEngine;

[RequireComponent(typeof(HingeJoint))]
public class FG_PendulumMotor : MonoBehaviour
{
    public float swingSpeed = 150f;
    public float limitAngle = 45f;
    private HingeJoint hinge;
    private int direction = 1;

    void Start()
    {
        hinge = GetComponent<HingeJoint>();
        hinge.useMotor = true;
        
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }
        
        JointMotor motor = hinge.motor;
        motor.force = 5000f; // Strong force to swing
        motor.freeSpin = false;
        hinge.motor = motor;
    }

    void FixedUpdate()
    {
        // Reverse direction if we hit the limits
        if (hinge.angle >= limitAngle) direction = -1;
        else if (hinge.angle <= -limitAngle) direction = 1;

        JointMotor motor = hinge.motor;
        motor.targetVelocity = swingSpeed * direction;
        hinge.motor = motor;
    }
}
