using UnityEngine;

namespace SteppeBastion.Data
{
    [CreateAssetMenu(menuName = "Steppe Bastion/Enemy", fileName = "Enemy_")]
    public class EnemyData : ScriptableObject
    {
        public string displayName;
        public Sprite sprite;
        public float scale = 1f;

        [Header("Stats")]
        public float maxHealth = 10f;
        [Tooltip("World units per second.")]
        public float speed = 1.5f;
        public int reward = 5;
        [Tooltip("Lives the player loses if this enemy reaches Ak Orda.")]
        public int livesDamage = 1;
        public bool flying;
        public bool isBoss;

        [Header("On death")]
        [Tooltip("Enemy spawned when this one dies (Mystan splits into spirits).")]
        public EnemyData splitInto;
        public int splitCount;
    }
}
