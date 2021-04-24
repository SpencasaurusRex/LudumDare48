using UnityEngine;

public class GridObject : MonoBehaviour
{
    public Vector2Int Location;
    public GridType GridType;

    public Vector2Int Left => Location.Offset(Vector2Int.left);
    public Vector2Int Down => Location.Offset(Vector2Int.down);
    public Vector2Int Right => Location.Offset(Vector2Int.right);
    public Vector2Int Up => Location.Offset(Vector2Int.up);

    void Start() {
        Location = transform.position.xy().RoundToInt();
        transform.position = Location.ToFloat();

        if (!GridManager.Instance.AddGridObject(Location, this)) {
            Destroy(this.gameObject);
        }
    }

    void OnDestroy() {
        GridManager.Instance.RemoveGridObject(this);
    }
}

public enum GridType {
    Block,
    Player
}