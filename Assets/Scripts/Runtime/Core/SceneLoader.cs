using UnityEngine.SceneManagement;

namespace SteppeBastion.Core
{
    public static class SceneLoader
    {
        public const string MainMenuScene = "MainMenu";

        public static void RestartLevel()
        {
            GameSpeed.Reset();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public static void LoadMainMenu() => Load(MainMenuScene);

        public static void Load(string sceneName)
        {
            GameSpeed.Reset();
            SceneManager.LoadScene(sceneName);
        }
    }
}
