using UnityEngine;

public class Box : MonoBehaviour {
    
    public void Drill() {
        print("Drilling");
        Destroy(this.gameObject);
    }
}

public enum Color {
    Blue,
    Red,
    Green
}