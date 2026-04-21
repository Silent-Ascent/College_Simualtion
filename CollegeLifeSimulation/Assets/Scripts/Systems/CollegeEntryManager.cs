using UnityEngine;
using TMPro;

public class CollegeEntryManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject ssdOfficeTrigger;
    [SerializeField] private GameObject classroomTrigger;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI entryStatusText;
    [SerializeField] private GameObject latePanel;
    [SerializeField] private GameObject onTimePanel;

    [Header("Settings")]
    [SerializeField] private float lateTimeThreshold = 120f; // 2 min remaining = late

    public static CollegeEntryManager Instance;

    public enum EntryStatus { NotArrived, OnTime, Late }
    public EntryStatus CurrentStatus { get; private set; } = EntryStatus.NotArrived;

    private bool hasEnteredCollege = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // SSD trigger off by default — only activate if late
        if (ssdOfficeTrigger != null)
            ssdOfficeTrigger.SetActive(false);
    }

    // Called by EntryTriggerZone when player walks through gate
    public void PlayerEnteredCollege()
    {
        if (hasEnteredCollege) return;
        hasEnteredCollege = true;

        GameTimer timer = GameTimer.Instance;

        if (timer != null && timer.IsLate)
        {
            TriggerLateEntry();
        }
        else
        {
            TriggerOnTimeEntry();
        }
    }

    private void TriggerOnTimeEntry()
    {
        CurrentStatus = EntryStatus.OnTime;
        Debug.Log("Player arrived ON TIME — proceeding to classroom");

        if (entryStatusText != null)
            entryStatusText.text = "✅ On time! Go to your classroom.";

        if (onTimePanel != null)
            onTimePanel.SetActive(true);

        // Enable classroom door trigger
        if (classroomTrigger != null)
            classroomTrigger.SetActive(true);

        // Stop the arrival timer
        if (GameTimer.Instance != null)
            GameTimer.Instance.StopTimer();
    }

    private void TriggerLateEntry()
    {
        CurrentStatus = EntryStatus.Late;
        Debug.Log("Player arrived LATE — must go to SSD");

        if (entryStatusText != null)
            entryStatusText.text = "⚠️ You are late! Go to the SSD office.";

        if (latePanel != null)
            latePanel.SetActive(true);

        // Enable SSD trigger — player must go there
        if (ssdOfficeTrigger != null)
            ssdOfficeTrigger.SetActive(true);

        // Add late strike
        LateCounter.Instance?.AddLateStrike();

        GameTimer.Instance?.StopTimer();
        ScreenFlash.Instance?.Flash(Color.red, 0.5f, 1.5f);
    }
}
