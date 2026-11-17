using System.Collections;
using SteppeBastion.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SteppeBastion.UI
{
    /// <summary>Victory and defeat window. On victory the earned stars pop in one by one.</summary>
    public class ResultScreen : MonoBehaviour
    {
        private const float StarDelay = 0.35f;
        private const float StarPopTime = 0.25f;

        [SerializeField] private GameObject window;
        [SerializeField] private Text titleText;
        [SerializeField] private Text infoText;
        [SerializeField] private Image[] stars;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button mainMenuButton;
        [Tooltip("Found in the scene automatically if left empty.")]
        [SerializeField] private WaveSpawner spawner;

        [Header("Colors")]
        [SerializeField] private Color earnedStar = new(1f, 0.85f, 0.4f);
        [SerializeField] private Color emptyStar = new(0.17f, 0.11f, 0.07f, 0.6f);

        private void Awake()
        {
            restartButton.onClick.AddListener(SceneLoader.RestartLevel);
            mainMenuButton.onClick.AddListener(SceneLoader.LoadMainMenu);
            window.SetActive(false);
            // The screen is a prefab shared by all levels, so it can't reference a scene object directly.
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
            window.SetActive(true);

            if (won)
            {
                titleText.text = "VICTORY!";
                infoText.text = $"Ak Orda stands. Lives kept: {manager.Lives}/{manager.Level.startLives}";
            }
            else
            {
                titleText.text = "AK ORDA HAS FALLEN";
                infoText.text = $"Waves held back: {Mathf.Max(0, spawner.CurrentWave - 1)}/{spawner.TotalWaves}";
            }

            // On defeat all three stars stay empty.
            foreach (Image star in stars)
            {
                star.color = emptyStar;
                star.transform.localScale = Vector3.one;
            }
            if (won) StartCoroutine(RevealStars(StarRating.ForCurrentLevel()));

            if (EventSystem.current != null)
                EventSystem.current.SetSelectedGameObject(restartButton.gameObject);
        }

        /// <summary>The game is stopped (timeScale 0) on this screen, so the animation runs on unscaled time.</summary>
        private IEnumerator RevealStars(int earned)
        {
            for (int i = 0; i < earned && i < stars.Length; i++)
            {
                yield return new WaitForSecondsRealtime(StarDelay);
                stars[i].color = earnedStar;
                for (float t = 0f; t < 1f; t += Time.unscaledDeltaTime / StarPopTime)
                {
                    // Grows past full size, then settles back: a small "pop".
                    float scale = 1f + 0.4f * Mathf.Sin(t * Mathf.PI);
                    stars[i].transform.localScale = Vector3.one * scale;
                    yield return null;
                }
                stars[i].transform.localScale = Vector3.one;
            }
        }
    }
}
