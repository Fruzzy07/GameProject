using System.Collections;
using SteppeBastion.Audio;
using SteppeBastion.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SteppeBastion.UI
{
    public class ResultScreen : MonoBehaviour
    {
        private const float StarDelay = 0.35f;
        private const float StarPopTime = 0.25f;

        [SerializeField] private GameObject window;
        [SerializeField] private Text titleText;
        [SerializeField] private Text infoText;
        [SerializeField] private Image[] stars;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button nextLevelButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private WaveSpawner spawner;

        [Header("Colors")]
        [SerializeField] private Color earnedStar = new(1f, 0.85f, 0.4f);
        [SerializeField] private Color emptyStar = new(0.17f, 0.11f, 0.07f, 0.6f);

        private void Awake()
        {
            restartButton.onClick.AddListener(SceneLoader.RestartLevel);
            mainMenuButton.onClick.AddListener(SceneLoader.LoadMainMenu);
            nextLevelButton.onClick.AddListener(() => SceneLoader.Load(LevelProgress.NextScene()));
            window.SetActive(false);
            if (spawner == null) spawner = FindAnyObjectByType<WaveSpawner>();
        }

        private void Start() => LevelManager.Instance.StateChanged += Show;

        private void OnDestroy()
        {
            if (LevelManager.Instance != null) LevelManager.Instance.StateChanged -= Show;
        }

        private void Show(GameState state)
        {
            if (state == GameState.Playing) return;

            LevelManager manager = LevelManager.Instance;
            bool won = state == GameState.Won;
            int earned = won ? StarRating.ForCurrentLevel() : 0;
            window.SetActive(true);
            GameAudio.Play(s => won ? s.victory : s.defeat, 1f, 0f);

            if (won)
            {
                bool record = LevelProgress.SaveStars(SceneManager.GetActiveScene().name, earned);
                titleText.text = "VICTORY!";
                infoText.text = $"Lives kept: {manager.Lives}/{manager.Level.startLives}" + (record ? "  ·  New best!" : "");
            }
            else
            {
                titleText.text = "AK ORDA HAS FALLEN";
                infoText.text = $"Waves held back: {Mathf.Max(0, spawner.CurrentWave - 1)}/{spawner.TotalWaves}";
            }

            nextLevelButton.gameObject.SetActive(won && LevelProgress.NextScene() != null);

            foreach (Image star in stars)
            {
                star.color = emptyStar;
                star.transform.localScale = Vector3.one;
            }
            if (won) StartCoroutine(RevealStars(earned));

            Button first = nextLevelButton.gameObject.activeSelf ? nextLevelButton : restartButton;
            if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(first.gameObject);
        }

        // timeScale is 0 on this screen
        private IEnumerator RevealStars(int earned)
        {
            for (int i = 0; i < earned && i < stars.Length; i++)
            {
                yield return new WaitForSecondsRealtime(StarDelay);
                stars[i].color = earnedStar;
                for (float t = 0f; t < 1f; t += Time.unscaledDeltaTime / StarPopTime)
                {
                    stars[i].transform.localScale = Vector3.one * (1f + 0.4f * Mathf.Sin(t * Mathf.PI));
                    yield return null;
                }
                stars[i].transform.localScale = Vector3.one;
            }
        }
    }
}
