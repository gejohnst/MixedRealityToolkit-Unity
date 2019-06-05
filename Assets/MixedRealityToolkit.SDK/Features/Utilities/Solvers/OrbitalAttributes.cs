using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using UnityEngine;
using UnityEditor;
using System.IO;

[CreateAssetMenu(fileName = "OrbitalAttributes.asset", menuName = "Orbital Attributes")]
public class OrbitalAttributes : SolverAttributes
{
    [Tooltip("The desired orientation of this object. Default sets the object to face the TrackedObject/TargetTransform. CameraFacing sets the object to always face the user.")]
    public SolverOrientationType orientationType = SolverOrientationType.FollowTrackedObject;

    [Range(2, 24)]
    [Tooltip("The division of steps this object can tether to. Higher the number, the more snapple steps.")]
    public int tetherAngleSteps = 6;

    [Tooltip("Lock the rotation to a specified number of steps around the tracked object.")]
    public bool useAngleStepping = false;

    [SerializeField]
    [Tooltip("XYZ offset for this object in relation to the TrackedObject/TargetTransform. Mixing local and world offsets is not recommended.")]
    public Vector3 localOffset = new Vector3(0, -1, 1);

    [SerializeField]
    [Tooltip("XYZ offset for this object in relation to the TrackedObject/TargetTransform. Mixing local and world offsets is not recommended.")]
    public Vector3 localRotationOffset = new Vector3(0, 0, 0);

    [SerializeField]
    [Tooltip("XYZ offset for this object in worldspace, best used with the YawOnly orientationType. Mixing local and world offsets is not recommended.")]
    public Vector3 worldOffset = Vector3.zero;
}

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