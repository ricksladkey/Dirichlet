using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace Decompose.Numerics
{
    public class IntegerSet : IEnumerable<int>
    {
        private class Entry
        {
            public int Integer { get; set; }
            public Entry Next { get; set; }
        }

        private class Bucket
        {
            public Entry Entry { get; set; }
            public Bucket Next { get; set; }
            public Bucket Prev { get; set; }
        }

        private const int n = 100;
        private Bucket[] buckets;
        private Bucket root;
        int count;

        public IntegerSet()
        {
            buckets = new Bucket[n];
            count = 0;
            root = null;
        }

        public int Count
        {
            get { return count; }
        }

        public void Add(int i)
        {
            int j = i % n;
            var bucket = buckets[j];
            if (bucket == null)
            {
                CheckBuckets();
                bucket = buckets[j] = new Bucket();
                if (root == null)
                {
                    bucket.Prev = bucket.Next = bucket;
                    root = bucket;
                }
                else
                {
                    bucket.Prev = root.Prev;
                    bucket.Next = root;
                    root.Prev = bucket;
                    bucket.Prev.Next = bucket;
                }
                CheckBuckets();
            }
            var entry = bucket.Entry;
            while (entry != null && entry.Integer != i)
                entry = entry.Next;
            if (entry == null)
            {
                bucket.Entry = new Entry { Integer = i, Next = bucket.Entry };
                ++count;
            }
        }

        public void Remove(int i)
        {
            int j = i % n;
            var bucket = buckets[j];
            if (bucket == null)
                return;
            var entry = bucket.Entry;
            var prev = null as Entry;
            while (entry != null)
            {
                if (entry.Integer == i)
                    break;
                prev = entry;
                entry = entry.Next;
            }
            if (entry == null)
                return;
            if (prev == null)
                bucket.Entry = entry.Next;
            else
                prev.Next = entry.Next;
            if (bucket.Entry == null)
            {
                CheckBuckets();
                buckets[j] = null;
                if (bucket.Next == bucket)
                    root = null;
                else
                {
                    bucket.Prev.Next = bucket.Next;
                    bucket.Next.Prev = bucket.Prev;
                    if (root == bucket)
                        root = bucket.Next;
                }
                CheckBuckets();
            }
            --count;
        }

        public bool Contains(int i)
        {
            var bucket = buckets[i % n];
            if (bucket == null)
                return false;
            var entry = bucket.Entry;
            while (entry != null)
            {
                if (entry.Integer == i)
                    return true;
                entry = entry.Next;
            }
            return false;
        }

        public void Clear()
        {
            for (int i = 0; i < n; i++)
                buckets[i] = null;
            count = 0;
            root = null;
        }

        public IEnumerator<int> GetEnumerator()
        {
            if (root == null)
                yield break;
            var bucket = root;
            do
            {
                for (var entry = bucket.Entry; entry != null; entry = entry.Next)
                    yield return entry.Integer;
                bucket = bucket.Next;
            } while (bucket != root);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        [Conditional("DEBUG")]
        private void CheckBuckets()
        {
            if (root == null)
            {
                Debug.Assert(buckets.All(b => b == null));
                return;
            }
            var bucket = root;
            do
            {
                Debug.Assert(bucket != null);
                bucket = bucket.Next;
            } while (bucket != root);
            do
            {
                Debug.Assert(bucket != null);
                bucket = bucket.Prev;
            } while (bucket != root);
        }
    }
}
