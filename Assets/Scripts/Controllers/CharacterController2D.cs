using UnityEngine;

public class CharacterController2D : RaycastController
{
    public CollisionInfo Collisions;
    public Vector2 PlayerInput { get; private set; }

    private float maxSlopeAngle = 50f;

    protected override void Awake()
    {
        collisionLayerName = "Obstacle";
        oneWayCollisionTag = "OneWayObject";
        Collisions.FaceDirection = 1;
        base.Awake();
    }

    public void Move(Vector2 moveAmount, bool standingOnPlatform)
    {
        Move(moveAmount, Vector2.zero, standingOnPlatform);
    }

    public void Move(Vector2 moveAmount, Vector2 input, bool standingOnPlatform = false)
    {
        UpdateRaycastOrigins();

        Collisions.Reset();
        Collisions.MoveAmountOld = moveAmount;
        PlayerInput = input;

        if (moveAmount.y < 0)
        {
            DescendSlope(ref moveAmount);
        }

        if (moveAmount.x != 0)
        {
            Collisions.FaceDirection = (int)Mathf.Sign(moveAmount.x);
        }

        HorizontalCollisions(ref moveAmount);

        if (moveAmount.y != 0)
        {
            VerticalCollisions(ref moveAmount);
        }

        transform.Translate(moveAmount);

        if (standingOnPlatform)
        {
            Collisions.Below = true;
        }
    }

    private void HorizontalCollisions(ref Vector2 moveAmount)
    {
        float directionX = Collisions.FaceDirection;
        float rayLength = Mathf.Abs(moveAmount.x) + SKIN_WIDTH;

        if (Mathf.Abs(moveAmount.x) < SKIN_WIDTH)
        {
            rayLength = 2 * SKIN_WIDTH;
        }

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.BottomLeft : raycastOrigins.BottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX, Color.red);

            if (hit)
            {
                if (hit.distance == 0)
                {
                    continue;
                }

                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                if (i == 0 && slopeAngle <= maxSlopeAngle)
                {
                    if (Collisions.DescendingSlope)
                    {
                        Collisions.DescendingSlope = false;
                        moveAmount = Collisions.MoveAmountOld;
                    }
                    float distanceToSlopeStart = 0;
                    if (slopeAngle != Collisions.SlopeAngleOld)
                    {
                        distanceToSlopeStart = hit.distance - SKIN_WIDTH;
                        moveAmount.x -= distanceToSlopeStart * directionX;
                    }
                    ClimbSlope(ref moveAmount, slopeAngle, hit.normal);
                    moveAmount.x += distanceToSlopeStart * directionX;
                }

                if (!Collisions.ClimbingSlope || slopeAngle > maxSlopeAngle)
                {
                    moveAmount.x = (hit.distance - SKIN_WIDTH) * directionX;
                    rayLength = hit.distance;

                    if (Collisions.ClimbingSlope)
                    {
                        moveAmount.y = Mathf.Tan(Collisions.SlopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x);
                    }

                    Collisions.Left = directionX == -1;
                    Collisions.Right = directionX == 1;
                }
            }
        }
    }

    private void VerticalCollisions(ref Vector2 moveAmount)
    {
        float directionY = Mathf.Sign(moveAmount.y);
        float rayLength = Mathf.Abs(moveAmount.y) + SKIN_WIDTH;

        for (int i = 0; i < verticalRayCount; i++)
        {

            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.BottomLeft : raycastOrigins.TopLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + moveAmount.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.red);

            if (hit)
            {
                if (hit.collider.CompareTag(oneWayCollisionTag))
                {
                    if (directionY == 1 || hit.distance == 0)
                    {
                        continue;
                    }
                    if (Collisions.FallingThroughPlatform)
                    {
                        continue;
                    }
                    if (PlayerInput.y == -1)
                    {
                        Collisions.FallingThroughPlatform = true;
                        Invoke("ResetFallingThroughPlatform", 0.5f);
                        continue;
                    }
                }

                moveAmount.y = (hit.distance - SKIN_WIDTH) * directionY;
                rayLength = hit.distance;

                if (Collisions.ClimbingSlope)
                {
                    moveAmount.x = moveAmount.y / Mathf.Tan(Collisions.SlopeAngle * Mathf.Deg2Rad) * Mathf.Sign(moveAmount.x);
                }

                Collisions.Below = directionY == -1;
                Collisions.Above = directionY == 1;
            }
        }

        if (Collisions.ClimbingSlope)
        {
            float directionX = Mathf.Sign(moveAmount.x);
            rayLength = Mathf.Abs(moveAmount.x) + SKIN_WIDTH;
            Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.BottomLeft : raycastOrigins.BottomRight) + Vector2.up * moveAmount.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != Collisions.SlopeAngle)
                {
                    moveAmount.x = (hit.distance - SKIN_WIDTH) * directionX;
                    Collisions.SlopeAngle = slopeAngle;
                    Collisions.SlopeNormal = hit.normal;
                }
            }
        }
    }

    private void ClimbSlope(ref Vector2 moveAmount, float slopeAngle, Vector2 slopeNormal)
    {
        float moveDistance = Mathf.Abs(moveAmount.x);
        float climbmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

        if (moveAmount.y <= climbmoveAmountY)
        {
            moveAmount.y = climbmoveAmountY;
            moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
            Collisions.Below = true;
            Collisions.ClimbingSlope = true;
            Collisions.SlopeAngle = slopeAngle;
            Collisions.SlopeNormal = slopeNormal;
        }
    }

    private void DescendSlope(ref Vector2 moveAmount)
    {
        RaycastHit2D maxSlopeHitLeft = Physics2D.Raycast(raycastOrigins.BottomLeft, Vector2.down, Mathf.Abs(moveAmount.y) + SKIN_WIDTH, collisionMask);
        RaycastHit2D maxSlopeHitRight = Physics2D.Raycast(raycastOrigins.BottomRight, Vector2.down, Mathf.Abs(moveAmount.y) + SKIN_WIDTH, collisionMask);
        if (maxSlopeHitLeft ^ maxSlopeHitRight)
        {
            SlideDownMaxSlope(maxSlopeHitLeft, ref moveAmount);
            SlideDownMaxSlope(maxSlopeHitRight, ref moveAmount);
        }

        if (!Collisions.SlidingDownMaxSlope)
        {
            float directionX = Mathf.Sign(moveAmount.x);
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.BottomRight : raycastOrigins.BottomLeft;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != 0 && slopeAngle <= maxSlopeAngle)
                {
                    if (Mathf.Sign(hit.normal.x) == directionX)
                    {
                        if (hit.distance - SKIN_WIDTH <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x))
                        {
                            float moveDistance = Mathf.Abs(moveAmount.x);
                            float descendmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                            moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
                            moveAmount.y -= descendmoveAmountY;

                            Collisions.SlopeAngle = slopeAngle;
                            Collisions.DescendingSlope = true;
                            Collisions.Below = true;
                            Collisions.SlopeNormal = hit.normal;
                        }
                    }
                }
            }
        }
    }

    private void SlideDownMaxSlope(RaycastHit2D hit, ref Vector2 moveAmount)
    {

        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (slopeAngle > maxSlopeAngle)
            {
                moveAmount.x = Mathf.Sign(hit.normal.x) * (Mathf.Abs(moveAmount.y) - hit.distance) / Mathf.Tan(slopeAngle * Mathf.Deg2Rad);

                Collisions.SlopeAngle = slopeAngle;
                Collisions.SlidingDownMaxSlope = true;
                Collisions.SlopeNormal = hit.normal;
            }
        }

    }

    private void ResetFallingThroughPlatform()
    {
        Collisions.FallingThroughPlatform = false;
    }

    public struct CollisionInfo
    {
        public bool Above, Below;
        public bool Left, Right;

        public bool ClimbingSlope;
        public bool DescendingSlope;
        public bool SlidingDownMaxSlope;

        public float SlopeAngle, SlopeAngleOld;
        public Vector2 SlopeNormal;
        public Vector2 MoveAmountOld;
        public int FaceDirection;
        public bool FallingThroughPlatform;

        public void Reset()
        {
            Above = Below = false;
            Left = Right = false;
            ClimbingSlope = false;
            DescendingSlope = false;
            SlidingDownMaxSlope = false;
            SlopeNormal = Vector2.zero;

            SlopeAngleOld = SlopeAngle;
            SlopeAngle = 0;
        }
    }
}