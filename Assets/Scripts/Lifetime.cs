using UnityEngine;

public class Lifetime : MonoBehaviour
{
    // Configuration
    public float Amount;

    public bool FadeAlpha;
    public bool FadeScale;
    public float DefaultScale;

    // Runtime
    float t;
    SpriteRenderer sr;

    void Start() {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update() {
        t += Time.deltaTime;
        
        float percent = 1f - (t / Amount);

        if (FadeAlpha) {
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, percent);
        }
        if (FadeScale) {
            transform.localScale = Vector3.one * percent * DefaultScale;
        }

        if (t >= Amount) {
            Destroy(this.gameObject);
        }
    }
}
