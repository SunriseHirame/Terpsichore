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
        [FieldOffset (sizeof (int))] public float4 FromXYZW;
        [FieldOffset (sizeof (int) * 5)] public float4 ToXYZW;

        // FROM
        [System.NonSerialized] 
        [FieldOffset (sizeof (int))] public float2 FromXY;
        [System.NonSerialized] 
        [FieldOffset (sizeof (int))] public float2 FromZW;
        
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

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void ApplyAsPosition (float t, ref float3 position)
        {
            t = RemapClamped (t, Range.Min, Range.Max, 0, 1);
            position += math.lerp (FromXYZ, ToXYZ, t);
        }
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void ApplyAsRotation (float t, ref float3 rotation)
        {
            t = RemapClamped (t, Range.Min, Range.Max, 0, 1);
            rotation += math.lerp (FromXYZ, ToXYZ, t);
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void ApplyAsScale (float t, ref float3 scale)
        {
            t = RemapClamped (t, Range.Min, Range.Max, 0, 1);
            scale = math.lerp (FromXYZ, ToXYZ, t);
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void ApplyAsColor (float t, ref Color color)
        {
            t = RemapClamped (t, Range.Min, Range.Max, 0, 1);
            color = Color.Lerp (FromRGBA, ToRGBA, t);
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void ApplyAsAnchors (float t, ref float2 min, ref float2 max)
        {
            t = RemapClamped (t, Range.Min, Range.Max, 0, 1);
            min = math.lerp (FromXY, ToXY, t);
            max = math.lerp (FromZW, ToZW, t);
        }

        private static float RemapClamped (float t, float a, float b, float c, float d)
        {
            t = math.remap (a, b, c, d, t);
            t = math.clamp (t, 0, 1);
            return t;
        }
    }
}
