using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathUtils.Utils
{
    public class ArrayEnumerator<T> : IEnumerator<T>
    {
        private readonly T[] array;
        private int index;

        public ArrayEnumerator(params T[] _array)
        {
            array = _array;
            index = -1;
        }

        public T Current => array[index];

        object? IEnumerator.Current => Current;

        public bool MoveNext()
        {
            index++;
            return index < array.Length;
        }

        public void Reset()
        {
            index = -1;
        }

        public void Dispose()
        {
        }
    }
}
