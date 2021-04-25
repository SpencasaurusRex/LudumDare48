using UnityEngine;
public class Follow : MonoBehaviour {
    public Transform Following;

    public float LerpSpeed = 1;

    void Update() {
        if (Following) {
            transform.position = Vector3.Lerp(transform.position, Following.position, LerpSpeed);
        }
    }
}