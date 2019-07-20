using System.Runtime.CompilerServices;

namespace Hirame.Terpsichore
{
    public enum TweenType
    {
        Position,
        Rotation,
        Scale,
        Color,
        Anchors
    }

    [System.Flags]
    public enum TweenPlayFlags
    {
        PlayOnStart = 1 << 0,
        PlayOnEnable = 1 << 1,
        Reverse = 1 << 2,
        Loop = 1 << 3,
        PingPong = 1 << 4
    }

    public static class TweenPlayFlagsExtensions
    {
        public static bool HasFlagNonAlloc (this TweenPlayFlags self, TweenPlayFlags flag)
        {
            return (self & flag) == flag;
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static bool FlagPingPong (this TweenPlayFlags self)
        {
            return self.HasFlagNonAlloc (TweenPlayFlags.PingPong);
        }
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static bool FlagLoop (this TweenPlayFlags self)
        {
            return self.HasFlagNonAlloc (TweenPlayFlags.Loop);
        }
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static bool FlagReversed (this TweenPlayFlags self)
        {
            return self.HasFlagNonAlloc (TweenPlayFlags.Reverse);
        }
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static bool FlagPlayOnStart (this TweenPlayFlags self)
        {
            return self.HasFlagNonAlloc (TweenPlayFlags.PlayOnStart);
        }
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static bool FlagPlayOnEnable (this TweenPlayFlags self)
        {
            return self.HasFlagNonAlloc (TweenPlayFlags.PlayOnEnable);
        }
    }
}