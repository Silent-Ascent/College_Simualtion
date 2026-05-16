using UnityEngine;

public class ClassroomEntryTrigger : MonoBehaviour
{
    private bool entered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (entered) return;

        if (other.CompareTag("Player"))
        {
            entered = true;
            Debug.Log("Player entered classroom!");


            // Stop timer
            GameTimer.Instance?.StopTimer();

            // Reset late strikes (new day)
            LateCounter.Instance?.ResetStrikes();
        }
    }
}
