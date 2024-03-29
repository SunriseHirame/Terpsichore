﻿using System;
using System.Runtime.CompilerServices;
using Hirame.Pantheon;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

namespace Hirame.Terpsichore
{
    public class UiTweener : MonoBehaviour, ITweener
    {
        [SerializeField] private float length = 1f;

        [MaskField] 
        [SerializeField] private TweenPlayFlags playPlayFlags = TweenPlayFlags.PlayOnEnable;

        [SerializeField] private TweenNode[] tweens;

        [SerializeField] private UnityEvent tweenFinished;

        private RectTransform attachedTransform;

        private int animationDirection = 1;
        private float time;

        public bool IsRunning { get; private set; }
        
        public void Play ()
        {
            enabled = true;
            IsRunning = true;
            TweenRunner.GetOrCreate ().AddTweener (this);
        }

        public void Stop ()
        {
            enabled = false;
            time = 0;
        }

        public void Pause ()
        {
            IsRunning = false;
        }

        public void SetTime (float t)
        {
            time = t;
            Internal_UpdateTweens (t);
        }

        private void OnEnable ()
        {
            if (attachedTransform == false)
                attachedTransform = GetComponent<RectTransform> ();

            if (playPlayFlags.FlagPlayOnEnable ())
                Play ();
            else
                enabled = false;
        }

        private void OnDisable ()
        {
            IsRunning = false;
        }

        public void OnUpdateTween ()
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
            var scale = new float3 (1, 1, 1);
            var color = Color.white;

            var anchorMin = float2.zero;
            var anchorMax = float2.zero;

            var ct = animationDirection;

            for (var i = 0; i < tweens.Length; i++)
            {
                ref var tween = ref tweens[i];

                switch (tween.Type)
                {
                    case FullTweenType.Position:
                        tween.ApplyAsPosition (t, ct, ref position);
                        break;
                    case FullTweenType.Rotation:
                        tween.ApplyAsRotation (t, ct, ref rotation);
                        break;
                    case FullTweenType.Scale:
                        tween.ApplyAsScale (t, ct, ref scale);
                        break;
                    case FullTweenType.Color:
                        tween.ApplyAsColor (t, ct, ref color);
                        break;
                    case FullTweenType.Anchors:
                        tween.ApplyAsAnchors (t, ct, ref anchorMin, ref anchorMax);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException ();
                }
            }

            attachedTransform.localPosition = position;
            attachedTransform.localRotation = Quaternion.Euler (rotation);
            attachedTransform.localScale = scale;

            attachedTransform.anchorMin = anchorMin;
            attachedTransform.anchorMax = anchorMax;
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

        private void OnValidate ()
        {
            if (attachedTransform == false)
                attachedTransform = GetComponent<RectTransform> ();
        }
    }
}