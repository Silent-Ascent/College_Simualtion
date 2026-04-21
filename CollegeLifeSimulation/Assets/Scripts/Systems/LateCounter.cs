using UnityEngine;
using TMPro;

public class LateCounter : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int maxLateStrikes = 3;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI lateCountText;
    [SerializeField] private GameObject punishmentPanel;

    public static LateCounter Instance;

    private int lateStrikes = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateLateUI();
    }

    public void AddLateStrike()
    {
        lateStrikes++;
        UpdateLateUI();

        Debug.Log($"Late strikes: {lateStrikes}/{maxLateStrikes}");

        if (lateStrikes >= maxLateStrikes)
        {
            TriggerPunishment();
        }
    }

    private void UpdateLateUI()
    {
        if (lateCountText != null)
            lateCountText.text = $"⚠️ Late: {lateStrikes}/{maxLateStrikes}";
    }

    private void TriggerPunishment()
    {
        Debug.Log("PUNISHMENT: Player was late 3 times in a row!");

        if (punishmentPanel != null)
            punishmentPanel.SetActive(true);

        GameTimer.Instance?.AddPenalty(30f);

        StartCoroutine(FreezePlayer());
    }

    private System.Collections.IEnumerator FreezePlayer()
    {
        // Frozen using timescale — no PlayerMovement needed
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(5f);
        Time.timeScale = 1f;
        Debug.Log("Player unfrozen!");
    }

    public void ResetStrikes()
    {
        lateStrikes = 0;
        UpdateLateUI();
    }
}