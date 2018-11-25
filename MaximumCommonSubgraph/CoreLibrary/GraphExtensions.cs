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
            if(from < 0 || from >= g.Size || to < 0 || to >= g.Size)
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
        public static bool IsCorrect(this Graph g)
        {
            if (g.AdjacencyMatrix.GetLength(0) != g.AdjacencyMatrix.GetLength(1))
                return false;
            for (var i = 0; i < g.AdjacencyMatrix.GetLength(0); i++)
                for (var j = i; j < g.AdjacencyMatrix.GetLength(1); j++)
                    if (j == i && g.AdjacencyMatrix[i, j] == 1) // loops
                        return false;
                    else if (g.AdjacencyMatrix[i, j] != g.AdjacencyMatrix[j, i])
                        return false;
            return true;
        }
    }
}
