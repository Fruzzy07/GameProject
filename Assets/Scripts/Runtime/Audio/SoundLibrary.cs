using UnityEngine;
using UnityEngine.Serialization;

namespace SteppeBastion.Audio
{
    [CreateAssetMenu(menuName = "Steppe Bastion/Sound Library", fileName = "SoundLibrary")]
    public class SoundLibrary : ScriptableObject
    {
        [Header("Towers")]
        public AudioClip[] arrowShot;
        public AudioClip[] catapultShot;
        public AudioClip[] catapultHit;
        public AudioClip[] shamanShot;
        public AudioClip[] lightning;

        [Header("Enemies")]
        public AudioClip[] enemyDeath;
        public AudioClip[] baseHit;

        [Header("Player")]
        public AudioClip[] build;
        public AudioClip[] upgrade;
        public AudioClip[] sell;
        public AudioClip[] packTower;
        public AudioClip[] berkutCry;
        public AudioClip[] berkutStrike;
        public AudioClip[] waveStart;
        public AudioClip[] uiClick;

        [Header("Music")]
        public AudioClip[] victory;
        public AudioClip[] defeat;
        public AudioClip menuMusic;
        [FormerlySerializedAs("music")] public AudioClip battleMusic;
    }
}
