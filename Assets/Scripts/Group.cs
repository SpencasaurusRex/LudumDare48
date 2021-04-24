using UnityEngine;
using System.Collections.Generic;

public class Group {
    public HashSet<Box> Contained = new HashSet<Box>();
    public List<Box> Items = new List<Box>();

    public void Add(Box b) {
        if (Contained.Contains(b)) return;
        Contained.Add(b);
        Items.Add(b);
        foreach (var box in b.Group.Items) {
            Add(box);
        }
        b.Group = this;
    }

    public void Remove(Box b) {
        Contained.Remove(b);
        Items.Remove(b);
    }

    void Update() {

    }

}