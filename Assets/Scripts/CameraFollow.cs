using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform Follow;
    public float LerpStrength = 0.05f;
    public float LeftLimit;
    public float RightLimit;

    public bool MatchY = true;
    PlayerController player;
    Camera cam;

    void Start() {
        cam = GetComponent<Camera>();
        player = FindObjectOfType<PlayerController>();
    }

    float lerpStrength;

    void LateUpdate() {
        if (player.victory) {
            lerpStrength = 0.1f * LerpStrength;
        }
        else {
            lerpStrength = Mathf.Clamp(lerpStrength + Time.deltaTime * LerpStrength, 0, LerpStrength);
        }

        if (Follow) {
            float halfHeight = cam.orthographicSize;
            float halfWidth = cam.aspect * halfHeight;
            var leftMost = LeftLimit + halfWidth;
            var rightMost = RightLimit - halfWidth;
            if (leftMost > rightMost) {
                rightMost = leftMost = (RightLimit + LeftLimit) * 0.5f;
            }

            var followPosition = Follow.position.WithZ(transform.position.z);
            if (player.victory) {
                followPosition = followPosition.WithX(followPosition.x + halfWidth * 0.5f);
            }
            if (!player.victory && !player.dead && lerpStrength == LerpStrength) {
                followPosition = followPosition.WithX(Mathf.Clamp(followPosition.x, leftMost, rightMost));    
            }
            
            transform.position = Vector3.Lerp(transform.position, followPosition, lerpStrength);

            if (MatchY) {
                transform.position = transform.position.WithY(Follow.position.y);
            }
        }
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(LeftLimit, -100, 0), new Vector3(LeftLimit, 100, 0));
        Gizmos.DrawLine(new Vector3(RightLimit, -100, 0), new Vector3(RightLimit, 100, 0));
    }
}
