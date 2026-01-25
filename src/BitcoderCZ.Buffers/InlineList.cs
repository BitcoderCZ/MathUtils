using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static BitcoderCZ.Utils.ThrowHelper;

namespace BitcoderCZ.Buffers;

[StructLayout(LayoutKind.Auto)]
public struct InlineList<TArray, TElement> : IList<TElement>, IReadOnlyList<TElement>
    where TArray : struct, IFixedArray<TElement>
    where TElement : unmanaged, IEquatable<TElement>
{
    private const int ListStartCapacity = 4;

    private static readonly int BufferCapacity = FixedArray.GetLength<TArray, TElement>();

    private int _count;
    private TArray _buffer = default;
    internal List<TElement>? _list;

    public InlineList(int capacity)
    {
        _count = 0;
        _list = capacity > BufferCapacity ? new List<TElement>(capacity - BufferCapacity) : null;
    }

    public InlineList(IEnumerable<TElement> collection)
    {
        _count = 0;

        if (collection is ICollection<TElement> c)
        {
            int count = c.Count;
            _list = count > BufferCapacity ? new List<TElement>(count - BufferCapacity) : null;
        }
        else if (collection is IReadOnlyCollection<TElement> roc)
        {
            int count = roc.Count;
            _list = count > BufferCapacity ? new List<TElement>(count - BufferCapacity) : null;
        }
        else
        {
            _list = null;
        }

        foreach (var item in collection)
        {
            Add(item);
        }
    }

    public readonly int Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _count;
    }

    public int Capacity
    {
        readonly get => _list is null ? BufferCapacity : BufferCapacity + _list.Capacity;
        set
        {
            ThrowIfNegative(value, nameof(value));

            if (value <= BufferCapacity)
            {
                _list = null;
            }
            else if (_list is null)
            {
                _list = new List<TElement>(value - BufferCapacity);
            }
            else
            {
                _list.Capacity = value - BufferCapacity;
            }
        }
    }

    public readonly bool IsReadOnly => false;

    [UnscopedRef]
#pragma warning disable IDE0251 // Make member 'readonly'
    private Span<TElement> BufferSpan => _buffer.AsSpan();
#pragma warning restore IDE0251 // Make member 'readonly'

    // IFixedArray.AsROSpan cannot be marked readonly, so AsROSpan will make a defensive copy, so need to use this workaround
    [UnscopedRef]
    private readonly ReadOnlySpan<TElement> ROBufferSpan => MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<TArray, TElement>(ref Unsafe.AsRef(in _buffer)), BufferCapacity);

    public TElement this[int index]
    {
        readonly get
        {
            ThrowIfGreaterThanOrEqualToOrNegative(index, Count, nameof(index));

            return index < BufferCapacity
                ? ROBufferSpan[index]
                : _list![index - BufferCapacity];
        }

        set
        {
            ThrowIfGreaterThanOrEqualToOrNegative(index, Count, nameof(index));

            if (index < BufferCapacity)
            {
                BufferSpan[index] = value;
            }
            else
            {
                _list![index - BufferCapacity] = value;
            }
        }
    }

    [UnscopedRef]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref TElement GetRef(int index)
        => ref (index < BufferCapacity
            ? ref BufferSpan[index]
            : ref CollectionsMarshal.AsSpan(_list!)[index - BufferCapacity]);

    public void Add(TElement item)
    {
        if (_count < BufferCapacity)
        {
            BufferSpan[_count++] = item;
            return;
        }

        if (_list is null)
        {
            _list = new List<TElement>(ListStartCapacity);
        }

        _list.Add(item);
        _count++;
    }

    public void Swap(int i, int j)
    {
        if (i == j)
        {
            return;
        }

        ref TElement a = ref GetRef(i);
        ref TElement b = ref GetRef(j);

        TElement temp = a;
        a = b;
        b = temp;
    }

    public void Clear()
    {
        BufferSpan.Clear();
        _list?.Clear();
        _count = 0;
    }

    public readonly bool Contains(TElement item)
        => IndexOf(item) >= 0;

    public readonly void CopyTo(TElement[] array, int arrayIndex)
        => CopyTo(array.AsSpan(arrayIndex));

    public readonly void CopyTo(Span<TElement> span)
    {
        if (span.Length < _count)
        {
            ThrowArgumentOutOfRangeException(nameof(span), $"{nameof(span)} is not large enough.");
        }

        var bufferSpan = ROBufferSpan;
        for (int i = 0; i < Math.Min(_count, BufferCapacity); i++)
        {
            span[i] = bufferSpan[i];
        }

        if (_list is not null)
        {
            CollectionsMarshal.AsSpan(_list)[..(_count - BufferCapacity)].CopyTo(span[BufferCapacity..]);
        }
    }

    public readonly int IndexOf(TElement item)
    {
        if (_count == 0)
        {
            return -1;
        }

        var bufferSpan = ROBufferSpan;
        for (int i = 0; i < Math.Min(_count, BufferCapacity); i++)
        {
#pragma warning disable HAM0001 // Operation causes the compiler to create a defensive copy
            if (bufferSpan[i].Equals(item))
#pragma warning restore HAM0001 // Operation causes the compiler to create a defensive copy
            {
                return i;
            }
        }

        if (_list is not null && _count > BufferCapacity)
        {
            int index = CollectionsMarshal.AsSpan(_list)[..(_count - BufferCapacity)].IndexOf(item);
            return index == -1 ? -1 : index + BufferCapacity;
        }

        return -1;
    }

    public void Insert(int index, TElement item)
    {
        ThrowIfGreaterThanOrEqualToOrNegative(index, Count + 1, nameof(index));

        if (index == _count)
        {
            Add(item);
            return;
        }

        var bufferSpan = BufferSpan;
        if (_count < BufferCapacity)
        {
            for (int i = _count; i > index; i--)
            {
                bufferSpan[i] = bufferSpan[i - 1];
            }

            bufferSpan[index] = item;
            _count++;
            return;
        }

        if (_list is null)
        {
            _list = new List<TElement>(ListStartCapacity);
        }

        if (index < BufferCapacity)
        {
            TElement lastBuffer = bufferSpan[BufferCapacity - 1];
            for (int i = BufferCapacity - 1; i > index; i--)
            {
                bufferSpan[i] = bufferSpan[i - 1];
            }

            bufferSpan[index] = item;

            _list.Insert(0, lastBuffer);
        }
        else
        {
            _list.Insert(index - BufferCapacity, item);
        }

        _count++;
    }

    public bool Remove(TElement item)
    {
        int index = IndexOf(item);
        if (index >= 0)
        {
            RemoveAt(index);
            return true;
        }

        return false;
    }

    public void RemoveAt(int index)
    {
        ThrowIfGreaterThanOrEqualToOrNegative(index, _count, nameof(index));

        var bufferSpan = BufferSpan;
        if (index < BufferCapacity)
        {
            for (int i = index; i < Math.Min(_count, BufferCapacity) - 1; i++)
            {
                bufferSpan[i] = bufferSpan[i + 1];
            }

            if (_count > BufferCapacity)
            {
                Debug.Assert(_list is not null, $"{nameof(_list)} should not be null.");

                bufferSpan[BufferCapacity - 1] = _list[0];
                _list.RemoveAt(0);
            }
        }
        else
        {
            Debug.Assert(_list is not null, $"{nameof(_list)} should not be null.");

            _list.RemoveAt(index - BufferCapacity);
        }

        _count--;
    }

    /// <summary>
    /// Determines whether two <see cref="ValueList{T}"/>s are equal by comparing the elements by using the default equality comparer for their type.
    /// </summary>
    /// <param name="other">The other <see cref="ValueList{T}"/>.</param>
    /// <returns>true if the two source sequences are of equal length and their corresponding elements are equal according to the default equality comparer for their type; otherwise, false.</returns>
    public readonly bool SequenceEqual(ref readonly InlineList<TArray, TElement> other)
    {
        if (other.Count != Count)
        {
            return false;
        }
        else if (Count is 0) // already tested that Count is equal
        {
            return true;
        }

        return ROBufferSpan[..Math.Min(Count, BufferCapacity)].SequenceEqual(other.ROBufferSpan[..Math.Min(Count, BufferCapacity)]) &&
            (_list is null || other._list is null || _list.SequenceEqual(other._list));
    }

    public readonly int CalculateHashCode()
    {
        HashCode hashCode = default;
        foreach (var item in this)
        {
            hashCode.Add(item);
        }

        return hashCode.ToHashCode();
    }

    public void Sort(IComparer<TElement>? comparer = null)
    {
        if (_count <= 1)
        {
            return;
        }

        comparer ??= Comparer<TElement>.Default;

#if NET5_0_OR_GREATER
        if (_count <= BufferCapacity)
        {
            BufferSpan[.._count].Sort(comparer);
            return;
        }

        if (_count <= 64)
        {
            Span<TElement> temp = stackalloc TElement[_count];

            BufferSpan.CopyTo(temp);

            CollectionsMarshal.AsSpan(_list!).CopyTo(temp[BufferCapacity..]);

            temp.Sort(comparer);

            temp[..BufferCapacity].CopyTo(BufferSpan);
            temp[BufferCapacity.._count].CopyTo(CollectionsMarshal.AsSpan(_list));

            return;
        }
#endif

        IntroSort(0, _count - 1, 2 * FloorLog2(_count), comparer);

        static int FloorLog2(int n)
        {
            int result = 0;
            while (n > 1)
            {
                n >>= 1;
                result++;
            }

            return result;
        }
    }

    private void IntroSort(int lo, int hi, int depthLimit, IComparer<TElement> comparer)
    {
        while (hi > lo)
        {
            int size = hi - lo + 1;

            if (size <= 16)
            {
                InsertionSort(lo, hi, comparer);
                return;
            }

            if (depthLimit == 0)
            {
                HeapSort(lo, hi, comparer);
                return;
            }

            depthLimit--;

            int p = Partition(lo, hi, comparer);

            IntroSort(p + 1, hi, depthLimit, comparer);
            hi = p - 1;
        }
    }

    private int Partition(int lo, int hi, IComparer<TElement> comparer)
    {
        Debug.Assert(hi - lo >= 2);

        int mid = lo + ((hi - lo) >> 1);

        if (comparer.Compare(GetRef(mid), GetRef(lo)) < 0)
        {
            Swap(mid, lo);
        }

        if (comparer.Compare(GetRef(hi), GetRef(lo)) < 0)
        {
            Swap(hi, lo);
        }

        if (comparer.Compare(GetRef(hi), GetRef(mid)) < 0)
        {
            Swap(hi, mid);
        }

        ref TElement pivot = ref GetRef(mid);
        Swap(mid, hi - 1);

        int left = lo;
        int right = hi - 1;

        while (true)
        {
            while (comparer.Compare(GetRef(++left), pivot) < 0)
            {
            }

            while (comparer.Compare(pivot, GetRef(--right)) < 0)
            {
            }

            if (left >= right)
            {
                break;
            }

            Swap(left, right);
        }

        Swap(left, hi - 1);
        return left;
    }

    private void InsertionSort(int lo, int hi, IComparer<TElement> comparer)
    {
        for (int i = lo + 1; i <= hi; i++)
        {
            TElement value = GetRef(i);
            int j = i - 1;

            while (j >= lo && comparer.Compare(value, GetRef(j)) < 0)
            {
                GetRef(j + 1) = GetRef(j);
                j--;
            }

            GetRef(j + 1) = value;
        }
    }

    private void HeapSort(int lo, int hi, IComparer<TElement> comparer)
    {
        int n = hi - lo + 1;

        for (int i = n / 2; i >= 1; i--)
        {
            DownHeap(i, n, lo, comparer);
        }

        for (int i = n; i > 1; i--)
        {
            Swap(lo, lo + i - 1);
            DownHeap(1, i - 1, lo, comparer);
        }
    }

    private void DownHeap(int i, int n, int lo, IComparer<TElement> comparer)
    {
        ref TElement d = ref GetRef(lo + i - 1);

        while (i <= n / 2)
        {
            int child = 2 * i;

            if (child < n && comparer.Compare(GetRef(lo + child - 1), GetRef(lo + child)) < 0)
            {
                child++;
            }

            if (comparer.Compare(d, GetRef(lo + child - 1)) >= 0)
            {
                break;
            }

            GetRef(lo + i - 1) = GetRef(lo + child - 1);
            i = child;
        }

        GetRef(lo + i - 1) = d;
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

    public struct Enumerator : IEnumerator<TElement>
    {
        private readonly InlineList<TArray, TElement> _list;

        private int _index;
        private TElement _current;

        internal Enumerator(InlineList<TArray, TElement> list)
        {
            _list = list;
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

            if ((uint)_index < (uint)_list.Count)
            {
                _current = _index < BufferCapacity
                    ? _list.ROBufferSpan[_index]
                    : _list._list![_index - BufferCapacity];

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
    }
}

internal sealed class InlineListDebugView<TArray, TElement>
    where TArray : struct, IFixedArray<TElement>
    where TElement : unmanaged, IEquatable<TElement>
{
    private readonly InlineList<TArray, TElement> _list;

    public InlineListDebugView(InlineList<TArray, TElement> list)
    {
        _list = list;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public TElement[] Items => [.. _list];
}