using System.Collections;
using SteppeBastion.Controls;
using SteppeBastion.Core;
using SteppeBastion.Heroes;
using UnityEngine;
using UnityEngine.UI;

namespace SteppeBastion.UI
{
    /// <summary>
    /// The level HUD: lives, gold and wave at the top left; call wave, speed and pause at the top right;
    /// the berkut with its cooldown at the bottom left; a hint for the current click mode at the bottom.
    /// It is a prefab shared by all levels, so it finds the level's systems in the scene by itself.
    /// </summary>
    public class LevelHud : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] private Text livesText;
        [SerializeField] private Text goldText;
        [SerializeField] private Text waveText;

        [Header("Buttons")]
        [SerializeField] private Button callWaveButton;
        [SerializeField] private Text callWaveText;
        [SerializeField] private Button speedButton;
        [SerializeField] private Text speedText;
        [SerializeField] private Button pauseButton;

        [Header("Berkut")]
        [SerializeField] private Button berkutButton;
        [SerializeField] private Image berkutCooldown;   // radial overlay that shrinks while the berkut recharges
        [SerializeField] private Image berkutRing;

        [Header("Hint")]
        [SerializeField] private Text hintText;

        [Header("Colors")]
        [SerializeField] private Color readyRing = new(1f, 0.85f, 0.4f);
        [SerializeField] private Color chargingRing = new(0.45f, 0.4f, 0.35f);
        [SerializeField] private Color lifeLost = new(1f, 0.35f, 0.3f);

        private LevelManager manager;
        private WaveSpawner spawner;
        private BerkutAbility berkut;
        private PlayerInputController controls;
        private Coroutine livesFlash;
        private Coroutine goldPulse;

        private void Start()
        {
            manager = LevelManager.Instance;
            spawner = FindAnyObjectByType<WaveSpawner>();
            berkut = FindAnyObjectByType<BerkutAbility>();
            controls = FindAnyObjectByType<PlayerInputController>();

            callWaveButton.onClick.AddListener(spawner.CallEarly);
            speedButton.onClick.AddListener(GameSpeed.ToggleFast);
            pauseButton.onClick.AddListener(() => GameSpeed.SetPaused(true));
            berkutButton.onClick.AddListener(controls.BeginBerkut);

            manager.LivesChanged += OnLivesChanged;
            manager.GoldChanged += OnGoldChanged;
            controls.ModeChanged += OnModeChanged;
            GameSpeed.Changed += OnSpeedChanged;

            livesText.text = manager.Lives.ToString();
            goldText.text = manager.Gold.ToString();
            OnModeChanged(controls.Mode);
            OnSpeedChanged();
        }

        private void OnDestroy()
        {
            GameSpeed.Changed -= OnSpeedChanged;
            if (manager != null)
            {
                manager.LivesChanged -= OnLivesChanged;
                manager.GoldChanged -= OnGoldChanged;
            }
            if (controls != null) controls.ModeChanged -= OnModeChanged;
        }

        private void Update()
        {
            if (manager == null) return;

            waveText.text = $"Wave {spawner.CurrentWave}/{spawner.TotalWaves}";

            bool canCall = spawner.CanCallEarly;
            callWaveButton.gameObject.SetActive(canCall);
            if (canCall)
            {
                string title = spawner.CurrentWave == 0 ? "START" : "NEXT WAVE";
                callWaveText.text = $"{title}\n<size=24><color=#FFD966>{Mathf.CeilToInt(spawner.Countdown)} s  ·  +{spawner.EarlyCallBonus}</color></size>";
            }

            berkutCooldown.fillAmount = 1f - berkut.Charge;
            berkutRing.color = berkut.Ready ? readyRing : chargingRing;
            berkutButton.interactable = berkut.Ready;
        }

        private void OnLivesChanged(int lives)
        {
            livesText.text = lives.ToString();
            if (livesFlash != null) StopCoroutine(livesFlash);
            livesFlash = StartCoroutine(Flash(livesText, lifeLost));
        }

        private void OnGoldChanged(int gold)
        {
            goldText.text = gold.ToString();
            if (goldPulse != null) StopCoroutine(goldPulse);
            goldPulse = StartCoroutine(Pulse(goldText.transform));
        }

        private void OnSpeedChanged() => speedText.text = GameSpeed.Fast ? "x2" : "x1";

        private void OnModeChanged(InputMode mode)
        {
            hintText.text = mode switch
            {
                InputMode.AimBerkut => "Click where the berkut should strike  ·  Esc to cancel",
                InputMode.MoveTower => "Kosh: click a free spot to move the tower  ·  Esc to cancel",
                _ => ""
            };
        }

        // HUD effects use unscaled time so they look the same at x1, x2 and on pause.
        private static IEnumerator Flash(Text text, Color color)
        {
            Color normal = UIFactory.Cream;
            for (float t = 0f; t < 1f; t += Time.unscaledDeltaTime / 0.5f)
            {
                text.color = Color.Lerp(color, normal, t);
                yield return null;
            }
            text.color = normal;
        }

        private static IEnumerator Pulse(Transform target)
        {
            for (float t = 0f; t < 1f; t += Time.unscaledDeltaTime / 0.2f)
            {
                target.localScale = Vector3.one * (1f + 0.15f * Mathf.Sin(t * Mathf.PI));
                yield return null;
            }
            target.localScale = Vector3.one;
        }
    }
}
