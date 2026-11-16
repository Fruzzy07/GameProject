using SteppeBastion.Controls;
using SteppeBastion.Core;
using SteppeBastion.Heroes;
using UnityEngine;
using UnityEngine.UI;

namespace SteppeBastion.UI
{
    /// <summary>
    /// Simple text HUD for testing the gameplay: lives, gold, waves, berkut and controls.
    /// It will be replaced by the final game UI.
    /// </summary>
    public class DebugHud : MonoBehaviour
    {
        [SerializeField] private Text statsText;    // top-left: lives, gold, wave
        [SerializeField] private Text controlsText; // bottom-left: keys and berkut status
        [SerializeField] private Text messageText;  // center: pause, victory, defeat
        [SerializeField] private Text hintText;     // bottom-center: what the current click will do

        [SerializeField] private WaveSpawner spawner;
        [SerializeField] private BerkutAbility berkut;
        [SerializeField] private PlayerInputController controls;

        private void Start()
        {
            controls.ModeChanged += OnModeChanged;
            OnModeChanged(controls.Mode);
        }

        private void Update()
        {
            LevelManager manager = LevelManager.Instance;

            string next = spawner.CanCallEarly
                ? $"\nNext wave in {Mathf.CeilToInt(spawner.Countdown)} s  (Space: call now, +{spawner.EarlyCallBonus} gold)"
                : "";
            statsText.text = $"Lives: {manager.Lives}    Gold: {manager.Gold}    Wave: {spawner.CurrentWave}/{spawner.TotalWaves}{next}";

            string berkutState = berkut.Ready ? "READY" : $"{Mathf.RoundToInt(berkut.Charge * 100f)}%";
            controlsText.text =
                $"Berkut: {berkutState}\n" +
                "LMB: build / upgrade / sell / Kosh    B or RMB: berkut\n" +
                $"Space: next wave    F: speed x{(GameSpeed.Fast ? 2 : 1)}    Esc: pause";

            messageText.text = manager.State switch
            {
                GameState.Won => "VICTORY!\nPress R to play again",
                GameState.Lost => "AK ORDA HAS FALLEN\nPress R to try again",
                _ => GameSpeed.Paused ? "PAUSED\nPress Esc to continue" : ""
            };
        }

        private void OnModeChanged(InputMode mode)
        {
            hintText.text = mode switch
            {
                InputMode.AimBerkut => "Click where the berkut should strike  ·  Esc to cancel",
                InputMode.MoveTower => "Kosh: click a free spot to move the tower  ·  Esc to cancel",
                _ => ""
            };
        }
    }
}
