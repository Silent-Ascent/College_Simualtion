using UnityEngine;
using TMPro;
using System.Collections;

public class SSDOfficeTrigger : MonoBehaviour
{
    [Header("NPC Reference")]
    [SerializeField] private GameObject ssdOfficerNPC;

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI speakerNameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI promptText;

    [Header("SSD Status Panel")]
    [SerializeField] private GameObject ssdPanel;
    [SerializeField] private TextMeshProUGUI ssdStatusText;

    [Header("Classroom")]
    [SerializeField] private GameObject classroomTrigger;

    [Header("Settings")]
    [SerializeField] private float permissionWaitTime = 5f;

    private string[] dialogueLines = new string[]
    {
        "You are late! Do you know what time it is?",
        "You need written permission to enter class.",
        "Wait here while I fill in the late slip...",
        "Alright. Here is your late slip. Do not let it happen again!"
    };

    private int currentLine = 0;
    private bool playerInRange = false;
    private bool dialogueActive = false;
    private bool permissionGranted = false;
    private bool hasVisited = false;

    private void Start()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (ssdPanel != null)
            ssdPanel.SetActive(false);

        if (promptText != null)
            promptText.gameObject.SetActive(false);
    }

    private void Update()
    {
        // Show Press E prompt when player is nearby but not yet talking
        if (playerInRange && !dialogueActive && !permissionGranted)
        {
            if (promptText != null)
            {
                promptText.gameObject.SetActive(true);
                promptText.text = "Press E to speak to SSD Officer";
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                StartDialogue();
            }
        }

        // Advance dialogue with E
        if (dialogueActive && Input.GetKeyDown(KeyCode.E))
        {
            NextLine();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasVisited)
        {
            playerInRange = true;

            // Show SSD panel when player enters the zone
            if (ssdPanel != null)
                ssdPanel.SetActive(true);

            if (ssdStatusText != null)
                ssdStatusText.text = "Speak to the SSD Officer to get permission.";

            // Screen flash yellow on entering SSD
            ScreenFlash.Instance?.Flash(new Color(1f, 0.8f, 0f), 0.3f, 1f);

            // Show on-screen toast
            StatusToast.Instance?.Show("Speak to the SSD Officer.", Color.yellow);

            Debug.Log("Player entered SSD office zone");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (promptText != null)
                promptText.gameObject.SetActive(false);

            // If player leaves mid dialogue without permission
            if (dialogueActive && !permissionGranted)
            {
                EndDialogue();

                StatusToast.Instance?.Show(
                    "Come back and speak to the officer!", 
                    Color.red
                );

                ScreenFlash.Instance?.Flash(Color.red, 0.3f, 1f);

                Debug.Log("Player left SSD office without permission");
            }
        }
    }

    private void StartDialogue()
    {
        dialogueActive = true;
        currentLine = 0;

        // Hide the SSD status panel, show dialogue panel instead
        if (ssdPanel != null)
            ssdPanel.SetActive(false);

        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        if (speakerNameText != null)
            speakerNameText.text = "SSD Officer";

        // Make NPC face the player
        if (ssdOfficerNPC != null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                Vector3 dir = player.transform.position - ssdOfficerNPC.transform.position;
                dir.y = 0;
                if (dir != Vector3.zero)
                    ssdOfficerNPC.transform.rotation = Quaternion.LookRotation(dir);
            }
        }

        ShowLine(currentLine);

        Debug.Log("SSD dialogue started");
    }

    private void NextLine()
    {
        currentLine++;

        if (currentLine < dialogueLines.Length)
        {
            ShowLine(currentLine);
        }
        else
        {
            // All lines done — grant permission
            GrantPermission();
        }
    }

    private void ShowLine(int index)
    {
        if (dialogueText != null)
            dialogueText.text = dialogueLines[index];

        bool isLastLine = (index == dialogueLines.Length - 1);

        if (promptText != null)
        {
            promptText.gameObject.SetActive(true);
            promptText.text = isLastLine ? "Press E to receive slip" : "Press E to continue";
        }

        Debug.Log("SSD Officer: " + dialogueLines[index]);
    }

    private void GrantPermission()
    {
        permissionGranted = true;
        hasVisited = true;
        dialogueActive = false;

        // Show permission granted message in dialogue box
        if (dialogueText != null)
            dialogueText.text = "Permission granted! Go to your classroom now.";

        if (speakerNameText != null)
            speakerNameText.text = "SSD Officer";

        if (promptText != null)
            promptText.gameObject.SetActive(false);

        // Enable classroom trigger
        if (classroomTrigger != null)
            classroomTrigger.SetActive(true);

        // Screen flash green
        ScreenFlash.Instance?.Flash(Color.green, 0.4f, 1.2f);

        // On-screen toast
        StatusToast.Instance?.Show(
            "Permission granted! Go to your classroom.", 
            Color.green, 
            3f
        );

        // Close dialogue panel after 3 seconds
        Invoke(nameof(EndDialogue), 3f);

        Debug.Log("SSD permission granted — classroom trigger activated");
    }

    private void EndDialogue()
    {
        dialogueActive = false;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (promptText != null)
            promptText.gameObject.SetActive(false);
    }
}