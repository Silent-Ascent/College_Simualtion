using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFlash : MonoBehaviour
{
    public static ScreenFlash Instance;

    [SerializeField] private Image overlayImage;

    private void Awake()
    {
        Instance = this;
    }

    // Call this from anywhere with a colour and duration
    public void Flash(Color colour, float holdTime = 0.5f, float fadeTime = 1f)
    {
        StopAllCoroutines();
        StartCoroutine(DoFlash(colour, holdTime, fadeTime));
    }

    private IEnumerator DoFlash(Color colour, float holdTime, float fadeTime)
    {
        // Set colour instantly
        colour.a = 0.45f;
        overlayImage.color = colour;

        // Hold
        yield return new WaitForSeconds(holdTime);

        // Fade out
        float elapsed = 0f;
        Color start = overlayImage.color;
        Color end = new Color(start.r, start.g, start.b, 0f);

        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            overlayImage.color = Color.Lerp(start, end, elapsed / fadeTime);
            yield return null;
        }

        overlayImage.color = end;
    }
}