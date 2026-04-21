using UnityEngine;

public class PhaseTransitionTrigger : MonoBehaviour
{
    [SerializeField] private GameObject phase2Objects;
    [SerializeField] private AudioSource phase2AmbientSound;

    private bool triggered = false;

    private void Start()
    {
        // Phase 2 objects hidden at start
        if (phase2Objects != null)
            phase2Objects.SetActive(true); // or false if you want to load them only on enter
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;
            Debug.Log("Player reached college side — Phase 2 active");

            if (phase2AmbientSound != null)
                phase2AmbientSound.Play();
        }
    }
}
