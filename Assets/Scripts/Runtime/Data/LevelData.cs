using System;
using UnityEngine;

namespace SteppeBastion.Data
{
    [Serializable]
    public class SpawnGroup
    {
        public EnemyData enemy;
        public int count = 5;
        [Tooltip("Seconds between enemies in this group.")]
        public float interval = 0.8f;
        [Tooltip("Seconds after the wave starts before this group spawns.")]
        public float startDelay;
    }

    [Serializable]
    public class Wave
    {
        public SpawnGroup[] groups;
    }

    [CreateAssetMenu(menuName = "Steppe Bastion/Level", fileName = "Level_")]
    public class LevelData : ScriptableObject
    {
        public string displayName;
        public int startGold = 220;
        public int startLives = 20;
        public float firstWaveDelay = 15f;
        public float timeBetweenWaves = 12f;
        [Tooltip("Bonus gold per second left on the timer when a wave is called early.")]
        public int earlyCallGoldPerSecond = 2;
        public TowerData[] towers;
        public Wave[] waves;
    }
}
