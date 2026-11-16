using System.Collections.Generic;
using SteppeBastion.Core;
using SteppeBastion.Data;
using SteppeBastion.Enemies;
using UnityEngine;

namespace SteppeBastion.Towers
{
    /// <summary>
    /// A built tower: finds the enemy furthest along the road and attacks it. All towers use one prefab;
    /// the TowerData asset decides its sprite, attack type and stats.
    /// </summary>
    public class Tower : MonoBehaviour
    {
        private const float Size = 1.35f;        // world height of a level 1 tower
        private const float GrowPerLevel = 0.08f;
        public const float PackTime = 2f;        // seconds a tower is packed while moving (Kosh)

        private static readonly List<Enemy> chainBuffer = new();

        [SerializeField] private Transform visual;      // scaled with the tower level
        [SerializeField] private SpriteRenderer body;
        [SerializeField] private SpriteRenderer range;

        [Header("Attacks")]
        [SerializeField] private Projectile projectilePrefab;
        [SerializeField] private LightningEffect lightningPrefab;

        private float cooldown;
        private float packedTimer;

        public TowerData Data { get; private set; }
        public int Level { get; private set; }
        public BuildSpot Spot { get; set; }
        public TowerLevel Stats => Data.levels[Level];
        public bool CanUpgrade => Level < Data.MaxLevel - 1;
        public int UpgradeCost => CanUpgrade ? Data.levels[Level + 1].cost : 0;
        public int SellValue => Data.SellValue(Level);
        public bool IsPacked => packedTimer > 0f;

        public void Init(TowerData data)
        {
            Data = data;
            name = data.displayName;
            body.sprite = data.sprite;
            range.enabled = false;
            ApplyLevel();
        }

        public void ShowRange(bool on) => range.enabled = on;

        public void Upgrade()
        {
            if (!CanUpgrade) return;
            Level++;
            ApplyLevel();
        }

        public void Sell()
        {
            LevelManager.Instance.AddGold(SellValue);
            Spot.Clear();
            Destroy(gameObject);
        }

        /// <summary>Kosh: pack the tower up and move it to another free spot. It can't attack while packed.</summary>
        public void MoveTo(BuildSpot target)
        {
            Spot.Clear();
            target.Place(this);
            packedTimer = PackTime;
        }

        private void ApplyLevel()
        {
            float size = Size * (1f + GrowPerLevel * Level);
            visual.localScale = Vector3.one * (size / Data.sprite.bounds.size.y);
            range.transform.localScale = Vector3.one * (Stats.range * 2f);
        }

        private void Update()
        {
            if (packedTimer > 0f)
            {
                // Packed towers blink while they are being set up again.
                packedTimer -= Time.deltaTime;
                body.color = new Color(1f, 1f, 1f, 0.45f + 0.25f * Mathf.Sin(Time.time * 12f));
                return;
            }
            body.color = Color.white;

            cooldown -= Time.deltaTime;
            if (cooldown > 0f) return;

            Enemy target = FindTarget();
            if (target == null) return;

            Attack(target);
            cooldown = Stats.fireInterval;
        }

        private Enemy FindTarget()
        {
            Enemy best = null;
            float rangeSqr = Stats.range * Stats.range;
            foreach (Enemy enemy in Enemy.Active)
            {
                if (!enemy.IsAlive || (enemy.Data.flying && !Data.canHitFlying)) continue;
                if ((enemy.transform.position - transform.position).sqrMagnitude > rangeSqr) continue;
                if (best == null || enemy.Progress > best.Progress) best = enemy;
            }
            return best;
        }

        private Vector3 Muzzle => transform.position + Vector3.up * (Size * 0.8f);

        private void Attack(Enemy target)
        {
            if (Data.attack == AttackType.Chain)
                ChainLightning(target);
            else
                Projectile.Launch(projectilePrefab, Muzzle, target, Data, Stats.damage);
        }

        /// <summary>Balbal: lightning hits the target, then jumps to the nearest enemies it hasn't hit yet.</summary>
        private void ChainLightning(Enemy first)
        {
            chainBuffer.Clear();
            Vector3 from = Muzzle;
            Enemy current = first;
            float damage = Stats.damage;

            for (int i = 0; i < Data.chainTargets && current != null; i++)
            {
                chainBuffer.Add(current);
                LightningEffect.Create(lightningPrefab, from, current.Center, Data.projectileColor);
                from = current.Center;
                current = NearestNotHit(from, Data.chainRange);
            }

            // Every jump does 25% less damage.
            foreach (Enemy enemy in chainBuffer)
            {
                enemy.TakeDamage(damage);
                damage *= 0.75f;
            }
        }

        private static Enemy NearestNotHit(Vector3 from, float maxDistance)
        {
            Enemy best = null;
            float bestSqr = maxDistance * maxDistance;
            foreach (Enemy enemy in Enemy.Active)
            {
                if (!enemy.IsAlive || chainBuffer.Contains(enemy)) continue;
                float sqr = (enemy.Center - from).sqrMagnitude;
                if (sqr < bestSqr)
                {
                    bestSqr = sqr;
                    best = enemy;
                }
            }
            return best;
        }
    }
}
