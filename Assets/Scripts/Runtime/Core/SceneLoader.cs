using UnityEngine.SceneManagement;

namespace SteppeBastion.Core
{
    public static class SceneLoader
    {
        public static void RestartLevel()
        {
            GameSpeed.Reset();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
