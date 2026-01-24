using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static BitcoderCZ.Utils.ThrowHelper;

namespace BitcoderCZ.Buffers;

public static class ImmutableInlineArray
{
    [OverloadResolutionPriority(1)]
    public static void Create<TArray, TElement>(out ImmutableInlineArray<TArray, TElement> array, params ReadOnlySpan<TElement> items)
        where TArray : struct, IFixedArray<TElement>
        where TElement : IEquatable<TElement>
    {
        if (items.IsEmpty)
        {
            array = ImmutableInlineArray<TArray, TElement>.Empty;
            return;
        }

        int inlineCapacity = ImmutableInlineArray<TArray, TElement>.InlineCapacity;

        TArray inline = default;
        items[..Math.Min(items.Length, inlineCapacity)].CopyTo(inline.AsSpan());

        TElement[]? overflow = null;
        if (items.Length > inlineCapacity)
        {
            overflow = new TElement[items.Length - inlineCapacity];
            items[inlineCapacity..].CopyTo(overflow);
        }

        array = new ImmutableInlineArray<TArray, TElement>(items.Length, inline, overflow);
    }

    [OverloadResolutionPriority(1)]
    public static ImmutableInlineArray<TArray, TElement> Create<TArray, TElement>(params ReadOnlySpan<TElement> items)
        where TArray : struct, IFixedArray<TElement>
        where TElement : IEquatable<TElement>
    {
        Create<TArray, TElement>(out var array, items);
        return array;
    }

    public static void Create<TArray, TElement>(out ImmutableInlineArray<TArray, TElement> array, IEnumerable<TElement> items, bool trim = false)
        where TArray : struct, IFixedArray<TElement>
        where TElement : IEquatable<TElement>
    {
        ThrowIfNull(items);

        int inlineCapacity = ImmutableInlineArray<TArray, TElement>.InlineCapacity;
#if NET6_0_OR_GREATER
        if (items.TryGetNonEnumeratedCount(out int count))
        {
            if (count == 0)
            {
                array = ImmutableInlineArray<TArray, TElement>.Empty;
                return;
            }

            TArray inline = default;

            TElement[]? overflow = count > inlineCapacity ? new TElement[count - inlineCapacity] : null;

            int i = 0;
            int o = 0;

            foreach (var item in items)
            {
                if (i < inlineCapacity)
                {
                    inline[i++] = item;
                }
                else
                {
                    overflow![o++] = item;
                }
            }

            array = new ImmutableInlineArray<TArray, TElement>(count, inline, overflow);
            return;
        }
#endif

        using var enumerator = items.GetEnumerator();

        if (!enumerator.MoveNext())
        {
            array = ImmutableInlineArray<TArray, TElement>.Empty;
            return;
        }

        TArray inlineFallback = default;

        TElement[]? overflowFallback = null;
        int overflowCount = 0;
        int length = 0;

        do
        {
            var item = enumerator.Current;

            if (length < inlineCapacity)
            {
                inlineFallback[length] = item;
            }
            else
            {
                if (overflowFallback is null)
                {
                    overflowFallback = new TElement[Math.Min(inlineCapacity, 4)];
                }
                else if (overflowCount == overflowFallback.Length)
                {
                    Array.Resize(ref overflowFallback, overflowFallback.Length * 2);
                }

                overflowFallback[overflowCount++] = item;
            }

            length++;
        }
        while (enumerator.MoveNext());

        if (overflowCount is 0)
        {
            overflowFallback = null;
        }
        else if (trim && overflowCount != overflowFallback!.Length)
        {
            Array.Resize(ref overflowFallback, overflowCount);
        }

        array = new ImmutableInlineArray<TArray, TElement>(length, inlineFallback, overflowFallback);
    }

    public static ImmutableInlineArray<TArray, TElement> Create<TArray, TElement>(IEnumerable<TElement> items)
        where TArray : struct, IFixedArray<TElement>
        where TElement : IEquatable<TElement>
    {
        Create<TArray, TElement>(out var array, items);
        return array;
    }
}

[StructLayout(LayoutKind.Auto)]
public readonly struct ImmutableInlineArray<TArray, TElement> : IReadOnlyList<TElement>
    where TArray : struct, IFixedArray<TElement>
    where TElement : IEquatable<TElement>
{
    public static readonly ImmutableInlineArray<TArray, TElement> Empty = new(0, default, null);

    internal static readonly int InlineCapacity = FixedArray.GetLength<TArray, TElement>();

    private readonly int _count;
    private readonly TArray _inline;
    private readonly TElement[]? _overflow;

    internal ImmutableInlineArray(int count, TArray inline, TElement[]? overflow)
    {
        _count = count;
        _inline = inline;
        _overflow = overflow;
    }

    public int Count => _count;

    public readonly TElement this[int index]
    {
        get
        {
            ThrowIfGreaterThanOrEqualToOrNegative(index, Count, nameof(index));

            return index < InlineCapacity
                ? _inline.GetElement(index)
                : _overflow![index - InlineCapacity];
        }
    }

    public readonly bool Contains(TElement item, EqualityComparer<TElement> comparer)
        => IndexOf(item, comparer) >= 0;

    public readonly void CopyTo(TElement[] array, int arrayIndex)
        => CopyTo(array.AsSpan(arrayIndex));

    public readonly void CopyTo(Span<TElement> span)
    {
        if (span.Length < _count)
        {
            ThrowArgumentOutOfRangeException(nameof(span), $"{nameof(span)} is not large enough.");
        }

        _inline.AsROSpan()[..Math.Min(_count, InlineCapacity)].CopyTo(span);

        _overflow?.AsSpan()[..(_count - InlineCapacity)].CopyTo(span[InlineCapacity..]);
    }

    public readonly int IndexOf(TElement item, EqualityComparer<TElement> comparer)
    {
        if (_count == 0)
        {
            return -1;
        }

        var bufferSpan = _inline.AsROSpan();
        for (int i = 0; i < Math.Min(_count, InlineCapacity); i++)
        {
#pragma warning disable HAM0001 // Operation causes the compiler to create a defensive copy
            if (comparer.Equals(bufferSpan[i], item))
#pragma warning restore HAM0001 // Operation causes the compiler to create a defensive copy
            {
                return i;
            }
        }

        if (_overflow is not null && _count > InlineCapacity)
        {
            int index = _overflow.AsSpan()[..(_count - InlineCapacity)].IndexOf(item);
            return index == -1 ? -1 : index + InlineCapacity;
        }

        return -1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Enumerator GetEnumerator()
       => new Enumerator(this);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly IEnumerator<TElement> IEnumerable<TElement>.GetEnumerator()
        => GetEnumerator();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    public struct Builder
    {
        private int _count;
        private TArray _inline;
        private TElement[]? _overflow;

        internal Builder(int capacity)
        {
            if (capacity > InlineCapacity)
                _overflow = new TElement[capacity - InlineCapacity];
        }

        public readonly int Count => _count;

        public void Add(TElement value)
        {
            if (_count < InlineCapacity)
            {
                _inline[_count] = value;
            }
            else
            {
                int overflowIndex = _count - InlineCapacity;
                EnsureOverflowCapacity(overflowIndex + 1);
                _overflow![overflowIndex] = value;
            }

            _count++;
        }

        public void Clear()
        {
            _count = 0;
            _inline = default;
            _overflow = null;
        }

        public readonly void ToImmutable(out ImmutableInlineArray<TArray, TElement> array)
        {
            if (_count <= InlineCapacity)
            {
                array = new ImmutableInlineArray<TArray, TElement>(_count, _inline, null);
                return;
            }

            int overflowCount = _count - InlineCapacity;
            TElement[] overflow = new TElement[overflowCount];
            _overflow.AsSpan(0, overflowCount).CopyTo(overflow);

            array = new ImmutableInlineArray<TArray, TElement>(_count, _inline, overflow);
        }

        public readonly ImmutableInlineArray<TArray, TElement> ToImmutable()
        {
            ToImmutable(out var array);
            return array;
        }

        public void DrainToImmutable(out ImmutableInlineArray<TArray, TElement> array, bool trim = false)
        {
            if (_count <= InlineCapacity)
            {
                array = new ImmutableInlineArray<TArray, TElement>(_count, _inline, null);
                Clear();
                return;
            }

            TElement[] overflow;

            int overflowCount = _count - InlineCapacity;
            if (trim && overflowCount != _overflow!.Length)
            {
                overflow = new TElement[overflowCount];
                _overflow.AsSpan(0, overflowCount).CopyTo(overflow);

                // can reuse _overflow
                _overflow.AsSpan().Clear();
            }
            else
            {
                overflow = _overflow!;
                _overflow = null;
            }

            array = new ImmutableInlineArray<TArray, TElement>(_count, _inline, overflow);
            _count = 0;
            _inline = default;
        }

        public ImmutableInlineArray<TArray, TElement> DrainToImmutable(bool trim = false)
        {
            DrainToImmutable(out var array, trim);
            return array;
        }

        private void EnsureOverflowCapacity(int required)
        {
            if (_overflow is null)
            {
                _overflow = new TElement[Math.Max(required, 4)];
                return;
            }

            if (_overflow.Length < required)
            {
                Array.Resize(ref _overflow, Math.Max(required, _overflow.Length * 2));
            }
        }
    }

    public struct Enumerator : IEnumerator<TElement>
    {
        private readonly ImmutableInlineArray<TArray, TElement> _array;

        private int _index;
        private TElement _current;

        internal Enumerator(ImmutableInlineArray<TArray, TElement> list)
        {
            _array = list;
            _index = -1;
            _current = default!;
        }

        public readonly TElement Current => _current;

        readonly object? IEnumerator.Current
        {
            get
            {
                if (_index <= 0)
                {
                    ThrowIndexArgumentOutOfRange();
                }

                return _current;
            }
        }

        public bool MoveNext()
        {
            _index++;

            if ((uint)_index < (uint)_array.Count)
            {
                _current = _index < InlineCapacity
                    ? _array._inline.GetElement(_index)
                    : _array._overflow![_index - InlineCapacity];

                return true;
            }

            _current = default!;
            _index = -1;
            return false;
        }

        void IEnumerator.Reset()
        {
            _index = -1;
            _current = default!;
        }

        readonly void IDisposable.Dispose()
        {
        }

        public void Reset() => throw new NotImplementedException();
    }
}