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

    void Update() {
        Stopwatch all = Stopwatch.StartNew();
        
        Stopwatch calculateGroups = Stopwatch.StartNew();
        // Calculate groups
        foreach (var box in Boxes) {
            box.CalculateGroups();
        }
        calculateGroups.Stop();

        Stopwatch uniqueGroups = Stopwatch.StartNew();
        HashSet<Group> containedGroups = new HashSet<Group>();
        List<Group> groups = new List<Group>();
        foreach (var box in Boxes) {
            if (!containedGroups.Contains(box.Group)) {
                containedGroups.Add(box.Group);
                groups.Add(box.Group);
            }
        }
        uniqueGroups.Stop();

        Stopwatch stability = Stopwatch.StartNew();
        foreach (var group in groups) {
            bool stable = false;
            foreach (var box in group.Items) {
                if (box.Falling || !box.CanFall) {
                    stable = true;
                    break;
                }
                var below = GridManager.Instance.GetGridObject(box.GridObject.Down);

                if (below != null && below.GridType == GridType.Block) {
                    var belowBox = below.GetComponent<Box>();
                    if (group != belowBox.Group || !belowBox.CanFall) {
                        stable = true;
                    }
                }
            }
            if (!stable) {
                var orderedBoxes = group.Items.OrderBy(x => x.GridObject.Location.y).ToList();
                foreach (var fallingBox in orderedBoxes) {
                    fallingBox.Fall();
                }
            }
        }
        stability.Stop();


        all.Stop();
        // print("CalculateGroups: " + calculateGroups.ElapsedMilliseconds);
        // print("UniqueGroups: " + uniqueGroups.ElapsedMilliseconds);
        // print("Stability: " + stability.ElapsedMilliseconds);
        // print("All: " + all.ElapsedMilliseconds);
        print(all.ElapsedMilliseconds + " " + calculateGroups.ElapsedMilliseconds + " " + uniqueGroups.ElapsedMilliseconds + " " + stability.ElapsedMilliseconds);
    }
}