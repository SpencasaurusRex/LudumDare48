using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    // Configuration
    public float FallingSpeed;
    public float MovementSpeed;
    public float DrillInputThreshold = 0.5f;
    public float FallingBlockWalkGap = 0.75f;
    public float FallingBlockKillGap = 0.3f;
    public int LeftMost = 0;
    public int RightMost = 8;
    public GameObject BreakingParticlesSourcePrefab;
    public float LandingTime = 0.2f;
    public Image[] CreditImages;
    public Sound SoundPrefab;

    // Runtime
    public bool grounded = false;
    public bool drilling = false;
    float drillTime;
    public bool moving = false;
    public bool falling = false;
    public bool dead = false;
    public bool landing = false;
    public bool victory = false;
    bool start = true;
    float landingTimer;
    int nextBottom;
    SpriteRenderer blackoutPanel;
    Reveal PressAnyText;

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

    GameObject lastSource;

    void Start() {
        grid = GridManager.Instance;
        Location = transform.position.xy().RoundToInt();
        transform.position = Location.ToFloat();
        animator = GetComponent<Animator>();
        SetState(State.Idle);
        blackoutPanel = transform.Find("BlackoutPanel").GetComponent<SpriteRenderer>();

        nextBottom = -FindObjectOfType<LevelGenerator>().Height - 10;

        PressAnyText = FindObjectOfType<Reveal>();
        cameraFollow = FindObjectOfType<CameraFollow>();
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

                if (nextGridPos.y <= nextBottom) {
                    Victory();
                }

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
            
            // Drill cancel
            var delta = drillingBlock.Location - Location;
            var input = new Vector2Int();
            var h = Input.GetAxisRaw("Horizontal");
            var v = Input.GetAxisRaw("Vertical");
            if (h < -DrillInputThreshold) {
                input.x = -1;
            }
            else if (h > DrillInputThreshold) {
                input.x = 1;
            }
            else if (v > DrillInputThreshold) {
                input.y = 1;
            }
            else if (v < -DrillInputThreshold) {
                input.y = -1;
            }
            if (delta != input && input != Vector2Int.zero) {
                drilling = false;
                timer = 0;
                drillingBlock = null;
                if (lastSource != null)
                    lastSource.GetComponent<Lifetime>().Amount = 0;
            }
            else {
                timer += Time.deltaTime;
                if (timer >= drillTime) {
                    timer = 0;
                    drilling = false;

                    drillingBlock.Drill();
                }
                return;
            }
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
            if (!box) return;
            if (!box.Falling) {
                drillTime = grid.GetGridObject(pos).DrillTime;
                drilling = true;
                drillingBlock = box;
                lastSource = Instantiate(BreakingParticlesSourcePrefab, (transform.position + pos.WithZ(0).ToFloat()) * 0.5f, Quaternion.identity); 
                var force = 2 * (transform.position - pos.WithZ(0).ToFloat());
                var source = lastSource.GetComponent<BreakingParticlesSource>();
                source.Force = force; 
                source.BlockColor = box.BlockColor;
                source.GetComponent<Lifetime>().Amount = drillTime;

                var delta = pos - Location;
                if (delta.y < 0) {
                    SetState(State.WindupDown);
                }
                else if (delta.y > 0) {
                    SetState(State.WindupUp);
                }
                else SetState(State.Windup);

            }
            else if (box.transform.position.y - box.Location.y > FallingBlockWalkGap && box.Location.y == Location.y) {
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
        else if (Input.GetAxisRaw("Vertical") > DrillInputThreshold) {
            MoveTowardsBlock(Up);
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

    public float DeathFadeSpeed = 0.5f;
    float deadTimer = 0;
    bool cameraUnassigned = true;
    CameraFollow cameraFollow;

    void Update() {
        lastCoinCreated += Time.deltaTime;

        if (start) {
            if (Input.anyKey) {
                start = false;
                transform.localScale = Vector3.one;
                foreach (var image in CreditImages) {
                    var lt = image.gameObject.AddComponent<Lifetime>();
                    lt.Amount = 1f;
                    lt.FadeAlpha = true;
                }
            }
            return;
        }
        if (cameraUnassigned) {
            if (transform.position.y < cameraFollow.transform.position.y) {
                cameraFollow.Follow = transform;
                cameraUnassigned = true;
            }
        }
        if (!dead) {
            deadTimer = Mathf.Clamp01(deadTimer - Time.deltaTime * DeathFadeSpeed);
            if (deadTimer < .001f) {
                blackoutPanel.sortingOrder = 1;
            }
            GridMovement();
        }
        else {
            deadTimer = Mathf.Clamp01(deadTimer + Time.deltaTime * DeathFadeSpeed);

            if (deadTimer > 0.8f) {
                PressAnyText.StartReveal();
            }

            if (deadTimer > 0.99f && Input.anyKey) {
                PressAnyText.Hide();
                ResetLevel();
            }
        }
        blackoutPanel.color = new Color(blackoutPanel.color.r, blackoutPanel.color.g, blackoutPanel.color.b, deadTimer);
    }

    public void ResetLevel() {
        LevelGenerator.Instance.Reset();
        blackoutPanel.sortingOrder = 3;
        GridManager.Instance.Clear();
        Location = new Vector2Int(4, 1);
        transform.position = Location.ToFloat();
        SetState(State.Idle);
        animator.SetTrigger("Reset");
        grounded = true;
        drilling = false;
        moving = false;
        falling = false;
        dead = false;
        landing = false;
        
        nextBottom = LevelGenerator.Instance.Generate(0);
    }

    float lastCoinCreated = 0;
    int coinCount;
    void CollectCoin() {
        if (lastCoinCreated >= 0.1f) {
            lastCoinCreated = 0;
            var sound = Instantiate(SoundPrefab);
        }
        
        coinCount++;
    }

    void OnCollisionEnter2D(Collision2D other) {
        Destroy(other.gameObject);
        CollectCoin();
    }

    public void Victory() {
        if (victory) return;
        victory = true;

        // Load next level
        victory = false;
        GridManager.Instance.Clear();
        nextBottom = LevelGenerator.Instance.Generate(Location.y - 15) - 10;
    }
}
