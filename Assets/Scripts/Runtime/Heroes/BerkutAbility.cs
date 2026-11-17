using System.Collections;
using System.Collections.Generic;
using SteppeBastion.Audio;
using SteppeBastion.Enemies;
using SteppeBastion.Towers;
using UnityEngine;

namespace SteppeBastion.Heroes
{
    /// <summary>The golden eagle: the player picks a point, the berkut dives there and hits every enemy around it.</summary>
    public class BerkutAbility : MonoBehaviour
    {
        [SerializeField] private GameObject birdPrefab;
        [SerializeField] private HitEffect hitEffectPrefab;
        [SerializeField] private SpriteRenderer aim;          // ring under the cursor while aiming
        [SerializeField] private float cooldown = 20f;
        [SerializeField] private float damage = 60f;
        [SerializeField] private float radius = 1.4f;
        [SerializeField] private float flightTime = 0.45f;

        private static readonly List<Enemy> targets = new();
        private float timer;

        public bool Ready => timer <= 0f;
        /// <summary>0 right after a strike, 1 when ready again.</summary>
        public float Charge => 1f - Mathf.Clamp01(timer / cooldown);

        private void Awake()
        {
            aim.transform.localScale = Vector3.one * (radius * 2f);
            aim.enabled = false;
        }

        private void Update()
        {
            if (timer > 0f) timer -= Time.deltaTime;
        }

        /// <summary>Shows the strike area under the cursor while the player is aiming.</summary>
        public void ShowAim(bool visible, Vector3 point)
        {
            aim.enabled = visible;
            aim.transform.position = point;
        }

        public void Strike(Vector3 point)
        {
            if (!Ready) return;
            timer = cooldown;
            GameAudio.Play(s => s.berkutCry, 0.9f, 0.05f);
            StartCoroutine(Fly(point));
        }

        private IEnumerator Fly(Vector3 point)
        {
            Vector3 from = point + new Vector3(-7f, 6f, 0f);
            Vector3 away = point + new Vector3(7f, 6f, 0f);
            GameObject bird = Instantiate(birdPrefab, from, Quaternion.identity);

            // Dive: t*t starts slow and speeds up, like a falling bird. The bird also shrinks as it gets "closer".
            for (float t = 0f; t < 1f; t += Time.deltaTime / flightTime)
            {
                bird.transform.position = Vector3.Lerp(from, point, t * t);
                bird.transform.localScale = Vector3.one * Mathf.Lerp(1.6f, 1f, t);
                yield return null;
            }

            // Copy the targets first: an enemy that dies can spawn new enemies and change the list.
            targets.Clear();
            foreach (Enemy enemy in Enemy.Active)
                if (enemy.IsAlive && Vector3.Distance(enemy.Center, point) <= radius)
                    targets.Add(enemy);
            foreach (Enemy enemy in targets)
                enemy.TakeDamage(damage);
            HitEffect.Create(hitEffectPrefab, point, radius * 2f, new Color(1f, 0.85f, 0.3f));
            GameAudio.Play(s => s.berkutStrike);

            for (float t = 0f; t < 1f; t += Time.deltaTime / flightTime)
            {
                bird.transform.position = Vector3.Lerp(point, away, t * t);
                yield return null;
            }
            Destroy(bird);
        }
    }
}
