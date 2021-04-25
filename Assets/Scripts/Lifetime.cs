using UnityEngine;

public class Lifetime : MonoBehaviour
{
    // Configuration
    public float Amount;

    public bool FadeAlpha;
    public bool FadeScale;
    public float DefaultScale;
    public float RandomVariation = .1f;

    // Runtime
    float t;
    SpriteRenderer sr;
    float speed;

    void Start() {
        sr = GetComponent<SpriteRenderer>();
        speed = 1 + Random.Range(-RandomVariation, RandomVariation);
    }

    void Update() {
        t += Time.deltaTime * speed;
        
        float percent = 1f - (t / Amount);
        float aboveCurve = -(t / Amount) * (t / Amount) + 1;

        if (FadeAlpha) {
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, aboveCurve);
        }
        if (FadeScale) {
            transform.localScale = Vector3.one * aboveCurve * DefaultScale;
        }

        if (t >= Amount) {
            Destroy(this.gameObject);
        }
    }
}
