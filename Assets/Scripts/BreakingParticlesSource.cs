using UnityEngine;

public class BreakingParticlesSource : MonoBehaviour
{
    public Sprite[] BreakingParticlesSprites;

    public Lifetime BreakingParticlesPrefab;
    public float Rate; // Number per frame
    public float SmallerBias = 0.4f;

    public BlockColor BlockColor;
    public Vector2 Force;
    public float YVariation;
    public float WaitTime;
    public float Chance;
    float t;

    void Update() {
        t += Time.deltaTime;
        if (t >= WaitTime) {
            for (int i = 0; i < Rate; i++) {
                if (Random.Range(0, 1f) > Chance) continue;
                var pos = transform.position + Vector3.up * Random.Range(-YVariation, YVariation);

                var life = Instantiate(BreakingParticlesPrefab, pos, Quaternion.identity);
                var sr = life.GetComponent<SpriteRenderer>();
                sr.sprite = BreakingParticlesSprites[(int)BlockColor];
                sr.sortingOrder = Random.Range(0, 1f) < 0.5f ? 3 : 0;
                
                float o = 1f;
                Vector2 offset = new Vector2(Random.Range(-o, o), Random.Range(-o, o));
                life.GetComponent<Rigidbody2D>().velocity = (Force + offset).WithZ(0);

                float r = Random.Range(0, 1f);
                float k = (1 - SmallerBias) * (1 - SmallerBias) * (1 - SmallerBias);
                int size = (int)((r * k * 4) / (r * k - r + 1)) + 3;
                life.transform.localScale = Vector3.one * size;
                life.DefaultScale = size;

                life.Amount = size * 0.25f;
            }
        }
    }

}
