using SteppeBastion.Core;
using UnityEngine;
using UnityEngine.UI;

namespace SteppeBastion.UI
{
    public class SettingsWindow : MonoBehaviour
    {
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private Toggle fullscreenToggle;
        [SerializeField] private Text musicValue;
        [SerializeField] private Text sfxValue;

        private void Awake()
        {
            musicSlider.onValueChanged.AddListener(v =>
            {
                GameSettings.SetMusicVolume(v);
                musicValue.text = Percent(v);
            });
            sfxSlider.onValueChanged.AddListener(v =>
            {
                GameSettings.SetSfxVolume(v);
                sfxValue.text = Percent(v);
            });
            fullscreenToggle.onValueChanged.AddListener(GameSettings.SetFullscreen);
        }

        private void OnEnable()
        {
            musicSlider.SetValueWithoutNotify(GameSettings.MusicVolume);
            sfxSlider.SetValueWithoutNotify(GameSettings.SfxVolume);
            fullscreenToggle.SetIsOnWithoutNotify(GameSettings.Fullscreen);
            musicValue.text = Percent(GameSettings.MusicVolume);
            sfxValue.text = Percent(GameSettings.SfxVolume);
        }

        private void OnDisable() => GameSettings.Save();

        private static string Percent(float v) => $"{Mathf.RoundToInt(v * 100f)}%";
    }
}
