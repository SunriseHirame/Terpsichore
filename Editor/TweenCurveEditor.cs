using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Hirame.Terpsichore.Editor
{
    
    [CustomEditor (typeof (TweenCurve))]
    public class TweenCurveEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI ()
        {
            serializedObject.Update ();

            var curve = serializedObject.FindProperty ("curve");

            using (var scope = new EditorGUI.ChangeCheckScope ())
            {
                EditorGUILayout.LabelField (curve.displayName);
                EditorGUILayout.PropertyField (curve, GUIContent.none,GUILayout.Height (72));

                if (scope.changed)
                {
                    serializedObject.ApplyModifiedProperties ();
                }
            }
        }
    }

}