using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using Unity.Cinemachine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private float respawnDelay = 3f;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject deathMessagePanel;
    [SerializeField] private TextMeshProUGUI deathMessageText;

    [Header("Respawn")]
    [SerializeField] private Transform respawnPoint;

    private int currentHealth;
    private bool isDead = false;
    private Animator animator;

    public static PlayerHealth Instance;

    private Camera mainCam;
    private Vector3 originalCamPos;
    private Quaternion originalCamRot;
    private CinemachineBrain cinemachineBrain;

    private void Awake()
    {
        Instance = this;
        animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
        mainCam = Camera.main;
        originalCamPos = mainCam.transform.position;
        originalCamRot = mainCam.transform.rotation;
        cinemachineBrain = mainCam.GetComponent<CinemachineBrain>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Vehicle") && !isDead)
        {
            StartCoroutine(StopVehicleTemporarily(hit.gameObject));
            TakeDamage(1);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Vehicle") && !isDead)
        {
            StartCoroutine(StopVehicleTemporarily(other.gameObject));
            TakeDamage(1);
        }
    }

    private void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        UpdateHealthUI();
        isDead = true;

        if (animator != null)
            animator.SetTrigger("Die");

        Debug.Log($"Player hit! Health: {currentHealth}");

        if (currentHealth <= 0)
            StartCoroutine(ShowMessageThenGameOver());
        else
            StartCoroutine(DieAndRespawn());
    }

    private IEnumerator DieAndRespawn()
    {
        var controller = GetComponent<CharacterController>();
        var mover = GetComponent<StarterAssets.ThirdPersonController>();

        if (controller != null) controller.enabled = false;
        if (mover != null) mover.enabled = false;

        if (cinemachineBrain != null) cinemachineBrain.enabled = false;

        originalCamPos = mainCam.transform.position;
        originalCamRot = mainCam.transform.rotation;

        // Left side slightly elevated - cinematic movie shot
        Vector3 targetPos = transform.position + (-transform.right * 4f) + (Vector3.up * 2f);
        Quaternion targetRot = Quaternion.LookRotation(transform.position - targetPos);

        // Camera smoothly moves to cinematic position
        float elapsed = 0f;
        float duration = 0.8f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            mainCam.transform.position = Vector3.Lerp(originalCamPos, targetPos, t);
            mainCam.transform.rotation = Quaternion.Lerp(originalCamRot, targetRot, t);
            yield return null;
        }

        // Slowly zoom in toward player
        elapsed = 0f;
        while (elapsed < 1.5f)
        {
            elapsed += Time.deltaTime;
            mainCam.transform.position = Vector3.Lerp(
                mainCam.transform.position,
                transform.position + (-transform.right * 3f) + (Vector3.up * 1.5f),
                Time.deltaTime * 1.5f
            );
            yield return null;
        }

        yield return new WaitForSeconds(respawnDelay - 2.3f);

        // Respawn
        if (respawnPoint != null)
            transform.position = respawnPoint.position;

        transform.rotation = Quaternion.identity;

        if (animator != null)
            animator.Rebind();

        if (cinemachineBrain != null) cinemachineBrain.enabled = true;
        if (controller != null) controller.enabled = true;
        if (mover != null) mover.enabled = true;

        isDead = false;
    }

    private IEnumerator ShowMessageThenGameOver()
    {
        var controller = GetComponent<CharacterController>();
        var mover = GetComponent<StarterAssets.ThirdPersonController>();

        if (controller != null) controller.enabled = false;
        if (mover != null) mover.enabled = false;

        if (cinemachineBrain != null) cinemachineBrain.enabled = false;

        originalCamPos = mainCam.transform.position;
        originalCamRot = mainCam.transform.rotation;

        // Left side slightly elevated - cinematic movie shot
        Vector3 targetPos = transform.position + (-transform.right * 4f) + (Vector3.up * 2f);
        Quaternion targetRot = Quaternion.LookRotation(transform.position - targetPos);

        // Camera smoothly moves to cinematic position
        float elapsed = 0f;
        float duration = 0.8f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            mainCam.transform.position = Vector3.Lerp(originalCamPos, targetPos, t);
            mainCam.transform.rotation = Quaternion.Lerp(originalCamRot, targetRot, t);
            yield return null;
        }

        // Slowly zoom in toward player
        elapsed = 0f;
        while (elapsed < 1.5f)
        {
            elapsed += Time.deltaTime;
            mainCam.transform.position = Vector3.Lerp(
                mainCam.transform.position,
                transform.position + (-transform.right * 3f) + (Vector3.up * 1.5f),
                Time.deltaTime * 1.5f
            );
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        if (deathMessagePanel != null)
        {
            deathMessagePanel.SetActive(true);
            if (deathMessageText != null)
                deathMessageText.text =
                    "You were going to college to build your future...\n" +
                    "not to destroy it.\n\n" +
                    "Don't make your parents regret raising you.\n" +
                    "Follow the traffic rules. Be safe on the road.\n\n" +
                    "They are waiting for you at home.\n" +
                    "They are proud of you.\n" +
                    "Don't break their hearts.\n\n" +
                    ". . . . . . . . . .\n\n" +
                    "Congratulations.\n\n" +
                    "You left your parents alone forever.\n" +
                    "No more college. No more future.\n" +
                    "No more coming home.\n\n" +
                    "Good Afterlife.";
        }

        yield return new WaitForSecondsRealtime(6f);

        if (deathMessagePanel != null)
            deathMessagePanel.SetActive(false);

        TriggerGameOver();
    }

    private IEnumerator StopVehicleTemporarily(GameObject vehicle)
    {
        var rb = vehicle.GetComponent<Rigidbody>();
        MonoBehaviour[] scripts = vehicle.GetComponents<MonoBehaviour>();

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        foreach (var script in scripts)
            script.enabled = false;

        yield return new WaitForSeconds(4f);

        foreach (var script in scripts)
            script.enabled = true;
    }

    private void UpdateHealthUI()
    {
        if (healthText != null)
            healthText.text = $"Health: {currentHealth}/{maxHealth}";
    }

    private void TriggerGameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        Time.timeScale = 0f;
        Debug.Log("GAME OVER");
    }

    public void RestartGame()
    {
        Debug.Log("Restarting...");
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}