using UnityEngine;

[RequireComponent(typeof(CharacterController2D))]
public class PlayerEngine : MonoBehaviour
{
    [SerializeField]
    private float minJumpHeight = 1f;
    [SerializeField]
    private float maxJumpHeight = 4f;
    [SerializeField]
    private float timeToJumpApex = 0.5f;
    [SerializeField]
    private float moveSpeed = 6f;
    [SerializeField]
    private float maxWallSlideSpeed = 3f;

    private float wallStickTime = 0.25f;
    private float timeToWallUnstick;

    private float accelerationTimeAirbourne = 0.2f;
    private float accelerationTimeGrounded = 0.1f;

    private float maxJumpVelocity;
    private float minJumpVelocity;
    private float gravity;
    private Vector2 velocity;
    private float velocityXSmoothing;

    private Vector2 wallJumpClimb = new Vector2(2.5f, 16);
    private Vector2 wallJumpOff = new Vector2(8.5f, 7);
    private Vector2 wallLeap = new Vector2(18, 17);

    private Vector2 directionalInput;
    private bool wallSliding;
    private int wallDirectionX;

    private CharacterController2D playerController;

    private void Start()
    {
        playerController = GetComponent<CharacterController2D>();

        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity * timeToJumpApex);
        minJumpVelocity = Mathf.Sqrt(2* Mathf.Abs(gravity) * minJumpHeight);
    }

    private void Update()
    {
        CalculateVelocity();
        HandleWallSliding();

        playerController.Move(velocity * Time.deltaTime, directionalInput);

        if (playerController.Collisions.Above || playerController.Collisions.Below)
        {
            if (playerController.Collisions.SlidingDownMaxSlope)
            {
                velocity.y += playerController.Collisions.SlopeNormal.y * -gravity * Time.deltaTime;
            }
            else
            {
                velocity.y = 0;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Collectable"))
        {
            ObjectPoolManager.Instance.SetObjectBackToPool(other.gameObject);
            UIManager.Instance.AddCollectable(1);
        }
        else if (other.CompareTag("DeathBox"))
        {
            UIManager.Instance.SetGameOverUI();
        }
        else if (other.CompareTag("Goal"))
        {
            LevelManager.Instance.OnLevelCompleted();
            UIManager.Instance.SetGameOverUI();
        }
    }

    private void HandleWallSliding()
    {
        wallDirectionX = (playerController.Collisions.Left) ? -1 : 1;
        wallSliding = false;
        if ((playerController.Collisions.Left || playerController.Collisions.Right) && !playerController.Collisions.Below && velocity.y < 0)
        {
            wallSliding = true;

            if (velocity.y < -maxWallSlideSpeed)
            {
                velocity.y = -maxWallSlideSpeed;
            }

            if (timeToWallUnstick > 0)
            {
                velocityXSmoothing = 0;
                velocity.x = 0;

                if (directionalInput.x != wallDirectionX && directionalInput.x != 0)
                {
                    timeToWallUnstick -= Time.deltaTime;
                }
                else
                {
                    timeToWallUnstick = wallStickTime;
                }
            }
            else
            {
                timeToWallUnstick = wallStickTime;
            }
        }
    }

    private void CalculateVelocity()
    {
        float targetVelocityX = directionalInput.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (playerController.Collisions.Below) ? accelerationTimeGrounded : accelerationTimeAirbourne);
        velocity.y += gravity * Time.deltaTime;
    }

    public void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;
    }

    public void OnJumpInputDown()
    {
        if (wallSliding)
        {
            if (wallDirectionX == directionalInput.x)
            {
                velocity.x = -wallDirectionX * wallJumpClimb.x;
                velocity.y = wallJumpClimb.y;
            }
            else if (directionalInput.x == 0)
            {
                velocity.x = -wallDirectionX * wallJumpOff.x;
                velocity.y = wallJumpOff.y;
            }
            else
            {
                velocity.x = -wallDirectionX * wallLeap.x;
                velocity.y = wallLeap.y;
            }
        }
        if (playerController.Collisions.Below)
        {
            if (playerController.Collisions.SlidingDownMaxSlope)
            {
                if (directionalInput.x != -Mathf.Sign(playerController.Collisions.SlopeNormal.x))
                { // not jumping against max slope
                    velocity.y = maxJumpVelocity * playerController.Collisions.SlopeNormal.y;
                    velocity.x = maxJumpVelocity * playerController.Collisions.SlopeNormal.x;
                }
            }
            else
            {
                velocity.y = maxJumpVelocity;
            }
        }
    }

    public void OnJumpInputUp()
    {
        if (velocity.y > minJumpVelocity)
        {
            velocity.y = minJumpVelocity;
        }
    }
}