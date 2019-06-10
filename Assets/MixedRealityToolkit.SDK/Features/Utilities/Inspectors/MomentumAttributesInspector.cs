using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Utilities.Solvers
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(MomentumAttributes), true)]
    public class MomentumAttributesInspector : SolverAttributeEditor
    {
        private bool showMomentumFoldout;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            showMomentumFoldout = EditorGUILayout.Foldout(showMomentumFoldout, "Advanced Momentum Attributes");
            if (showMomentumFoldout)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(MomentumAttributes.resistance)));
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(MomentumAttributes.resistanceVelocityPower)));
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(MomentumAttributes.accelerationRate)));
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(MomentumAttributes.springiness)));
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(MomentumAttributes.snapZ)));
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}