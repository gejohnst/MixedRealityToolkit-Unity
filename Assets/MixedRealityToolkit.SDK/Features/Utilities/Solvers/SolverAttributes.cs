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
        [Tooltip("If 0, the position will update immediately.  Otherwise, the higher this attribute the slower the position updates")]
        public float moveLerpTime = 0.1f;

        [SerializeField]
        [Tooltip("If 0, the rotation will update immediately.  Otherwise, the higher this attribute the slower the rotation updates")]
        public float rotateLerpTime = 0.1f;

        [SerializeField]
        [Tooltip("If 0, the scale will update immediately.  Otherwise, the higher this attribute the slower the scale updates")]
        public float scaleLerpTime = 0;

        [SerializeField]
        [Tooltip("Working output is smoothed if true. Otherwise, snapped")]
        public bool smoothing = true;

        [SerializeField]
        [Tooltip("If true, the Solver will respect the object's original scale values")]
        public bool maintainScale = true;

        [SerializeField]
        [Tooltip("If > 0, this solver will deactivate after this much time, even if the state is still active")]
        public float lifetime = 0;
    }

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
 
    [CustomPropertyDrawer(typeof(ScriptableObject), true)]
    public class ScriptableObjectDrawer : PropertyDrawer
    {
        // Cached scriptable object editor
        private UnityEditor.Editor editor = null;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Draw label
            EditorGUI.PropertyField(position, property, label, true);

            // Draw foldout arrow
            if (property.objectReferenceValue != null)
            {
                property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, GUIContent.none);
            }

            // Draw foldout properties
            if (property.isExpanded)
            {
                // Make child fields be indented
                EditorGUI.indentLevel++;

                // Draw object properties
                if (!editor)
                {
                    UnityEditor.Editor.CreateCachedEditor(property.objectReferenceValue, null, ref editor);
                }
                
                if (editor)
                {
                    editor.OnInspectorGUI();
                }
                

                // Set indent back to what it was
                EditorGUI.indentLevel--;
            }
        }
    }
}
