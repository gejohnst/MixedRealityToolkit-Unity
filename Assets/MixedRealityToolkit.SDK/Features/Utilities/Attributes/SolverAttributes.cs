using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Solvers
{
    [CreateAssetMenu(fileName = "SolverAttributes.asset", menuName = "Solver Attributes")]
    public class SolverAttributes : ScriptableObject
    {
        [SerializeField]
        [Tooltip("If true, the position and orientation will be calculated, but not applied, for other components to use")]
        public bool updateLinkedTransform = false;

        [SerializeField]
        [Tooltip("If 0, the position will update immediately.  Otherwise, the greater this attribute the slower the position updates")]
        public float moveLerpTime = 0.1f;

        [SerializeField]
        [Tooltip("If 0, the rotation will update immediately.  Otherwise, the greater this attribute the slower the rotation updates")]
        public float rotateLerpTime = 0.1f;

        [SerializeField]
        [Tooltip("If 0, the scale will update immediately.  Otherwise, the greater this attribute the slower the scale updates")]
        public float scaleLerpTime = 0;

        [SerializeField]
        [Tooltip("If true, updates are smoothed to the target. Otherwise, they are snapped to the target")]
        public bool smoothing = true;

        [SerializeField]
        [Tooltip("If true, the Solver will respect the object's original scale values")]
        public bool maintainScale = true;

        [SerializeField]
        [Tooltip("If > 0, this solver will deactivate after this much time, even if the state is still active")]
        public float lifetime = 0;
    }
}
