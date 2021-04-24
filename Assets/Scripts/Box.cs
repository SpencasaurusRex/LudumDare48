using System.Collections;
using UnityEngine;

public class Box : MonoBehaviour {

    // Configuration
    public bool Drillable = true;
    public BlockColor BlockColor;
    public Sprite[] Sprites;
    public float FallingSpeed = 8;
    public bool CanFall = true;
    public AnimationCurve WobbleCurve;
    public float WobbleAnimationLength = 0.5f;

    // Runtime
    public bool Stable = true;
    public bool Wobble = false;
    float animationT;
    public Group Group;
    public GridObject GridObject;
    GridManager grid;
    SpriteRenderer sr;
    bool wobbled = false;

    void Awake() {
        GridObject = GetComponent<GridObject>();
        GridObject.GridType = GridType.Block;
        if (BlockColor == BlockColor.None) {
            BlockColor = (BlockColor)Random.Range(0, (int)BlockColor.Max);
        }
        sr = GetComponent<SpriteRenderer>();
    }

    void Start() {
        grid = GridManager.Instance;
        BoxManager.Instance.Boxes.Add(this);
    }

    void OnDestroy() {
        BoxManager.Instance.Boxes.Remove(this);
    }

    public bool Falling;
    float nextFallCheck;

    void Update() {
        if (Wobble) {
            animationT += Time.deltaTime;
            transform.position = GridObject.Location.WithZ(0).ToFloat() + Vector3.right * WobbleCurve.Evaluate(animationT) + Vector3.up;
        }
        else {
            animationT = 0;
            if (Falling) {
                transform.position += Vector3.down * Time.deltaTime * FallingSpeed;
                transform.position = transform.position.WithX(GridObject.Location.x);
                if (transform.position.y <= nextFallCheck) {
                    transform.position = GridObject.Location.ToFloat();
                    Falling = false;
                }
            }
        }
    }

    public void Fall() {
        Stable = false;
        StartCoroutine(WobbleCoroutine());
    }

    public void DontFall() {
        wobbled = false;
    }

    IEnumerator WobbleCoroutine() {
        if (!wobbled) {
            Wobble = true;
            wobbled = true;

            Falling = true;
            nextFallCheck = GridObject.Down.y;
            grid.MoveGridObject(GridObject, GridObject.Down);
            
            yield return new WaitForSeconds(WobbleAnimationLength);
            Wobble = false;
            animationT = 0;
        }
        else {
            Falling = true;
            nextFallCheck = GridObject.Down.y;
            grid.MoveGridObject(GridObject, GridObject.Down);
        }
    }

    public void Drill() {
        foreach (var box in Group.Items) {
            Destroy(box.gameObject);
        }
    }

    IEnumerator DrillCoroutine() {
        yield return new WaitForSeconds(0.1f);
        Destroy(this.gameObject);
    }
}

public enum BlockColor {
    None = -1,
    Cyan,
    Orange,
    Purple,
    Pink,
    Brown,
    Green,
    Max
}