using System.Runtime.CompilerServices;

namespace Among.Switch.Util; 

public static class AlignmentUtility {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int AlignInt(this int value, int mask) {
        return value + mask & ~mask;
    }
}