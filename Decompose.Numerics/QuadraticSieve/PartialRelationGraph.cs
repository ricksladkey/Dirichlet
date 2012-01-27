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
        private class EdgeMap
        {
            private Dictionary<long, object> map;
            public EdgeMap()
            {
                map = new Dictionary<long, object>();
            }
            public void Add(long vertex, TEdge edge)
            {
                object value;
                if (map.TryGetValue(vertex, out value))
                {
                    if (value is TEdge)
                        map[vertex] = new List<TEdge> { (TEdge)value, edge };
                    else
                        (value as List<TEdge>).Add(edge);
                }
                else
                    map.Add(vertex, edge);
            }
            public void Remove(long vertex, TEdge edge)
            {
                var value = map[vertex];
                if (value is TEdge)
                    map.Remove(vertex);
                else
                {
                    var list = value as List<TEdge>;
                    list.Remove(edge);
                    if (list.Count == 1)
                        map[vertex] = list[0];
                }
            }
            public bool HasEdges(long vertex)
            {
                return map.ContainsKey(vertex);
            }
            public bool GetEdges(long vertex, out TEdge edge, out List<TEdge> edges)
            {
                if (!map.ContainsKey(vertex))
                {
                    edge = null;
                    edges = null;
                    return false;
                }
                edge = map[vertex] as TEdge;
                edges = map[vertex] as List<TEdge>;
                return true;
            }
        }

        private Dictionary<long, TEdge> prMap;
        private EdgeMap pprMap;
        private int count;

        public int Count { get { return count; } }
        public int PartialRelations { get { return prMap.Count; } }
        public int PartialPartialRelations { get { return count - prMap.Count; } }

        public PartialRelationGraph()
        {
            prMap = new Dictionary<long, TEdge>();
            pprMap = new EdgeMap();
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
                pprMap.Add(edge.Vertex1, edge);
                pprMap.Add(edge.Vertex2, edge);
            }
            ++count;
        }

        public void RemoveEdge(TEdge edge)
        {
            if (edge.Vertex2 == 1)
                prMap.Remove(edge.Vertex1);
            else
            {
                pprMap.Remove(edge.Vertex1, edge);
                pprMap.Remove(edge.Vertex2, edge);
            }
            --count;
        }

        public TEdge FindEdge(long vertex1, long vertex2)
        {
            TEdge edge;
            List<TEdge> edges;
            if (vertex2 == 1)
            {
                return prMap.TryGetValue(vertex1, out edge) ? edge : null;
            }
            if (!pprMap.GetEdges(vertex1, out edge, out edges))
                return null;
            if (edge != null)
            {
                if (edge.Vertex1 == vertex2 || edge.Vertex2 == vertex2)
                    return edge;
                return null;
            }
            foreach (var other in edges)
            {
                if (other.Vertex1 == vertex2 || other.Vertex2 == vertex2)
                    return other;
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
            if (pprMap.HasEdges(end))
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

        private List<TEdge> FindPathRecursive(long start, long end, TEdge previous)
        {
            TEdge edge;
            List<TEdge> edges;
            if (end == 1)
            {
                if (prMap.TryGetValue(start, out edge) && edge != previous)
                    return new List<TEdge> { prMap[start] };
            }
            if (!pprMap.GetEdges(start, out edge, out edges))
                return null;
            if (edge != null)
            {
                if (edge == previous)
                    return null;
                var next = edge.Vertex1 == start ? edge.Vertex2 : edge.Vertex1;
                if (next == end)
                    return new List<TEdge> { edge };
                var result = FindPathRecursive(next, end, edge);
                if (result != null)
                {
                    result.Add(edge);
                    return result;
                }
                return null;
            }
            foreach (var other in edges)
            {
                if (other == previous)
                    continue;
                var next = other.Vertex1 == start ? other.Vertex2 : other.Vertex1;
                if (next == end)
                    return new List<TEdge> { other };
                var result = FindPathRecursive(next, end, other);
                if (result != null)
                {
                    result.Add(other);
                    return result;
                }
            }
            return null;
        }
    }
}
