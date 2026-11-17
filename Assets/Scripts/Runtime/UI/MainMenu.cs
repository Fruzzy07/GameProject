using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace SteppeBastion.UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private Button playButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;

        [Header("Windows")]
        [SerializeField] private LevelSelect levelSelect;
        [SerializeField] private Button levelSelectBackButton;
        [SerializeField] private GameObject settingsWindow;
        [SerializeField] private Button settingsBackButton;

        private Button returnTo;

        private void Awake()
        {
            playButton.onClick.AddListener(() => Open(levelSelect.gameObject, levelSelect.FirstCard, playButton));
            settingsButton.onClick.AddListener(() => Open(settingsWindow, settingsBackButton, settingsButton));
            quitButton.onClick.AddListener(Quit);
            levelSelectBackButton.onClick.AddListener(CloseWindows);
            settingsBackButton.onClick.AddListener(CloseWindows);

            quitButton.gameObject.SetActive(Application.platform != RuntimePlatform.WebGLPlayer);
            levelSelect.gameObject.SetActive(false);
            settingsWindow.SetActive(false);
        }

        private void Start() => Select(playButton);

        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
                CloseWindows();
        }

        private void Open(GameObject window, Selectable first, Button from)
        {
            window.SetActive(true);
            returnTo = from;
            Select(first);
        }

        private void CloseWindows()
        {
            if (!levelSelect.gameObject.activeSelf && !settingsWindow.activeSelf) return;
            levelSelect.gameObject.SetActive(false);
            settingsWindow.SetActive(false);
            if (returnTo != null) Select(returnTo);
        }

        private static void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private static void Select(Selectable selectable)
        {
            if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(selectable.gameObject);
        }
    }
}
