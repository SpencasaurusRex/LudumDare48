using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour {

    // Configuration
    public float FallingSpeed;
    public float MovementSpeed;
    public float DrillInputThreshold = 0.5f;

    // Runtime
    InputMethod currentInputMethod;
    string GamepadAnyButton = "Gamepad any button";
    Rigidbody2D rb;
    Transform feet;
    Transform leftSide;
    Transform rightSide;

    bool grounded;
    bool leftCol;
    bool rightCol;
    bool drilling;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        feet = transform.Find("Feet");
        leftSide = transform.Find("LeftSide");
        rightSide = transform.Find("RightSide");
    }

    void Update() {
        float x = Input.GetAxisRaw("Horizontal") * MovementSpeed;
        float y = -FallingSpeed;
    
        RaycastHit2D[] belowCols = Physics2D.BoxCastAll(feet.position, feet.localScale, 0, Vector2.down, 1, LayerMask.GetMask("Default"));
        if (belowCols.Length > 0) {
            float distance = belowCols.Min(x => x.distance);
            if (distance < 0.05f) {
                grounded = true;
                y = 0;
            }
            else grounded = false;
        }
        else grounded = false;

        RaycastHit2D[] leftCols = Physics2D.BoxCastAll(leftSide.position, leftSide.localScale, 0, Vector2.left, 1, LayerMask.GetMask("Default"));
        if (leftCols.Length > 0) {
            float distance = leftCols.Min(x => x.distance);
            if (distance < 0.05f) {
                leftCol = true;
                if (x < 0) x = 0;
            }
            else leftCol = false;
        }
        else leftCol = false;

        RaycastHit2D[] rightCols = Physics2D.BoxCastAll(rightSide.position, rightSide.localScale, 0, Vector2.right, 1, LayerMask.GetMask("Default"));
        if (rightCols.Length > 0) {
            float distance = rightCols.Min(x => x.distance);
            if (distance < 0.05f) {
                rightCol = true;
                if (x > 0) x = 0;
            }
            else rightCol = false;
        }
        else rightCol = false;

        rb.velocity = new Vector2(x, y);

        if (grounded) {
            if (Input.GetAxisRaw("Vertical") < -DrillInputThreshold) {
                DrillDown(belowCols);
            }
            if (leftCol) {
                if (Input.GetAxisRaw("Horizontal") < -DrillInputThreshold) {
                    DrillHorizontal(leftCols);
                }
            }
            if (rightCol) {
                if (Input.GetAxisRaw("Horizontal") > DrillInputThreshold) {
                    DrillHorizontal(rightCols);
                }
            }
        }
    }

    void DrillDown(RaycastHit2D[] cols) {
        var col = cols.Where(x => x.collider.CompareTag("Box")).OrderBy(x => x.distance).ThenBy(x => Mathf.Abs(x.centroid.x - transform.position.x)).FirstOrDefault();
        if (col) {
            // TODO activate animation
            col.collider.GetComponent<Box>().Drill();
        }
    }

    void DrillHorizontal(RaycastHit2D[] cols) {
        var col = cols.Where(x => x.collider.CompareTag("Box")).OrderBy(x => x.distance).ThenBy(x => Mathf.Abs(x.centroid.y - transform.position.y)).FirstOrDefault();
        if (col) {
            // TODO activate animation
            col.collider.GetComponent<Box>().Drill();
        }
    }

    void DetermineInputMethod() {
        // Mouse click
        if (Input.GetMouseButtonDown((int) MouseButton.LeftMouse) || Input.GetMouseButtonDown((int) MouseButton.RightMouse))
        {
            TransitionInputMethod(InputMethod.KeyboardMouse);
        }
        // Escape key
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            TransitionInputMethod(InputMethod.KeyboardMouse);
        }
        // GamePad Button press
        else if (Input.GetButtonDown(GamepadAnyButton))
        {
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
