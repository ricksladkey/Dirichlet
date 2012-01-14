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

        private const int initialCapacity = 64;
        private const int initialBuckets = 64;
        private const int lastSentinel = -1;

        private Entry[] entries;
        private int[] buckets;
        private int freeList;
        private int count;
        private int used;
        private int nMask;

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
            for (int bucket = 0; bucket < buckets.Length; bucket++)
                buckets[bucket] = lastSentinel;
            entries = new Entry[initialCapacity];
            count = 0;
            used = 0;
            freeList = -1;
            nMask = buckets.Length - 1;
        }

        public bool Contains(int value)
        {
            return ContainsEntry(value);
        }

        public void Add(int value)
        {
            if (ContainsEntry(value))
                return;
            var bucket = value & nMask;
            var entry = freeList;
            if (entry == lastSentinel)
            {
                if (used + 1 == entries.Length)
                {
                    Resize();
                    bucket = value & nMask;
                }
                entry = used++;
            }
            else
                freeList = GetFreeListEntry(entries[freeList].Next);
            entries[entry].Value = value;
            entries[entry].Next = buckets[bucket];
            buckets[bucket] = entry;
            ++count;
        }

        public void Remove(int value)
        {
            if (!ContainsEntry(value))
                return;
            var bucket = value & nMask;
            var prev = lastSentinel;
            var entry = buckets[bucket];
            while (entries[entry].Value != value)
            {
                prev = entry;
                entry = entries[entry].Next;
            }
            if (prev == lastSentinel)
                buckets[bucket] = entries[entry].Next;
            else
                entries[prev].Next = entries[entry].Next;
            entries[entry].Next = GetFreeListEntry(freeList);
            freeList = entry;
            --count;
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

        private bool ContainsEntry(int value)
        {
            var bucket = value & nMask;
            for (var index = buckets[bucket]; index != lastSentinel; index = entries[index].Next)
            {
                if (entries[index].Value == value)
                    return true;
            }
            return false;
        }

        private int GetFreeListEntry(int entry)
        {
            return -3 - entry;
        }

        private void Resize()
        {
            int n = buckets.Length;
            Array.Resize(ref entries, entries.Length * 2);
            Array.Resize(ref buckets, buckets.Length * 2);
            nMask = buckets.Length - 1;
            for (int bucket = 0; bucket < n; bucket++)
            {
                var list1 = lastSentinel;
                var list2 = lastSentinel;
                var next = lastSentinel;
                for (var entry = buckets[bucket]; entry != lastSentinel; entry = next)
                {
                    next = entries[entry].Next;
                    int value = entries[entry].Value;
                    if ((value & nMask) == bucket)
                    {
                        entries[entry].Next = list1;
                        list1 = entry;
                    }
                    else
                    {
                        Debug.Assert((value & nMask) == bucket + nMask);
                        entries[entry].Next = list2;
                        list2 = entry;
                    }
                }
                buckets[bucket] = list1;
                buckets[bucket + n] = list2;
            }
        }
    }
}
