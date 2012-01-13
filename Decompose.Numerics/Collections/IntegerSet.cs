using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Decompose.Numerics
{
    public class IntegerSet : IEnumerable<int>
    {
        private struct Entry
        {
            public int Value { get; set; }
            public int Next { get; set; }
            public override string ToString() { return string.Format("Value = {0}, Next = {1}", Value, Next); }
        }

        private const int initialBuckets = 1000;
        private const int initialCapacity = 100;
        private const int lastSentinel = -1;
        private const int freeSentinel = -2;
        private Entry[] entries;
        private int[] buckets;
        int count;
        int used;

        public IntegerSet()
        {
            Clear();
        }

        public int Count
        {
            get { return count; }
        }

        public void Clear()
        {
            buckets = new int[initialBuckets];
            entries = new Entry[initialCapacity];
            count = 0;
            used = 0;
        }

        public bool Contains(int value)
        {
            return ContainsEntry(GetBucket(value), value);
        }

        public void Add(int value)
        {
            var bucket = GetBucket(value);
            if (!ContainsEntry(bucket, value))
                AddEntry(bucket, value);
        }

        public void Remove(int value)
        {
            var bucket = GetBucket(value);
            if (!ContainsEntry(bucket, value))
                return;
            RemoveEntry(bucket, value);
        }

        public IEnumerator<int> GetEnumerator()
        {
            for (int entry = 0; entry < used; entry++)
            {
                if (entries[entry].Next != freeSentinel)
                    yield return entries[entry].Value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private int GetBucket(int value)
        {
            return value % buckets.Length;
        }

        private int GetBucketEntry(int bucket)
        {
            return buckets[bucket] - 1;
        }

        private void SetBucketEntry(int bucket, int entry)
        {
            buckets[bucket] = entry + 1;
        }

        private bool ContainsEntry(int bucket, int value)
        {
            for (var entry = GetBucketEntry(bucket); entry != lastSentinel; entry = entries[entry].Next)
            {
                if (entries[entry].Value == value)
                    return true;
            }
            return false;
        }

        private int AddEntry(int bucket, int value)
        {
            int entry = used++;
            if (used == entries.Length)
                Array.Resize(ref entries, entries.Length * 2);
            entries[entry].Value = value;
            entries[entry].Next = GetBucketEntry(bucket);
            SetBucketEntry(bucket, entry);
            ++count;
            return entry;
        }

        private void RemoveEntry(int bucket, int value)
        {
            var prev = lastSentinel;
            var entry = GetBucketEntry(bucket);
            while (entries[entry].Value != value)
            {
                prev = entry;
                entry = entries[entry].Next;
            }
            if (prev == lastSentinel)
                SetBucketEntry(bucket, entries[entry].Next);
            else
                entries[prev].Next = entries[entry].Next;
            entries[entry].Next = freeSentinel;
            --count;
        }
    }
}
