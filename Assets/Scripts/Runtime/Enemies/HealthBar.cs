using UnityEngine;

namespace SteppeBastion.Enemies
{
    /// <summary>Small red/green bar above an enemy. Hidden while the enemy is at full health.</summary>
    public class HealthBar : MonoBehaviour
    {
        private const float Height = 0.08f;

        [SerializeField] private Transform back;
        [SerializeField] private Transform fill;

        private float width;

        /// <summary>Sets the bar's height above the enemy's feet and its width (in world units).</summary>
        public void Place(float heightAbove, float barWidth)
        {
            width = barWidth;
            transform.localPosition = Vector3.up * heightAbove;
            back.localScale = new Vector3(width + 0.04f, Height + 0.04f, 1f);
            SetValue(1f);
        }

        public void SetValue(float value)
        {
            value = Mathf.Clamp01(value);
            gameObject.SetActive(value < 1f);
            fill.localScale = new Vector3(width * value, Height, 1f);
            fill.localPosition = new Vector3(-width * (1f - value) / 2f, 0f, 0f);
        }
    }
}
