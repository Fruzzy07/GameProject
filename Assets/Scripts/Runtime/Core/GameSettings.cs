using UnityEngine;
using UnityEngine.Audio;

namespace SteppeBastion.Core
{
    public static class GameSettings
    {
        private const string MusicKey = "settings.music";
        private const string SfxKey = "settings.sfx";
        private const string FullscreenKey = "settings.fullscreen";

        private static AudioMixer mixer;

        public static float MusicVolume { get; private set; } = 0.8f;
        public static float SfxVolume { get; private set; } = 0.8f;
        public static bool Fullscreen { get; private set; } = true;

        public static AudioMixer Mixer => mixer ??= Resources.Load<AudioMixer>("Audio/GameMixer");

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            MusicVolume = PlayerPrefs.GetFloat(MusicKey, MusicVolume);
            SfxVolume = PlayerPrefs.GetFloat(SfxKey, SfxVolume);
            Fullscreen = PlayerPrefs.GetInt(FullscreenKey, Fullscreen ? 1 : 0) == 1;

            // AudioMixer.SetFloat is ignored before the first Start, so apply from a helper object.
            var go = new GameObject("GameSettings");
            Object.DontDestroyOnLoad(go);
            go.hideFlags = HideFlags.HideInHierarchy;
            go.AddComponent<Applier>();
        }

        public static void SetMusicVolume(float value)
        {
            MusicVolume = Mathf.Clamp01(value);
            PlayerPrefs.SetFloat(MusicKey, MusicVolume);
            ApplyVolumes();
        }

        public static void SetSfxVolume(float value)
        {
            SfxVolume = Mathf.Clamp01(value);
            PlayerPrefs.SetFloat(SfxKey, SfxVolume);
            ApplyVolumes();
        }

        public static void SetFullscreen(bool value)
        {
            Fullscreen = value;
            PlayerPrefs.SetInt(FullscreenKey, value ? 1 : 0);
            ApplyFullscreen();
        }

        public static void Save() => PlayerPrefs.Save();

        private static void ApplyVolumes()
        {
            if (Mixer == null) return;
            Mixer.SetFloat("MusicVolume", ToDecibels(MusicVolume));
            Mixer.SetFloat("SfxVolume", ToDecibels(SfxVolume));
        }

        private static void ApplyFullscreen() =>
            Screen.fullScreenMode = Fullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;

        private static float ToDecibels(float volume) => volume <= 0.0001f ? -80f : Mathf.Log10(volume) * 20f;

        private class Applier : MonoBehaviour
        {
            private void Start()
            {
                ApplyVolumes();
                ApplyFullscreen();
            }
        }
    }
}
