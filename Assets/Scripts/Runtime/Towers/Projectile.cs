using System.Collections.Generic;
using SteppeBastion.Data;
using SteppeBastion.Enemies;
using UnityEngine;

namespace SteppeBastion.Towers
{
    /// <summary>Arrow, stone or kobyz wave flying towards an enemy. Applies the tower's effect on impact.</summary>
    public class Projectile : MonoBehaviour
    {
        private static readonly List<Enemy> splashBuffer = new();

        [SerializeField] private SpriteRenderer sprite;
        [SerializeField] private Sprite arrowSprite;   // thin stretched square
        [SerializeField] private Sprite roundSprite;   // stone and kobyz wave
        [SerializeField] private HitEffect hitEffectPrefab;

        private Enemy target;
        private Vector3 targetPoint;
        private TowerData data;
        private float damage;
        private Vector3 start;
        private float travelled;
        private float arcHeight;

        public static void Launch(Projectile prefab, Vector3 from, Enemy target, TowerData data, float damage)
        {
            Projectile p = Instantiate(prefab, from, Quaternion.identity);
            p.target = target;
            p.targetPoint = target.Center;
            p.data = data;
            p.damage = damage;
            p.start = from;

            bool arrow = data.attack == AttackType.Arrow;
            p.sprite.sprite = arrow ? p.arrowSprite : p.roundSprite;
            p.sprite.color = data.projectileColor;
            p.transform.localScale = arrow
                ? new Vector3(data.projectileSize * 3f, data.projectileSize * 0.4f, 1f)
                : Vector3.one * data.projectileSize;
            p.arcHeight = data.attack == AttackType.Splash ? 1.2f : 0f; // catapult stones fly in an arc
        }

        private void Update()
        {
            // Follow the target while it is alive; otherwise fly to where it was.
            if (target != null && target.IsAlive)
                targetPoint = target.Center;

            // t goes from 0 (tower) to 1 (target). Position is a Lerp between them,
            // plus a parabola 4·t·(1−t) that is 0 at both ends and 1 in the middle.
            Vector3 previous = transform.position;
            float total = Vector3.Distance(start, targetPoint);
            travelled += data.projectileSpeed * Time.deltaTime;
            float t = total > 0.001f ? Mathf.Clamp01(travelled / total) : 1f;
            Vector3 next = Vector3.Lerp(start, targetPoint, t) + Vector3.up * (arcHeight * 4f * t * (1f - t));

            Vector3 direction = next - previous;
            if (direction.sqrMagnitude > 0.0001f)
                transform.right = direction;   // point the arrow where it flies
            transform.position = next;

            if (t >= 1f) Hit();
        }

        private void Hit()
        {
            switch (data.attack)
            {
                case AttackType.Splash:
                    // Copy the targets first: an enemy that dies can spawn new enemies and change the list.
                    splashBuffer.Clear();
                    foreach (Enemy enemy in Enemy.Active)
                        if (enemy.IsAlive && !enemy.Data.flying && Vector3.Distance(enemy.Center, targetPoint) <= data.splashRadius)
                            splashBuffer.Add(enemy);
                    foreach (Enemy enemy in splashBuffer)
                        enemy.TakeDamage(damage);
                    HitEffect.Create(hitEffectPrefab, targetPoint, data.splashRadius * 2f, data.projectileColor);
                    break;

                case AttackType.Slow:
                    if (target != null && target.IsAlive)
                    {
                        target.TakeDamage(damage);
                        target.ApplySlow(data.slowFactor, data.slowDuration);
                    }
                    HitEffect.Create(hitEffectPrefab, targetPoint, 0.6f, data.projectileColor);
                    break;

                default:
                    if (target != null && target.IsAlive)
                        target.TakeDamage(damage);
                    break;
            }
            Destroy(gameObject);
        }
    }
}
