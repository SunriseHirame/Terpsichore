using UnityEditor;
using UnityEngine;

namespace Hirame.Terpsichore.Editor
{
    [CustomEditor (typeof (Tween))]
    public class TweenEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI ()
        {
            serializedObject.Update ();

            using (var scope = new EditorGUI.ChangeCheckScope ())
            {
                DrawPropertiesExcluding (serializedObject, "m_Script", "tweens", "tweenFinished");

                DrawTweens ();
                
                if (scope.changed)
                    serializedObject.ApplyModifiedProperties ();
            }
        }

        private void DrawTweens ()
        {
            var tweensProp = serializedObject.FindProperty ("tweens");

            using (new EditorGUILayout.HorizontalScope ())
            {
                EditorGUILayout.PropertyField (tweensProp);
                var add = EditorGUILayout.Popup (GUIContent.none, -1, new string[] {" asd", "va"});

                if (add != -1)
                {
                    tweensProp.InsertArrayElementAtIndex (tweensProp.arraySize);
                }
            }

            if (!tweensProp.isExpanded)
                return;

            var tweensArraySize = tweensProp.arraySize;

            for (var i = 0; i < tweensArraySize; i++)
            {
                using (new GUILayout.VerticalScope (GUI.skin.box))
                {
                    EditorGUILayout.PropertyField (tweensProp.GetArrayElementAtIndex (i), true);
                }
            }

        }
    }

}