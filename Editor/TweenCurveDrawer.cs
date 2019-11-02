using UnityEditor;
using UnityEngine;

namespace Hirame.Terpsichore.Editor
{
    public class TweenCurveDrawer
    {
        private static string[] assetNames;
        private static TweenCurve[] tweenCurves;

        public static string GetCurveAssetName (int index) => assetNames[index];
        public static TweenCurve GetCurve (int index) => tweenCurves[index];
        
        public static void DrawCurveSelection (SerializedProperty property, int curveIndex, ref Rect lineRect)
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
        
        public static void UpdateCurveAssets (SerializedProperty property)
        {
            var curveIndex = -1;

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
        }

        public static int GetCurveIndex (SerializedProperty property)
        {
            UpdateCurveAssets (property);
            
            var curveProp = property.FindPropertyRelative ("curve");
            var curveAssetId = curveProp.objectReferenceValue;

            var curveIndex = 0;
            
            for (var i = 1; i < assetNames.Length; i++)
            {
                if (curveAssetId == tweenCurves[i])
                    curveIndex = i;
            }

            return curveIndex;
        }
        
    }

}
