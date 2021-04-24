using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    // Configuration
    public Box[] BlockPrefabs;
    public GameObject NonDrillableBlockPrefab;
    
    public int Width = 9;
    public int Height = 30;
    public int MaxNonDrillablePerRow = 2;
    public float NonDrillableChance = .1f;

    // Runtime
    public int Difficulty = 4;

    void Start() {
        Generate();
    }

    void Update() {
        
    }

    void Generate() {
        for (int y = 0; y > -Height; y--) {
            int nonDrillableThisRow = 0;
            for (int x = 0; x < Width; x++) {
                int i = Random.Range(0, Difficulty + 1);
                // TODO: Nondrillable blocks
                // if (Random.Range(0f, 1f) <= NonDrillableChance && nonDrillableThisRow < MaxNonDrillablePerRow) {
                //     nonDrillableThisRow++;
                // }
                var box = Instantiate(BlockPrefabs[i], new Vector3(x, y, 0), Quaternion.identity);
                if (y == -Height+1) {
                    box.CanFall = false;
                }
            }
        }
    }
}
