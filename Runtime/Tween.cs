using System;
using System.Runtime.CompilerServices;
using Hirame.Pantheon;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

namespace Hirame.Terpsichore
{
    public sealed class Tween : MonoBehaviour, ITween
    {
        [SerializeField] private float length = 1f;

        [MaskField] [SerializeField] private TweenPlayFlags playFlags = TweenPlayFlags.PlayOnEnable;

        [SerializeField] private TweenNode[] tweens;

        [SerializeField] private UnityEvent tweenFinished;

        private Transform attachedTransform;

        private int animationDirection = 1;
        private float time;

        private TweenType tweenTypeMask;

        public void Play ()
        {
            enabled = true;
        }

        public void Stop ()
        {
            enabled = false;
            time = 0;
        }

        public void Pause ()
        {
            enabled = false;
        }

        public void SetTime (float t)
        {
#if UNITY_EDITOR
            if (!attachedTransform)
                attachedTransform = GetComponent<Transform> ();
#endif
            time = t;
            Internal_UpdateTweens (t);
        }

        private void OnEnable ()
        {
            if (attachedTransform == false)
                attachedTransform = GetComponent<Transform> ();

            if (playFlags.FlagPlayOnEnable ())
                Play ();
            else
                enabled = false;
        }

        private void Update ()
        {
            time += Time.deltaTime / length * animationDirection;
            time = math.clamp (time, 0, 1);

            Internal_UpdateTweens (time);

            if (time > 0 && time < 1)
                return;

            if (CheckFinish ())
            {
                enabled = false;
                tweenFinished.Invoke ();
            }
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        private void Internal_UpdateTweens (float t)
        {
            var position = float3.zero;
            var rotation = float3.zero;
            var scale = float3.zero;

            var color = Color.white;
            var mask = (FullTweenType) 0;

            var ct = animationDirection;
            
            for (var i = 0; i < tweens.Length; i++)
            {
                ref var tween = ref tweens[i];
                
                switch (tween.Type)
                {
                    case FullTweenType.Position:
                        mask |= FullTweenType.Position;
                        tween.ApplyAsPosition (t, ct, ref position);
                        break;
                    case FullTweenType.Rotation:
                        mask |= FullTweenType.Rotation;
                        tween.ApplyAsRotation (t, ct, ref rotation);
                        break;
                    case FullTweenType.Scale:
                        mask |= FullTweenType.Scale;
                        tween.ApplyAsScale (t, ct, ref scale);
                        break;
                    case FullTweenType.Color:
                        mask |= FullTweenType.Color;
                        tween.ApplyAsColor (t, ct, ref color);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException ();
                }
            }

            if ((mask & FullTweenType.Position) == FullTweenType.Position)
                attachedTransform.localPosition = position;

            if ((mask & FullTweenType.Rotation) == FullTweenType.Rotation)
                attachedTransform.localRotation = Quaternion.Euler (rotation);

            if ((mask & FullTweenType.Scale) == FullTweenType.Scale)
                attachedTransform.localScale = scale;
        }

        private bool CheckFinish ()
        {
            if (playFlags.FlagPingPong ())
            {
                // Switch the animation direction
                animationDirection *= -1;
                return false;
            }

            if (playFlags.FlagLoop ())
            {
                time = 0;
                return false;
            }

            return true;
        }
    }
}