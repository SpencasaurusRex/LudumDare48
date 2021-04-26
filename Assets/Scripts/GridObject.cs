using System.Collections.Generic;
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

    List<SpriteRenderer> breaks = new List<SpriteRenderer>();

    void Start() {
        if (BlockColor == BlockColor.None) {
            BlockColor = (BlockColor)Random.Range(0, (int)BlockColor.NonDrillable);
        }
        Location = transform.position.xy().RoundToInt();
        transform.position = Location.ToFloat();

        if (!GridManager.Instance.AddGridObject(Location, this)) {
            Destroy(this.gameObject);
        }

        if (transform.childCount > 0) {
            for (int i = 0 ; i < 4; i++) {
                breaks.Add(transform.GetChild(i).GetComponent<SpriteRenderer>());
            }
        }
    }

    void OnDestroy() {
        GridManager.Instance.RemoveGridObject(this);
    }

    public void Break() {
        if (HasParticles) {
            var breakBlock = Instantiate(BreakBlockPrefab, transform.position, Quaternion.identity);
            breakBlock.BlockColor = BlockColor;
            breakBlock.Setup();
        }
        Destroy(gameObject);
    }

    public Sprite[] BreakSprites;

    bool updatedThisFrame;

    void Update() {
        if (!updatedThisFrame && breaks.Count > 0) {
            for (int i = 0; i < 4; i++){
                breaks[i].enabled = false;
            }
        }
    }

    public void UpdateBreakTexture(Vector2Int direction, float amount) {
        if (breaks.Count == 0) return;
        updatedThisFrame = true;
        int a = (int)Mathf.Clamp(amount / 0.2f, 0, 3);

        if (direction == Vector2Int.down) {
            breaks[0].enabled = true;
            breaks[0].sprite = BreakSprites[a];
        }
        if (direction == Vector2Int.right) {
            breaks[2].enabled = true;
            breaks[2].sprite = BreakSprites[a + 4];
        }
        if (direction == Vector2Int.left) {
            breaks[1].enabled = true;
            breaks[1].sprite = BreakSprites[a + 8];
        }
        if (direction == Vector2Int.up) {
            breaks[3].enabled = true;
            breaks[3].sprite = BreakSprites[a + 12];
        }
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