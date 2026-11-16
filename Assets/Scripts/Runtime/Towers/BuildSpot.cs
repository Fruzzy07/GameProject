using SteppeBastion.Data;
using UnityEngine;

namespace SteppeBastion.Towers
{
    /// <summary>A marked place where the player can build one tower.</summary>
    [RequireComponent(typeof(Collider2D))]
    public class BuildSpot : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer marker;
        [SerializeField] private Tower towerPrefab;

        public Tower Tower { get; private set; }
        public bool IsFree => Tower == null;

        public Tower Build(TowerData data)
        {
            Tower tower = Instantiate(towerPrefab, transform.position, Quaternion.identity);
            tower.Init(data);
            Place(tower);
            return tower;
        }

        /// <summary>Puts an existing tower on this spot (used by Kosh when moving a tower).</summary>
        public void Place(Tower tower)
        {
            Tower = tower;
            tower.Spot = this;
            tower.transform.position = transform.position;
            marker.enabled = false;
        }

        public void Clear()
        {
            Tower = null;
            marker.enabled = true;
        }

        public void SetHighlight(bool on)
        {
            marker.color = on ? new Color(1f, 0.95f, 0.6f) : Color.white;
        }
    }
}
