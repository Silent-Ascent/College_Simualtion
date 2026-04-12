using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class IntroManager : MonoBehaviour
{
    [Header("UI Texts")]
    [SerializeField] private TextMeshProUGUI topText;
    [SerializeField] private TextMeshProUGUI topLeftText;
    [SerializeField] private TextMeshProUGUI bottomRightText;
    [SerializeField] private TextMeshProUGUI centerText;
    [SerializeField] private TextMeshProUGUI taglineText;
    [SerializeField] private TextMeshProUGUI skipText;

    [Header("Camera")]
    [SerializeField] private Camera introCamera;
    [SerializeField] private Transform cameraEndPoint;

    [Header("Settings")]
    [SerializeField] private float cameraMoveSpeed = 0.3f;
    [SerializeField] private float typeSpeed = 0.05f; // lower = faster typing
    [SerializeField] private string gameSceneName = "SampleScene";

    private void Start()
    {
        HideAll();
        StartCoroutine(PlayIntro());
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            StopAllCoroutines();
            LoadGame();
        }

        // Slow camera pan
        if (introCamera != null && cameraEndPoint != null)
        {
            introCamera.transform.position = Vector3.Lerp(
                introCamera.transform.position,
                cameraEndPoint.position,
                cameraMoveSpeed * Time.deltaTime
            );
        }
    }

    private void HideAll()
    {
        if (topText != null) topText.text = "";
        if (topLeftText != null) topLeftText.text = "";
        if (bottomRightText != null) bottomRightText.text = "";
        if (centerText != null) centerText.text = "";
        if (taglineText != null) taglineText.text = "";
        if (skipText != null)
        {
            skipText.text = "Press any key to skip";
            skipText.color = new Color(1, 1, 1, 0.4f);
        }
    }

    private IEnumerator PlayIntro()
    {
        yield return new WaitForSeconds(0.5f);

        // Step 1: Game title types at top
        topText.color = Color.yellow;
        yield return TypeWrite(topText, "COLLEGE LIFE SIMULATION");
        yield return new WaitForSeconds(1f);

        // Step 2: Tagline types below
        taglineText.color = Color.white;
        yield return TypeWrite(taglineText, "Not real... but close to it");
        yield return new WaitForSeconds(2f);

        // Step 3: Teachers type in center
        centerText.color = Color.white;
        yield return TypeWrite(centerText, "Under the Guidance of");
        yield return new WaitForSeconds(0.3f);
        yield return TypeWrite(centerText,
            "Under the Guidance of\n\nDesp Sanjeev Chamling\nAbhimanu Rimal",
            true // append mode
        );
        yield return new WaitForSeconds(2.5f);

        // Fade out center
        yield return FadeOut(centerText, 1.5f);

        // Step 4: Team name types in center
        centerText.color = Color.yellow;
        yield return TypeWrite(centerText, "— Team Undefined —");
        yield return new WaitForSeconds(2f);
        yield return FadeOut(centerText, 1f);

        // Step 5: Members type GTA style
        topLeftText.color = Color.white;
        bottomRightText.color = Color.white;

        yield return TypeWrite(topLeftText, "Aryan Shahi");
        yield return new WaitForSeconds(0.5f);
        yield return TypeWrite(bottomRightText, "Shuvam Ojha");
        yield return new WaitForSeconds(3f);

        // Fade everything out
        StartCoroutine(FadeOut(topText, 1f));
        StartCoroutine(FadeOut(taglineText, 1f));
        StartCoroutine(FadeOut(topLeftText, 1f));
        StartCoroutine(FadeOut(bottomRightText, 1f));
        if (skipText != null) StartCoroutine(FadeOut(skipText, 1f));

        yield return new WaitForSeconds(1.5f);

        LoadGame();
    }

    private IEnumerator TypeWrite(TextMeshProUGUI text, string message, bool append = false)
    {
        if (!append) text.text = "";

        string currentText = append ? text.text : "";
        string newPart = append ? message.Substring(currentText.Length) : message;

        foreach (char letter in newPart)
        {
            currentText += letter;
            text.text = currentText;
            yield return new WaitForSeconds(typeSpeed);
        }
    }

    private IEnumerator FadeOut(TextMeshProUGUI text, float speed)
    {
        Color c = text.color;
        float alpha = 1f;
        while (alpha > 0f)
        {
            alpha -= Time.deltaTime * speed;
            c.a = alpha;
            text.color = c;
            yield return null;
        }
        text.text = "";
    }

    private void LoadGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }
}