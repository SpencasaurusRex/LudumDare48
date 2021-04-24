using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour {
    public Dictionary<Vector2Int, GridObject> Grid = new Dictionary<Vector2Int, GridObject>();
    public static GridManager Instance { get; set; }

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
            return true;
        }
        throw new System.Exception("Adding to invalid spot");
    }

    public bool RemoveGridObject(GridObject go) {
        if (Grid.ContainsKey(go.Location) && Grid[go.Location] == go) {
            Grid.Remove(go.Location);
            return true;
        }
        throw new System.Exception("Removing from invalid spot");
    }

    public bool MoveGridObject(GridObject go, Vector2Int newLocation) {
        if (RemoveGridObject(go) && !Grid.ContainsKey(newLocation)) {
            Grid.Add(newLocation, go);
            go.Location = newLocation;
            return true;
        }
        return false;
    }

    public bool HasGridObject(Vector2Int coord) {
        return Grid.ContainsKey(coord);
    }

    void OnDrawGizmos() {
        foreach (var coord in Grid.Keys) {
            Gizmos.color = UnityEngine.Color.blue;
            Gizmos.DrawCube(coord.ToFloat(), Vector3.one * 0.5f);
        } 
    }
}