// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.IO;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Solvers
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Solver), true)]
    public class SolverInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            Solver solver = (Solver)target;
            if (!solver.SolverAttributes)
            {
                if (GUILayout.Button("Create profile from current settings"))
                {
                    CreateProfile(solver);
                }
            }
            else
            {
                solver.CopyValuesFromSolverAttributes();
            }

            DrawDefaultInspector();
        }

        protected void CreateProfile(Solver solver)
        {
            SolverAttributes solverAttributes = GenerateAttributes();
            SerializedObject serializedObject = new SerializedObject(solverAttributes);

            CopyFields(solverAttributes, solver);

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

        protected virtual SolverAttributes GenerateAttributes()
        {
            return CreateInstance<SolverAttributes>();
        }

        protected virtual void CopyFields(SolverAttributes solverAttributes, Solver solver)
        {
            solverAttributes.updateLinkedTransform = solver.UpdateLinkedTransform;
            solverAttributes.moveLerpTime = solver.MoveLerpTime;
            solverAttributes.rotateLerpTime = solver.RotateLerpTime;
            solverAttributes.scaleLerpTime = solver.ScaleLerpTime;
            solverAttributes.maintainScale = solver.MaintainScale;
            solverAttributes.smoothing = solver.Smoothing;
            solverAttributes.lifetime = solver.Lifetime;
        }
    }
}