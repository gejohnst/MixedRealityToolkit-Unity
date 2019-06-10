// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Solvers
{
    /// <summary>
    /// The base abstract class for all Solvers to derive from. It provides state tracking, smoothing parameters
    /// and implementation, automatic solver system integration, and update order. Solvers may be used without a link,
    /// as long as updateLinkedTransform is false.
    /// </summary>
    [RequireComponent(typeof(SolverHandler))]
    public abstract class Solver : MonoBehaviour
    {
        [SerializeField]
        private SolverAttributes solverAttributes;

        public SolverAttributes SolverAttributes
        {
            get
            {
                return solverAttributes;
            }
            set
            {
                solverAttributes = value;
            }
        }

        [SerializeField]
        [HideInInspector]
        [ReadOnly]
        [Tooltip("If true, the position and orientation will be calculated, but not applied, for other components to use")]
        private bool updateLinkedTransform = false;

        public bool UpdateLinkedTransform
        {
            get
            {
                if (solverAttributes)
                {
                    return solverAttributes.updateLinkedTransform;
                }
                else
                {
                    return updateLinkedTransform;
                }
            }
        }

        [SerializeField]
        [HideInInspector]
        [ReadOnly]
        [Tooltip("If 0, the position will update immediately.  Otherwise, the greater this attribute the slower the position updates")]
        private float moveLerpTime = 0.1f;

        public float MoveLerpTime
        {
            get
            {
                if (solverAttributes)
                {
                    return solverAttributes.moveLerpTime;
                }
                else
                {
                    return moveLerpTime;
                }
            }
        }

        [SerializeField]
        [HideInInspector]
        [ReadOnly]
        [Tooltip("If 0, the rotation will update immediately.  Otherwise, the greater this attribute the slower the rotation updates")]
        private float rotateLerpTime = 0.1f;

        public float RotateLerpTime
        {
            get
            {
                if (solverAttributes)
                {
                    return solverAttributes.rotateLerpTime;
                }
                else
                {
                    return rotateLerpTime;
                }
            }
        }

        [SerializeField]
        [HideInInspector]
        [ReadOnly]
        [Tooltip("If 0, the scale will update immediately.  Otherwise, the greater this attribute the slower the scale updates")]
        private float scaleLerpTime = 0;

        public float ScaleLerpTime
        {
            get
            {
                if (solverAttributes)
                {
                    return solverAttributes.scaleLerpTime;
                }
                else
                {
                    return scaleLerpTime;
                }
            }
        }

        [SerializeField]
        [HideInInspector]
        [ReadOnly]
        [Tooltip("If true, the Solver will respect the object's original scale values")]
        private bool maintainScale = true;

        public bool MaintainScale
        {
            get
            {
                if (solverAttributes)
                {
                    return solverAttributes.maintainScale;
                }
                else
                {
                    return maintainScale;
                }
            }
        }

        [SerializeField]
        [HideInInspector]
        [ReadOnly]
        [Tooltip("If true, updates are smoothed to the target. Otherwise, they are snapped to the target")]
        public bool smoothing = true;

        public bool Smoothing
        {
            get
            {
                if (solverAttributes)
                {
                    return solverAttributes.smoothing;
                }
                else
                {
                    return smoothing;
                }
            }
        }

        [SerializeField]
        [HideInInspector]
        [ReadOnly]
        [Tooltip("If > 0, this solver will deactivate after this much time, even if the state is still active")]
        private float lifetime = 0;

        public float Lifetime
        {
            get
            {
                if (solverAttributes)
                {
                    return solverAttributes.lifetime;
                }
                else
                {
                    return lifetime;
                }
            }
        }

        private float currentLifetime;

        

        /// <summary>
        /// The handler reference for this solver that's attached to this <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see>
        /// </summary>
        [SerializeField]
        [HideInInspector]
        protected SolverHandler SolverHandler;

        /// <summary>
        /// The final position to be attained
        /// </summary>
        protected Vector3 GoalPosition
        {
            get
            {
                return SolverHandler.GoalPosition;
            }
            set
            {
                SolverHandler.GoalPosition = value;
            }
        }

        /// <summary>
        /// The final rotation to be attained
        /// </summary>
        protected Quaternion GoalRotation
        {
            get
            {
                return SolverHandler.GoalRotation;
            }
            set
            {
                SolverHandler.GoalRotation = value;
            }
        }

        /// <summary>
        /// The final scale to be attained
        /// </summary>
        protected Vector3 GoalScale
        {
            get
            {
                return SolverHandler.GoalScale;
            }
            set
            {
                SolverHandler.GoalScale = value;
            }
        }

        /// <summary>
        /// Automatically uses the shared position if the solver is set to use the 'linked transform'.
        /// UpdateLinkedTransform may be set to false, and a solver will automatically update the object directly,
        /// and not inherit work done by other solvers to the shared position
        /// </summary>
        public Vector3 WorkingPosition
        {
            get
            {
                return UpdateLinkedTransform ? GoalPosition : transform.position;
            }
            protected set
            {
                if (UpdateLinkedTransform)
                {
                    GoalPosition = value;
                }
                else
                {
                    transform.position = value;
                }
            }
        }

        /// <summary>
        /// Rotation version of WorkingPosition
        /// </summary>
        public Quaternion WorkingRotation
        {
            get
            {
                return UpdateLinkedTransform ? GoalRotation : transform.rotation;
            }
            protected set
            {
                if (UpdateLinkedTransform)
                {
                    GoalRotation = value;
                }
                else
                {
                    transform.rotation = value;
                }
            }
        }

        /// <summary>
        /// Scale version of WorkingPosition
        /// </summary>
        public Vector3 WorkingScale
        {
            get
            {
                return UpdateLinkedTransform ? GoalScale : transform.localScale;
            }
            protected set
            {
                if (UpdateLinkedTransform)
                {
                    GoalScale = value;
                }
                else
                {
                    transform.localScale = value;
                }
            }
        }

        #region MonoBehaviour Implementation

        protected virtual void OnValidate()
        {
            if (SolverHandler == null)
            {
                SolverHandler = GetComponent<SolverHandler>();
            }
            CopyValuesFromSolverAttributes();
        }

        public virtual void CopyValuesFromSolverAttributes()
        {
            if (solverAttributes)
            {
                updateLinkedTransform = solverAttributes.updateLinkedTransform;
                moveLerpTime = solverAttributes.moveLerpTime;
                rotateLerpTime = solverAttributes.rotateLerpTime;
                scaleLerpTime = solverAttributes.scaleLerpTime;
                maintainScale = solverAttributes.maintainScale;
                smoothing = solverAttributes.smoothing;
                lifetime = solverAttributes.lifetime;
            }
        }

        protected virtual void Awake()
        {
            if (UpdateLinkedTransform && SolverHandler == null)
            {
                //TODO: make this work better to set the attributes if possible
                Debug.LogError("No SolverHandler component found on " + name + " when UpdateLinkedTransform was set to true! Disabling UpdateLinkedTransform.");
                updateLinkedTransform = false;
            }

            GoalScale = MaintainScale ? transform.localScale : Vector3.one;
        }

        /// <summary>
        /// Typically when a solver becomes enabled, it should update its internal state to the system, in case it was disabled far away
        /// </summary>
        protected virtual void OnEnable()
        {
            if (SolverHandler != null)
            {
                SnapGoalTo(GoalPosition, GoalRotation, GoalScale);
            }

            currentLifetime = 0;
        }

        #endregion MonoBehaviour Implementation

        /// <summary>
        /// Should be implemented in derived classes, but Solver can be used to flush shared transform to real transform
        /// </summary>
        public abstract void SolverUpdate();

        /// <summary>
        /// Tracks lifetime of the solver, disabling it when expired, and finally runs the orientation update logic
        /// </summary>
        public void SolverUpdateEntry()
        {
            currentLifetime += SolverHandler.DeltaTime;

            if (lifetime > 0 && currentLifetime >= lifetime)
            {
                enabled = false;
                return;
            }

            SolverUpdate();
            UpdateWorkingToGoal();
        }

        /// <summary>
        /// Snaps the solver to the desired pose.
        /// </summary>
        /// <remarks>
        /// SnapTo may be used to bypass smoothing to a certain position if the object is teleported or spawned.
        /// </remarks>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public virtual void SnapTo(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            SnapGoalTo(position, rotation, scale);

            WorkingPosition = position;
            WorkingRotation = rotation;
            WorkingScale = scale;
        }

        [Obsolete("Use SnapTo(Vector3, Quaternion, Vector3) instead.")]
        public virtual void SnapTo(Vector3 position, Quaternion rotation)
        {
            GoalPosition = position;
            GoalRotation = rotation;
        }

        [Obsolete("Use SnapGoalTo(Vector3, Quaternion, Vector3) instead.")]
        public virtual void SnapGoalTo(Vector3 position, Quaternion rotation)
        {
            GoalPosition = position;
            GoalRotation = rotation;
        }

        /// <summary>
        /// SnapGoalTo only sets the goal orientation.  Not really useful.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public virtual void SnapGoalTo(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            GoalPosition = position;
            GoalRotation = rotation;
            GoalScale = scale;
        }

        /// <summary>
        /// Add an offset position to the target goal position.
        /// </summary>
        /// <param name="offset"></param>
        public virtual void AddOffset(Vector3 offset)
        {
            GoalPosition += offset;
        }

        /// <summary>
        /// Lerps Vector3 source to goal.
        /// </summary>
        /// <remarks>
        /// Handles lerpTime of 0.
        /// </remarks>
        /// <param name="source"></param>
        /// <param name="goal"></param>
        /// <param name="deltaTime"></param>
        /// <param name="lerpTime"></param>
        /// <returns></returns>
        public static Vector3 SmoothTo(Vector3 source, Vector3 goal, float deltaTime, float lerpTime)
        {
            return Vector3.Lerp(source, goal, lerpTime.Equals(0.0f) ? 1f : deltaTime / lerpTime);
        }

        /// <summary>
        /// Slerps Quaternion source to goal, handles lerpTime of 0
        /// </summary>
        /// <param name="source"></param>
        /// <param name="goal"></param>
        /// <param name="deltaTime"></param>
        /// <param name="lerpTime"></param>
        /// <returns></returns>
        public static Quaternion SmoothTo(Quaternion source, Quaternion goal, float deltaTime, float lerpTime)
        {
            return Quaternion.Slerp(source, goal, lerpTime.Equals(0.0f) ? 1f : deltaTime / lerpTime);
        }

        /// <summary>
        /// Updates all object orientations to the goal orientation for this solver, with smoothing accounted for (smoothing may be off)
        /// </summary>
        protected void UpdateTransformToGoal()
        {
            if (Smoothing)
            {
                Vector3 pos = transform.position;
                Quaternion rot = transform.rotation;
                Vector3 scale = transform.localScale;

                pos = SmoothTo(pos, GoalPosition, SolverHandler.DeltaTime, MoveLerpTime);
                rot = SmoothTo(rot, GoalRotation, SolverHandler.DeltaTime, RotateLerpTime);
                scale = SmoothTo(scale, GoalScale, SolverHandler.DeltaTime, ScaleLerpTime);

                transform.position = pos;
                transform.rotation = rot;
                transform.localScale = scale;
            }
            else
            {
                transform.position = GoalPosition;
                transform.rotation = GoalRotation;
                transform.localScale = GoalScale;
            }
        }

        /// <summary>
        /// Updates the Working orientation (which may be the object, or the shared orientation) to the goal with smoothing, if enabled
        /// </summary>
        public void UpdateWorkingToGoal()
        {
            UpdateWorkingPositionToGoal();
            UpdateWorkingRotationToGoal();
            UpdateWorkingScaleToGoal();
        }

        /// <summary>
        /// Updates only the working position to goal with smoothing, if enabled
        /// </summary>
        public void UpdateWorkingPositionToGoal()
        {
            WorkingPosition = Smoothing ? SmoothTo(WorkingPosition, GoalPosition, SolverHandler.DeltaTime, MoveLerpTime) : GoalPosition;
        }

        /// <summary>
        /// Updates only the working rotation to goal with smoothing, if enabled
        /// </summary>
        public void UpdateWorkingRotationToGoal()
        {
            WorkingRotation = Smoothing ? SmoothTo(WorkingRotation, GoalRotation, SolverHandler.DeltaTime, RotateLerpTime) : GoalRotation;
        }

        /// <summary>
        /// Updates only the working scale to goal with smoothing, if enabled
        /// </summary>
        public void UpdateWorkingScaleToGoal()
        {
            WorkingScale = Smoothing ? SmoothTo(WorkingScale, GoalScale, SolverHandler.DeltaTime, ScaleLerpTime) : GoalScale;
        }
    }
}
