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

    [Header("Traffic Light Stop Settings")]
    [SerializeField] private Transform stopPoint;   // assign near signal
    [SerializeField] private float stopDistance = 10f;

    private float speed;
    private bool isMoving = true;
    private TrafficLightController trafficLight;

    private void Start()
    {
        speed = Random.Range(minSpeed, maxSpeed);
        trafficLight = TrafficLightController.Instance;

        if (startWaypoint != null)
            transform.position = startWaypoint.position;

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

        // 🔥 FIXED LOGIC (stop only near signal)
        if (trafficLight != null && stopPoint != null)
        {
            bool lightIsRed = trafficLight.CurrentState == TrafficLightController.LightState.Red;
            float distanceToStop = Vector3.Distance(transform.position, stopPoint.position);

            if (lightIsRed && distanceToStop < stopDistance && !breaksTrafficRules)
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

        // 🔍 Show stop distance in Scene
        if (stopPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(stopPoint.position, stopDistance);
        }
    }
}