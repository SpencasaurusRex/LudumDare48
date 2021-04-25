using System;
using System.Collections.Generic;

public class Group : IEquatable<Group>{
    public int ID;
    public HashSet<GridObject> Contained;
    public List<GridObject> Items;

    public static int currentId;

    public Group() {
        ID = currentId++;
        Contained = new HashSet<GridObject>();
        Items = new List<GridObject>();
    }

    public Group(GridObject go) {
        ID = currentId++;
        Contained = new HashSet<GridObject>();
        Items = new List<GridObject>();
        go.Group = this;
        Add(go);
    }

    public void Add(GridObject go) {
        if (Contained.Contains(go)) return;
        Contained.Add(go);
        Items.Add(go);
        foreach (var box in go.Group.Items) {
            Add(box);
        }
        go.Group = this;
    }

    public void Remove(GridObject go) {
        Contained.Remove(go);
        Items.Remove(go);
    }

    public override int GetHashCode() {
        return ID;
    }

    bool IEquatable<Group>.Equals(Group other) {
        return other.ID == ID;
    }
}