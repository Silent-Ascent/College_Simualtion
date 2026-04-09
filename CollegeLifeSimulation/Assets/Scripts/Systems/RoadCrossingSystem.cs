using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoadCrossingSystem : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI warningText;
    [SerializeField] private TextMeshProUGUI statusText;

    [Header("Settings")]
    [SerializeField] private float timePenalty = 5f;

    private TrafficLightController trafficLight;
    private GameTimer gameTimer;
    private bool playerInRoad = false;

    public static bool PlayerBrokeRules { get; private set; }

    private void Start()
    {
        trafficLight = TrafficLightController.Instance;
        gameTimer = GameTimer.Instance;

        if (warningText != null)
            warningText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!playerInRoad) return;

        // Check if player crossed on red
        if (trafficLight != null &&
            trafficLight.CurrentState == TrafficLightController.LightState.Red)
        {
            ShowWarning("⚠️ RED LIGHT! DANGEROUS!");
            PlayerBrokeRules = true;
        }
        else if (trafficLight != null &&
                 trafficLight.CurrentState == TrafficLightController.LightState.Green)
        {
            ShowWarning("✅ Safe to cross");
            PlayerBrokeRules = false;
        }
        else
        {
            ShowWarning("🟡 Caution - Yellow Light");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRoad = true;
            Debug.Log("Player entered road");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRoad = false;

            if (warningText != null)
                warningText.gameObject.SetActive(false);

            // Apply time penalty if rule was broken
            if (PlayerBrokeRules && gameTimer != null)
            {
                gameTimer.AddPenalty(timePenalty);
                Debug.Log($"Time penalty applied: {timePenalty}s");
                PlayerBrokeRules = false;
            }
        }
    }

    private void ShowWarning(string message)
    {
        if (warningText != null)
        {
            warningText.gameObject.SetActive(true);
            warningText.text = message;
        }
    }
}