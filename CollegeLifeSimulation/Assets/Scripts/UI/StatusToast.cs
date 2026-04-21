using UnityEngine;
using TMPro;
using System.Collections;

public class StatusToast : MonoBehaviour
{
    public static StatusToast Instance;

    [SerializeField] private TextMeshProUGUI toastText;
    [SerializeField] private GameObject toastObject;

    private void Awake()
    {
        Instance = this;
    }

    public void Show(string message, Color colour, float duration = 3f)
    {
        StopAllCoroutines();
        StartCoroutine(ShowToast(message, colour, duration));
    }

    private IEnumerator ShowToast(string message, Color colour, float duration)
    {
        toastText.text = message;
        toastText.color = colour;
        toastObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        toastObject.SetActive(false);
    }
}