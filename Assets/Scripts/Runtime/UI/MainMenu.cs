using SteppeBastion.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace SteppeBastion.UI
{
    /// <summary>Main menu screen: start the game, open settings, quit.</summary>
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private Button playButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;

        [Header("Settings window")]
        [SerializeField] private GameObject settingsWindow;
        [SerializeField] private Button settingsBackButton;

        private void Awake()
        {
            playButton.onClick.AddListener(Play);
            settingsButton.onClick.AddListener(OpenSettings);
            quitButton.onClick.AddListener(Quit);
            settingsBackButton.onClick.AddListener(CloseSettings);

            // Quitting makes no sense in a browser tab.
            quitButton.gameObject.SetActive(Application.platform != RuntimePlatform.WebGLPlayer);
            settingsWindow.SetActive(false);
        }

        private void Start() => Select(playButton);

        private void Update()
        {
            if (settingsWindow.activeSelf && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
                CloseSettings();
        }

        private void Play() => SceneLoader.Load(SceneLoader.FirstLevelScene);

        private void OpenSettings()
        {
            settingsWindow.SetActive(true);
            Select(settingsBackButton);
        }

        private void CloseSettings()
        {
            settingsWindow.SetActive(false);
            Select(settingsButton);
        }

        private static void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        /// <summary>Gives keyboard and gamepad navigation a starting point.</summary>
        private static void Select(Selectable selectable)
        {
            if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(selectable.gameObject);
        }
    }
}
