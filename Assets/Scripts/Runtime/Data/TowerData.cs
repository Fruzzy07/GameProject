using UnityEngine;

namespace SteppeBastion.Data
{
    public enum AttackType
    {
        Arrow,  // single target
        Splash, // area damage on impact
        Slow,   // damage + slow
        Chain   // instant lightning that jumps between enemies
    }

    [System.Serializable]
    public struct TowerLevel
    {
        [Tooltip("Gold to build (level 1) or to upgrade to this level.")]
        public int cost;
        public float damage;
        public float range;
        [Tooltip("Seconds between attacks.")]
        public float fireInterval;
    }

    [CreateAssetMenu(menuName = "Steppe Bastion/Tower", fileName = "Tower_")]
    public class TowerData : ScriptableObject
    {
        public string displayName;
        [TextArea] public string description;
        public Sprite sprite;
        public AttackType attack;
        public bool canHitFlying = true;
        public TowerLevel[] levels = new TowerLevel[3];

        [Header("Projectile")]
        public Color projectileColor = Color.white;
        public float projectileSpeed = 8f;
        public float projectileSize = 0.15f;

        [Header("Effects")]
        public float splashRadius = 1f;
        [Range(0f, 1f)] public float slowFactor = 0.5f;
        public float slowDuration = 1.5f;
        public int chainTargets = 3;
        public float chainRange = 1.8f;

        public int MaxLevel => levels.Length;
        public int BuildCost => levels[0].cost;

        /// <summary>Gold returned when a tower of the given level (0-based) is sold.</summary>
        public int SellValue(int level)
        {
            int spent = 0;
            for (int i = 0; i <= level && i < levels.Length; i++)
                spent += levels[i].cost;
            return Mathf.RoundToInt(spent * 0.6f);
        }
    }
}
