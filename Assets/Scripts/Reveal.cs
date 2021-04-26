using UnityEngine;

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
                child.GetComponent<SpriteRenderer>().enabled = true;
            }
        }
    }

    public void Hide() {
        revealing = false;
        t = 0;

        for (int i = 0; i < transform.childCount; i++) {
            transform.GetChild(i).GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    public void StartReveal() {
        revealing = true;
    }
}
