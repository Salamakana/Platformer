using UnityEngine;

[RequireComponent(typeof(PlayerEngine))]
public class PlayerInput : Singelton<PlayerInput>
{
    private const string HORIZONTAL_AXIS = "Horizontal";
    private const string VERTICAL_AXIS = "Vertical";

    public PlayerEngine PlayerEngine { get; private set; }
    public CharacterController2D PlayerCharacterController2D { get; private set; }

    private void Awake()
    {
        PlayerEngine = GetComponent<PlayerEngine>();
        PlayerCharacterController2D = GetComponent<CharacterController2D>();
    }

    private void Update()
    {
        if (Time.timeScale.Equals(0))
            return;
#if UNITY_EDITOR

        Vector2 directionalInput = new Vector2(Input.GetAxisRaw(HORIZONTAL_AXIS), Input.GetAxisRaw(VERTICAL_AXIS));

#else
        Vector2 directionalInput = new Vector2(Input.acceleration.x, Input.acceleration.y);
#endif

        PlayerEngine.SetDirectionalInput(directionalInput);

        if (Input.GetMouseButtonDown(0))
        {
            if (!UIManager.Instance.IsOverUIElement)
            {
                PlayerEngine.OnJumpInputDown();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            PlayerEngine.OnJumpInputUp();
        }
    }
}
