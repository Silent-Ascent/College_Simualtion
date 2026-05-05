using UnityEngine;
using TMPro;

public class PlayerSitDown : MonoBehaviour
{
    [Header("Sit Settings")]
    [SerializeField] private Vector3 seatPosition;
    [SerializeField] private Vector3 seatRotation = new Vector3(0, 0, 0);
    [SerializeField] private TextMeshProUGUI promptText;

    public static bool IsSeated { get; private set; } = false;
    public static PlayerSitDown Instance;

    private bool playerNearSeat = false;
    private Transform playerTransform;

    private StarterAssets.ThirdPersonController playerController;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        playerController = FindObjectOfType<StarterAssets.ThirdPersonController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearSeat = true;
            playerTransform = other.transform;

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

        if (Input.GetKeyDown(KeyCode.E))
        {
            SitDown();
        }
    }

    private void SitDown()
    {
        if (playerTransform == null) return;

        IsSeated = true;

        playerTransform.position = seatPosition;
        playerTransform.rotation = Quaternion.Euler(seatRotation);

        // ✅ Disable movement (Starter Assets)
        if (playerController != null)
            playerController.enabled = false;

        if (promptText != null)
            promptText.gameObject.SetActive(false);

        Debug.Log("Player sat down");

        RollCallSystem.Instance?.PlayerSeated();
    }

    public void StandUp()
    {
        IsSeated = false;

        // ✅ Enable movement again
        if (playerController != null)
            playerController.enabled = true;

        Debug.Log("Player stood up");
    }
}