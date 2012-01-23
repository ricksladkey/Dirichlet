using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompose.Numerics
{
    public class Edge<TVertex>
    {
        public TVertex Vertex1 { get; set; }
        public TVertex Vertex2 { get; set; }
    }

    public class Graph<TVertex, TEdge> where TEdge : Edge<TVertex>
    {
        private Dictionary<TVertex, List<TEdge>> graph;
        private IEqualityComparer<TVertex> comparer;

        public Graph()
        {
            graph = new Dictionary<TVertex, List<TEdge>>();
            comparer = EqualityComparer<TVertex>.Default;
        }

        public void AddEdge(TEdge edge)
        {
            AddToVertex(edge, edge.Vertex1);
            AddToVertex(edge, edge.Vertex2);
        }

        public void RemoveEdge(TEdge edge)
        {
            RemoveFromVertex(edge, edge.Vertex1);
            RemoveFromVertex(edge, edge.Vertex2);
        }

        public List<TEdge> FindPath(TVertex start, TVertex end)
        {
            return FindPath(start, end, null);
        }

        private void AddToVertex(TEdge edge, TVertex vertex)
        {
            List<TEdge> value;
            if (graph.TryGetValue(vertex, out value))
                value.Add(edge);
            else
                graph.Add(vertex, new List<TEdge> { edge });
        }

        private void RemoveFromVertex(TEdge edge, TVertex vertex)
        {
            var value = graph[vertex];
            if (value.Count == 0)
                graph.Remove(vertex);
        }

        private List<TEdge> FindPath(TVertex start, TVertex end, TEdge previous)
        {
            List<TEdge> edges;
            if (!graph.TryGetValue(start, out edges))
                return null;
            foreach (var edge in edges)
            {
                if (edge == previous)
                    continue;
                var next = comparer.Equals(edge.Vertex1, start) ? edge.Vertex2: edge.Vertex1;
                if (comparer.Equals(next, end))
                    return new List<TEdge> { edge };
                var result = FindPath(next, end, edge);
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
