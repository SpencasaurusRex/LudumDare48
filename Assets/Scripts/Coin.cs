using UnityEngine;

public class Coin : MonoBehaviour
{
    Animator animator;
    Rigidbody2D rb;

    void Start() {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        float theta = Random.Range(Mathf.PI / 4, 3 * Mathf.PI / 4);
        float speed = Random.Range(1, 3);
        rb.velocity = new Vector2(Mathf.Cos(theta), Mathf.Sin(theta)) * speed;
    }

    void Update() {
        var location = transform.position.xy().RoundToInt();
        var go = GridManager.Instance.GetGridObject(location);
        if (go && !go.Falling) {
            Destroy(this.gameObject);
        }
        if (rb.velocity.magnitude < 0.05f) {
            animator.SetTrigger("Flash");
        }
    }
}
