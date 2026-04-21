using UnityEngine;
using TMPro;

public class TeacherEventTrigger : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI eventText;
    [SerializeField] private GameObject eventPanel;

    [Header("Settings")]
    [SerializeField] private float teacherArrivalDelay = 5f;
    [SerializeField] private float lateTeacherChance = 0.5f;

    [Header("Explosion Effect")]
    [SerializeField] private ParticleSystem explosionEffect;

    private bool eventTriggered = false;

    private string[] lateEvents = {
        "💥 The door explodes open!",
        "🔒 The door is locked. Waiting...",
        "🚨 Fire alarm goes off!",
        "😴 Teacher is still not here...",
        "📵 Teacher sent a voice note instead"
    };

    private void Start()
    {
        Invoke(nameof(TriggerTeacherArrival), teacherArrivalDelay);
    }

    public void TriggerTeacherArrival()
    {
        if (eventTriggered) return;
        eventTriggered = true;

        bool teacherIsLate = Random.value < lateTeacherChance;

        if (teacherIsLate)
            TriggerLateTeacherEvent();
        else
            TriggerNormalClass();
    }

    private void TriggerNormalClass()
    {
        ShowEvent("📚 Teacher arrived on time. Class begins!");
    }

    private void TriggerLateTeacherEvent()
    {
        int randomIndex = Random.Range(0, lateEvents.Length);
        string eventMessage = lateEvents[randomIndex];
        ShowEvent(eventMessage);

        if (randomIndex == 0 && explosionEffect != null)
        {
            explosionEffect.Play();
            StartCoroutine(ShakeCamera());
        }
    }

    private void ShowEvent(string message)
    {
        if (eventPanel != null) eventPanel.SetActive(true);
        if (eventText != null) eventText.text = message;
        Invoke(nameof(HideEvent), 4f);
    }

    private void HideEvent()
    {
        if (eventPanel != null) eventPanel.SetActive(false);
    }

    private System.Collections.IEnumerator ShakeCamera()
    {
        Camera cam = Camera.main;
        if (cam == null) yield break;

        Vector3 originalPos = cam.transform.localPosition;
        float elapsed = 0;

        while (elapsed < 0.5f)
        {
            float x = Random.Range(-0.15f, 0.15f);
            float y = Random.Range(-0.15f, 0.15f);
            cam.transform.localPosition = originalPos + new Vector3(x, y, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        cam.transform.localPosition = originalPos;
    }
}