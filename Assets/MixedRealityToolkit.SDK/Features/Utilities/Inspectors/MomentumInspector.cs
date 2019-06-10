using System.IO;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Solvers
{
    class MomentumInspector : SolverInspector
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Solver solver = (Solver)target;
            SolverAttributes solverAttributes = solver.SolverAttributes;
            MomentumAttributes momentumAttributes = solverAttributes as MomentumAttributes;
            if (momentumAttributes)
            {
                solver.CopyValuesFromSolverAttributes();
            }
            else if (solverAttributes)
            {
                if (GUILayout.Button("Create momentum settings from current solver settings"))
                {
                    momentumAttributes = CreateInstance<MomentumAttributes>();
                    SerializedObject serializedObject = new SerializedObject(momentumAttributes);

                    CopyFields(momentumAttributes, solver);

                    if (!Directory.Exists("Assets/MixedRealityToolkit.Generated/CustomProfiles/Solver/"))
                    {
                        Directory.CreateDirectory("Assets/MixedRealityToolkit.Generated/CustomProfiles/Solver/");
                    }

                    string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath($"Assets/MixedRealityToolkit.Generated/CustomProfiles/Solver/CustomSolverAttributes.asset");
                    AssetDatabase.CreateAsset(momentumAttributes, assetPathAndName);

                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    solver.SolverAttributes = momentumAttributes;
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
            return CreateInstance<MomentumAttributes>();
        }

        protected override void CopyFields(SolverAttributes solverAttributes, Solver solver)
        {
            base.CopyFields(solverAttributes, solver);
            Momentum momentum = solver as Momentum;
            MomentumAttributes momentumAttributes = solverAttributes as MomentumAttributes;
            if (momentumAttributes)
            {
                momentumAttributes.resistance = momentum.Resistance;
                momentumAttributes.resistanceVelocityPower = momentum.ResistanceVelocityPower;
                momentumAttributes.accelerationRate = momentum.AccelerationRate;
                momentumAttributes.springiness = momentum.Springiness;
                momentumAttributes.snapZ = momentum.SnapZ;
            }
        }
    }
}