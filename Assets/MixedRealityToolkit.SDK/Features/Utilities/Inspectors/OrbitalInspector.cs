using System.IO;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Solvers
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Orbital), true)]
    public class OrbitalEditor : SolverInspector
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
                    orbitalAttributes = CreateInstance<OrbitalAttributes>();
                    SerializedObject serializedObject = new SerializedObject(orbitalAttributes);

                    CopyFields(orbitalAttributes, solver);

                    if (!Directory.Exists("Assets/MixedRealityToolkit.Generated/CustomProfiles/Solver/"))
                    {
                        Directory.CreateDirectory("Assets/MixedRealityToolkit.Generated/CustomProfiles/Solver/");
                    }

                    string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath($"Assets/MixedRealityToolkit.Generated/CustomProfiles/Solver/CustomSolverAttributes.asset");
                    AssetDatabase.CreateAsset(orbitalAttributes, assetPathAndName);

                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    solver.SolverAttributes = orbitalAttributes;
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
