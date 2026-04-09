using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private float invincibilityDuration = 1.5f;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private GameObject gameOverPanel;

    private int currentHealth;
    private bool isInvincible = false;
    private float invincibilityTimer = 0;

    public static PlayerHealth Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    private void Update()
    {
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0)
                isInvincible = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if hit by a vehicle
        if (collision.gameObject.CompareTag("Vehicle") && !isInvincible)
        {
            TakeDamage(1);
        }
    }

    private void TakeDamage(int amount)
    {
        currentHealth -= amount;
        UpdateHealthUI();

        Debug.Log($"Player hit! Health: {currentHealth}");

        // Brief invincibility after hit
        isInvincible = true;
        invincibilityTimer = invincibilityDuration;

        if (currentHealth <= 0)
        {
            TriggerGameOver();
        }
        else
        {
            // Push player back slightly
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
                rb.AddForce(Vector3.back * 5f, ForceMode.Impulse);
        }
    }

    private void UpdateHealthUI()
    {
        if (healthText != null)
            healthText.text = $"❤️ {currentHealth}/{maxHealth}";
    }

    private void TriggerGameOver()
    {
        Time.timeScale = 0;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        Debug.Log("GAME OVER - Player hit by vehicle");
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}