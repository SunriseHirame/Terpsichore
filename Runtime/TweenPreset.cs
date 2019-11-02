using UnityEngine;

namespace Hirame.Terpsichore
{
    [CreateAssetMenu (menuName = "Hirame/Terpsichore/Tween Preset")]
    public class TweenPreset : ScriptableObject
    {
        [SerializeField] private TweenNode[] tweens;
    }
}