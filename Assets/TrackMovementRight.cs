using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class TrackMovementRight : MonoBehaviour
{
    
    private Renderer rend;
    public float speed = 2f;
    private Vector3 lastPos;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        Vector3 pos = transform.position;

        // Use world Y or Z depending on direction you want
        float offsetY = pos.y * speed;
        float offsetx = pos.x * speed;


        float rotation = transform.eulerAngles.z;

        rend.material.mainTextureOffset = new Vector2(
            rend.material.mainTextureOffset.x,
            offsetY - offsetx - rotation
        );
    }
}





