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
    [SerializeField] private KeyCode closeKey = KeyCode.C;

    private InfoData currentInfo;
    private bool panelOpen;

    private void Start()
    {
        infoPanel.SetActive(false);

        if (promptText != null)
            promptText.gameObject.SetActive(false);
    }

    private void Update()
    {
        CheckRaycast();

        if (Input.GetKeyDown(openKey))
        {
            if (currentInfo != null && !panelOpen)
            {
                OpenPanel();
            }
        }

        if (Input.GetKeyDown(closeKey))
        {
            if (panelOpen)
            {
                ClosePanel();
            }
        }
    }

    private void CheckRaycast()
    {
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

                promptText.gameObject.SetActive(true);
                promptText.text = "[O] Learn about: " + info.Title;

                return;
            }
        }

        currentInfo = null;
        promptText.gameObject.SetActive(false);
    }

    private void OpenPanel()
    {
        panelOpen = true;

        titleText.text = currentInfo.Title;
        descriptionText.text = currentInfo.Description;

        if (objectImage != null)
            objectImage.sprite = currentInfo.ObjectSprite;

        infoPanel.SetActive(true);

        promptText.gameObject.SetActive(false);
    }

    private void ClosePanel()
    {
        panelOpen = false;
        infoPanel.SetActive(false);
    }
}