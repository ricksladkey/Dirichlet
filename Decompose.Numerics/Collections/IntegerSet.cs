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

        private const int capacityFactor = 1;
        private const int initialBuckets = 64;
        private const int initialCapacity = initialBuckets * capacityFactor;
        private const int lastSentinel = -1;
        private Entry[] entries;
        private int[] buckets;
        private int freeList;
        private int count;
        private int used;
        private int bucketsLength;

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
            freeList = -1;
            bucketsLength = buckets.Length;
        }

        public bool Contains(int value)
        {
            return ContainsEntry(value);
        }

        public void Add(int value)
        {
            if (!ContainsEntry(value))
                AddEntry(value);
        }

        public void Remove(int value)
        {
            if (ContainsEntry(value))
                RemoveEntry(value);
        }

        public IEnumerator<int> GetEnumerator()
        {
            for (int entry = 0; entry < used; entry++)
            {
                if (entries[entry].Next >= lastSentinel)
                    yield return entries[entry].Value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private int GetBucketEntry(int bucket)
        {
            return buckets[bucket] - 1;
        }

        private void SetBucketEntry(int bucket, int entry)
        {
            buckets[bucket] = entry + 1;
        }

        private bool ContainsEntry(int value)
        {
            var bucket = value % bucketsLength;
            for (var entry = GetBucketEntry(bucket); entry != lastSentinel; entry = entries[entry].Next)
            {
                if (entries[entry].Value == value)
                    return true;
            }
            return false;
        }

        private int AddEntry(int value)
        {
            var bucket = value % bucketsLength;
            var entry = freeList;
            if (entry == lastSentinel)
            {
                if (used + 1 == entries.Length)
                {
                    Resize();
                    bucket = value % bucketsLength;
                }
                entry = used++;
            }
            else
                freeList = GetFreeListEntry(entries[freeList].Next);
            entries[entry].Value = value;
            entries[entry].Next = GetBucketEntry(bucket);
            SetBucketEntry(bucket, entry);
            ++count;
            return entry;
        }

        private void RemoveEntry(int value)
        {
            var bucket = value % bucketsLength;
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
            entries[entry].Next = GetFreeListEntry(freeList);
            freeList = entry;
            --count;
        }

        private int GetFreeListEntry(int entry)
        {
            int result = -2 - entry;
            return result;
        }

        private void Resize()
        {
            Array.Resize(ref entries, entries.Length * 2);
            Array.Resize(ref buckets, bucketsLength * 2);
            for (int i = 0; i < bucketsLength; i++)
            {
                var list1 = lastSentinel;
                var list2 = lastSentinel;
                var bucket = GetBucketEntry(i);
                for (var entry = GetBucketEntry(i); entry != lastSentinel; entry = entries[entry].Next)
                {
                    int value = entries[entry].Value;
                    if (value % buckets.Length == value % bucketsLength)
                    {
                        entries[entry].Next = list1;
                        list1 = entry;
                    }
                    else
                    {
                        entries[entry].Next = list2;
                        list2 = entry;
                    }
                }
                SetBucketEntry(i, list1);
                SetBucketEntry(2 * i, list2);
            }
            bucketsLength = buckets.Length;
        }
    }
}
