using System;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace Hirame.Terpsichore.Editor
{
    [CustomEditor (typeof (Tweener))]
    public class TweenEditor : TweenEditorBase<Tweener, TweenType>
    {
    }
    
    [CustomEditor (typeof (UiTweener))]
    public class UiTweenEditor : TweenEditorBase<UiTweener, UiTweenType>
    {
    }

    public class TweenEditorBase<TTweenType, TTweenFlagType> : UnityEditor.Editor 
        where TTweenType : MonoBehaviour, ITweener 
        where TTweenFlagType : Enum
    {
        protected string[] tweenTypeNames = Enum.GetNames (typeof (TTweenFlagType));

        private float previewValue;

        private TTweenType tween;

        protected void OnEnable ()
        {
            tween = target as TTweenType;
            tweenTypeNames = Enum.GetNames (typeof (TTweenFlagType));
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