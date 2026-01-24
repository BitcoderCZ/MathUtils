using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BitcoderCZ.Buffers;

public static class FixedArray
{
    extension<TArray, TElement>(TArray) where TArray : struct, IFixedArray<TElement>
    {
        [SkipLocalsInit]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetLength()
#if NET6_0_OR_GREATER
            => TArray.Length;
#else
        {
            Unsafe.SkipInit<TArray>(out var array);
            return array.Length;
        }
#endif
    }
}