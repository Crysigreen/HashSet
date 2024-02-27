using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Diagnostics;
using System.Collections.Immutable;
using Newtonsoft.Json.Linq;



namespace HashSet
{


    public static class HashHelpers
    {

        public const int HashPrime = 101;

        private static readonly ImmutableArray<int> s_primes = ImmutableArray.Create(
            3, 7, 11, 17, 23, 29, 37, 47, 59, 71, 89, 107, 131, 163, 197, 239, 293, 353, 431, 521, 631, 761, 919,
            1103, 1327, 1597, 1931, 2333, 2801, 3371, 4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591,
            17519, 21023, 25229, 30293, 36353, 43627, 52361, 62851, 75431, 90523, 108631, 130363, 156437,
            187751, 225307, 270371, 324449, 389357, 467237, 560689, 672827, 807403, 968897, 1162687, 1395263,
            1674319, 2009191, 2411033, 2893249, 3471899, 4166287, 4999559, 5999471, 7199369);
        public static bool IsPrime(int candidate)
        {
            if ((candidate & 1) != 0)
            {
                var limit = (int)Math.Sqrt(candidate);
                for (var divisor = 3; divisor <= limit; divisor += 2)
                {
                    if ((candidate % divisor) == 0)
                        return false;
                }
                return true;
            }
            return candidate == 2;
        }

        public static int GetPrime(int min)
        {
            if (min < 0)
                throw new ArgumentException();

            foreach (var prime in s_primes)
            {
                if (prime >= min)
                    return prime;
            }

            for (var i = (min | 1); i < int.MaxValue; i += 2)
            {
                if (IsPrime(i) && ((i - 1) % HashPrime != 0))
                    return i;
            }
            return min;
        }
    }

    internal class RHashSet<T> : IEnumerable<T>
    {
        private int[]? _buckets;
        private Entry[]? _entries;
        private int _count;
        private int _freeList;
        private int _freeCount;
        private int _version;
        private readonly IEqualityComparer<T>? _comparer;

        private const int StartOfFreeList = -3;

        public RHashSet(IEqualityComparer<T>? comparer = null)
        {
            Initialize(0);
            _comparer = comparer ?? EqualityComparer<T>.Default;

        }
        
        private struct Entry
        {
            public int HashCode;    
            public T Value;         
            public int Next;        
        }

        private ref int GetBucketIndex(int hashCode)
        {
            int[] buckets = _buckets!;
            return ref buckets[(uint)hashCode % buckets.Length];

        }

        private void Resize()
        {
            int newSize = HashHelpers.GetPrime(_entries.Length * 2);
            var entires = new Entry[newSize];
            int count = _count;
            Array.Copy(_entries, entires, count);
            _buckets = new int[newSize];

            for (int i = 0; i < count; i++)
            {
                ref Entry entry = ref entires[i];
                if (entry.Next >= -1)
                {
                    ref int bucket = ref GetBucketIndex(entry.HashCode);
                    entry.Next = bucket - 1;
                    bucket = i + 1;
                }
            }
            _entries = entires;
        }

        public bool Remove(T item)
        {
            if (_buckets != null)
            {
                Entry[]? entries = _entries;

                uint collisionCount = 0;
                int last = -1;

                IEqualityComparer<T>? comparer = _comparer;
                int hashCode = item != null ? item.GetHashCode() : 0;

                ref int bucket = ref GetBucketIndex(hashCode);
                int i = bucket - 1;

                while (i >= 0)
                {
                    ref Entry entry = ref entries[i];

                    if (entry.HashCode == hashCode && comparer.Equals(entry.Value, item))
                    {
                        if (last < 0)
                        {
                            bucket = entry.Next + 1; 
                        }
                        else
                        {
                            entries[last].Next = entry.Next;
                        }

                        entry.Next = StartOfFreeList - _freeList;

                        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
                        {
                            entry.Value = default!;
                        }

                        _freeList = i;
                        _freeCount++;
                        return true;
                    }

                    last = i;
                    i = entry.Next;

                    collisionCount++;
                    
                }
            }

            return false;
        }

        public void Add(T value)
        {
            if (_buckets == null)
            {
                Initialize(0);
            }

            Entry[]? entries = _entries;
            IEqualityComparer<T>? comparer = _comparer;

            int hashCode;

            uint collisionCount = 0;
            ref int bucket = ref Unsafe.NullRef<int>();

            hashCode = value != null ? value.GetHashCode() : 0;
            bucket = ref GetBucketIndex(hashCode);
            int i = bucket - 1;

            while (i >= 0)
            {
                ref Entry entry = ref entries[i];
                if (entry.HashCode == hashCode && Equals(entry.Value, value))
                {
                    return;
                }
                i = entry.Next;

                collisionCount++;
                
            }


            int index;
            if (_freeCount > 0)
            {
                index = _freeList;
                _freeCount--;
                _freeList = StartOfFreeList - entries[_freeList].Next;
            }
            else
            {
                int count = _count;
                if (count == entries.Length)
                {
                    Resize();
                    bucket = ref GetBucketIndex(hashCode);
                }
                index = count;
                _count = count + 1;
                entries = _entries;
            }

            {
                ref Entry entry = ref entries![index];
                entry.HashCode = hashCode;
                entry.Next = bucket - 1;
                entry.Value = value;
                bucket = index + 1;
                _version++;
            }
            return;

        }

        public bool Contains(T item)
        {
            int[]? buckets = _buckets;
            if (buckets != null)
            {
                Entry[]? entries = _entries;

                int collisionCount = 0;
                IEqualityComparer<T>? comparer = _comparer;

                int hashCode = item != null ? item.GetHashCode() : 0;
                int i = GetBucketIndex(hashCode) - 1;
                while (i >= 0)
                {
                    ref Entry entry = ref entries[i];
                    if (entry.HashCode == hashCode && comparer.Equals(entry.Value, item))
                    {
                        return true;
                    }
                    i = entry.Next;

                    collisionCount++;
                    
                }
            }
            return false;
        }

        public void Clear()
        {
            int count = _count;
            if (count > 0)
            {
                Array.Clear(_buckets);
                _count = 0;
                _freeList = -1;
                _freeCount = 0;
                Array.Clear(_entries, 0, count);
            }
        }

        public int Count => _count - _freeCount;

        private int Initialize(int capacity)
        {
            int size = HashHelpers.GetPrime(capacity);
            var buckets = new int[size];
            var entries = new Entry[size];

            _freeList = -1;
            _buckets = buckets;
            _entries = entries;

            return size;
        }

        public Enumerator GetEnumerator() => new Enumerator(this);

        IEnumerator<T> IEnumerable<T>.GetEnumerator() =>
            Count == 0 ? Enumerable.Empty<T>().GetEnumerator() :
            GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<T>)this).GetEnumerator();


        public struct Enumerator : IEnumerator<T>
        {
            private readonly RHashSet<T> _hashSet;
            private readonly int _version;
            private int _index;
            private T _current;

            internal Enumerator(RHashSet<T> hashSet)
            {
                _hashSet = hashSet;
                _version = hashSet._version;
                _index = 0;
                _current = default!;
            }

            public bool MoveNext()
            {
                if (_version != _hashSet._version)
                {
                    throw new InvalidOperationException("_InvalidOperation_EnumFailedVersion()");
                }

                while ((uint)_index < (uint)_hashSet._count)
                {
                    ref Entry entry = ref _hashSet._entries![_index++];
                    if (entry.Next >= -1)
                    {
                        _current = entry.Value;
                        return true;
                    }
                }

                _index = _hashSet._count + 1;
                _current = default!;
                return false;
            }

            public T Current => _current;

            public void Dispose() { }

            object? IEnumerator.Current
            {
                get
                {
                    if (_index == 0 || (_index == _hashSet._count + 1))
                    {
                        throw new InvalidOperationException("_InvalidOperation_EnumFailedVersion()");
                    }

                    return _current;
                }
            }

            void IEnumerator.Reset()
            {
                if (_version != _hashSet._version)
                {
                    throw new InvalidOperationException("_InvalidOperation_EnumFailedVersion()");
                }

                _index = 0;
                _current = default!;
            }
        }

    }
}


