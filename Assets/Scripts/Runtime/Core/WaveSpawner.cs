using System;
using System.Collections;
using SteppeBastion.Audio;
using SteppeBastion.Data;
using SteppeBastion.Enemies;
using UnityEngine;

namespace SteppeBastion.Core
{
    /// <summary>Spawns the level's waves on a timer. The player can call the next wave early for bonus gold.</summary>
    public class WaveSpawner : MonoBehaviour
    {
        [SerializeField] private Enemy enemyPrefab;

        private LevelManager manager;
        private int spawningGroups;

        /// <summary>Number of waves started so far (0 before the first wave).</summary>
        public int CurrentWave { get; private set; }
        public int TotalWaves => manager.Level.waves.Length;
        /// <summary>Seconds until the next wave, or a negative value when no wave is queued.</summary>
        public float Countdown { get; private set; } = -1f;
        public bool CanCallEarly => Countdown > 0f && manager.State == GameState.Playing;
        public int EarlyCallBonus => Mathf.CeilToInt(Mathf.Max(0f, Countdown)) * manager.Level.earlyCallGoldPerSecond;

        public event Action<int> WaveStarted;

        private void Start()
        {
            manager = LevelManager.Instance;
            Countdown = manager.Level.firstWaveDelay;
        }

        private void Update()
        {
            if (manager.State != GameState.Playing) return;

            if (Countdown > 0f)
            {
                Countdown -= Time.deltaTime;
                if (Countdown <= 0f) StartNextWave();
            }
            else if (CurrentWave == TotalWaves && spawningGroups == 0 && Enemy.Active.Count == 0)
            {
                manager.Win();
            }
        }

        public void CallEarly()
        {
            if (!CanCallEarly) return;
            manager.AddGold(EarlyCallBonus);
            StartNextWave();
        }

        private void StartNextWave()
        {
            Countdown = -1f;
            Wave wave = manager.Level.waves[CurrentWave];
            CurrentWave++;
            foreach (SpawnGroup group in wave.groups)
                StartCoroutine(SpawnGroup(group));
            WaveStarted?.Invoke(CurrentWave);
            GameAudio.Play(s => s.waveStart, 0.6f, 0f);
        }

        private IEnumerator SpawnGroup(SpawnGroup group)
        {
            spawningGroups++;
            yield return new WaitForSeconds(group.startDelay);
            for (int i = 0; i < group.count; i++)
            {
                Enemy.Spawn(enemyPrefab, group.enemy, manager.Path);
                yield return new WaitForSeconds(group.interval);
            }
            spawningGroups--;

            // The timer for the next wave starts once the current wave has fully spawned.
            if (spawningGroups == 0 && CurrentWave < TotalWaves && Countdown < 0f)
                Countdown = manager.Level.timeBetweenWaves;
        }
    }
}
