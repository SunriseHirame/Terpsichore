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
            
            using (var scope = new EditorGUI.ChangeCheckScope ())
            {
                var tweenTypeProp = property.FindPropertyRelative ("Type");

                var lineRect = position;
                lineRect.height = EditorGUIUtility.singleLineHeight;
            
                EditorGUI.PropertyField (lineRect, property);

                if (!property.isExpanded)
                    return;
            
                lineRect.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField (lineRect, tweenTypeProp);

                DrawTypeFields (property, ref lineRect);
                DrawCurveSelection (property, ref lineRect);

                if (scope.changed)
                    property.serializedObject.ApplyModifiedProperties ();
            }
            
            
        }

        private void DrawTypeFields (SerializedProperty property, ref Rect lineRect)
        {
            var fromProp = property.FindPropertyRelative ("FromXYZW");
            var toProp = property.FindPropertyRelative ("ToXYZW");
            var tweenTypeProp = property.FindPropertyRelative ("Type");

            var from = fromProp.vector4Value;
            var to = toProp.vector4Value;

            switch ((TweenType) tweenTypeProp.intValue)
            {
                case TweenType.Position:
                    goto case TweenType.Scale;
                case TweenType.Rotation:
                    goto case TweenType.Scale;
                case TweenType.Scale:
                    lineRect.y += EditorGUIUtility.singleLineHeight;
                    from = EditorGUI.Vector3Field (lineRect, fromGuiContent, from);

                    lineRect.y += EditorGUIUtility.singleLineHeight;
                    to = EditorGUI.Vector3Field (lineRect, toGuiContent, to);
                    break;
                case TweenType.Color:
                    lineRect.y += EditorGUIUtility.singleLineHeight;
                    from = EditorGUI.ColorField (lineRect, fromGuiContent, from);

                    lineRect.y += EditorGUIUtility.singleLineHeight;
                    to = EditorGUI.ColorField (lineRect, toGuiContent, to);
                    break;
            }

            fromProp.vector4Value = from;
            toProp.vector4Value = to;
        }

        private void DrawCurveSelection (SerializedProperty property, ref Rect lineRect)
        {
            var curveToggleProp = property.FindPropertyRelative ("useCurve");
            var curveProp = property.FindPropertyRelative ("curve");

            curveToggleProp.boolValue = curveProp.objectReferenceValue != null;

            
            lineRect.y += EditorGUIUtility.singleLineHeight;
            lineRect.y += EditorGUIUtility.singleLineHeight;
            
            var ci = EditorGUI.Popup (lineRect, "Curve", curveIndex, assetNames);

            if (curveToggleProp.boolValue)
            {
                var curveCurve = new SerializedObject (curveProp.objectReferenceValue).FindProperty ("curve");
                lineRect.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.CurveField (lineRect, curveCurve.animationCurveValue);
            }
            
            if (ci == curveIndex)
                return;

            curveIndex = ci;
            curveProp.objectReferenceValue = tweenCurves[curveIndex];
            property.serializedObject.ApplyModifiedProperties ();
        }

        private void FindCurveAssets (SerializedProperty property)
        {
            curveIndex = -1;
            
            var curveProp = property.FindPropertyRelative ("curve");
            var assets = AssetDatabase.FindAssets ("t:tweenCurve");

            var curveAssetId = curveProp.objectReferenceValue;
            
            assetNames = new string[assets.Length + 1];
            assetNames[0] = "None";
            
            tweenCurves = new TweenCurve[assets.Length + 1];

            for (var i = 1; i < assetNames.Length; i++)
            {
                tweenCurves[i] = AssetDatabase.LoadAssetAtPath<TweenCurve> (AssetDatabase.GUIDToAssetPath (assets[i - 1]));
                assetNames[i] = tweenCurves[i].name;

                if (curveIndex == -1 && curveAssetId == tweenCurves[i])
                    curveIndex = i;
            }

            curveFetched = true;
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
            return property.isExpanded ? 128 : 16;
        }
    }

}