using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float totalTime = 180f; // 3 minutes to reach college

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameObject lateWarningPanel;

    public static GameTimer Instance;

    private float timeRemaining;
    private bool timerRunning = true;
    public bool IsLate { get; private set; } = false;

    private void Awake()
    {
        Instance = this;
        timeRemaining = totalTime;
    }

    private void Update()
    {
        if (!timerRunning) return;

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 30f && !IsLate)
        {
            IsLate = true;
            if (lateWarningPanel != null)
                lateWarningPanel.SetActive(true);
            Debug.Log("Player is running late!");
        }

        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            timerRunning = false;
            TriggerTimeOut();
        }

        UpdateTimerUI();
    }

    public void AddPenalty(float seconds)
    {
        timeRemaining -= seconds;
        Debug.Log($"Penalty! -{seconds}s. Remaining: {timeRemaining}");
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

        // Turn red when low
        if (timeRemaining <= 30f)
            timerText.color = Color.red;
    }

    private void TriggerTimeOut()
    {
        Debug.Log("TIME OUT - Player too slow!");
        // This will link to the SSD system in Phase 2
    }
}