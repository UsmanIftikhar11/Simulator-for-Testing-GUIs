using UnityEngine;
using System.Collections;

public class ShipPart : MonoBehaviour
{
    [Header("Visuals")]
    public MeshRenderer meshRenderer;
    public Color hotColor = new Color(1f, 0.4f, 0f); // Orange-Hot
    public float fallSpeed = 2.0f;

    private Color _originalColor;
    private bool _isDetached = false;

    void Start()
    {
        if (meshRenderer == null) meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            _originalColor = meshRenderer.material.color;
            meshRenderer.material.EnableKeyword("_EMISSION");
        }

        // DESTROY RIGIDBODY to prevent game hang/physics crash
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb) Destroy(rb);
    }

    public void HeatUp(float percentage)
    {
        if (_isDetached || meshRenderer == null) return;
        // Glow Effect
        Color glow = Color.Lerp(Color.black, hotColor, percentage);
        meshRenderer.material.SetColor("_EmissionColor", glow);
    }

    public void Detach()
    {
        if (_isDetached) return;
        _isDetached = true;

        // Start manual slide animation
        StartCoroutine(FallAnimation());
    }

    IEnumerator FallAnimation()
    {
        float timer = 0;
        while (timer < 4.0f) // Slide for 4 seconds
        {
            timer += Time.deltaTime;
            // Move Down and Out
            transform.Translate((Vector3.down + Vector3.forward * 0.1f) * fallSpeed * Time.deltaTime, Space.Self);
            yield return null;
        }
        gameObject.SetActive(false);
    }
}