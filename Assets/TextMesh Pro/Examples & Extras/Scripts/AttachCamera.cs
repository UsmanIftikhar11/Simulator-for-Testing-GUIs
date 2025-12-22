using UnityEngine;

public class AttachLikeParent : MonoBehaviour
{
    public Transform robot; // Assign your robot/crawler in Inspector

    private Vector3 localOffset;
    private Quaternion localRotation;

    void Start()
    {
        if (robot == null)
        {
            Debug.LogError("Robot/crawler target not assigned!");
            enabled = false;
            return;
        }
        // Capture local offset and rotation at start
        localOffset = robot.InverseTransformPoint(transform.position);
        localRotation = Quaternion.Inverse(robot.rotation) * transform.rotation;
    }

    void LateUpdate()
    {
        // Maintain offset/rotation *as if parented*
        transform.position = robot.TransformPoint(localOffset);
        transform.rotation = robot.rotation * localRotation;
    }
}