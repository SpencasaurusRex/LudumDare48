using System.Collections;
using UnityEngine;

public class Box : MonoBehaviour {

    // Configuration
    public bool Drillable = true;
    public BlockColor BlockColor;
    public Sprite[] Sprites;
    public float FallingSpeed = 8;
    public bool CanFall = true;

    // Runtime
    public bool Stable = true;
    public Group Group;
    public GridObject GridObject;
    GridManager grid;
    SpriteRenderer sr;

    void Awake() {
        GridObject = GetComponent<GridObject>();
        GridObject.GridType = GridType.Block;
        if (BlockColor == BlockColor.None) {
            BlockColor = (BlockColor)Random.Range(0, 3);
        }
        sr = GetComponent<SpriteRenderer>();
    }

    void Start() {
        grid = GridManager.Instance;
        GetComponent<SpriteRenderer>().color = BlockColor == BlockColor.Red ? Color.red : BlockColor == BlockColor.Green ? Color.green : Color.blue;
        BoxManager.Instance.Boxes.Add(this);

        CalculateGroups();
    }

    void OnDestroy() {
        BoxManager.Instance.Boxes.Remove(this);
    }

    public bool Falling;
    float nextFallCheck;

    public void CalculateGroups() {
        // if (Falling) return;        
        // Group = new Group(this);

        // int combo = 0;

        // // Find group
        // Vector2Int[] dirs = new [] {Vector2Int.right, Vector2Int.up, Vector2Int.left, Vector2Int.down};
        // for (int i = 0; i < 4; i++) {
        //     var neighborPos = GridObject.Location.Offset(dirs[i]);
        //     if (!grid.HasGridObject(neighborPos)) continue;
            
        //     var neighbor = grid.GetGridObject(neighborPos);
        //     if (neighbor.GridType != GridType.Block) continue;
            
        //     var box = neighbor.GetComponent<Box>();
        //     if (BlockColor == box.BlockColor) {
        //         combo += 1 << i;
        //         Group.Add(box);
        //     }
        // }

        // sr.sprite = Sprites[combo];        
    }

    void Update() {
        if (Falling) {
            transform.position += Vector3.down * Time.deltaTime * FallingSpeed;
            if (transform.position.y <= nextFallCheck) {
                transform.position = GridObject.Location.ToFloat();
                Falling = false;
            }
            else return;
        }
    }

    public void Fall() {
        Falling = true;
        nextFallCheck = GridObject.Down.y;
        grid.MoveGridObject(GridObject, GridObject.Down);
    }

    public void Drill() {
        // if (Drillable) {
        //     // StartCoroutine(DrillCoroutine());
        //     Destroy(this.gameObject);
        // }

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
    Blue,
    Red,
    Green
}