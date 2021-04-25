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
        // Don't look at this
        transform.position = ((Camera.main.transform.position.xy() * 0.5f).WithZ(0).RoundToInt() * 2).ToFloat() + new Vector3(1, -1, 0) * t;
    }
}
