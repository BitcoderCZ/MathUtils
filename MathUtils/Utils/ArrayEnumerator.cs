using System.Collections;
using System.Collections.Generic;

namespace MathUtils.Utils;

public struct ArrayEnumerator<T> : IEnumerator<T>, IEnumerator
{
	private readonly T[] _array;
	private int _index;

	public ArrayEnumerator(params T[] array)
	{
		_array = array;
		_index = -1;
	}

	public readonly T Current => _array[_index];

	readonly object? IEnumerator.Current => Current;

	public bool MoveNext()
	{
		_index++;
		return _index < _array.Length;
	}

	public void Reset()
		=> _index = -1;

	public readonly void Dispose()
	{
	}
}
