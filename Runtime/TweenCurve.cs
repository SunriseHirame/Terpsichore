using UnityEngine;

namespace Hirame.Terpsichore
{
    [CreateAssetMenu (menuName = "Hirame/Terpsichore/Tween Curve")]
    public class TweenCurve : ScriptableObject
    {
        [SerializeField] private AnimationCurve curve;

        public float Evaluate (float t) => curve.Evaluate (t);

        private void Reset ()
        {
            curve = AnimationCurve.EaseInOut (0,0,1,1);
        }
    }

}