using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Decompose.Numerics
{
    public class PartialRelationEdge
    {
        public long Vertex1 { get; set; }
        public long Vertex2 { get; set; }
        public override string ToString()
        {
            return string.Format("Vertex1 = {0}, Vertex2 = {1}", Vertex1, Vertex2);
        }
    }

    public class PartialRelationGraph<TEdge> where TEdge : PartialRelationEdge, new()
    {
        private Dictionary<long, TEdge> prMap;
        private Dictionary<long, List<TEdge>> pprMap;
        private IEqualityComparer<long> comparer;

        public PartialRelationGraph()
        {
            prMap = new Dictionary<long, TEdge>();
            pprMap = new Dictionary<long, List<TEdge>>();
            comparer = EqualityComparer<long>.Default;
        }

        public void AddEdge(long vertex1, long vertex2)
        {
            AddEdge(new TEdge { Vertex1 = vertex1, Vertex2 = vertex2 });
        }

        public void AddEdge(TEdge edge)
        {
            if (edge.Vertex2 == 1)
                prMap.Add(edge.Vertex1, edge);
            else
            {
                AddToVertex(edge, edge.Vertex1);
                AddToVertex(edge, edge.Vertex2);
            }
        }

        public void RemoveEdge(TEdge edge)
        {
            if (edge.Vertex2 == 1)
                prMap.Remove(edge.Vertex1);
            else
            {
                RemoveFromVertex(edge, edge.Vertex1);
                RemoveFromVertex(edge, edge.Vertex2);
            }
        }

        public TEdge FindEdge(long vertex1, long vertex2)
        {
            if (vertex2 == 1)
            {
                TEdge edge;
                return prMap.TryGetValue(vertex1, out edge) ? edge : null;
            }
            List<TEdge> edges;
            if (!pprMap.TryGetValue(vertex1, out edges))
                return null;
            foreach (var edge in edges)
            {
                if (comparer.Equals(edge.Vertex1, vertex2))
                    return edge;
                if (comparer.Equals(edge.Vertex2, vertex2))
                    return edge;
            }
            return null;
        }

        public List<TEdge> FindPath(long start, long end)
        {
            if (end == 1)
            {
                // Look for a matching partial.
                if (prMap.ContainsKey(start))
                    return new List<TEdge> { prMap[start] };

                // Look for a route that terminates with a partial.
                return FindPathRecursive(start, 1, null);
            }

            var hasStart = prMap.ContainsKey(start);
            var hasEnd = prMap.ContainsKey(end);

            if (hasStart && hasEnd)
                return new List<TEdge> { prMap[end], prMap[start] };
            var result = null as List<TEdge>;
            if (pprMap.ContainsKey(end))
            {
                result  = FindPathRecursive(start, end, null);
                if (result != null)
                    return result;
            }
            if (!hasStart && !hasEnd)
            {
                var part1 = FindPathRecursive(start, 1, null);
                if (part1 != null)
                {
                    var part2 = FindPathRecursive(end, 1, null);
                    if (part2 != null)
                        return part1.Concat(part2).ToList();
                }
            }
            if (hasStart)
            {
                result = FindPathRecursive(end, 1, null);
                if (result != null)
                    result.Add(prMap[start]);
            }
            if (hasEnd)
            {
                result = FindPathRecursive(start, 1, null);
                if (result != null)
                    result.Add(prMap[end]);
            }
            return result;
        }

        private void AddToVertex(TEdge edge, long vertex)
        {
            List<TEdge> value;
            if (pprMap.TryGetValue(vertex, out value))
                value.Add(edge);
            else
                pprMap.Add(vertex, new List<TEdge> { edge });
        }

        private void RemoveFromVertex(TEdge edge, long vertex)
        {
            var edges = pprMap[vertex];
            edges.Remove(edge);
            if (edges.Count == 0)
                pprMap.Remove(vertex);
        }

        private List<TEdge> FindPathRecursive(long start, long end, TEdge previous)
        {
            List<TEdge> edges;
            if (!pprMap.TryGetValue(start, out edges))
                return null;
            if (end == 1)
            {
                TEdge edge;
                if (prMap.TryGetValue(start, out edge) && edge != previous)
                    return new List<TEdge> { prMap[start] };
            }
            foreach (var edge in edges)
            {
                if (edge == previous)
                    continue;
                var next = edge.Vertex1 == start ? edge.Vertex2 : edge.Vertex1;
                if (next == end)
                    return new List<TEdge> { edge };
                var result = FindPathRecursive(next, end, edge);
                if (result != null)
                {
                    result.Add(edge);
                    return result;
                }
            }
            return null;
        }
    }
}
