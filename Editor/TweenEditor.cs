using System;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace Hirame.Terpsichore.Editor
{
    [CustomEditor (typeof (Tween))]
    public class TweenEditor : TweenEditorBase<Tween, TweenType>
    {
    }
    
    [CustomEditor (typeof (UiTween))]
    public class UiTweenEditor : TweenEditorBase<UiTween, UiTweenType>
    {
    }

    public class TweenEditorBase<T1, T2> : UnityEditor.Editor 
        where T1 : MonoBehaviour, ITween 
        where T2 : Enum
    {
        protected string[] tweenTypeNames = Enum.GetNames (typeof (T2));

        private float previewValue;

        private T1 tween;

        protected void OnEnable ()
        {
            tween = target as T1;
            tweenTypeNames = Enum.GetNames (typeof (T2));
        }

        public override void OnInspectorGUI ()
        {
            if (tween == null)
            {
                OnEnable ();
                return;
            }
            
            serializedObject.Update ();

            using (var scope = new EditorGUI.ChangeCheckScope ())
            {
                EditorGUILayout.Space ();

                using (new GUILayout.VerticalScope (GUI.skin.box))
                {
                    var newPreview = EditorGUILayout.Slider ("Preview", previewValue, 0, 1);
                    
                    if (newPreview != previewValue)
                    {
                        previewValue = newPreview;
                        tween.SetTime (previewValue);
                    }
                }

                using (new GUILayout.VerticalScope (GUI.skin.box))
                {
                    DrawPropertiesExcluding (serializedObject, "m_Script", "tweens", "tweenFinished");
                }


                using (new GUILayout.VerticalScope (GUI.skin.box))
                {
                    DrawTeenNodes ();
                }

                if (scope.changed)
                    serializedObject.ApplyModifiedProperties ();
            }
        }

        private void DrawTeenNodes ()
        {
            var tweensProp = serializedObject.FindProperty ("tweens");

            using (new EditorGUILayout.HorizontalScope ())
            {
                EditorGUILayout.PropertyField (tweensProp, GUILayout.MaxWidth (EditorGUIUtility.labelWidth));
                EditorGUILayout.LabelField ("Add", GUILayout.Width (50));
                
                var add = EditorGUILayout.Popup (GUIContent.none, -1, tweenTypeNames);

                if (add != -1)
                {
                    AddTweenNode (tweensProp, add);
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

        private void AddTweenNode (SerializedProperty tweensProp, int type)
        {
            var insertIndex = tweensProp.arraySize;
                    
            tweensProp.InsertArrayElementAtIndex (tweensProp.arraySize);
            tweensProp.GetArrayElementAtIndex (insertIndex).FindPropertyRelative ("Type").intValue = (int) math.pow (2, type);
            
            var range = tweensProp.GetArrayElementAtIndex (insertIndex).FindPropertyRelative ("Range");
            range.FindPropertyRelative ("Min").floatValue = 0;
            range.FindPropertyRelative ("Max").floatValue = 1;
        }
    }

}