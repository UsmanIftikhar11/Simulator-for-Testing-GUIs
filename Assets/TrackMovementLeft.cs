using UnityEngine;

public class TrackMovementLeft : MonoBehaviour
{
    private Renderer rend;

    public float moveSpeed = 10f;
    public float turnSpeed = 10f;

    private Vector3 lastPos;
    private float lastRotZ;

    void Start()
    {
        rend = GetComponent<Renderer>();
        lastPos = transform.position;
        lastRotZ = transform.eulerAngles.z;
    }

    void Update()
    {
     
        Vector3 deltaPos = transform.position - lastPos;

  
        float forwardMovement = Vector3.Dot(deltaPos, transform.up);

        float moveOffset = forwardMovement * moveSpeed;

  
        float deltaRot = Mathf.DeltaAngle(lastRotZ, transform.eulerAngles.z);

        float turnOffset = deltaRot * turnSpeed;

        Vector2 off = rend.material.mainTextureOffset;
        off.y -= moveOffset - turnOffset;
        rend.material.mainTextureOffset = off;

   
        lastPos = transform.position;
        lastRotZ = transform.eulerAngles.z;
    }
}
