using UnityEngine;

public class TrafficLightController : MonoBehaviour
{
    [Header("Light Objects")]
    [SerializeField] private GameObject redLight;
    [SerializeField] private GameObject yellowLight;
    [SerializeField] private GameObject greenLight;

    [Header("Light Materials")]
    [SerializeField] private Material matRedOn;
    [SerializeField] private Material matRedOff;
    [SerializeField] private Material matYellowOn;
    [SerializeField] private Material matYellowOff;
    [SerializeField] private Material matGreenOn;
    [SerializeField] private Material matGreenOff;

    [Header("Timing Settings")]
    [SerializeField] private float greenDuration = 8f;
    [SerializeField] private float yellowDuration = 2f;
    [SerializeField] private float redDuration = 8f;

    // Public state — vehicles and player will read this
    public enum LightState { Red, Yellow, Green }
    public LightState CurrentState { get; private set; }

    // Static reference so vehicles can find it easily
    public static TrafficLightController Instance;

    private float timer;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SetState(LightState.Green);
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            // Cycle through states
            if (CurrentState == LightState.Green)
                SetState(LightState.Yellow);
            else if (CurrentState == LightState.Yellow)
                SetState(LightState.Red);
            else if (CurrentState == LightState.Red)
                SetState(LightState.Green);
        }
    }

    private void SetState(LightState newState)
    {
        CurrentState = newState;

        // Turn all lights off first
        SetLightMaterial(redLight, matRedOff);
        SetLightMaterial(yellowLight, matYellowOff);
        SetLightMaterial(greenLight, matGreenOff);

        // Turn correct light on and set timer
        switch (newState)
        {
            case LightState.Red:
                SetLightMaterial(redLight, matRedOn);
                timer = redDuration;
                break;
            case LightState.Yellow:
                SetLightMaterial(yellowLight, matYellowOn);
                timer = yellowDuration;
                break;
            case LightState.Green:
                SetLightMaterial(greenLight, matGreenOn);
                timer = greenDuration;
                break;
        }

        Debug.Log($"Traffic Light: {newState}");
    }

    private void SetLightMaterial(GameObject lightObj, Material mat)
    {
        if (lightObj != null)
        {
            Renderer rend = lightObj.GetComponentInChildren<Renderer>();
            if (rend != null)
                rend.material = mat;
        }
    }
}