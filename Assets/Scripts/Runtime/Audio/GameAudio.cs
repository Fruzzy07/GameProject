using System;
using System.Collections;
using System.Collections.Generic;
using SteppeBastion.Core;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace SteppeBastion.Audio
{
    public static class GameAudio
    {
        private const int Voices = 10;
        private const float MinRepeatDelay = 0.06f;
        private const float FadeTime = 1f;

        private static SoundLibrary library;
        private static AudioSource[] voices;
        private static AudioSource musicSource;
        private static int nextVoice;
        private static MonoBehaviour runner;
        private static Coroutine fade;
        private static readonly Dictionary<AudioClip[], float> lastPlayed = new();

        public static SoundLibrary Library => library;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Init()
        {
            library = Resources.Load<SoundLibrary>("Audio/SoundLibrary");
            AudioMixer mixer = GameSettings.Mixer;
            AudioMixerGroup sfxGroup = Group(mixer, "SFX");
            AudioMixerGroup musicGroup = Group(mixer, "Music");

            var host = new GameObject("GameAudio", typeof(AudioListener), typeof(Runner));
            runner = host.GetComponent<Runner>();
            Object.DontDestroyOnLoad(host);
            voices = new AudioSource[Voices];
            for (int i = 0; i < Voices; i++)
            {
                voices[i] = host.AddComponent<AudioSource>();
                voices[i].playOnAwake = false;
                voices[i].outputAudioMixerGroup = sfxGroup;
            }

            musicSource = host.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
            musicSource.outputAudioMixerGroup = musicGroup;

            SceneManager.activeSceneChanged += (_, scene) => PlayMusicFor(scene);
            PlayMusicFor(SceneManager.GetActiveScene());
        }

        private static void PlayMusicFor(Scene scene)
        {
            if (library == null) return;
            AudioClip clip = scene.name == SceneLoader.MainMenuScene ? library.menuMusic : library.battleMusic;
            if (clip == null || (clip == musicSource.clip && musicSource.isPlaying)) return;
            if (fade != null) runner.StopCoroutine(fade);
            fade = runner.StartCoroutine(SwitchMusic(clip));
        }

        // unscaled time: the result screens pause the game
        private static IEnumerator SwitchMusic(AudioClip clip)
        {
            if (musicSource.isPlaying)
            {
                for (float v = musicSource.volume; v > 0f; v -= Time.unscaledDeltaTime / FadeTime)
                {
                    musicSource.volume = v;
                    yield return null;
                }
            }
            musicSource.clip = clip;
            musicSource.volume = 0f;
            musicSource.Play();
            for (float v = 0f; v < 1f; v += Time.unscaledDeltaTime / FadeTime)
            {
                musicSource.volume = v;
                yield return null;
            }
            musicSource.volume = 1f;
            fade = null;
        }

        public static void Play(Func<SoundLibrary, AudioClip[]> pick, float volume = 1f, float pitchVariation = 0.08f)
        {
            if (library == null || voices == null) return;
            AudioClip[] clips = pick(library);
            if (clips == null || clips.Length == 0) return;

            // many towers fire on the same frame, especially at x2 speed
            if (lastPlayed.TryGetValue(clips, out float last) && Time.unscaledTime - last < MinRepeatDelay) return;
            lastPlayed[clips] = Time.unscaledTime;

            AudioSource source = voices[nextVoice];
            nextVoice = (nextVoice + 1) % voices.Length;
            source.clip = clips[UnityEngine.Random.Range(0, clips.Length)];
            source.volume = volume;
            source.pitch = 1f + UnityEngine.Random.Range(-pitchVariation, pitchVariation);
            source.Play();
        }

        private class Runner : MonoBehaviour { }

        private static AudioMixerGroup Group(AudioMixer mixer, string name)
        {
            if (mixer == null) return null;
            AudioMixerGroup[] groups = mixer.FindMatchingGroups(name);
            return groups.Length > 0 ? groups[0] : null;
        }
    }
}
