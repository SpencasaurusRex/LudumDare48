using UnityEngine;

public class BackgroundMover : MonoBehaviour
{
    // public float XSpeed;
    // public float YSpeed;
    public float Speed;

    float t;

    void Update() {
        t += Speed * Time.deltaTime;
        t %= 2;
        transform.position = new Vector3(1, -1, 0) * t;
    }
}
