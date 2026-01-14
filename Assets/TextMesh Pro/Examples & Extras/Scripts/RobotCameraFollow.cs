using UnityEngine;

public class RobotCameraFollow : MonoBehaviour
{
    public Transform target;           // The robot to follow
    public Vector3 offset = new Vector3(0, 2, -5);   // Position offset from the robot
    public bool lookAtTarget = true;   // Should this camera look at the robot?
    public Vector3 lookDirectionOffset = Vector3.zero; // Offset for look direction (robot forward, backward, etc.)

    void LateUpdate()
    {
        if (target == null) return;

        // Camera position
        transform.position = target.TransformPoint(offset);

        if (lookAtTarget)
        {
            // Third person: Look directly at robot
            transform.LookAt(target.position);
        }
        else
        {
            // First/Back person: Look ahead/back, not at robot
            // E.g. look ahead (forward): target.position + target.forward * distance + offset
            Vector3 lookAtPoint = target.position + target.TransformDirection(lookDirectionOffset);
            transform.LookAt(lookAtPoint);
        }
    }
}