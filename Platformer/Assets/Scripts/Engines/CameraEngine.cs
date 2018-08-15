using UnityEngine;

public class CameraEngine : Singelton<CameraEngine>
{
    public bool DebugMode;

    #region VARIABLES

    [SerializeField]
    private float verticalOffset = 0f;
    private float lookAheadDistanceX = 4f;
    private float lookSmoothTimeX = 0.5f;
    private float verticalSmoothTime = 0.2f;
    private float currentLookAheadX;
    private float targetLookAheadX;
    private float lookAheadDirectionX;
    private float smoothLookVelocityX;
    private float smoothVelocityY;

    private bool isLookAheadStopped;

    private Color focusAreaSizeColor = new Color(0, 0, 1, 0.4f);
    private CharacterController2D targetController2D;
    private Collider2D targetCollider2D;
    private Vector2 focusAreaSize = new Vector2(2, 4);
    private FocusArea focusArea;

    #endregion VARIABLES

    public void SearchTarget()
    {      
        targetController2D = PlayerInput.Instance.PlayerCharacterController2D;
        targetCollider2D = targetController2D.GetComponent<Collider2D>();
        focusArea = new FocusArea(targetCollider2D.bounds, focusAreaSize);
    }

    private void Update()
    {
        if (targetCollider2D != null)
        {
            focusArea.UpdateBounds(targetCollider2D.bounds);

            Vector2 focusPosition = focusArea.Centre + Vector2.up * verticalOffset;

            if (focusArea.Velocity.x != 0)
            {
                lookAheadDirectionX = Mathf.Sign(focusArea.Velocity.x);
                if (Mathf.Sign(targetController2D.PlayerInput.x) == Mathf.Sign(focusArea.Velocity.x) && targetController2D.PlayerInput.x != 0)
                {
                    isLookAheadStopped = false;
                    targetLookAheadX = lookAheadDirectionX * lookAheadDistanceX;
                }
                else
                {
                    if (!isLookAheadStopped)
                    {
                        isLookAheadStopped = true;
                        targetLookAheadX = currentLookAheadX + (lookAheadDirectionX * lookAheadDistanceX - currentLookAheadX) / 4f;
                    }
                }
            }

            currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, lookSmoothTimeX);
            focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothVelocityY, verticalSmoothTime);
            focusPosition += Vector2.right * currentLookAheadX;
            transform.position = (Vector3)focusPosition + Vector3.forward * -10;
        }

    }

    private void OnDrawGizmos()
    {
        if (DebugMode)
        {
            Gizmos.color = focusAreaSizeColor;
            Gizmos.DrawCube(focusArea.Centre, focusAreaSize);

            /////////////////////////////////////////////////////
            Camera mainCamera = GetComponent<Camera>();
            Vector3 p = mainCamera.WorldToViewportPoint(new Vector3(1, 1, mainCamera.farClipPlane));
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(p, new Vector2(2,2));
        }
    }

    private struct FocusArea
    {
        public Vector2 Centre;
        public Vector2 Velocity;

        private float left, right;
        private float top, bottom;

        public FocusArea(Bounds targetBounds, Vector2 size)
        {
            left = targetBounds.center.x - (size.x / 2f);
            right = targetBounds.center.x + (size.x / 2f);
            top = targetBounds.min.y + size.y;
            bottom = targetBounds.min.y;

            Centre = new Vector2((left + right) / 2, (top + bottom) / 2);
            Velocity = Vector2.zero;
        }

        public void UpdateBounds(Bounds targetBounds)
        {
            float shiftX = 0f;

            if(targetBounds.min.x < left)
            {
                shiftX = targetBounds.min.x - left;
            }
            else if (targetBounds.max.x > right)
            {
                shiftX = targetBounds.max.x - right;
            }

            left += shiftX;
            right += shiftX;

            float shiftY = 0f;

            if (targetBounds.min.y < bottom)
            {
                shiftY = targetBounds.min.y - bottom;
            }
            else if (targetBounds.max.y > top)
            {
                shiftY = targetBounds.max.y - top;
            }

            top += shiftY;
            bottom += shiftY;

            Centre = new Vector2((left + right) / 2, (top + bottom) / 2);
            Velocity = new Vector2(shiftX, shiftY);
        }
    }
}
