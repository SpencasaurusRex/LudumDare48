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

    void Update() {
        for (int i = 0; i < Rate; i++) {
            
            var pos = transform.position + Vector3.up * Random.Range(-YVariation, YVariation);

            var life = Instantiate(BreakingParticlesPrefab, pos, Quaternion.identity);
            life.GetComponent<SpriteRenderer>().sprite = BreakingParticlesSprites[(int)BlockColor];
            life.GetComponent<Rigidbody2D>().velocity = Force.WithZ(0);

            float r = Random.Range(0, 1f);
            float k = (1 - SmallerBias) * (1 - SmallerBias) * (1 - SmallerBias);
            int size = (int)((r * k * 4) / (r * k - r + 1)) + 1;
            life.transform.localScale = Vector3.one * size;
            life.DefaultScale = size;

            life.Amount = size * 0.25f;
        }
    }

}
