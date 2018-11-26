using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreLibrary
{
    public static class GraphExtensions
    {
        public static Graph ConstructGraphFromEdges(int matchingSize, List<Edge> edges)
        {
            Dictionary<int, int> verticesMatching = new Dictionary<int, int>();
            int vertexCounter = 0;
            Graph result = new Graph(matchingSize);
            foreach (var edge in edges)
            {
                int from, to;
                if (verticesMatching.ContainsKey(edge.v1))
                {
                    from = verticesMatching[edge.v1];
                }
                else
                {
                    from = vertexCounter;
                    verticesMatching.Add(edge.v1, vertexCounter++);
                }

                if (verticesMatching.ContainsKey(edge.v2))
                {
                    to = verticesMatching[edge.v2];
                }
                else
                {
                    to = vertexCounter;
                    verticesMatching.Add(edge.v2, vertexCounter++);
                }

                result.AddEdge(from, to);
            }

            return result;
        }

        public static void AddEdge(this Graph g, int from, int to)
        {
            if (from < 0 || from >= g.Size || to < 0 || to >= g.Size)
                throw new ArgumentException("Invalid edge");
            g.AdjacencyMatrix[from, to] = 1;
            g.AdjacencyMatrix[to, from] = 1;
            g.Edges.Add(new Edge(from, to));
        }
        public static bool IsEdge(this Graph g, int from, int to)
        {
            return g.AdjacencyMatrix[from, to] == 1;
        }
        public static Graph Subgraph(this Graph g, HashSet<int> vertices)
        {
            var verticesArray = vertices.ToArray();
            var mapping = new int[g.Size];
            for (var i = 0; i < verticesArray.Length; i++)
                mapping[verticesArray[i]] = i;

            var subgraph = new Graph(vertices.Count);
            foreach (var v in vertices)
            {
                foreach (var w in vertices)
                {
                    if (g.IsEdge(v, w))
                        subgraph.AddEdge(mapping[v], mapping[w]);
                }
            }
            return subgraph;
        }
        private void AddRandomEdges(int numberOfEdgesToAdd)
        {
            var possibleEdges = GetPossibleEdges();
            if (possibleEdges.Count > 0)
            {
                for (var i = 0; i < numberOfEdgesToAdd; i++)
                {
                    var edge = possibleEdges[GoodRandom.Next(possibleEdges.Count)];
                    AdjacencyMatrix[edge.from, edge.to] = 1;
                    AdjacencyMatrix[edge.to, edge.from] = 1;
                    possibleEdges.Remove(edge);
                    if (possibleEdges.Count <= 0) break;
                }
            }
        }
        private void RemoveRandomEdges(int numberOfEdgesToRemove)
        {
            var edges = GetEdges();
            if (edges.Count > 0)
            {
                for (var i = 0; i < numberOfEdgesToRemove; i++)
                {
                    var edge = edges[GoodRandom.Next(edges.Count)];
                    AdjacencyMatrix[edge.from, edge.to] = 0;
                    AdjacencyMatrix[edge.to, edge.from] = 0;
                    edges.Remove(edge);
                    if (edges.Count <= 0) break;
                }
            }
        }
        private void AddRandomVertex(int numberOfVerticesToAdd)
        {
            var newSize = Size + numberOfVerticesToAdd;
            var newMatrix = new int[newSize, newSize];
            for (var i = 0; i < Size; i++)
            {
                for (var j = 0; j < Size; j++)
                {
                    newMatrix[i, j] = AdjacencyMatrix[i, j];
                }
            }

            AdjacencyMatrix = newMatrix;
        }
        private void RemoveRandomVertex(int numberOfVerticesToDelete)
        {
            int[,] newMatrix = null;
            var sortedVertices = SortVerticesBasedOnDegree();
            var verticesToDelete = sortedVertices
                .Take(numberOfVerticesToDelete).ToList();
            verticesToDelete.Sort((v1, v2) => -v1.CompareTo(v2));
            foreach (var vertex in verticesToDelete)
            {
                newMatrix = DeleteVertex(AdjacencyMatrix, vertex);
            }
            AdjacencyMatrix = newMatrix;
        }
    }
}