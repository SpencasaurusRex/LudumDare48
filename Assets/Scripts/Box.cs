using System.Collections;
using UnityEngine;

public class Box : MonoBehaviour {

    // Configuration
    public bool Drillable = true;
    public BlockColor BlockColor;
    public Sprite[] Sprites;
    public float FallingSpeed = 8;

    // Runtime
    public bool Stable = true;
    public Group Group = new Group();
    GridObject gridObject;
    GridManager grid;
    SpriteRenderer sr;

    void Awake() {
        gridObject = GetComponent<GridObject>();
        gridObject.Type = GridType.Block;
        sr = GetComponent<SpriteRenderer>();
    }

    void Start() {
        grid = GridManager.Instance;
    }

    bool falling;
    float nextFallCheck;

    void Update() {
        if (falling) {
            transform.position += Vector3.down * Time.deltaTime * FallingSpeed;
            if (transform.position.y <= nextFallCheck) {
                transform.position = gridObject.Location.ToFloat();
                var nextGridPos = gridObject.Location + Vector2Int.down;

                if (grid.HasGridObject(nextGridPos)) {
                    falling = false;
                }
                else {
                    grid.MoveGridObject(gridObject, nextGridPos);
                    nextFallCheck--;
                }
            }

            return;
        }
        
        // Check if still stable
        if (!grid.HasGridObject(gridObject.Down)) {
            Stable = false;
        }
        
        int combo = 0;

        // Find group
        Vector2Int[] dirs = new [] {Vector2Int.right, Vector2Int.up, Vector2Int.left, Vector2Int.down};
        for (int i = 0; i < 4; i++) {
            var neighborPos = gridObject.Location.Offset(dirs[i]);
            if (!grid.HasGridObject(neighborPos)) continue;
            
            var neighbor = grid.GetGridObject(neighborPos);
            if (neighbor.Type != GridType.Block) continue;
            
            var box = neighbor.GetComponent<Box>();
            if (BlockColor == box.BlockColor) {
                combo += 1 << i;
                Group.Add(box);
            }
        }

        sr.sprite = Sprites[combo];

        bool groupStable = false;
        foreach (var groupBox in Group.Items) {
            if (groupBox.Stable) groupStable = true;
        }

        if (!groupStable) {
            falling = true;
            nextFallCheck = gridObject.Down.y;
            grid.MoveGridObject(gridObject, gridObject.Down);
            return;
        }
    }

    public void Drill() {
        if (Drillable) {
            StartCoroutine(DrillCoroutine());
        }
    }

    IEnumerator DrillCoroutine() {
        yield return new WaitForSeconds(0.1f);
        Destroy(this.gameObject);
    }
}

public enum BlockColor {
    Blue,
    Red,
    Green
}