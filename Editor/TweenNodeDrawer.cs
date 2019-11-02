using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace Hirame.Terpsichore.Editor
{
    [CustomPropertyDrawer (typeof (TweenNode))]
    public class TweenNodeDrawer : PropertyDrawer
    {
        private static GUIContent fromGuiContent;
        private static GUIContent toGuiContent;

        private int curveIndex;

        public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
        {
            SetupGUIContents ();

            var tweenTypeProp = property.FindPropertyRelative ("Type");

            if (property.name.Equals ("data"))
            {
                var enumIndex = (int) math.sqrt (tweenTypeProp.enumValueIndex);
                label.text = enumIndex >= 0 && enumIndex < tweenTypeProp.enumDisplayNames.Length 
                    ? tweenTypeProp.enumDisplayNames[enumIndex]
                    : "None";
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
            
            curveIndex = TweenCurveDrawer.GetCurveIndex (property);
            TweenCurveDrawer.DrawCurveSelection (property, curveIndex, ref lineRect);
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