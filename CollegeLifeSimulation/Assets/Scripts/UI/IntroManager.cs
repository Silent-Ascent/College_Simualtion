using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class IntroManager : MonoBehaviour
{
    // Text elements used in the intro
    [SerializeField] private TextMeshProUGUI topText;
    [SerializeField] private TextMeshProUGUI topLeftText;
    [SerializeField] private TextMeshProUGUI bottomRightText;
    [SerializeField] private TextMeshProUGUI centerText;
    [SerializeField] private TextMeshProUGUI taglineText;
    [SerializeField] private TextMeshProUGUI skipText;

    // Camera for intro movement
    [SerializeField] private Camera introCamera;
    [SerializeField] private Transform cameraEndPoint;

    // Basic settings
    [SerializeField] private float cameraMoveSpeed = 0.3f;
    [SerializeField] private float typeSpeed = 0.05f;
    [SerializeField] private string gameSceneName = "SampleScene";

    private void Start()
    {
        ClearAllText(); // make sure nothing is visible at start
        StartCoroutine(PlayIntro());
    }

    private void Update()
    {
        // skip intro on any key press
        if (Input.anyKeyDown)
        {
            StopAllCoroutines();
            LoadGame();
        }

        // slow camera movement
        if (introCamera != null && cameraEndPoint != null)
        {
            introCamera.transform.position = Vector3.Lerp(
                introCamera.transform.position,
                cameraEndPoint.position,
                cameraMoveSpeed * Time.deltaTime
            );
        }
    }

    // clears all text fields
    private void ClearAllText()
    {
        if (topText) topText.text = "";
        if (topLeftText) topLeftText.text = "";
        if (bottomRightText) bottomRightText.text = "";
        if (centerText) centerText.text = "";
        if (taglineText) taglineText.text = "";

        if (skipText)
        {
            skipText.text = "Press any key to skip";
            skipText.color = new Color(1f, 1f, 1f, 0.4f);
        }
    }

    private IEnumerator PlayIntro()
    {
        yield return new WaitForSeconds(0.5f);

        // title
        topText.color = new Color(1f, 0.92f, 0.4f);
        yield return TypeText(topText, "COLLEGE LIFE SIMULATION");

        yield return new WaitForSeconds(1f);

        // tagline
        taglineText.color = Color.white;
        yield return TypeText(taglineText, "Not real... but close to it");

        yield return new WaitForSeconds(2f);

        // teachers
        centerText.color = new Color(1f, 0.84f, 0f);

        yield return TypeText(centerText, "Under the Guidance of");
        yield return new WaitForSeconds(0.3f);

        yield return TypeText(centerText,
            "Under the Guidance of\n\nSanjeev Chamling\nAvimanyu Rimal",
            true
        );

        yield return new WaitForSeconds(2.5f);
        yield return FadeOut(centerText, 1.5f);

        // group name
        centerText.color = new Color(1f, 0.92f, 0.4f);
        yield return TypeText(centerText, "Group X");

        yield return new WaitForSeconds(2f);
        yield return FadeOut(centerText, 1f);

        // members
        topLeftText.color = new Color(0.4f, 0.8f, 1f);
        bottomRightText.color = new Color(1f, 0.4f, 0.7f);

        yield return TypeText(topLeftText, "Aryan Shahi");
        yield return new WaitForSeconds(0.5f);

        yield return TypeText(bottomRightText, "Shuvam Ojha");
        yield return new WaitForSeconds(3f);

        // fade everything
        StartCoroutine(FadeOut(topText, 1f));
        StartCoroutine(FadeOut(taglineText, 1f));
        StartCoroutine(FadeOut(topLeftText, 1f));
        StartCoroutine(FadeOut(bottomRightText, 1f));

        if (skipText)
            StartCoroutine(FadeOut(skipText, 1f));

        yield return new WaitForSeconds(1.5f);

        LoadGame();
    }

    // simple typing effect
    private IEnumerator TypeText(TextMeshProUGUI text, string message, bool append = false)
    {
        if (!append) text.text = "";

        string current = append ? text.text : "";
        string newPart = append ? message.Substring(current.Length) : message;

        foreach (char c in newPart)
        {
            current += c;
            text.text = current;
            yield return new WaitForSeconds(typeSpeed);
        }
    }

    // fade text out smoothly
    private IEnumerator FadeOut(TextMeshProUGUI text, float speed)
    {
        if (text == null) yield break;

        Color c = text.color;
        float a = 1f;

        while (a > 0f)
        {
            a -= Time.deltaTime * speed;
            c.a = a;
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