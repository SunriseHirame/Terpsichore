using System;
using System.Collections;
using System.Collections.Generic;
using Hirame.Pantheon;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

namespace Hirame.Terpsichore
{
    public class UiTween : MonoBehaviour
{
    [SerializeField] private float length = 1f;
        
        [MaskField]
        [SerializeField] private TweenPlayFlags playPlayFlags = TweenPlayFlags.PlayOnEnable;

        [SerializeField] private TweenNode[] tweens;

        [SerializeField] private UnityEvent tweenFinished;
        
        private Transform attachedTransform;
        
        private int animationDirection = 1;
        private float time;

        public void Play ()
        {
            enabled = true;
            time = 0;
        }

        public void Stop ()
        {
            enabled = false;
        }

        private void OnEnable ()
        {
            if (attachedTransform == false)
                attachedTransform = GetComponent<Transform> ();
            
            if (playPlayFlags.FlagPlayOnEnable ())
                Play ();
            else
                enabled = false;
        }

        private void Update ()
        {
            var position = float3.zero;
            var rotation = float3.zero;
            var scale = new float3 (1, 1, 1);
            var color = Color.white;

            //var anchorMin = float2.zero;
            //var anchorMax = float2.zero;

            time += Time.deltaTime / length * animationDirection;
            time = math.clamp (time, 0, 1);
            
            for (var i = 0; i < tweens.Length; i++)
            {
                ref var tween = ref tweens[i];

                switch (tween.Type)
                {
                    case TweenType.Position:
                        tween.ApplyAsPosition (time, ref position);
                        break;
                    case TweenType.Rotation:
                        tween.ApplyAsRotation (time, ref rotation);
                        break;
                    case TweenType.Scale:
                        tween.ApplyAsScale (time, ref scale);
                        break;
                    case TweenType.Color:
                        tween.ApplyAsColor (time, ref color);
                        break;
                    //case TweenType.Anchors:
                    //    tween.ApplyAsAnchors (time, ref anchorMin, ref anchorMax);
                     //   break;
                    default:
                        throw new ArgumentOutOfRangeException ();
                }
            }
            
            attachedTransform.localPosition = position;
            attachedTransform.localRotation = Quaternion.Euler (rotation);
            attachedTransform.localScale = scale;

            //if (image)
            //{
            //    image.color = color;
            //}

            if (time > 0 && time < 1)
                return;
            
            if (CheckFinish ())
            {
                enabled = false;
                tweenFinished.Invoke ();
            }
        }

        private bool CheckFinish ()
        {
            if (playPlayFlags.FlagPingPong ())
            {
                // Switch the animation direction
                animationDirection *= -1;
                return false;
            }

            if (playPlayFlags.FlagLoop ())
            {
                time = 0;
                return false;
            }

            return true;
        }
}

}