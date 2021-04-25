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
    public int LeftMost = 0;
    public int RightMost = 8;
    public GameObject BreakingParticlesSourcePrefab;
    public float LandingTime = 0.2f;

    // Runtime
    public bool grounded = false;
    public bool drilling = false;
    public bool moving = false;
    public bool falling = false;
    public bool dead = false;
    public bool landing = false;
    float landingTimer;

    Animator animator;
    GridObject drillingBlock;

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
        animator = GetComponent<Animator>();
        SetState(State.Idle);
    }

    enum State {
        Idle = 0, 
        Walking = 1,
        
        Windup = 2,
        Drill = 3,
        
        WindupUp = 4,
        DrillUp = 5,
        
        WindupDown = 6,
        DrillDown = 7,

        Falling = 8,
        Landing = 9,

        Dead = 10
    }

    void SetState(State state) {
        if (animator.GetInteger("State") != (int)state)
            animator.SetInteger("State", (int)state);
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
                    landing = true;
                    SetState(State.Landing);
                }
                else {
                    // grid.MoveGridObject(gridObject, nextGridPos);
                    Location = nextGridPos;
                    nextFallCheck--;
                }
            }

            return;
        }
        
        if (landing) {
            landingTimer += Time.deltaTime;
            if (landingTimer >= LandingTime) {
                landing = false;
                landingTimer = 0;
            }
        }

        if (moving) {
            var target = Location.ToFloat();
            transform.position = Vector3.MoveTowards(transform.position, target, MovementSpeed * Time.deltaTime);
            if ((transform.position.xy() - target).magnitude < 0.01f) {
                transform.position = target;
                moving = false;
                SetState(State.Idle);
            }
            return;
        }

        if (drilling) {
            
            timer += Time.deltaTime;
            if (timer >= DrillingTime) {
                timer = 0;
                drilling = false;

                drillingBlock.Drill();
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

            SetState(State.Falling);
            animator.SetTrigger("Fall");
            return;
        }

        void MoveTowardsBlock(Vector2Int pos) {
                var box = grid.GetGridObject(pos);
                if (!box.Falling) {
                    if (grid.GetGridObject(pos).Drillable) {
                        drilling = true;
                        drillingBlock = box;
                        var sourceGo = Instantiate(BreakingParticlesSourcePrefab, (transform.position + pos.WithZ(0).ToFloat()) * 0.5f, Quaternion.identity); 
                        var force = 2 * (transform.position - pos.WithZ(0).ToFloat());
                        var source = sourceGo.GetComponent<BreakingParticlesSource>();
                        source.Force = force; 
                        source.BlockColor = box.BlockColor;

                        var delta = pos - Location;
                        if (delta.y < 0) {
                            SetState(State.WindupDown);
                        }
                        else if (delta.y > 0) {
                            SetState(State.WindupUp);
                        }
                        else SetState(State.Windup);
                    }

                }
                else if (box.transform.position.y - box.Location.y > FallingBlockWalkGap) {
                    moving = true;
                    Location = pos;
                }
        }

        if (Input.GetAxisRaw("Horizontal") < -DrillInputThreshold) { 
            transform.localScale = Vector3.one.WithX(-1);
            if (grid.HasGridObject(Left)) {
                MoveTowardsBlock(Left);
            }
            else {
                if (Left.x >= LeftMost) {
                    // Move left
                    moving = true;
                    Location = Left;
                }
            }
        }
        else if (Input.GetAxisRaw("Horizontal") > DrillInputThreshold) {
            transform.localScale = Vector3.one.WithX(1);
            if (grid.HasGridObject(Right)) {
                // Drill right
                MoveTowardsBlock(Right);
            }
            else {
                if (Right.x <= RightMost) {
                    // Move right
                    moving = true;
                    Location = Right;
                }
            }
        }
        else if (Input.GetAxisRaw("Vertical") < -DrillInputThreshold) {
            MoveTowardsBlock(Down);
        }

        var fallingGrid = grid.GetGridObject(Location);
        if (fallingGrid) {
            float y = 0;
            if (fallingGrid.Falling) {
                y = fallingGrid.transform.position.y - fallingGrid.Location.y;
            }
            if (y < FallingBlockKillGap) {
                dead = true;
                SetState(State.Dead);
                return;
            }
        }

        if (moving) {
            SetState(State.Walking);
        }
        else if (drilling) {
             
        }
        else if (landing) {

        }
        else {
            SetState(State.Idle);
        }
    }

    void Update() {
        if (!dead) {
            GridMovement();
        }
    }
}
