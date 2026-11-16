using System;
using SteppeBastion.Data;
using UnityEngine;

namespace SteppeBastion.Core
{
    public enum GameState { Playing, Won, Lost }

    /// <summary>Owns the level's gold, lives and win/lose state. One per level scene.</summary>
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance { get; private set; }

        [SerializeField] private LevelData level;
        [SerializeField] private EnemyPath path;

        public LevelData Level => level;
        public EnemyPath Path => path;
        public int Gold { get; private set; }
        public int Lives { get; private set; }
        public GameState State { get; private set; } = GameState.Playing;

        public event Action<int> GoldChanged;
        public event Action<int> LivesChanged;
        public event Action<GameState> StateChanged;

        private void Awake()
        {
            Instance = this;
            Gold = level.startGold;
            Lives = level.startLives;
            GameSpeed.Reset();
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        public bool CanAfford(int amount) => Gold >= amount;

        public bool TrySpend(int amount)
        {
            if (Gold < amount) return false;
            Gold -= amount;
            GoldChanged?.Invoke(Gold);
            return true;
        }

        public void AddGold(int amount)
        {
            Gold += amount;
            GoldChanged?.Invoke(Gold);
        }

        public void LoseLives(int amount)
        {
            if (State != GameState.Playing) return;
            Lives = Mathf.Max(0, Lives - amount);
            LivesChanged?.Invoke(Lives);
            if (Lives == 0) SetState(GameState.Lost);
        }

        public void Win()
        {
            if (State == GameState.Playing) SetState(GameState.Won);
        }

        private void SetState(GameState state)
        {
            State = state;
            GameSpeed.SetPaused(true);
            StateChanged?.Invoke(state);
        }
    }
}
