using UnityEngine;
using TMPro;

public class RoadCrossingSystem : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI warningText;

    [Header("Settings")]
    [SerializeField] private float timePenalty = 5f;

    private TrafficLightController trafficLight;
    private GameTimer gameTimer;
    private bool playerInRoad = false;
    private bool penaltyApplied = false;

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
        if (trafficLight == null) return;

        if (trafficLight.CurrentState == TrafficLightController.LightState.Red)
        {
            ShowWarning(
                "<size=40><color=green>[ OK ]</color></size>\n" +
                "<size=28>SAFE TO CROSS</size>\n" +
                "<size=22>Vehicles have stopped!</size>",
                Color.white
            );
            PlayerBrokeRules = false;
        }
        else if (trafficLight.CurrentState == TrafficLightController.LightState.Green)
        {
            ShowWarning(
                "<size=40><color=red>[ !! ]</color></size>\n" +
                "<size=28>DANGER - DO NOT CROSS</size>\n" +
                "<size=22>Vehicles are moving!\nWait for red light!</size>",
                Color.white
            );
            PlayerBrokeRules = true;

            if (!penaltyApplied && gameTimer != null)
            {
                gameTimer.AddPenalty(timePenalty);
                penaltyApplied = true;
                Debug.Log("Penalty! Crossed on green light!");
            }
        }
        else if (trafficLight.CurrentState == TrafficLightController.LightState.Yellow)
        {
            ShowWarning(
                "<size=40><color=yellow>[ .. ]</color></size>\n" +
                "<size=28>CAUTION</size>\n" +
                "<size=22>Light changing soon!\nGet ready!</size>",
                Color.white
            );
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRoad = true;
            penaltyApplied = false;
            Debug.Log("Player entered crossing zone");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRoad = false;
            PlayerBrokeRules = false;
            penaltyApplied = false;

            if (warningText != null)
                warningText.gameObject.SetActive(false);
        }
    }

    private void ShowWarning(string message, Color color)
    {
        if (warningText != null)
        {
            warningText.gameObject.SetActive(true);
            warningText.text = message;
            warningText.color = color;
        }
    }
}