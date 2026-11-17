using UnityEngine;
using UnityEngine.SceneManagement;

namespace SteppeBastion.Core
{
    public static class LevelProgress
    {
        public static readonly (string scene, string title)[] Levels =
        {
            ("Level_01", "Steppe Road"),
            ("Level_02", "Golden Steppe"),
        };

        public static int GetStars(string scene) => PlayerPrefs.GetInt("stars." + scene, 0);

        public static bool IsUnlocked(int index) => index == 0 || GetStars(Levels[index - 1].scene) > 0;

        /// <summary>Keeps the best result. Returns true if it is a new record.</summary>
        public static bool SaveStars(string scene, int stars)
        {
            if (stars <= GetStars(scene)) return false;
            PlayerPrefs.SetInt("stars." + scene, stars);
            PlayerPrefs.Save();
            return true;
        }

        public static int CurrentIndex()
        {
            string scene = SceneManager.GetActiveScene().name;
            for (int i = 0; i < Levels.Length; i++)
                if (Levels[i].scene == scene) return i;
            return -1;
        }

        public static string NextScene()
        {
            int next = CurrentIndex() + 1;
            return next > 0 && next < Levels.Length ? Levels[next].scene : null;
        }
    }
}
