using UnityEngine;
using UnityEngine.UI;

public class Reveal : MonoBehaviour
{
    public float Speed;
    float t;
    bool revealing;

    void Update() {
        if (revealing) {
            t += Time.deltaTime * Speed;
            for (int i = 0; i < t && i < transform.childCount; i++) {
                var child = transform.GetChild(i);
                var img = child.GetComponent<Image>();
                if (img) {
                    img.enabled = true;
                }
                else {
                    child.GetComponent<SpriteRenderer>().enabled = true;
                }
            }
        }
    }

    public void Hide() {
        if (!revealing) return;
        revealing = false;
        t = 0;

        for (int i = 0; i < transform.childCount; i++) {
            var img = transform.GetChild(i).GetComponent<Image>();
            if (img) {
                img.enabled = true;
            }
            else {
                transform.GetChild(i).GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }

    public void StartReveal() {
        revealing = true;
    }
}
