using UnityEngine;
using System.Collections.Generic;

public class Juice : MonoBehaviour
{
    public float xScale;
    public float yScale;
    public float xFreq;
    public float yFreq;
    
    public float Speed;
    float t;

    List<Vector3> childOffsets;

    void Start() {
        childOffsets = new List<Vector3>();
        for (int i = 0; i < transform.childCount; i++) {
            childOffsets.Add(transform.GetChild(i).localPosition);
        }
    }

    void Update() {
        t += Time.deltaTime * Speed;
        for (int i = 0; i < transform.childCount; i++) {
            var child = transform.GetChild(i);
            var offset = new Vector2(xScale * Mathf.Cos(xFreq * (child.localPosition.x + t)), yScale * Mathf.Cos(yFreq * (child.localPosition.x + t)));
            child.localPosition = childOffsets[i].xy() + offset;
        }
    }
}
