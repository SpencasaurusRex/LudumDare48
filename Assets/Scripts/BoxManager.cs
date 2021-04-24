using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

public class BoxManager : MonoBehaviour {
    public static BoxManager Instance { get; set; }
    
    public List<Box> Boxes;
    
    void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
        }
        else Instance = this;
    }

    Dictionary<Box, Group> groups = new Dictionary<Box, Group>();
    Vector2Int[] dirs = new [] {Vector2Int.right, Vector2Int.up, Vector2Int.left, Vector2Int.down};

    void Connected(Box block) {
        if (block.Falling) return;
        if (groups.ContainsKey(block)){
            return;
        }

        List<Box> gos = new List<Box>();
        HashSet<Box> counted = new HashSet<Box>();
        Queue<Box> toProcess = new Queue<Box>();

        Group group = new Group();

        counted.Add(block);
        toProcess.Enqueue(block);

        while (toProcess.Count > 0) {
            var processBox = toProcess.Dequeue();
            // if (groups.ContainsKey(processBox)) continue;
            groups.Add(processBox, group);
            processBox.Group = group;
            group.Add(processBox);
            int combo = (int)processBox.BlockColor * 16;
            for (int i = 0; i < 4; i++) {
                var neighborPos = processBox.GridObject.Location.Offset(dirs[i]);
                var neighbor = GridManager.Instance.GetGridObject(neighborPos);
                if (neighbor != null) {
                    var neighborBox = neighbor.GetComponent<Box>();
                    if (!neighborBox.Falling && neighborBox.BlockColor == processBox.BlockColor) {
                        combo += 1 << i;
                        if (!counted.Contains(neighborBox)) {
                            counted.Add(neighborBox);
                            toProcess.Enqueue(neighborBox);
                        }
                    }
                }
            }
            processBox.GetComponent<SpriteRenderer>().sprite = processBox.Sprites[combo];
        }
    }

    void Update() {
        Stopwatch all = Stopwatch.StartNew();
        Stopwatch calculateGroups = Stopwatch.StartNew();
        
        groups.Clear();

        // Calculate groups
        for (int i = 0; i < Boxes.Count; i++) {
            Connected(Boxes[i]);
        }
        
        calculateGroups.Stop();
        
        Stopwatch uniqueGroupsSw = Stopwatch.StartNew();
        HashSet<Group> containedGroups = new HashSet<Group>();
        List<Group> uniqueGroups = new List<Group>();
        foreach (var box in Boxes) {
            if (!containedGroups.Contains(box.Group)) {
                containedGroups.Add(box.Group);
                uniqueGroups.Add(box.Group);
            }
        }
        uniqueGroupsSw.Stop();

        Stopwatch stability = Stopwatch.StartNew();
        foreach (var group in uniqueGroups) {
            bool stable = false;
            bool wobble = false;
            bool falling = false;
            foreach (var box in group.Items) {
                if (box.Wobble) {
                    wobble = true;
                    break;
                }
                if (box.Falling) {
                    falling = true;
                }
                if (box.Falling || !box.CanFall) {
                    stable = true;
                    break;
                }
                var below = GridManager.Instance.GetGridObject(box.GridObject.Down);

                if (below != null && below.GridType == GridType.Block) {
                    var belowBox = below.GetComponent<Box>();
                    if (!belowBox.Wobble && group != belowBox.Group || !belowBox.CanFall) {
                        stable = true;
                    }
                }
            }
            if (wobble) {

            }
            else if (!stable && !falling) {
                var orderedBoxes = group.Items.OrderBy(x => x.GridObject.Location.y).ToList();
                foreach (var fallingBox in orderedBoxes) {
                    fallingBox.Fall();
                }
            }
            else if (!falling) {
                foreach (var box in group.Items) {
                    box.DontFall();
                }
            }
        }
        stability.Stop();


        all.Stop();
        // print(all.ElapsedMilliseconds + " " + calculateGroups.ElapsedMilliseconds + " " + uniqueGroupsSw.ElapsedMilliseconds + " " + stability.ElapsedMilliseconds);
    }
}