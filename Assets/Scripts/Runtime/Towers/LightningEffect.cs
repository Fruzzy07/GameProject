using System.Collections.Generic;
using UnityEngine;

namespace SteppeBastion.Towers
{
    /// <summary>A short zigzag lightning bolt between two points, built from thin segment sprites.</summary>
    public class LightningEffect : MonoBehaviour
    {
        private const float Duration = 0.18f;
        private const int Segments = 6;

        [SerializeField] private SpriteRenderer segmentPrefab;

        private readonly List<SpriteRenderer> parts = new();
        private Color color;
        private float age;

        public static void Create(LightningEffect prefab, Vector3 from, Vector3 to, Color color)
        {
            LightningEffect bolt = Instantiate(prefab);
            bolt.color = color;

            // Split the line into segments and push the middle points sideways a random amount.
            Vector3 side = Vector3.Cross(to - from, Vector3.forward).normalized;
            Vector3 previous = from;
            for (int i = 1; i <= Segments; i++)
            {
                Vector3 point = Vector3.Lerp(from, to, (float)i / Segments);
                if (i < Segments) point += side * Random.Range(-0.18f, 0.18f);
                bolt.AddSegment(previous, point);
                previous = point;
            }
        }

        private void AddSegment(Vector3 a, Vector3 b)
        {
            SpriteRenderer segment = Instantiate(segmentPrefab, transform);
            segment.transform.position = (a + b) / 2f;
            segment.transform.right = b - a;
            segment.transform.localScale = new Vector3(Vector3.Distance(a, b) + 0.03f, 0.07f, 1f);
            parts.Add(segment);
        }

        private void Update()
        {
            age += Time.deltaTime;
            float alpha = 1f - age / Duration;
            foreach (SpriteRenderer part in parts)
                part.color = new Color(color.r, color.g, color.b, alpha);
            if (alpha <= 0f) Destroy(gameObject);
        }
    }
}
