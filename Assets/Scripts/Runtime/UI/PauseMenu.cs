using SteppeBastion.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SteppeBastion.UI
{
    /// <summary>
    /// Pause window: shown while the game is paused during play. Esc (handled by PlayerInputController)
    /// toggles the pause, so this window only reacts to GameSpeed.
    /// </summary>
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private GameObject window;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button mainMenuButton;

        private void Awake()
        {
            resumeButton.onClick.AddListener(() => GameSpeed.SetPaused(false));
            restartButton.onClick.AddListener(SceneLoader.RestartLevel);
            mainMenuButton.onClick.AddListener(SceneLoader.LoadMainMenu);
            window.SetActive(false);
        }

        private void OnEnable() => GameSpeed.Changed += Refresh;
        private void OnDisable() => GameSpeed.Changed -= Refresh;

        private void Refresh()
        {
            // Victory and defeat also pause the game; the result screen handles those.
            bool show = GameSpeed.Paused && LevelManager.Instance != null && LevelManager.Instance.State == GameState.Playing;
            if (show == window.activeSelf) return;

            window.SetActive(show);
            if (show && EventSystem.current != null)
                EventSystem.current.SetSelectedGameObject(resumeButton.gameObject);
        }
    }
}
