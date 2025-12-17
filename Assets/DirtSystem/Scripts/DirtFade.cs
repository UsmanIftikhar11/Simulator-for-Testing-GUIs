using UnityEngine;
using System.Collections;

public class DirtFade : MonoBehaviour
{
    private Renderer rend;
    private Color originalColor;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        originalColor = rend.material.color;
    }

    public void StartFade(float delay, float fadeSpeed)
    {
        StartCoroutine(FadeOut(delay, fadeSpeed));
    }

    IEnumerator FadeOut(float delay, float fadeSpeed)
    {
        // ننتظر مدة قصيرة بعد الرش
        yield return new WaitForSeconds(delay);

        float alpha = originalColor.a;

        while (alpha > 0)
        {
            alpha -= Time.deltaTime * fadeSpeed;
            rend.material.color = new Color(
                originalColor.r,
                originalColor.g,
                originalColor.b,
                alpha
            );
            yield return null;
        }

        Destroy(gameObject);
    }
}
