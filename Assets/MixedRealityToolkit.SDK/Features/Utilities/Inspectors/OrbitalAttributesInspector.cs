using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Utilities.Solvers
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(OrbitalAttributes), true)]
    public class OrbitalAttributeEditor : SolverAttributeEditor
    {
        private bool showOrbitalFoldout;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(OrbitalAttributes.orientationType)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(OrbitalAttributes.localOffset)));
            showOrbitalFoldout = EditorGUILayout.Foldout(showOrbitalFoldout, "Advanced Orbital Attributes");
            if (showOrbitalFoldout)
            {
                OrbitalAttributes orbitalAttributes = (OrbitalAttributes)target;
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(OrbitalAttributes.useAngleStepping)));
                if (orbitalAttributes.useAngleStepping)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(OrbitalAttributes.tetherAngleSteps)));
                }
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(OrbitalAttributes.localRotationOffset)));
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(OrbitalAttributes.worldOffset)));
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
