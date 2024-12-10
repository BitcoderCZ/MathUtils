using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace MathUtils.Generators.Utils;

// from: https://github.com/Ilia-Kosenkov/Backports/blob/master/Backports/System/Text/ValueStringBuilder.cs
internal ref struct ValueStringBuilder : IDisposable
{
	private char[]? _arrayToReturnToPool;
	private Span<char> _chars;
	private int _pos;

	public ValueStringBuilder(Span<char> initialBuffer)
	{
		_arrayToReturnToPool = null;
		_chars = initialBuffer;
		_pos = 0;
	}

	public ValueStringBuilder(int initialCapacity)
	{
		_arrayToReturnToPool = ArrayPool<char>.Shared.Rent(initialCapacity);
		_chars = _arrayToReturnToPool;
		_pos = 0;
	}

	public int Length
	{
		readonly get => _pos;
		internal set
		{
			Debug.Assert(value >= 0);
			Debug.Assert(value <= _chars.Length);
			_pos = value;
		}
	}

	public readonly int Capacity => _chars.Length;

	public void EnsureCapacity(int capacity)
	{
		if (capacity > _chars.Length)
			Grow(capacity - _pos);
	}

	/// <summary>
	/// Get a pinnable reference to the builder.
	/// Does not ensure there is a null char after <see cref="Length"/>
	/// This overload is pattern matched in the C# 7.3+ compiler so you can omit
	/// the explicit method call, and write eg "fixed (char* c = builder)"
	/// </summary>
	public readonly ref char GetPinnableReference()
		=> ref MemoryMarshal.GetReference(_chars);

	/// <summary>
	/// Get a pinnable reference to the builder.
	/// </summary>
	/// <param name="terminate">Ensures that the builder has a null char after <see cref="Length"/></param>
	public ref char GetPinnableReference(bool terminate)
	{
		if (!terminate)
		{
			return ref MemoryMarshal.GetReference(_chars);
		}

		EnsureCapacity(Length + 1);
		_chars[Length] = '\0';
		return ref MemoryMarshal.GetReference(_chars);
	}

	public ref char this[int index]
	{
		get
		{
			Debug.Assert(index < _pos);
			return ref _chars[index];
		}
	}

	public override string ToString()
	{
		string s = _chars.Slice(0, _pos).ToString();
		Dispose();
		return s;
	}

	/// <summary>Returns the underlying storage of the builder.</summary>
	public readonly Span<char> RawChars => _chars;

	/// <summary>
	/// Returns a span around the contents of the builder.
	/// </summary>
	/// <param name="terminate">Ensures that the builder has a null char after <see cref="Length"/></param>
	public ReadOnlySpan<char> AsSpan(bool terminate)
	{
		if (!terminate)
		{
			return _chars.Slice(0, _pos);
		}

		EnsureCapacity(Length + 1);
		_chars[Length] = '\0';
		return _chars.Slice(0, _pos);
	}

	public readonly ReadOnlySpan<char> AsSpan()
		=> _chars.Slice(0, _pos);

	public readonly ReadOnlySpan<char> AsSpan(int start)
		=> _chars.Slice(start, _pos - start);

	public readonly ReadOnlySpan<char> AsSpan(int start, int length)
		=> _chars.Slice(start, length);

	public bool TryCopyTo(Span<char> destination, out int charsWritten)
	{
		if (_chars.Slice(0, _pos).TryCopyTo(destination))
		{
			charsWritten = _pos;
			Dispose();
			return true;
		}

		charsWritten = 0;
		Dispose();
		return false;
	}

	public void Insert(int index, char value, int count)
	{
		if (_pos > _chars.Length - count)
		{
			Grow(count);
		}

		int remaining = _pos - index;
		_chars.Slice(index, remaining).CopyTo(_chars.Slice(index + count));
		_chars.Slice(index, count).Fill(value);
		_pos += count;
	}

	public void Insert(int index, string? value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return;
		}

		int count = value!.Length;

		if (_pos > _chars.Length - count)
			Grow(count);

		int remaining = _pos - index;
		_chars.Slice(index, remaining).CopyTo(_chars.Slice(index + count));
		value.AsSpan().CopyTo(_chars.Slice(index));
		_pos += count;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Append(char value)
	{
		int pos = _pos;

		if ((uint)pos < (uint)_chars.Length)
		{
			_chars[pos] = value;
			_pos = pos + 1;
		}
		else
		{
			GrowAndAppend(value);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Append(string? value)
	{
		if (string.IsNullOrEmpty(value))
			return;

		int pos = _pos;
		if (value!.Length == 1 && (uint)pos < (uint)_chars.Length) // very common case, e.g. appending strings from NumberFormatInfo like separators, percent symbols, etc.
		{
			_chars[pos] = value[0];
			_pos = pos + 1;
		}
		else
		{
			AppendSlow(value);
		}
	}

	private void AppendSlow(string s)
	{
		int pos = _pos;
		if (pos > _chars.Length - s.Length)
		{
			Grow(s.Length);
		}

		s.AsSpan().CopyTo(_chars.Slice(pos));
		_pos += s.Length;
	}

	public void Append(char c, int count)
	{
		if (_pos > _chars.Length - count)
		{
			Grow(count);
		}

		Span<char> dst = _chars.Slice(_pos, count);
		for (int i = 0; i < dst.Length; i++)
		{
			dst[i] = c;
		}

		_pos += count;
	}

	public void Append(in char value, int length)
	{
		int pos = _pos;
		if (pos > _chars.Length - length)
		{
			Grow(length);
		}

		Span<char> dst = _chars.Slice(_pos, length);
		for (int i = 0; i < dst.Length; i++)
		{
			dst[i] = value;

			value = ref Unsafe.Add(ref Unsafe.AsRef(in value), 1);
		}

		_pos += length;
	}

	public void Append(ReadOnlySpan<char> value)
	{
		int pos = _pos;
		if (pos > _chars.Length - value.Length)
		{
			Grow(value.Length);
		}

		value.CopyTo(_chars.Slice(_pos));
		_pos += value.Length;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Span<char> AppendSpan(int length)
	{
		int origPos = _pos;
		if (origPos > _chars.Length - length)
		{
			Grow(length);
		}

		_pos = origPos + length;
		return _chars.Slice(origPos, length);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void GrowAndAppend(char c)
	{
		Grow(1);
		Append(c);
	}

	/// <summary>
	/// Resize the internal buffer either by doubling current buffer size or
	/// by adding <paramref name="additionalCapacityBeyondPos"/> to
	/// <see cref="_pos"/> whichever is greater.
	/// </summary>
	/// <param name="additionalCapacityBeyondPos">
	/// Number of chars requested beyond current position.
	/// </param>
	[MethodImpl(MethodImplOptions.NoInlining)]
	private void Grow(int additionalCapacityBeyondPos)
	{
		Debug.Assert(additionalCapacityBeyondPos > 0);
		Debug.Assert(_pos > _chars.Length - additionalCapacityBeyondPos, "Grow called incorrectly, no resize is needed.");

		char[] poolArray = ArrayPool<char>.Shared.Rent(Math.Max(_pos + additionalCapacityBeyondPos, _chars.Length * 2));

		_chars.Slice(0, _pos).CopyTo(poolArray);

		char[]? toReturn = _arrayToReturnToPool;
		_chars = _arrayToReturnToPool = poolArray;
		if (toReturn is not null)
			ArrayPool<char>.Shared.Return(toReturn);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Dispose()
	{
		char[]? toReturn = _arrayToReturnToPool;
		this = default; // for safety, to avoid using pooled array if this instance is erroneously appended to again
		if (toReturn is not null)
		{
			ArrayPool<char>.Shared.Return(toReturn);
		}
	}
}