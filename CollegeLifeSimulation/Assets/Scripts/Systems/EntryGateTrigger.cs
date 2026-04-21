using UnityEngine;

public class EntryGateTrigger : MonoBehaviour
{
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;
            Debug.Log("Player entered college gate");
            CollegeEntryManager.Instance?.PlayerEnteredCollege();
        }
    }
}