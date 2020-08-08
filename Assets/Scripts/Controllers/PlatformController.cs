using UnityEngine;
using System.Collections.Generic;

public class PlatformController : RaycastController
{
    public Vector3[] localWaypoints;
    private Vector3[] globalWaypoints;

    private Stack<PassengerMovement> passengerMovements;
    private Dictionary<Transform, CharacterController2D> passengerDictionary = new Dictionary<Transform, CharacterController2D>();

    [SerializeField]
    private bool isRotatingCyclic = false;
    [SerializeField]
    private float waitTime = 0f;
    [SerializeField]
    [Range(0, 2)]
    private float easeAmount = 0f;
    [SerializeField]
    private float movementSpeed = 1f;
    private float nextMoveTime;
    private float percentBetweenWaypoints;
    private int fromWaypointIndex;

    protected override void Awake()
    {
        collisionLayerName = "Player";

        globalWaypoints = new Vector3[localWaypoints.Length];
        for (int i = 0; i < localWaypoints.Length; i++)
        {
            globalWaypoints[i] = localWaypoints[i] + transform.position;
        }

        base.Awake();
    }

    private void Update()
    {
        UpdateRaycastOrigins();

        Vector2 velocity = CalculatePlatformMovement();

        CalculatePassengerMovement(velocity);

        MovePassengers(true);
        transform.Translate(velocity);
        MovePassengers(false);
    }

    private float Ease(float x)
    {
        float a = easeAmount + 1;
        return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
    }

    private Vector3 CalculatePlatformMovement()
    {
        if(Time.time < nextMoveTime)
        {
            return Vector3.zero;
        }

        fromWaypointIndex %= globalWaypoints.Length;

        int toWaypointIndex = (fromWaypointIndex + 1) % globalWaypoints.Length;
        float distanceBetweenWaypoints = Vector3.Distance(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex]);
        percentBetweenWaypoints += Time.deltaTime * movementSpeed / distanceBetweenWaypoints;
        percentBetweenWaypoints = Mathf.Clamp01(percentBetweenWaypoints);
        float easedPercentBetweenWaypoints = Ease(percentBetweenWaypoints); 

        Vector3 newPosition = Vector3.Lerp(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex], easedPercentBetweenWaypoints);

        if(percentBetweenWaypoints >= 1)
        {
            percentBetweenWaypoints = 0;
            fromWaypointIndex++;

            if (!isRotatingCyclic)
            {
                if (fromWaypointIndex >= globalWaypoints.Length - 1)
                {
                    fromWaypointIndex = 0;
                    System.Array.Reverse(globalWaypoints);
                }
            }

            nextMoveTime = Time.time + waitTime;
        }

        return newPosition - transform.position;
    }

    private void MovePassengers(bool isMovedBeforePlatform)
    {
        foreach (var passenger in passengerMovements)
        {
            if (!passengerDictionary.ContainsKey(passenger.Transform))
            {
                passengerDictionary.Add(passenger.Transform, passenger.Transform.GetComponent<CharacterController2D>());
            }

            if(passenger.IsMovedBeforePlatform == isMovedBeforePlatform)
            {
                passengerDictionary[passenger.Transform].Move(passenger.Velocity, passenger.IsStandingOnPlatform);
            }
        }
    }

    private void CalculatePassengerMovement(Vector2 velocity)
    {
        HashSet<Transform> movedPassengers = new HashSet<Transform>();
        passengerMovements = new Stack<PassengerMovement>();

        float directionX = Mathf.Sign(velocity.x);
        float directionY = Mathf.Sign(velocity.y);

        // Vertical moving platform
        if (velocity.y != 0)
        {
            float rayLength = Mathf.Abs(velocity.y) + SKIN_WIDTH;

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.BottomLeft : raycastOrigins.TopLeft;
                rayOrigin += Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

                if (hit && hit.distance != 0)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);

                        float pushX = (directionY == 1) ? velocity.x : 0;
                        float pushY = velocity.y - (hit.distance - SKIN_WIDTH) * directionY;

                        passengerMovements.Push(new PassengerMovement(hit.transform,
                          new Vector2(pushX, pushY),
                          directionX == 1,
                          true));
                    }                  
                }
            }
        }

        // Horizontal moving platform
        if(velocity.x != 0)
        {
            float rayLength = Mathf.Abs(velocity.x) + SKIN_WIDTH;

            for (int i = 0; i < horizontalRayCount; i++)
            {
                Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.BottomLeft : raycastOrigins.BottomRight;
                rayOrigin += Vector2.up * (horizontalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

                if (hit && hit.distance != 0)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);

                        float pushX = velocity.x - (hit.distance - SKIN_WIDTH) * directionX;
                        float pushY = -SKIN_WIDTH;

                        passengerMovements.Push(new PassengerMovement(hit.transform,
                           new Vector2(pushX, pushY),
                           false,
                           true));
                    }
                }
            }
        }

        // Passenger on top a horizontally or downward moving platform
        if (directionY == -1 || velocity.y == 0f && velocity.x != 0f)
        {
            float rayLength = SKIN_WIDTH * 2;

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = raycastOrigins.TopLeft + Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, collisionMask);

                if (hit && hit.distance != 0)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);

                        float pushX = velocity.x;
                        float pushY = velocity.y;

                        passengerMovements.Push(new PassengerMovement(hit.transform,
                            new Vector2(pushX, pushY),
                            true,
                            false));
                    }
                }
            }
        }
    }

    private struct PassengerMovement
    {
        private Transform transform;
        private Vector2 velocity;
        private bool isStandingOnPlatform;
        private bool isMovedBeforePlatform;

        public Transform Transform { get { return transform; } }
        public Vector2 Velocity { get { return velocity; } }
        public bool IsStandingOnPlatform { get { return isStandingOnPlatform; } }
        public bool IsMovedBeforePlatform { get { return isMovedBeforePlatform; } }

        public PassengerMovement(Transform transform, Vector2 velocity, bool isStandingOnPlatform, bool isMovedBeforePlatform)
        {
            this.transform = transform;
            this.velocity = velocity;
            this.isStandingOnPlatform = isStandingOnPlatform;
            this.isMovedBeforePlatform = isMovedBeforePlatform;
        }
    }

    private void OnDrawGizmos()
    {
        if (localWaypoints != null)
        {
            Gizmos.color = Color.red;
            float size = 0.3f;

            for (int i = 0; i < localWaypoints.Length; i++)
            {
                Vector3 globalWaypointPosition = (Application.isPlaying) ? globalWaypoints[i] : localWaypoints[i] + transform.position;
                Gizmos.DrawLine(globalWaypointPosition - Vector3.up * size, globalWaypointPosition + Vector3.up * size);
                Gizmos.DrawLine(globalWaypointPosition - Vector3.left * size, globalWaypointPosition + Vector3.left * size);
            }
        }
    }
}
