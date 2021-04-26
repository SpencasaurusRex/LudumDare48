using UnityEngine;

public class GridObject : MonoBehaviour {
    // Configuration
    public BlockColor BlockColor;
    public float FallingSpeed = 8;
    public bool CanFall = true;
    public AnimationCurve WobbleCurve;
    public float WobbleAnimationLength = 0.5f;
    public BreakBlock BreakBlockPrefab;
    public float DrillTime;
    public Rigidbody2D CoinPrefab;
    public float CoinAmount;
    public int CoinAmountFlucuate;
    public bool CanConnect;
    public bool HasParticles = true;

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
        if (BlockColor == BlockColor.NonDrillable) {
            Instantiate(GridManager.Instance.MetalBlockBreakSound);
        }
        else Instantiate(GridManager.Instance.BlockBreakSound);
        foreach (var other in Group.Items) {
            other.GetComponent<Collider2D>().enabled = false;
            
            if (other.HasParticles) {
                var breakBlock = Instantiate(BreakBlockPrefab, other.Location.ToFloat(), Quaternion.identity);
                breakBlock.BlockColor = other.BlockColor;
                breakBlock.Setup();
            }
            
            if (other.CoinAmount < 1) {
                if (Random.Range(0, 1f) < other.CoinAmount)
                    Instantiate(CoinPrefab, other.Location.ToFloat(), Quaternion.identity);
            }
            else {
                int change = Random.Range(-CoinAmountFlucuate, CoinAmountFlucuate);
                for (int i = 0; i < other.CoinAmount + change; i++) {
                    Instantiate(CoinPrefab, other.Location.ToFloat(), Quaternion.identity);
                }
            }
            
            Destroy(other.gameObject);
        }
    }
}

public enum GridType {
    Block,
    Player
}