using UnityEngine;
using TMPro;
using System.Collections;

public class GameTimer : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float totalTime = 180f;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI messageText; // ONE text for all messages

    public static GameTimer Instance;

    private float timeRemaining;
    private bool timerRunning = true;
    public bool IsLate { get; private set; } = false;
    private bool timeOutTriggered = false;

    private void Awake()
    {
        Instance = this;
        timeRemaining = totalTime;

        // Hide message at start
        if (messageText != null)
            messageText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!timerRunning) return;

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 120f && !IsLate)
        {
            IsLate = true;
            ShowMessage(
                "⚠️ HURRY UP!\nYou are running late to college!\nYour future is waiting!",
                Color.yellow,
                4f
            );
        }

        if (timeRemaining <= 0 && !timeOutTriggered)
        {
            timeRemaining = 0;
            timerRunning = false;
            timeOutTriggered = true;
            TriggerTimeOut();
        }

        UpdateTimerUI();
    }

    private void ShowMessage(string text, Color color, float duration)
    {
        if (messageText == null) return;

        messageText.gameObject.SetActive(true);
        messageText.text = text;
        messageText.color = color;

        // Auto hide after duration (0 = stay forever)
        if (duration > 0)
            StartCoroutine(HideMessageAfterDelay(duration));
    }

    private IEnumerator HideMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (messageText != null)
            messageText.gameObject.SetActive(false);
    }

    private void TriggerTimeOut()
    {
        // Stop player
        var player = PlayerHealth.Instance;
        if (player != null)
        {
            var controller = player.GetComponent<CharacterController>();
            var mover = player.GetComponent<StarterAssets.ThirdPersonController>();
            if (controller != null) controller.enabled = false;
            if (mover != null) mover.enabled = false;
        }

        ShowMessage(
            "⏰ TIME'S UP!\n\n" +
            "You were too late for college.\n\n" +
            "Your parents woke up early,\n" +
            "made your breakfast,\n" +
            "and waited at the door...\n\n" +
            "But you never made it on time.\n\n" +
            "Press R to try again.",
            Color.red,
            0f // 0 = stays on screen forever
        );

        Time.timeScale = 0f;
    }

    public void AddPenalty(float seconds)
    {
        timeRemaining -= seconds;
        ShowMessage($"⚠️ PENALTY! -{seconds} seconds!", Color.yellow, 2f);
    }

    public void StopTimer()
    {
        timerRunning = false;
    }

    private void UpdateTimerUI()
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = $"⏱ {minutes:00}:{seconds:00}";

        if (timeRemaining <= 30f)
            timerText.color = Color.red;
        else
            timerText.color = Color.white;
    }
}