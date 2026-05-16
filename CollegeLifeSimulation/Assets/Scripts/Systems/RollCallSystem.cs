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

    public enum RollCallState
    {
        WaitingForStudent,
        WaitingForScan,
        ScanComplete
    }

    public RollCallState State { get; private set; } =
        RollCallState.WaitingForStudent;

    private bool scanComplete = false;

    // Player references
    private GameObject player;
    private Animator playerAnimator;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // Hide UI at start
        if (rollCallPanel != null)
            rollCallPanel.SetActive(false);

        if (scanPromptText != null)
            scanPromptText.gameObject.SetActive(false);

        // Find player
        player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerAnimator = player.GetComponent<Animator>();
        }
    }

    // Called after player sits
    public void PlayerSeated()
    {
        Debug.Log("Player seated. Roll call starting soon...");
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

        // Show UI
        if (rollCallPanel != null)
            rollCallPanel.SetActive(true);

        if (rollCallStatusText != null)
        {
            rollCallStatusText.text =
                "ROLL CALL - Please scan your fingerprint";
        }

        // Make player stand up
        if (PlayerSitDown.Instance != null)
            PlayerSitDown.Instance.StandUp();

        if (scanPromptText != null)
        {
            scanPromptText.gameObject.SetActive(true);
            scanPromptText.text =
                "Walk to the biometric scanner";
        }

        Debug.Log("Player must now scan fingerprint");
    }

    private void Update()
    {
        if (State != RollCallState.WaitingForScan)
            return;

        if (scanComplete)
            return;

        if (biometricScannerObject == null)
            return;

        if (player == null)
            return;

        // Distance check
        float distance = Vector3.Distance(
            player.transform.position,
            biometricScannerObject.transform.position
        );

        if (distance < scanRange)
        {
            if (scanPromptText != null)
            {
                scanPromptText.text =
                    "Press [E] to scan fingerprint";
            }

            // Scan input
            if (Input.GetKeyDown(KeyCode.E))
            {
                StartCoroutine(PerformScan());
            }
        }
        else
        {
            if (scanPromptText != null)
            {
                scanPromptText.text =
                    "Walk to the biometric scanner";
            }
        }
    }

    private IEnumerator PerformScan()
    {
        scanComplete = true;

        State = RollCallState.ScanComplete;

        // PLAY FINGERPRINT ANIMATION
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("ScanFingerprint");
        }

        // Hide prompt
        if (scanPromptText != null)
            scanPromptText.gameObject.SetActive(false);

        // Scanner green flash
        if (biometricScannerObject != null)
        {
            Renderer scannerRenderer =
                biometricScannerObject
                .transform.Find("Scanner_Sensor")
                ?.GetComponent<Renderer>();

            if (scannerRenderer != null)
            {
                Color originalColor =
                    scannerRenderer.material.color;

                scannerRenderer.material.color = Color.green;

                yield return new WaitForSeconds(0.5f);

                scannerRenderer.material.color = originalColor;
            }
        }

        // Success UI
        if (rollCallStatusText != null)
        {
            rollCallStatusText.text =
                "Attendance Recorded Successfully!";
        }

        // Optional effect
        if (scanSuccessEffect != null)
        {
            scanSuccessEffect.SetActive(true);
        }

        yield return new WaitForSeconds(2f);

        // Hide roll call UI
        if (rollCallPanel != null)
            rollCallPanel.SetActive(false);


        Debug.Log("Fingerprint scanned successfully");
    }
}