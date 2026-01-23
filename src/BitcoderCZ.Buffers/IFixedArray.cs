using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using BitcoderCZ.Utils;

namespace BitcoderCZ.Buffers;

public interface IFixedArray<T> : IEnumerable<T>
{
#if NET6_0_OR_GREATER
    static abstract int Length { get; }
#else
    int Length { get; }
#endif

    [UnscopedRef]
    ref T Element0 { get; }

    [UnscopedRef]
    ref T this[int index] { get; }

    T GetElement(int index);

    [UnscopedRef]
    ref readonly T GetElementRef(int index);

    Span<T> AsSpan();

    ReadOnlySpan<T> AsROSpan();

    void Swap(int index1, int index2);
}