using UnityEngine;

public class VehicleController : MonoBehaviour
{
    [Header("Waypoints")]
    [SerializeField] private Transform startWaypoint;
    [SerializeField] private Transform endWaypoint;

    [Header("Speed Settings")]
    [SerializeField] private float minSpeed = 8f;
    [SerializeField] private float maxSpeed = 18f;

    [Header("Behaviour")]
    [SerializeField] private bool breaksTrafficRules = false;

    private float speed;
    private bool isMoving = true;
    private TrafficLightController trafficLight;

    private void Start()
    {
        // Randomise speed for each vehicle
        speed = Random.Range(minSpeed, maxSpeed);

        // Find the traffic light in the scene
        trafficLight = TrafficLightController.Instance;

        // Place vehicle at start waypoint
        if (startWaypoint != null)
            transform.position = startWaypoint.position;

        // Face toward end waypoint
        if (endWaypoint != null)
        {
            Vector3 dir = endWaypoint.position - transform.position;
            dir.y = 0;
            if (dir != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(dir);
        }
    }

    private void Update()
    {
        if (endWaypoint == null) return;

        // Check traffic light
        if (trafficLight != null)
        {
            bool lightIsRed = trafficLight.CurrentState == TrafficLightController.LightState.Red;

            // Stop for red unless this vehicle breaks rules
            if (lightIsRed && !breaksTrafficRules)
            {
                isMoving = false;
            }
            else
            {
                isMoving = true;
            }
        }

        if (!isMoving) return;

        // Move toward end waypoint
        Vector3 direction = endWaypoint.position - transform.position;
        direction.y = 0;
        float distance = direction.magnitude;

        if (distance > 1f)
        {
            transform.position += direction.normalized * speed * Time.deltaTime;
        }
        else
        {
            // Reached end — teleport back to start (loop)
            transform.position = startWaypoint.position;
        }
    }

    private void OnDrawGizmos()
    {
        if (startWaypoint != null && endWaypoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(startWaypoint.position, endWaypoint.position);
        }
    }
}