using System;
using System.Collections.Generic;

public class Group : IEquatable<Group>{
    public int ID;
    public HashSet<Box> Contained = new HashSet<Box>();
    public List<Box> Items = new List<Box>();

    public static int currentId;

    public Group(Box b) {
        ID = currentId++;
        Contained.Add(b);
        Items.Add(b);
    }

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

    public override int GetHashCode() {
        return ID;
    }

    bool IEquatable<Group>.Equals(Group other)
    {
        return other.ID == ID;
    }
}