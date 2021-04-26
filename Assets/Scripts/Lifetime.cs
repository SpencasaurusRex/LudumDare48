using UnityEngine;
using UnityEngine.UI;

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
    Image image;
    float speed;

    void Start() {
        sr = GetComponent<SpriteRenderer>();
        image = GetComponent<Image>();
        speed = 1 + Random.Range(-RandomVariation, RandomVariation);
    }

    void Update() {
        t += Time.deltaTime * speed;
        
        float percent = 1f - (t / Amount);
        float aboveCurve = -(t / Amount) * (t / Amount) + 1;

        if (FadeAlpha && sr) {
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, aboveCurve);
        }
        if (FadeAlpha && image) {
            image.color = new Color(image.color.r, image.color.g, image.color.b, aboveCurve);
        }
        if (FadeScale) {
            transform.localScale = Vector3.one * aboveCurve * DefaultScale;
        }

        if (t >= Amount) {
            Destroy(this.gameObject);
        }
    }
}
