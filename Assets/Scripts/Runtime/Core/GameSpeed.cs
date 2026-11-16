using System;
using UnityEngine;

namespace SteppeBastion.Core
{
    /// <summary>Pause and x1/x2 speed, both applied through Time.timeScale.</summary>
    public static class GameSpeed
    {
        public static bool Paused { get; private set; }
        public static bool Fast { get; private set; }

        public static event Action Changed;

        public static void Reset()
        {
            Paused = false;
            Fast = false;
            Apply();
        }

        public static void SetPaused(bool paused)
        {
            Paused = paused;
            Apply();
        }

        public static void ToggleFast()
        {
            Fast = !Fast;
            Apply();
        }

        private static void Apply()
        {
            Time.timeScale = Paused ? 0f : Fast ? 2f : 1f;
            Changed?.Invoke();
        }
    }
}
