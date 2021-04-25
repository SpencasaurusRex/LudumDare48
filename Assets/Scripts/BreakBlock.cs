using UnityEngine;

public class BreakBlock : MonoBehaviour
{
    public Sprite[] BreakBlockSprites;
    public BlockColor BlockColor;
    public float ForceMagnify;

    public void Setup() {
        for (int i = 0; i < 4; i++) {
            var child = transform.GetChild(i);
            child.GetComponent<SpriteRenderer>().sprite = BreakBlockSprites[(int)BlockColor];
            child.GetComponent<Rigidbody2D>().velocity = (child.position - transform.position) * ForceMagnify;
        }
    } 
}
