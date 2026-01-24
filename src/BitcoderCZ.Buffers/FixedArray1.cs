using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using BitcoderCZ.Utils;

namespace BitcoderCZ.Buffers;

[CollectionBuilder(
    typeof(FixedArray1),
    nameof(FixedArray1.Create))]
#if NET8_0_OR_GREATER
[InlineArray(1)]
#endif
public struct FixedArray1<T> : IFixedArray<T>
{
    private const int LengthConst = 1;

#if NET6_0_OR_GREATER
    static int IFixedArray<T>.Length => LengthConst;
#else
    readonly int IFixedArray<T>.Length => LengthConst;
#endif

#if NET8_0_OR_GREATER
    private T _value0;
#else
#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable CS0169
            private T _value0;
#pragma warning restore CS0169
#pragma warning restore IDE0044 // Add readonly modifier
#endif

    [UnscopedRef]
    public ref T Element0
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref _value0;
    }

    [UnscopedRef]
#pragma warning disable CS9181 // Inline array indexer will not be used for element access expression. - required for IFixedArray<T>
    public ref T this[int index]
#pragma warning restore CS9181 // Inline array indexer will not be used for element access expression.
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if (index is not 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(nameof(index));
            }

            return ref _value0;
        }
    }

    public readonly T GetElement(int index)
    {
        if (index is not 0)
        {
            ThrowHelper.ThrowArgumentOutOfRangeException(nameof(index));
        }

        return _value0;
    }

    [UnscopedRef]
    public readonly ref readonly T GetElementRef(int index)
    {
        if (index is not 0)
        {
            ThrowHelper.ThrowArgumentOutOfRangeException(nameof(index));
        }

        return ref Unsafe.AsRef(in _value0);
    }

    public readonly Span<T> AsSpan()
        =>  System.Runtime.InteropServices.MemoryMarshal.CreateSpan(ref Unsafe.AsRef(in _value0), LengthConst);

    public readonly ReadOnlySpan<T> AsROSpan()
        => System.Runtime.InteropServices.MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(in _value0), LengthConst);

    public void Swap(int index1, int index2)
    {
        ref T ref1 = ref this[index1];
        ref T ref2 = ref this[index2];

        T temp = ref1;
        ref1 = ref2;
        ref2 = temp;
    }

#if NET7_0_OR_GREATER
    [UnscopedRef]
    public readonly RefEnumerator GetEnumerator()
        => new RefEnumerator(in this);
#else
    public readonly Enumerator GetEnumerator()
        => new Enumerator(this);
#endif

    readonly IEnumerator<T> IEnumerable<T>.GetEnumerator()
        => new Enumerator(this);

    readonly IEnumerator IEnumerable.GetEnumerator()
        => new Enumerator(this);

    public struct Enumerator : IEnumerator<T>
    {
        private FixedArray1<T> _array;
        private int _index;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Enumerator(FixedArray1<T> array)
        {
            _array = array;
            _index = -1;
        }

        [UnscopedRef]
        public ref readonly T Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _array[_index];
        }

        T IEnumerator<T>.Current => Current;

        object IEnumerator.Current => Current!;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            int next = _index + 1;
            if (next < LengthConst)
            {
                _index = next;
                return true;
            }

            return false;
        }

        public void Reset()
            => _index = -1;

        public readonly void Dispose()
        {
        }
    }

#if NET7_0_OR_GREATER
    public ref struct RefEnumerator : IEnumerator<T>
    {
        private ref readonly FixedArray1<T> _array;
        private int _index;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal RefEnumerator(ref readonly FixedArray1<T> array)
        {
            _array = ref array;
            _index = -1;
        }

        [UnscopedRef]
        public ref readonly T Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _array[_index];
        }

        T IEnumerator<T>.Current => Current;

        object IEnumerator.Current => Current!;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            int next = _index + 1;
            if (next < LengthConst)
            {
                _index = next;
                return true;
            }

            return false;
        }

        public void Reset()
            => _index = -1;

        public readonly void Dispose()
        {
        }
    }
#endif
}

public static class FixedArray1
{
    [System.Runtime.CompilerServices.SkipLocalsInit]
    public static FixedArray1<T> Create<T>(ReadOnlySpan<T> items)
    {
        if (items.Length is not 1)
        {
            ThrowHelper.ThrowArgumentException($"{nameof(items)} must contain exactly 1 elements.", nameof(items));
        }

        System.Runtime.CompilerServices.Unsafe.SkipInit<FixedArray1<T>>(out var result);
        items.CopyTo(result.AsSpan());
        return result;
    }

    [System.Runtime.CompilerServices.SkipLocalsInit]
    public static void Create<T>(ReadOnlySpan<T> items, out FixedArray1<T> result)
    {
        if (items.Length is not 1)
        {
            ThrowHelper.ThrowArgumentException($"{nameof(items)} must contain exactly 1 elements.", nameof(items));
        }

        System.Runtime.CompilerServices.Unsafe.SkipInit(out result);
        items.CopyTo(result.AsSpan());
    }
}