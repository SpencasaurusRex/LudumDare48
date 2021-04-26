using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour {
    public Dictionary<Vector2Int, GridObject> Grid = new Dictionary<Vector2Int, GridObject>();
    List<GridObject> GridObjects = new List<GridObject>();
    public static GridManager Instance { get; set; }
    public bool Debug;

    void Awake() {
        if (Instance != null) {
            Destroy(this);
        }
        else Instance = this;
    }
    
    public GridObject GetGridObject(Vector2Int coord) {
        if (Grid.ContainsKey(coord)) {
            return Grid[coord];
        }
        return null;
    }

    public bool AddGridObject(Vector2Int coord, GridObject go) {
        if (!Grid.ContainsKey(coord)) {
            Grid.Add(coord, go);
            go.Location = coord;
            go.Group = new Group(go);
            GridObjects.Add(go);
            return true;
        }
        throw new System.Exception("Adding to invalid spot");
    }

    public bool RemoveGridObject(GridObject go) {
        if (Grid.ContainsKey(go.Location) && Grid[go.Location] == go) {
            go.Group.Remove(go);
            Grid.Remove(go.Location);
            GridObjects.Remove(go);
            return true;
        }
        return false;
    }

    public void RemoveGridObjectAt(Vector2Int coord) {
        Grid.Remove(coord);
    }

    public bool HasGridObject(Vector2Int coord) {
        return Grid.ContainsKey(coord);
    }

    Vector2Int[] dirs = new [] {Vector2Int.right, Vector2Int.up, Vector2Int.left, Vector2Int.down};

    void Connected(GridObject go) {
        if (go == null) return;
        if (go.Wobbling || go.Falling) return;
        int combo = (int)go.BlockColor * 16;
        for (int i = 0; i < 4; i++) {
            var neighborPos = go.Location.Offset(dirs[i]);
            var neighbor = GridManager.Instance.GetGridObject(neighborPos);
            if (neighbor != null && !neighbor.Falling && !neighbor.Wobbling && neighbor.BlockColor == go.BlockColor) {
                combo += 1 << i;
                if (neighbor.Group != go.Group) {
                    go.Group.Add(neighbor);
                }

                if (neighbor.BlockColor != BlockColor.NonDrillable) {
                    int nCombo = (int)neighbor.BlockColor * 16;
                    for (int j = 0; j < 4; j++) {
                        var neighborNeighborPos = neighborPos.Offset(dirs[j]);
                        var neighborNeighbor = GridManager.Instance.GetGridObject(neighborNeighborPos);
                        if (neighborNeighbor != null && !neighborNeighbor.Falling && !neighborNeighbor.Wobbling && neighborNeighbor.BlockColor == neighbor.BlockColor) {
                            nCombo += 1 << j;
                            if (neighborNeighbor.Group != neighbor.Group) {
                                go.Group.Add(neighbor);
                            }
                        }
                    }
                    neighbor.GetComponent<SpriteRenderer>().sprite = neighbor.Sprites[nCombo];
                }
            }
        }
        if (go.BlockColor != BlockColor.NonDrillable) {
            go.GetComponent<SpriteRenderer>().sprite = go.Sprites[combo];
        }
    }

    List<Move> moves = new List<Move>();
    void MoveGridObject(GridObject go, Vector2Int newLocation) {
        if (go == null) return;
        moves.Add(new Move(go, go.Location, newLocation));
    }

    void Update() {
        foreach (var move in moves) {
            Grid.Remove(move.From);
        }
        foreach (var move in moves) {
            if (Grid.ContainsKey(move.To)) {
                move.GridObject.Drill();
            }
            else {
                Grid.Add(move.To, move.GridObject);
                move.GridObject.Location = move.To;
            }
        }
        moves.Clear();

        // Recalculate groups
        foreach (var go in GridObjects) {
            if (go.RecalculateGroup) {
                Connected(go);
                go.RecalculateGroup = false;
            }
        }

        // Assume all gridObjects are unstable
        foreach (var go in GridObjects) {
            if (go == null) continue;
            go.Stable = false;
        }

        // Calculate stability
        HashSet<Vector2Int> checkedCoords = new HashSet<Vector2Int>();
        Queue<GridObject> toProcess = new Queue<GridObject>();        
        foreach (var seed in GridObjects.Where(x => !x.CanFall)) {
            toProcess.Enqueue(seed);
        }

        while(toProcess.Any()) {
            var processing = toProcess.Dequeue();
            if (checkedCoords.Contains(processing.Location)) continue;
            if (processing.Falling || processing.Wobbling) continue;
            checkedCoords.Add(processing.Location);
            processing.Stable = true;
            // Set group as stable
            foreach (var groupMember in processing.Group.Items) {
                if (!checkedCoords.Contains(groupMember.Location)) {
                    toProcess.Enqueue(groupMember);
                }
            }
            // Check above
            if (!checkedCoords.Contains(processing.Up)) {
                var above = GetGridObject(processing.Up);
                if (above && above.Group != processing.Group && !above.Falling && !above.Wobbling) {
                    toProcess.Enqueue(above);
                }
            }
        }

        // Make all unstable blocks fall
        foreach (var block in GridObjects) {
            if (block == null) continue;
            if (block.Wobbling || block.Falling) continue;
            if (!block.Stable) {
                if (block.JustFinishedFalling) {
                    block.Falling = true;
                    MoveGridObject(block, block.Down);
                }
                else {
                    block.Wobbling = true;
                    block.TimeWobbling = 0;
                }
            }
        }

        // Handle wobbling blocks
        foreach (var block in GridObjects) {
            if (block.Wobbling) {
                block.TimeWobbling += Time.deltaTime;
                block.transform.position = block.Location.ToFloat() + Vector2.right * block.WobbleCurve.Evaluate(block.TimeWobbling);
                if (block.TimeWobbling > block.WobbleAnimationLength) {
                    block.Wobbling = false;
                    block.Falling = true;
                    block.TimeWobbling = 0;
                    MoveGridObject(block, block.Down);
                }
            }
            else if (block.JustFinishedFalling) {
                block.JustFinishedFalling = false;
            }
            else if (block.Falling) {
                block.transform.position += Vector3.down * Time.deltaTime * block.FallingSpeed;
                if (block.transform.position.y - block.Location.y <= 0) {
                    block.transform.position = block.Location.ToFloat();
                    block.JustFinishedFalling = true;
                    block.RecalculateGroup = true;
                    block.Falling = false;
                }
            }
        }

        if (Debug) {
            if (Random.Range(0, 1f) < 0.2f) {
                int i = Random.Range(0, GridObjects.Count);
                if (GridObjects[i] != null) {
                    GridObjects[i].Drill();
                }
            }
        }
    }

    public void Clear() {
        foreach (var obj in GridObjects) {
            if (obj != null)
                Destroy(obj.gameObject);
        }
        moves.Clear();
        GridObjects.Clear();
        Grid.Clear();
    }

    void OnDrawGizmos() {
        foreach (var coord in Grid.Keys) {
            Gizmos.color = UnityEngine.Color.blue;
            Gizmos.DrawCube(coord.ToFloat(), Vector3.one * 0.5f);
        } 
    }

    class Move {
        public Move(GridObject go, Vector2Int from, Vector2Int to) {
            GridObject = go;
            From = from;
            To = to;
        }

        public GridObject GridObject;
        public Vector2Int From;
        public Vector2Int To;
    }
}