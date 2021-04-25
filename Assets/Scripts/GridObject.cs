using UnityEngine;

public class GridObject : MonoBehaviour {
    // Configuration
    public bool Drillable = true;
    public BlockColor BlockColor;
    public Sprite[] Sprites;
    public float FallingSpeed = 8;
    public bool CanFall = true;
    public AnimationCurve WobbleCurve;
    public float WobbleAnimationLength = 0.5f;
    public BreakBlock BreakBlockPrefab;

    // Runtime
    public bool Falling;
    public bool Stable;
    public bool Wobbling;
    float animationT;
    public Group Group;
    public bool RecalculateGroup = true;
    public bool JustFinishedFalling = false;
    public float TimeWobbling;

    public Vector2Int Location;

    public Vector2Int Left => Location.Offset(Vector2Int.left);
    public Vector2Int Down => Location.Offset(Vector2Int.down);
    public Vector2Int Right => Location.Offset(Vector2Int.right);
    public Vector2Int Up => Location.Offset(Vector2Int.up);

    void Start() {
        if (BlockColor == BlockColor.None) {
            BlockColor = (BlockColor)Random.Range(0, (int)BlockColor.NonDrillable);
        }
        Location = transform.position.xy().RoundToInt();
        transform.position = Location.ToFloat();

        if (!GridManager.Instance.AddGridObject(Location, this)) {
            Destroy(this.gameObject);
        }
    }

    void OnDestroy() {
        GridManager.Instance.RemoveGridObject(this);
    }

    public void Drill() {
        if (!Drillable) return;

        foreach (var other in Group.Items) {
            other.GetComponent<Collider2D>().enabled = false;
            var breakBlock = Instantiate(BreakBlockPrefab, other.Location.ToFloat(), Quaternion.identity);
            breakBlock.BlockColor = other.BlockColor;
            breakBlock.Setup();
            Destroy(other.gameObject);
        }
    }
}

public enum GridType {
    Block,
    Player
}