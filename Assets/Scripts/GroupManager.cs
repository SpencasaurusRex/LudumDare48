using UnityEngine;
using System.Collections.Generic;

public class GroupManager : MonoBehaviour {
    public static GroupManager Instance { get; set; }
    
    public List<Group> Groups;
    
    void Update() {
        // Check groups for stable
    }
}