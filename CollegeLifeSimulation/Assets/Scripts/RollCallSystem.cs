using UnityEngine;
using TMPro;
using System.Collections;

public class RollCallSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject biometricScannerObject;
    [SerializeField] private PlayerSitDown playerSeat;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI rollCallStatusText;
    [SerializeField] private GameObject rollCallPanel;
    [SerializeField] private TextMeshProUGUI scanPromptText;
    [SerializeField] private GameObject scanSuccessEffect;

    [Header("Settings")]
    [SerializeField] private float rollCallDelay = 5f;
    [SerializeField] private float scanRange = 2f;

    public static RollCallSystem Instance;

    public enum RollCallState { WaitingForStudent, WaitingForScan, ScanComplete }
    public RollCallState State { get; private set; } = RollCallState.WaitingForStudent;

    private bool scanComplete = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (rollCallPanel != null)
            rollCallPanel.SetActive(false);

        if (scanPromptText != null)
            scanPromptText.gameObject.SetActive(false);
    }

    // Called when player sits down
    public void PlayerSeated()
    {
        Debug.Log("Player seated — roll call will begin shortly");
        StartCoroutine(StartRollCallAfterDelay());
    }

    private IEnumerator StartRollCallAfterDelay()
    {
        yield return new WaitForSeconds(rollCallDelay);
        BeginRollCall();
    }

    private void BeginRollCall()
    {
        State = RollCallState.WaitingForScan;

        if (rollCallPanel != null)
            rollCallPanel.SetActive(true);

        if (rollCallStatusText != null)
            rollCallStatusText.text = "ROLL CALL — Please scan your attendance at the door!";

        // Re-enable player movement so they can walk to scanner
        if (PlayerSitDown.Instance != null)
            PlayerSitDown.Instance.StandUp();

        if (scanPromptText != null)
        {
            scanPromptText.gameObject.SetActive(true);
            scanPromptText.text = "Walk to the biometric scanner by the door";
        }

        Debug.Log("Roll call started — player must go scan");
    }

    private void Update()
    {
        if (State != RollCallState.WaitingForScan) return;
        if (scanComplete) return;
        if (biometricScannerObject == null) return;

        // Check if player is close enough to scanner
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        float dist = Vector3.Distance(player.transform.position,
            biometricScannerObject.transform.position);

        if (dist < scanRange)
        {
            if (scanPromptText != null)
                scanPromptText.text = "Press [E] to scan fingerprint";

            if (Input.GetKeyDown(KeyCode.E))
            {
                StartCoroutine(PerformScan());
            }
        }
        else
        {
            if (scanPromptText != null)
                scanPromptText.text = "Walk to the biometric scanner by the door";
        }
    }

    private IEnumerator PerformScan()
    {
        scanComplete = true;
        State = RollCallState.ScanComplete;

        if (scanPromptText != null)
            scanPromptText.gameObject.SetActive(false);

        // Green flash on scanner
        if (biometricScannerObject != null)
        {
            Renderer scannerRenderer = biometricScannerObject
                .transform.Find("Scanner_Sensor")?.GetComponent<Renderer>();

            if (scannerRenderer != null)
            {
                Color original = scannerRenderer.material.color;
                scannerRenderer.material.color = Color.green;
                yield return new WaitForSeconds(0.5f);
                scannerRenderer.material.color = original;
            }
        }

        if (rollCallStatusText != null)
            rollCallStatusText.text = "Attendance recorded!";

        yield return new WaitForSeconds(2f);

        if (rollCallPanel != null)
            rollCallPanel.SetActive(false);

        // Trigger teacher arrival event
        TeacherEventTrigger teacherEvent = FindObjectOfType<TeacherEventTrigger>();
        teacherEvent?.SendMessage("TriggerTeacherArrival",
            SendMessageOptions.DontRequireReceiver);

        Debug.Log("Biometric scan complete — attendance marked!");
    }
}
