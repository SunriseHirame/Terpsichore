﻿using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace Hirame.Terpsichore.Editor
{
    [CustomPropertyDrawer (typeof (TweenNode))]
    public class TweenNodeDrawer : PropertyDrawer
    {
        private static GUIContent fromGuiContent;
        private static GUIContent toGuiContent;

        private static string[] assetNames;
        private static TweenCurve[] tweenCurves;

        private bool curveFetched;
        private int curveIndex;

        public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
        {
            SetupGUIContents ();

            if (!curveFetched)
                FindCurveAssets (property);
            else
                UpdateCurveIndex (property);

            var tweenTypeProp = property.FindPropertyRelative ("Type");

            if (property.name.Equals ("data"))
            {
                label.text = tweenTypeProp.enumDisplayNames[(int) math.sqrt (tweenTypeProp.enumValueIndex)];
            }


            var lineRect = position;
            lineRect.height = EditorGUIUtility.singleLineHeight;

            EditorGUI.PropertyField (lineRect, property, label);

            if (!property.isExpanded)
                return;

            lineRect.y += EditorGUIUtility.singleLineHeight * 1.5f;

            EditorGUI.PropertyField (lineRect, tweenTypeProp);

            DrawTypeFields (property, ref lineRect);
            DrawRange (property, ref lineRect);
            DrawCurveSelection (property, ref lineRect);
        }

        private void DrawTypeFields (SerializedProperty property, ref Rect lineRect)
        {
            var fromProp = property.FindPropertyRelative ("FromXYZW");
            var toProp = property.FindPropertyRelative ("ToXYZW");
            var tweenTypeProp = property.FindPropertyRelative ("Type");

            var from = fromProp.vector4Value;
            var to = toProp.vector4Value;

            switch ((FullTweenType) tweenTypeProp.intValue)
            {
                case FullTweenType.Position:
                    goto case FullTweenType.Scale;
                case FullTweenType.Rotation:
                    goto case FullTweenType.Scale;
                case FullTweenType.Scale:
                    lineRect.y += EditorGUIUtility.singleLineHeight + 2;
                    from = EditorGUI.Vector3Field (lineRect, fromGuiContent, from);

                    lineRect.y += EditorGUIUtility.singleLineHeight + 2;
                    to = EditorGUI.Vector3Field (lineRect, toGuiContent, to);
                    break;
                case FullTweenType.Color:
                    lineRect.y += EditorGUIUtility.singleLineHeight + 2;
                    from = EditorGUI.ColorField (lineRect, fromGuiContent, from);

                    lineRect.y += EditorGUIUtility.singleLineHeight + 2;
                    to = EditorGUI.ColorField (lineRect, toGuiContent, to);
                    break;
                case FullTweenType.Anchors:
                    from = DrawAnchor (fromGuiContent, in from, ref lineRect);
                    to = DrawAnchor (toGuiContent, in to, ref lineRect);
                    break;
            }

            fromProp.vector4Value = from;
            toProp.vector4Value = to;
        }

        private Vector4 DrawAnchor (GUIContent label, in Vector4 anchors, ref Rect lineRect)
        {
            var min = new Vector2 (anchors.x, anchors.y);
            var max = new Vector2 (anchors.z, anchors.w);
            
            lineRect.y += EditorGUIUtility.singleLineHeight + 2;

            var contentRect = EditorGUI.PrefixLabel (lineRect, label);
            var contentLabelRect = contentRect;

            contentRect.x += 40;
            contentRect.width = contentRect.width * 0.5f - 40;
            contentLabelRect.width -= 40;
                    
            EditorGUI.LabelField (contentLabelRect, "Min");
            min = EditorGUI.Vector2Field (contentRect, GUIContent.none, min);

            contentRect.x += contentRect.width + 40;

            contentLabelRect.x += contentRect.width + 40;
            EditorGUI.LabelField (contentLabelRect, "Max");
            max = EditorGUI.Vector2Field (contentRect, GUIContent.none, max);

            return new Vector4 (min.x, min.y, max.x, max.y);
        }

        private void DrawRange (SerializedProperty property, ref Rect lineRect)
        {
            var rangeProp = property.FindPropertyRelative ("Range");

            lineRect.y += EditorGUIUtility.singleLineHeight + 2;
            EditorGUI.PropertyField (lineRect, rangeProp);
        }

        private void DrawCurveSelection (SerializedProperty property, ref Rect lineRect)
        {
            var curveToggleProp = property.FindPropertyRelative ("useCurve");
            var curveProp = property.FindPropertyRelative ("curve");

            curveToggleProp.boolValue = curveProp.objectReferenceValue != null;

            lineRect.y += EditorGUIUtility.singleLineHeight * 1.5f;

            var curveSelectRect = lineRect;
            curveSelectRect.width -= 52;

            var newCurveIndex = EditorGUI.Popup (curveSelectRect, "Curve", curveIndex, assetNames);
            if (newCurveIndex != curveIndex)
            {
                curveIndex = newCurveIndex;
                curveProp.objectReferenceValue = tweenCurves[curveIndex];
                return;
            }

            var buttonRect = lineRect;
            buttonRect.x += lineRect.width - 50;
            buttonRect.width = 50;

            if (GUI.Button (buttonRect, "Show"))
            {
                EditorGUIUtility.PingObject (curveProp.objectReferenceValue);
            }

            if (curveToggleProp.boolValue)
            {
                var curveCurve = new SerializedObject (curveProp.objectReferenceValue).FindProperty ("curve");
                lineRect.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.CurveField (lineRect, curveCurve.animationCurveValue);
            }
        }


        private void FindCurveAssets (SerializedProperty property)
        {
            curveIndex = -1;

            var assets = AssetDatabase.FindAssets ("t:tweenCurve");
            var curveProp = property.FindPropertyRelative ("curve");
            var curveAssetId = curveProp.objectReferenceValue;

            assetNames = new string[assets.Length + 1];
            assetNames[0] = "None";

            tweenCurves = new TweenCurve[assets.Length + 1];

            for (var i = 1; i < assetNames.Length; i++)
            {
                tweenCurves[i] =
                    AssetDatabase.LoadAssetAtPath<TweenCurve> (AssetDatabase.GUIDToAssetPath (assets[i - 1]));
                assetNames[i] = tweenCurves[i].name;

                if (curveIndex == -1 && curveAssetId == tweenCurves[i])
                    curveIndex = i;
            }

            if (curveIndex == -1)
                curveIndex = 0;

            curveFetched = true;
        }

        private void UpdateCurveIndex (SerializedProperty property)
        {
            var curveProp = property.FindPropertyRelative ("curve");
            var curveAssetId = curveProp.objectReferenceValue;

            for (var i = 1; i < assetNames.Length; i++)
            {
                if (curveAssetId == tweenCurves[i])
                    curveIndex = i;
            }
        }

        private static void SetupGUIContents ()
        {
            if (fromGuiContent != null)
                return;

            fromGuiContent = new GUIContent ("From");
            toGuiContent = new GUIContent ("To");
        }

        public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
        {
            return (property.isExpanded ? 7.5f : 1) * 18;
        }
    }
}