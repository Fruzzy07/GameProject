using UnityEngine;

namespace SteppeBastion.Towers
{
    /// <summary>A disc that grows and fades out, shown where a stone, kobyz wave or the berkut lands.</summary>
    public class HitEffect : MonoBehaviour
    {
        private const float Duration = 0.3f;

        [SerializeField] private SpriteRenderer sprite;

        private Color color;
        private float size;
        private float age;

        public static void Create(HitEffect prefab, Vector3 position, float size, Color color)
        {
            HitEffect effect = Instantiate(prefab, position, Quaternion.identity);
            effect.color = color;
            effect.size = size;
        }

        private void Update()
        {
            age += Time.deltaTime;
            float t = age / Duration;
            transform.localScale = Vector3.one * (size * Mathf.Lerp(0.4f, 1f, t));
            sprite.color = new Color(color.r, color.g, color.b, 0.6f * (1f - t));
            if (t >= 1f) Destroy(gameObject);
        }
    }
}
