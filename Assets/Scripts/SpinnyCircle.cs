using UnityEngine;

public class SpinnyCircle : MonoBehaviour
{
    public float RadiusSmall;
    public float RadiusLarge;
    public float RadiusSpeed;

    public float XScale;
    public float YScale;

    public float Speed;

    float spinT;
    float expandT;

    void Update() {
        expandT += Time.deltaTime * RadiusSpeed;
        var radius = Mathf.Lerp(RadiusSmall, RadiusLarge, Mathf.Sin(expandT) * 0.5f + 0.5f);

        spinT += Time.deltaTime * Speed;
        float deltaTheta = Mathf.PI * 2 / transform.childCount;
        float theta = spinT;
        for (int i = 0; i < transform.childCount; i++) {
            theta += deltaTheta;
            theta %= 2 * Mathf.PI;
            var pos = transform.position + new Vector3(Mathf.Cos(theta) * XScale, Mathf.Sin(theta) * YScale, 0) * radius;
            var child = transform.GetChild(i);
            child.position = pos;
            child.GetComponent<SpriteRenderer>().sortingOrder = theta < Mathf.PI ? 0 : 2;
        }
    }
}
