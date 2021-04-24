using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour {

    // Configuration
    public float FallingSpeed;
    public float MovementSpeed;
    public float DrillInputThreshold = 0.5f;
    public float DrillingTime;
    public float FallingBlockWalkGap = 0.75f;
    public float FallingBlockKillGap = 0.3f;

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
            Location = groundGridPos;
            return;
        }

        if (Input.GetAxisRaw("Horizontal") < -DrillInputThreshold) { 
            if (grid.HasGridObject(Left)) {
                // Drill left
                var box = grid.GetGridObject(Left).GetComponent<Box>();
                if (!box.Falling) {
                    grid.GetGridObject(Left).GetComponent<Box>().Drill();
                    drilling = true;
                }
                else if (box.transform.position.y - box.GridObject.Location.y > FallingBlockWalkGap) {
                    moving = true;
                    Location = Left;
                }
            }
            else {
                // Move left
                moving = true;
                Location = Left;
            }
        }
        else if (Input.GetAxisRaw("Horizontal") > DrillInputThreshold) {
            if (grid.HasGridObject(Right)) {
                // Drill right
                var box =  grid.GetGridObject(Right).GetComponent<Box>();
                if (!box.Falling) {
                    box.Drill();
                    drilling = true;
                } 
                else if (box.transform.position.y - box.GridObject.Location.y > FallingBlockWalkGap) {
                    moving = true;
                    Location = Right;
                }
            }
            else {
                // Move right
                moving = true;
                Location = Right;
            }
        }
        else if (Input.GetAxisRaw("Vertical") < -DrillInputThreshold) {
            // Drill down
            var box = grid.GetGridObject(Down).GetComponent<Box>();
            if (!box.Falling) {
                box.Drill();
                drilling = true;
            }
        }

        var fallingGrid = grid.GetGridObject(Location);
        if (fallingGrid) {
            var fallingBlock = fallingGrid.GetComponent<Box>();
            float y = 0;
            if (fallingBlock.Falling) {
                y = fallingBlock.transform.position.y - fallingGrid.Location.y;
            }
            if (y < FallingBlockKillGap) {
                print("Die");
            }
        }
    }

    void Update() {
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
