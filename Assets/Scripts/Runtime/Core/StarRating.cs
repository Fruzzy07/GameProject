using UnityEngine;

namespace SteppeBastion.Core
{
    /// <summary>Stars for a won level, based on the lives the player kept.</summary>
    public static class StarRating
    {
        public const int MaxStars = 3;

        /// <summary>
        /// 90% of lives kept or more: 3 stars, 50% or more: 2 stars, otherwise 1.
        /// With the usual 20 lives that is 18+ lives for 3 stars and 10+ for 2.
        /// </summary>
        public static int ForLives(int livesLeft, int startLives)
        {
            if (startLives <= 0) return 1;
            float kept = (float)livesLeft / startLives;
            if (kept >= 0.9f) return 3;
            if (kept >= 0.5f) return 2;
            return 1;
        }

        public static int ForCurrentLevel()
        {
            LevelManager manager = LevelManager.Instance;
            return ForLives(manager.Lives, manager.Level.startLives);
        }
    }
}
