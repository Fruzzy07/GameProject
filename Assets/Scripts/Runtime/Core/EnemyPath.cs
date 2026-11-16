using UnityEngine;

namespace SteppeBastion.Core
{
    /// <summary>The road enemies walk along. Waypoints are the child transforms, in order.</summary>
    public class EnemyPath : MonoBehaviour
    {
        private Vector3[] points;

        public int Count => Points.Length;
        public Vector3 this[int index] => Points[index];

        private Vector3[] Points
        {
            get
            {
                if (points == null || points.Length != transform.childCount)
                {
                    points = new Vector3[transform.childCount];
                    for (int i = 0; i < points.Length; i++)
                        points[i] = transform.GetChild(i).position;
                }
                return points;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            for (int i = 1; i < transform.childCount; i++)
                Gizmos.DrawLine(transform.GetChild(i - 1).position, transform.GetChild(i).position);
        }
    }
}
