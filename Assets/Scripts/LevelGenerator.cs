using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public static LevelGenerator Instance { get; set; }

    // Configuration
    public GridObject[] BlockPrefabs;
    public GridObject NonDrillableBlockPrefab;
    public GridObject JarPrefab;
    public GridObject CratePrefab;
    public Transform BackgroundGrid;

    public int Width = 9;
    public int Height = 30;
    public int MaxNonDrillablePerRow = 2;
    public float NonDrillableChance = .1f;
    public float CrateChance = .03f;
    public float JarChance = .01f;

    // Runtime
    public int StartingDifficulty = 3;
    float Difficulty;
    Transform lastGrid;

    void Awake() {
        if (Instance) {
            Destroy(this);
        }
        else Instance = this;
    }

    void Start() {
        Difficulty = StartingDifficulty;
        Generate(0);
    }

    void Update() {
        
    }

    public void Reset() {
        Difficulty = StartingDifficulty;
    }

    public int Generate(int atY) {
        if (lastGrid) {
            Destroy(lastGrid.gameObject);
        }
        lastGrid = Instantiate(BackgroundGrid, new Vector3(-0.5f, -0.5f + atY, 0), Quaternion.identity);
        for (int y = 0; y > -Height; y--) {
            int nonDrillableThisRow = 0;
            for (int x = 0; x < Width; x++) {
                int i = Random.Range(0, Mathf.Clamp((int)Difficulty, 0, 5) + 1);
                var pos = new Vector3(x, y + atY, 0);
                
                GridObject box;
                if (Random.Range(0f, 1f) <= NonDrillableChance + Difficulty / 50f && nonDrillableThisRow < MaxNonDrillablePerRow) {
                    nonDrillableThisRow++;
                    box = Instantiate(NonDrillableBlockPrefab, pos, Quaternion.identity);
                }
                else if (Random.Range(0f, 1f) <= CrateChance) {
                    box = Instantiate(CratePrefab, pos, Quaternion.identity);
                }
                else if (Random.Range(0f, 1f) <= JarChance) {
                    box = Instantiate(JarPrefab, pos, Quaternion.identity);
                }
                else {
                    box = Instantiate(BlockPrefabs[i], pos, Quaternion.identity);
                }
                if (y == -Height+1) {
                    box.CanFall = false;
                }
            }
        }
        Difficulty += 0.75f;
        return -Height + atY;
    }
}
