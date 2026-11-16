using System.Collections.Generic;
using SteppeBastion.Core;
using SteppeBastion.Data;
using UnityEngine;

namespace SteppeBastion.Enemies
{
    /// <summary>
    /// A creature walking along the road towards Ak Orda. All enemies use one prefab;
    /// the EnemyData asset decides its sprite, size and stats.
    /// </summary>
    public class Enemy : MonoBehaviour
    {
        private const float BaseSize = 1.1f;    // world width of an enemy with scale 1
        private const float FlyingHeight = 0.35f;

        private static readonly List<Enemy> active = new();
        public static IReadOnlyList<Enemy> Active => active;

        [SerializeField] private Transform visual;      // scaled to the enemy's size, lifted for flying enemies
        [SerializeField] private SpriteRenderer body;
        [SerializeField] private HealthBar healthBar;

        private Enemy prefab;
        private EnemyPath path;
        private int nextPoint;
        private float health;
        private float slowFactor = 1f;
        private float slowTimer;
        private float flashTimer;

        public EnemyData Data { get; private set; }
        public bool IsAlive => health > 0f;
        /// <summary>How far along the road the enemy is; towers attack the one closest to the end.</summary>
        public float Progress { get; private set; }
        /// <summary>World position of the enemy's body center, used for aiming.</summary>
        public Vector3 Center => visual.position + Vector3.up * (BaseSize * Data.scale * 0.45f);

        public static Enemy Spawn(Enemy prefab, EnemyData data, EnemyPath path) =>
            Spawn(prefab, data, path, path[0], 1, 0f);

        public static Enemy Spawn(Enemy prefab, EnemyData data, EnemyPath path, Vector3 position, int nextPoint, float progress)
        {
            Enemy enemy = Instantiate(prefab, position, Quaternion.identity);
            enemy.name = data.displayName;
            enemy.Init(prefab, data, path, nextPoint, progress);
            return enemy;
        }

        private void Init(Enemy source, EnemyData data, EnemyPath enemyPath, int next, float progress)
        {
            prefab = source;
            Data = data;
            path = enemyPath;
            nextPoint = next;
            Progress = progress;
            health = data.maxHealth;

            float size = BaseSize * data.scale;
            body.sprite = data.sprite;
            visual.localScale = Vector3.one * (size / data.sprite.bounds.size.x);
            visual.localPosition = data.flying ? Vector3.up * FlyingHeight : Vector3.zero;
            healthBar.Place(size + (data.flying ? FlyingHeight + 0.1f : 0.1f), Mathf.Max(0.6f, size * 0.6f));
        }

        private void OnEnable() => active.Add(this);
        private void OnDisable() => active.Remove(this);

        private void Update()
        {
            Move();

            if (slowTimer > 0f)
            {
                slowTimer -= Time.deltaTime;
                if (slowTimer <= 0f) slowFactor = 1f;
            }

            if (flashTimer > 0f) flashTimer -= Time.deltaTime;
            if (flashTimer > 0f) body.color = new Color(1f, 0.6f, 0.6f);
            else if (slowFactor < 1f) body.color = new Color(0.7f, 0.85f, 1f);
            else body.color = Color.white;
        }

        private void Move()
        {
            float step = Data.speed * slowFactor * Time.deltaTime;
            while (step > 0f && nextPoint < path.Count)
            {
                Vector3 target = path[nextPoint];
                Vector3 delta = target - transform.position;
                float distance = delta.magnitude;
                if (Mathf.Abs(delta.x) > 0.01f)
                    body.flipX = delta.x < 0f;

                if (distance <= step)
                {
                    transform.position = target;
                    Progress += distance;
                    step -= distance;
                    nextPoint++;
                }
                else
                {
                    transform.position += delta / distance * step;
                    Progress += step;
                    step = 0f;
                }
            }

            if (nextPoint >= path.Count)
                ReachBase();
        }

        public void TakeDamage(float amount)
        {
            if (!IsAlive) return;
            health -= amount;
            flashTimer = 0.08f;
            healthBar.SetValue(health / Data.maxHealth);
            if (health <= 0f) Die();
        }

        public void ApplySlow(float factor, float duration)
        {
            slowFactor = Mathf.Min(slowFactor, factor);
            slowTimer = Mathf.Max(slowTimer, duration);
        }

        private void Die()
        {
            LevelManager.Instance.AddGold(Data.reward);

            // Mystan splits into spirits that continue from the same place on the road.
            for (int i = 0; i < Data.splitCount && Data.splitInto != null; i++)
            {
                Vector3 offset = Random.insideUnitCircle * 0.3f;
                Spawn(prefab, Data.splitInto, path, transform.position + offset, nextPoint, Progress - i * 0.05f);
            }
            Destroy(gameObject);
        }

        private void ReachBase()
        {
            health = 0f;
            LevelManager.Instance.LoseLives(Data.livesDamage);
            Destroy(gameObject);
        }
    }
}
