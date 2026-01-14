using UnityEngine;
using UnityEngine.InputSystem;

public class WaterCleaner : MonoBehaviour
{
    public Transform nozzle;
    public ParticleSystem waterFX;

    public AudioSource spraySound;

    public float sprayDistance = 5f;
    public LayerMask dirtLayer;

    public float cleanDelay = 0.4f;
    public float fadeSpeed = 5f;

    bool spraying = false;

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.kKey.wasPressedThisFrame)
        {
            spraying = !spraying;

            if (waterFX != null)
            {
                if (spraying) waterFX.Play();
                else waterFX.Stop();
            }

            if (spraySound != null)
            {
                if (spraying)
                {
                    if (!spraySound.isPlaying) spraySound.Play();
                }
                else
                {
                    if (spraySound.isPlaying) spraySound.Stop();
                }
            }
        }

        if (spraying)
        {
            SprayWater();
        }
    }

    void SprayWater()
    {
        RaycastHit hit;

        Vector3 origin = Camera.main != null
            ? (Camera.main.transform.position + Camera.main.transform.forward * 0.5f)
            : (nozzle != null ? nozzle.position : transform.position);

        Vector3 dir = Camera.main != null
            ? Camera.main.transform.forward
            : (nozzle != null ? nozzle.forward : transform.forward);

        float radius = 1.0f;

        Debug.DrawRay(origin, dir * sprayDistance, Color.cyan);

        if (Physics.SphereCast(origin, radius, dir, out hit, sprayDistance, ~0, QueryTriggerInteraction.Collide))
        {
            if (hit.collider.CompareTag("Dirty"))
            {
                if (!hit.collider.GetComponent<DirtFade>())
                {
                    hit.collider.gameObject
                        .AddComponent<DirtFade>()
                        .StartFade(cleanDelay, fadeSpeed);
                }
            }
        }
    }
}
