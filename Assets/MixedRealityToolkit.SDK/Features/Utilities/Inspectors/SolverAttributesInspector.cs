using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Utilities.Solvers
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SolverAttributes), true)]
    public class SolverAttributeEditor : UnityEditor.Editor
    {
        private bool showSolverFoldout;
        public override void OnInspectorGUI()
        {
            showSolverFoldout = EditorGUILayout.Foldout(showSolverFoldout, "Base Solver Attributes");
            if (showSolverFoldout)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(SolverAttributes.updateLinkedTransform)));
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(SolverAttributes.moveLerpTime)));
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(SolverAttributes.rotateLerpTime)));
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(SolverAttributes.scaleLerpTime)));
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(SolverAttributes.smoothing)));
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(SolverAttributes.maintainScale)));
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(SolverAttributes.lifetime)));
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
