using UnityEngine;
using TMPro;
using StarterAssets;

public class PlayerSitDown : MonoBehaviour
{
    [Header("Sit Settings")]
    [SerializeField] private Vector3 seatPosition = new Vector3(32f, 1.2f, 84.6f);
    [SerializeField] private Vector3 seatRotation = new Vector3(0f, 0f, 0f);
    [SerializeField] private TextMeshProUGUI promptText;

    public static bool IsSeated { get; private set; } = false;
    public static PlayerSitDown Instance;

    private bool playerNearSeat = false;
    private bool hasAlreadySat = false;        // ← FIX 1: stops repeat sitting
    private Transform playerTransform;
    private ThirdPersonController playerController;
    private CharacterController characterController; // ← FIX 2: stops road teleport
    private Animator playerAnimator;               // ← FIX 3: animation

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        playerController = FindObjectOfType<ThirdPersonController>();
        characterController = FindObjectOfType<CharacterController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasAlreadySat)
        {
            playerNearSeat = true;
            playerTransform = other.transform;
            playerAnimator = other.GetComponent<Animator>(); // get animator

            if (promptText != null)
            {
                promptText.gameObject.SetActive(true);
                promptText.text = "Press [E] to sit down";
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearSeat = false;
            if (promptText != null)
                promptText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!playerNearSeat) return;
        if (IsSeated) return;
        if (hasAlreadySat) return;

        if (Input.GetKeyDown(KeyCode.E))
            SitDown();
    }

    private void SitDown()
    {
        if (playerTransform == null) return;

        hasAlreadySat = true;
        IsSeated = true;

        // FIX: disable CharacterController BEFORE moving position
        // otherwise it fights the position change and sends player to 0,0,0
        if (characterController != null)
            characterController.enabled = false;

        playerTransform.position = seatPosition;
        playerTransform.rotation = Quaternion.Euler(seatRotation);

        if (characterController != null)
            characterController.enabled = true;

        // Disable movement
        if (playerController != null)
            playerController.enabled = false;

        // Play sitting animation
        if (playerAnimator != null)
            playerAnimator.SetBool("IsSitting", true);

        if (promptText != null)
            promptText.gameObject.SetActive(false);

        Debug.Log("Player sat at: " + seatPosition);
        RollCallSystem.Instance?.PlayerSeated();
    }

    public void StandUp()
    {
        IsSeated = false;

        if (playerController != null)
            playerController.enabled = true;

        // Return to idle animation
        if (playerAnimator != null)
            playerAnimator.SetBool("IsSitting", false);

        Debug.Log("Player stood up");
    }
}