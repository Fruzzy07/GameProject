using UnityEngine.SceneManagement;

namespace SteppeBastion.Core
{
    public static class SceneLoader
    {
        public const string MainMenuScene = "MainMenu";
        public const string FirstLevelScene = "Level_01";

        public static void RestartLevel()
        {
            GameSpeed.Reset();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public static void LoadMainMenu() => Load(MainMenuScene);

        /// <summary>Loads a scene by name. The scene must be listed in Build Settings.</summary>
        public static void Load(string sceneName)
        {
            // A level can be left while paused or after victory, both of which stop time.
            GameSpeed.Reset();
            SceneManager.LoadScene(sceneName);
        }
    }
}
