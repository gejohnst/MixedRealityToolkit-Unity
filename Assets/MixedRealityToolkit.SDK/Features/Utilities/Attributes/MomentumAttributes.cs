using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Solvers
{
    class MomentumAttributes : SolverAttributes
    {
        [SerializeField]
        [Tooltip("Friction to slow down the current velocity")]
        public float resistance = 0.99f;

        [SerializeField]
        [Tooltip("Apply more resistance when going faster- applied resistance is resistance * (velocity ^ resistanceVelocityPower)")]
        public float resistanceVelocityPower = 1.5f;

        [SerializeField]
        [Tooltip("Accelerate to goal position at this rate")]
        public float accelerationRate = 10f;

        [SerializeField]
        [Tooltip("Apply more acceleration if farther from target- applied acceleration is accelerationRate + springiness * distance")]
        public float springiness = 0;

        [SerializeField]
        [Tooltip("Instantly maintain a constant depth from the view point instead of simulating Z-velocity")]
        public bool snapZ = true;
    }
}
