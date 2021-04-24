using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour {

    // Configuration
    public float FallingSpeed;
    public float MovementSpeed;
    public float DrillInputThreshold = 0.5f;
    public float DrillingTime;

    // Runtime
    InputMethod currentInputMethod;
    string GamepadAnyButton = "Gamepad any button";
    // GridObject gridObject;

    public bool grounded = false;
    public bool drilling = false;
    public bool moving = false;
    public bool falling = false;

    float nextFallCheck;

    GridManager grid;
    float timer;

    Vector2Int Location;
    public Vector2Int Left => Location.Offset(Vector2Int.left);
    public Vector2Int Right => Location.Offset(Vector2Int.right);
    public Vector2Int Down => Location.Offset(Vector2Int.down);
    public Vector2Int Up => Location.Offset(Vector2Int.up);

    void Start() {
        // gridObject = GetComponent<GridObject>();
        // gridObject.GridType = GridType.Player;
        grid = GridManager.Instance;
        Location = transform.position.xy().RoundToInt();
        transform.position = Location.ToFloat();
    }

    void GridMovement() {
        if (falling) {
            transform.position += Vector3.down * Time.deltaTime * FallingSpeed;
            if (transform.position.y <= nextFallCheck) {
                transform.position = Location.ToFloat();
                var nextGridPos = Location + Vector2Int.down;

                if (grid.HasGridObject(nextGridPos)) {
                    grounded = true;
                    falling = false;
                }
                else {
                    // grid.MoveGridObject(gridObject, nextGridPos);
                    Location = nextGridPos;
                    nextFallCheck--;
                }
            }

            return;
        }

        if (moving) {
            var target = Location.ToFloat();
            transform.position = Vector3.MoveTowards(transform.position, target, MovementSpeed * Time.deltaTime);
            if ((transform.position.xy() - target).magnitude < 0.01f) {
                transform.position = target;
                moving = false;
            }
            return;
        }

        if (drilling) {
            timer += Time.deltaTime;
            if (timer >= DrillingTime) {
                timer = 0;
                drilling = false;
            }
            return;
        }

        // Check for ground
        var groundGridPos = Location.xy();
        groundGridPos.y--;
        if (!grid.HasGridObject(groundGridPos)) {
            grounded = false;
            falling = true;
            nextFallCheck = groundGridPos.y;
            // grid.MoveGridObject(gridObject, groundGridPos); 
            Location = groundGridPos;
            return;
        }

        if (Input.GetAxisRaw("Horizontal") < -DrillInputThreshold) { 
            if (grid.HasGridObject(Left)) {
                // Drill left
                grid.GetGridObject(Left).GetComponent<Box>().Drill();
                drilling = true;
            }
            else {
                // Move left
                moving = true;
                // grid.MoveGridObject(gridObject, gridObject.Left);
                Location = Left;
            }
        }
        else if (Input.GetAxisRaw("Horizontal") > DrillInputThreshold) {
            if (grid.HasGridObject(Right)) {
                // Drill right
                grid.GetGridObject(Right).GetComponent<Box>().Drill();
                drilling = true;
            }
            else {
                // Move right
                moving = true;
                // grid.MoveGridObject(gridObject, gridObject.Right);
                Location = Right;
            }
        }
        else if (Input.GetAxisRaw("Vertical") < -DrillInputThreshold) {
            // Drill down
            grid.GetGridObject(Down).GetComponent<Box>().Drill();
            drilling = true;
        }

        if (grid.HasGridObject(Location)) {
            // Die
            print("Die");
        }
    }

    void Update() {
        // FreeMovement();
        GridMovement();
    }

    void DetermineInputMethod() {
        // Mouse click
        if (Input.GetMouseButtonDown((int) MouseButton.LeftMouse)) {
            TransitionInputMethod(InputMethod.KeyboardMouse);
        }
        // Escape key
        else if (Input.GetKeyDown(KeyCode.Escape)) {
            TransitionInputMethod(InputMethod.KeyboardMouse);
        }
        // GamePad Button press
        else if (Input.GetButtonDown(GamepadAnyButton)) {
            TransitionInputMethod(InputMethod.Controller);
        }
    }

    void TransitionInputMethod(InputMethod method) {
        if (currentInputMethod != method) {
            currentInputMethod = method;
        }
    }

    enum InputMethod {
        KeyboardMouse, 
        Controller
    }
}
