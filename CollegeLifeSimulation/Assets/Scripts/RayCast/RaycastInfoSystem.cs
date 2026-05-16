using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RaycastInfoSystem : MonoBehaviour
{
    [Header("Raycast")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float raycastRange = 10f;
    [SerializeField] private LayerMask infoLayer;

    [Header("UI")]
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private Image objectImage;

    [Header("Keys")]
    [SerializeField] private KeyCode openKey = KeyCode.O;

    private InfoData currentInfo;
    private bool panelOpen;

    private void Start()
    {
        if (infoPanel != null)
            infoPanel.SetActive(false);

        if (promptText != null)
            promptText.gameObject.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // Don't raycast while panel is open
        if (!panelOpen)
        {
            CheckRaycast();
        }

        // Open panel
        if (Input.GetKeyDown(openKey))
        {
            if (currentInfo != null && !panelOpen)
            {
                OpenPanel();
            }
        }
    }

    private void CheckRaycast()
    {
        if (playerCamera == null)
            return;

        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        RaycastHit hit;

        Debug.DrawRay(
            playerCamera.transform.position,
            playerCamera.transform.forward * raycastRange,
            Color.red
        );

        if (Physics.Raycast(ray, out hit, raycastRange, infoLayer))
        {
            Debug.Log("Hit: " + hit.collider.name);

            InfoData info = hit.collider.GetComponentInParent<InfoData>();

            if (info != null)
            {
                currentInfo = info;

                if (promptText != null)
                {
                    promptText.gameObject.SetActive(true);
                    promptText.text = "[O] Learn about: " + info.Title;
                }

                return;
            }
        }

        currentInfo = null;

        if (promptText != null)
        {
            promptText.gameObject.SetActive(false);
        }
    }

    private void OpenPanel()
    {
        panelOpen = true;

        if (titleText != null)
            titleText.text = currentInfo.Title;

        if (descriptionText != null)
            descriptionText.text = currentInfo.Description;

        if (objectImage != null)
            objectImage.sprite = currentInfo.ObjectSprite;

        if (infoPanel != null)
            infoPanel.SetActive(true);

        if (promptText != null)
            promptText.gameObject.SetActive(false);

        // Unlock cursor for UI interaction
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("Panel Opened");
    }

    public void ClosePanel()
    {
        panelOpen = false;

        if (infoPanel != null)
            infoPanel.SetActive(false);

        // Lock cursor back to game
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("Panel Closed");
    }
}