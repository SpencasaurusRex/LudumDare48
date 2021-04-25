using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform Follow;
    public float LerpStrength = 0.05f;
    public float LeftLimit;
    public float RightLimit;

    Camera cam;

    void Start() {
        cam = GetComponent<Camera>();
    }

    void Update() {
        if (Follow) {
            float halfHeight = cam.orthographicSize;
            float halfWidth = cam.aspect * halfHeight;
            var leftMost = LeftLimit + halfWidth;
            var rightMost = RightLimit - halfWidth;
            if (leftMost > rightMost) {
                rightMost = leftMost = (RightLimit + LeftLimit) * 0.5f;
            }

            transform.position = Vector3.Lerp(transform.position, Follow.position.WithZ(transform.position.z), LerpStrength);
            transform.position = transform.position.WithX(Mathf.Clamp(transform.position.x, leftMost, rightMost)); 
        }
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(LeftLimit, -100, 0), new Vector3(LeftLimit, 100, 0));
        Gizmos.DrawLine(new Vector3(RightLimit, -100, 0), new Vector3(RightLimit, 100, 0));
    }
}
