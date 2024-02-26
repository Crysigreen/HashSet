using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Diagnostics;
using System.Collections.Immutable;



namespace HashSet
{


    public static class HashHelpers
    {

        public const int HashPrime = 101;

        //Table of prime numbers to use as hash table sizes.
        //A typical resize algorithm would pick the smallest prime number in this array
        // that is larger than twice the previous capacity.
        // Suppose our Hashtable currently has capacity x and enough elements are added
        // such that a resize needs to occur. Resizing first computes 2x then finds the
        // first prime in the table greater than 2x, i.e. if primes are ordered
        // p_1, p_2, ..., p_i, ..., it finds p_n such that p_n-1 < 2x<p_n.
        // Doubling is important for preserving the asymptotic complexity of the
        // hashtable operations such as add.Having a prime guarantees that double
        // hashing does not lead to infinite loops.IE, your hash function will be
        // h1(key) + i* h2(key), 0 <= i<size.h2 and the size must be relatively prime.
        // We prefer the low computation costs of higher prime numbers over the increased
        // memory allocation of a fixed prime number i.e.when right sizing a HashSet.
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

            // Outside of our predefined table. Compute the hard way.
            for (var i = (min | 1); i < int.MaxValue; i += 2)
            {
                if (IsPrime(i) && ((i - 1) % HashPrime != 0))
                    return i;
            }
            return min;
        }
    }

    internal class RHashSet<T>
    {
        private int[]? _buckets;
        private Entry[]? _entries;
        private int _count;
        private int _freeList;
        private int _freeCount;
        private int _version;
        private readonly IEqualityComparer<T>? _comparer;

        private const int StartOfFreeList = -3;

        public RHashSet( IEqualityComparer<T>? comparer = null)
        {
            Initialize(0);
            _comparer = comparer ?? EqualityComparer<T>.Default;
            
        }
        /// <summary>
        /// Структура, представляющая элемент хэш-таблицы
        /// </summary>
        private struct Entry
        {
            /// <summary>
            /// HashCode элемента
            /// </summary>
            public int HashCode;    // Хэш-код элемента
            public T Value;         // Значение элемента
            public int Next;        // Ссылка на следующий элемент в случае коллизии
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
                int hashCode =
                    typeof(T).IsValueType && comparer == null ? item!.GetHashCode() :
                    item is not null ? comparer!.GetHashCode(item) :
                    0;

                ref int bucket = ref GetBucketIndex(hashCode);
                int i = bucket - 1; // Value in buckets is 1-based

                while (i >= 0)
                {
                    ref Entry entry = ref entries[i];

                    if (entry.HashCode == hashCode && comparer.Equals(entry.Value, item))
                    {
                        if (last < 0)
                        {
                            bucket = entry.Next + 1; // Value in buckets is 1-based
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
                    if (collisionCount > (uint)entries.Length)
                    {
                        // The chain of entries forms a loop; which means a concurrent update has happened.
                        // Break out of the loop and throw, rather than looping forever.
                        throw new InvalidOperationException("Concurrent operations are not supported");
                    }
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

            hashCode = value != null ? comparer.GetHashCode(value) : 0;
            bucket = ref GetBucketIndex(hashCode);
            int i = bucket - 1; // Value in _buckets is 1-based

            while (i >= 0)
            {
                ref Entry entry = ref entries[i];
                if (entry.HashCode == hashCode && comparer.Equals(entry.Value, value))
                {
                    return;
                }
                i = entry.Next;

                collisionCount++;
                if (collisionCount > entries.Length)
                {
                    // The chain of entries forms a loop, which means a concurrent update has happened.
                    throw new InvalidOperationException("Concurrent operations are not supported");
                }
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
                entry.Next = bucket - 1; // Value in _buckets is 1-based
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

                int hashCode = item != null ? comparer.GetHashCode(item) : 0;
                int i = GetBucketIndex(hashCode) - 1; // Value in _buckets is 1-based
                while (i >= 0)
                {
                    ref Entry entry = ref entries[i];
                    if (entry.HashCode == hashCode && comparer.Equals(entry.Value, item))
                    {
                        return true;
                    }
                    i = entry.Next;

                    collisionCount++;
                    if (collisionCount > (uint)entries.Length)
                    {
                        // The chain of entries forms a loop, which means a concurrent update has happened.
                        throw new InvalidOperationException("Concurrent operations are not supported");
                    }
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

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var entry in _entries)
            {
                yield return entry.Value;
            }
        }


    }
}


