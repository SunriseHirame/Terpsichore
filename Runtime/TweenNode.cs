using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Hirame.Pantheon;
using Unity.Mathematics;
using UnityEngine;

namespace Hirame.Terpsichore
{
    [System.Serializable, StructLayout (LayoutKind.Explicit)]
    public struct TweenNode
    {
        [FieldOffset (0)] public TweenType Type;

        // Base serialized
        [FieldOffset (sizeof (int))] public Vector4 FromXYZW;
        [FieldOffset (sizeof (int) * 5)] public Vector4 ToXYZW;

        // FROM
        [System.NonSerialized] 
        [FieldOffset (sizeof (int))] public float2 FromXY;
        [System.NonSerialized] 
        [FieldOffset (sizeof (int) * 2)] public float2 FromZW;
        
        [System.NonSerialized] 
        [FieldOffset (sizeof (int))] public float3 FromXYZ;
        [System.NonSerialized] 
        [FieldOffset (sizeof (int))] public Color FromRGBA;
        
        // TO
        [System.NonSerialized]
        [FieldOffset (sizeof (int) * 5)] public float2 ToXY;
        [System.NonSerialized]
        [FieldOffset (sizeof (int) * 7)] public float2 ToZW;
        
        [System.NonSerialized]
        [FieldOffset (sizeof (int) * 5)] public float3 ToXYZ;
        [System.NonSerialized]
        [FieldOffset (sizeof (int) * 5)] public Color ToRGBA;

        [MinMax (0, 1), FieldOffset (sizeof (int) * 9)]
        public FloatMinMax Range;

        [FieldOffset (sizeof (int) * 11)]
        public bool useCurve;
        
        // For some reason 11 crashes?
        [FieldOffset (sizeof (int) * 12)]
        public TweenCurve curve;
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void ApplyAsPosition (float t, float ct, ref float3 position)
        {
            t = RemapClamped (Range.Min, Range.Max, t);
            t = SampleCurve (t, ct);

            position += math.lerp (FromXYZ, ToXYZ, t);
        }
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void ApplyAsRotation (float t, float ct, ref float3 rotation)
        {
            t = RemapClamped (Range.Min, Range.Max, t);
            t = SampleCurve (t, ct);

            rotation += math.lerp (FromXYZ, ToXYZ, t);
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void ApplyAsScale (float t, float ct, ref float3 scale)
        {
            t = RemapClamped (Range.Min, Range.Max, t);
            t = SampleCurve (t, ct);

            scale = math.lerp (FromXYZ, ToXYZ, t);
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void ApplyAsColor (float t, float ct, ref Color color)
        {
            t = RemapClamped (Range.Min, Range.Max, t);
            t = SampleCurve (t, ct);
            
            color = Color.Lerp (FromRGBA, ToRGBA, t);
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void ApplyAsAnchors (float t, float ct, ref float2 min, ref float2 max)
        {
            t = RemapClamped (Range.Min, Range.Max, t);
            t = SampleCurve (t, ct);

            min = math.lerp (FromXY, ToXY, t);
            max = math.lerp (FromZW, ToZW, t);
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        private float SampleCurve (float t, float ct)
        {
            if (!useCurve || curve == false)
                return t;

            return ct > 0 ? curve.Evaluate (t) : 1 - curve.Evaluate (1 - t);
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        private static float RemapClamped (float a, float b, float t)
        {
            t = math.remap (a, b, 0, 1, t);
            t = math.clamp (t, 0, 1);
            return t;
        }
        
    }
}
