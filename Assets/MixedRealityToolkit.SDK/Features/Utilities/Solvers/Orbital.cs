// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.Utilities.Solvers
{
    /// <summary>
    /// Provides a solver that follows the TrackedObject/TargetTransform in an orbital motion.
    /// </summary>
    public class Orbital : Solver
    {
        public OrbitalAttributes OrbitalAttributes
        {
            get
            {
                return SolverAttributes as OrbitalAttributes;
            }
        }

        [SerializeField]
        [HideInInspector]
        [Tooltip("The desired orientation of this object. Default sets the object to face the TrackedObject/TargetTransform. CameraFacing sets the object to always face the user.")]
        private SolverOrientationType orientationType = SolverOrientationType.FollowTrackedObject;

        /// <summary>
        /// The desired orientation of this object.
        /// </summary>
        /// <remarks>
        /// Default sets the object to face the TrackedObject/TargetTransform. CameraFacing sets the object to always face the user.
        /// </remarks>
        public SolverOrientationType OrientationType
        {
            get { return orientationType; }
            set { orientationType = value; }
        }

        [SerializeField]
        [HideInInspector]
        [Tooltip("XYZ offset for this object oriented with the TrackedObject/TargetTransform's forward. Mixing local and world offsets is not recommended. Local offsets are applied before world offsets.")]
        private Vector3 localOffset = new Vector3(0, -1, 1);

        /// <summary>
        /// XYZ offset for this object in relation to the TrackedObject/TargetTransform.
        /// </summary>
        /// <remarks>
        /// Mixing local and world offsets is not recommended.
        /// </remarks>
        public Vector3 LocalOffset
        {
            get { return localOffset; }
            set { localOffset = value; }
        }

        [SerializeField]
        [HideInInspector]
        [Tooltip("Yaw-Pitch-Roll offset for this object oriented with the TrackedObject/TargetTransform's forward. Mixing local and world offsets is not recommended. Local offsets are applied before world offsets.")]
        private Vector3 localRotationOffset = Vector3.zero;

        /// <summary>
        /// XYZ offset for this object in relation to the TrackedObject/TargetTransform.
        /// </summary>
        /// <remarks>
        /// Mixing local and world offsets is not recommended.
        /// </remarks>
        public Vector3 LocalRotationOffset
        {
            get { return localRotationOffset; }
            set { localRotationOffset = value; }
        }

        [SerializeField]
        [HideInInspector]
        [Tooltip("XYZ offset for this object in worldspace, best used with the YawOnly orientationType. Mixing local and world offsets is not recommended. Local offsets are applied before world offsets.")]
        private Vector3 worldOffset = Vector3.zero;

        /// <summary>
        /// XYZ offset for this object in worldspace, best used with the YawOnly orientationType.
        /// </summary>
        /// <remarks>
        /// Mixing local and world offsets is not recommended.
        /// </remarks>
        public Vector3 WorldOffset
        {
            get { return worldOffset; }
            set { worldOffset = value; }
        }

        [SerializeField]
        [HideInInspector]
        [FormerlySerializedAs(oldName: "useAngleSteppingForWorldOffset")]
        [Tooltip("Lock the rotation to a specified number of steps around the tracked object.")]
        private bool useAngleStepping = false;

        /// <summary>
        /// Lock the rotation to a specified number of steps around the tracked object.
        /// </summary>
        public bool UseAngleStepping
        {
            get { return useAngleStepping; }
            set { useAngleStepping = value; }
        }

        [Range(2, 24)]
        [HideInInspector]
        [SerializeField]
        [Tooltip("The division of steps this object can tether to. Higher the number, the more snapple steps.")]
        private int tetherAngleSteps = 6;

        /// <summary>
        /// The division of steps this object can tether to. Higher the number, the more snapple steps.
        /// </summary>
        public int TetherAngleSteps
        {
            get { return tetherAngleSteps; }
            set
            {
                tetherAngleSteps =  Mathf.Clamp(value, 2, 24);
            }
        }

        public override void SolverUpdate()
        {
            Vector3 desiredPos = SolverHandler.TransformTarget != null ? SolverHandler.TransformTarget.position : Vector3.zero;

            Quaternion targetRot = SolverHandler.TransformTarget != null ? SolverHandler.TransformTarget.rotation : Quaternion.Euler(0, 1, 0);
            Quaternion yawOnlyRot = Quaternion.Euler(0, targetRot.eulerAngles.y, 0);
            desiredPos = desiredPos + (SnapToTetherAngleSteps(targetRot) * LocalOffset);
            desiredPos = desiredPos + (SnapToTetherAngleSteps(yawOnlyRot) * WorldOffset);

            Quaternion desiredRot = CalculateDesiredRotation(desiredPos);
            desiredRot = Quaternion.Euler(LocalRotationOffset) * desiredRot;

            GoalPosition = desiredPos;
            GoalRotation = desiredRot;
        }


        private Quaternion SnapToTetherAngleSteps(Quaternion rotationToSnap)
        {
            if (!UseAngleStepping || SolverHandler.TransformTarget == null)
            {
                return rotationToSnap;
            }

            float stepAngle = 360f / tetherAngleSteps;
            int numberOfSteps = Mathf.RoundToInt(SolverHandler.TransformTarget.transform.eulerAngles.y / stepAngle);

            float newAngle = stepAngle * numberOfSteps;

            return Quaternion.Euler(rotationToSnap.eulerAngles.x, newAngle, rotationToSnap.eulerAngles.z);
        }

        private Quaternion CalculateDesiredRotation(Vector3 desiredPos)
        {
            Quaternion desiredRot = Quaternion.identity;

            switch (orientationType)
            {
                case SolverOrientationType.YawOnly:
                    float targetYRotation = SolverHandler.TransformTarget != null ? SolverHandler.TransformTarget.eulerAngles.y : 0.0f;
                    desiredRot = Quaternion.Euler(0f, targetYRotation, 0f);
                    break;
                case SolverOrientationType.Unmodified:
                    desiredRot = transform.rotation;
                    break;
                case SolverOrientationType.CameraAligned:
                    desiredRot = CameraCache.Main.transform.rotation;
                    break;
                case SolverOrientationType.FaceTrackedObject:
                    desiredRot = SolverHandler.TransformTarget != null ? Quaternion.LookRotation(SolverHandler.TransformTarget.position - desiredPos) : Quaternion.identity;
                    break;
                case SolverOrientationType.CameraFacing:
                    desiredRot = SolverHandler.TransformTarget != null ? Quaternion.LookRotation(CameraCache.Main.transform.position - desiredPos) : Quaternion.identity;
                    break;
                case SolverOrientationType.FollowTrackedObject:
                    desiredRot = SolverHandler.TransformTarget != null ? SolverHandler.TransformTarget.rotation : Quaternion.identity;
                    break;
                default:
                    Debug.LogError($"Invalid OrientationType for Orbital Solver on {gameObject.name}");
                    break;
            }

            if (UseAngleStepping)
            {
                desiredRot = SnapToTetherAngleSteps(desiredRot);
            }

            return desiredRot;
        }

        public override void CopyValuesFromSolverAttributes()
        {
            base.CopyValuesFromSolverAttributes();
            if (OrbitalAttributes)
            {
                orientationType = OrbitalAttributes.orientationType;
                tetherAngleSteps = OrbitalAttributes.tetherAngleSteps;
                useAngleStepping = OrbitalAttributes.useAngleStepping;
                localOffset = OrbitalAttributes.localOffset;
                localRotationOffset = OrbitalAttributes.localRotationOffset;
                worldOffset = OrbitalAttributes.worldOffset;
            }
        }
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(Orbital), true)]
    public class OrbitalEditor : SolverEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Solver solver = (Solver)target;
            SolverAttributes solverAttributes = solver.SolverAttributes;
            OrbitalAttributes orbitalAttributes = solverAttributes as OrbitalAttributes;
            if (orbitalAttributes)
            {
                solver.CopyValuesFromSolverAttributes();
            }
            else if (solverAttributes)
            {
                if (GUILayout.Button("Create orbital settings from current solver settings"))
                {
                    solverAttributes = CreateInstance<SolverAttributes>();
                    SerializedObject serializedObject = new SerializedObject(solverAttributes);

                    solverAttributes.updateLinkedTransform = solver.UpdateLinkedTransform;
                    solverAttributes.moveLerpTime = solver.MoveLerpTime;
                    solverAttributes.rotateLerpTime = solver.RotateLerpTime;
                    solverAttributes.scaleLerpTime = solver.ScaleLerpTime;
                    solverAttributes.maintainScale = solver.MaintainScale;
                    solverAttributes.smoothing = solver.Smoothing;
                    solverAttributes.lifetime = solver.Lifetime;

                    if (!Directory.Exists("Assets/MixedRealityToolkit.Generated/CustomProfiles/Solver/"))
                    {
                        Directory.CreateDirectory("Assets/MixedRealityToolkit.Generated/CustomProfiles/Solver/");
                    }

                    string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath($"Assets/MixedRealityToolkit.Generated/CustomProfiles/Solver/CustomSolverAttributes.asset");
                    AssetDatabase.CreateAsset(solverAttributes, assetPathAndName);

                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    solver.SolverAttributes = solverAttributes;
                }
            }
            else
            {
                //TODO: figure out what to do here
                //CreateProfile(solver);
            }
        }

        protected override SolverAttributes GenerateAttributes()
        {
            return CreateInstance<OrbitalAttributes>();   
        }

        protected override void CopyFields(SolverAttributes solverAttributes, Solver solver)
        {
            base.CopyFields(solverAttributes, solver);
            Orbital orbital = solver as Orbital;
            OrbitalAttributes orbitalAttributes = solverAttributes as OrbitalAttributes;
            if (orbitalAttributes)
            {
                orbitalAttributes.orientationType = orbital.OrientationType;
                orbitalAttributes.tetherAngleSteps = orbital.TetherAngleSteps;
                orbitalAttributes.useAngleStepping = orbital.UseAngleStepping;
                orbitalAttributes.localOffset = orbital.LocalOffset;
                orbitalAttributes.localRotationOffset = orbital.LocalRotationOffset;
                orbitalAttributes.worldOffset = orbital.WorldOffset;
            }
        }
    }
}

