using UnityEngine;
using UnityEngine.InputSystem;

public class WaterCleaner : MonoBehaviour
{
    [Header("References")]
    public Transform nozzle;                 // WaterNozzle
    public ParticleSystem waterFX;           // WaterSprayFX

    [Header("Water Spray Settings")]
    public float sprayDistance = 5f;
    public LayerMask dirtLayer;

    [Header("Cleaning Settings")]
    public float cleanDelay = 0.4f;
    public float fadeSpeed = 5f;

    void Update()
    {
        bool spraying = Keyboard.current != null && Keyboard.current.spaceKey.isPressed;

        // تشغيل/إيقاف الماء بصريًا
        if (waterFX != null)
        {
            if (spraying && !waterFX.isPlaying) waterFX.Play();
            if (!spraying && waterFX.isPlaying) waterFX.Stop();
        }

        if (spraying)
        {
            SprayWater();
        }
    }

    void SprayWater()
{
      Debug.Log("SPRAY");
    RaycastHit hit;

    Vector3 origin = Camera.main != null
    ? (Camera.main.transform.position + Camera.main.transform.forward * 0.5f)
    : (nozzle != null ? nozzle.position : transform.position);

    Vector3 dir = Camera.main != null ? Camera.main.transform.forward
    : (nozzle != null ? nozzle.forward : transform.forward);

    float radius = 1.0f; // كبّريها إذا بدك الرش أوسع (0.2 - 0.5)

    Debug.DrawRay(origin, dir * sprayDistance, Color.cyan);

    if (Physics.SphereCast(origin, radius, dir, out hit, sprayDistance, ~0, QueryTriggerInteraction.Collide))
{
    Debug.Log("Hit: " + hit.collider.name + " | Tag=" + hit.collider.tag + " | Layer=" + LayerMask.LayerToName(hit.collider.gameObject.layer));
    
    if (hit.collider.CompareTag("Dirty"))
    {
        if (!hit.collider.GetComponent<DirtFade>())
        {
            hit.collider.gameObject.AddComponent<DirtFade>()
                .StartFade(cleanDelay, fadeSpeed);
        }
    }
}
else
{
    Debug.Log("NO HIT");
}
}
}
