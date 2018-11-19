using System;
using System.Collections.Generic;

namespace CliqueHeuristic
{
    public class Graph
    {
        private bool[,] AdjacencyMatrix { get; }

        public int VertexCount => AdjacencyMatrix.GetLength(0);

        public int EdgeCount
        {
            get
            {
                var count = 0;
                for (var i = 0; i < AdjacencyMatrix.GetLength(0); i++)
                    for (var j = i; j < AdjacencyMatrix.GetLength(1); j++)
                        count++;
                return count;
            }
        }

        public Graph(int size)
        {
            AdjacencyMatrix = new bool[size, size];
        }
        public Graph(bool[,] g)
        {
            AdjacencyMatrix = g.Clone() as bool[,];
            // TODO: remove unnecessary check
            if (!IsCorrect())
                throw new ArgumentException("Invalid input graph");
        }
        public override string ToString()
        {
            var result = "";
            for (var i = 0; i < AdjacencyMatrix.GetLength(0); i++)
                for (var j = i; j < AdjacencyMatrix.GetLength(1); j++)
                    if (IsEdge(i, j))
                    {
                        if (result.Length > 0)
                            result += ", ";
                        result += "<" + i + ", " + j + ">";
                    }
            return result;
        }
        public void AddEdge(int from, int to)
        {
            if(from < 0 || from >= VertexCount || to < 0 || to >= VertexCount)
                throw new ArgumentException("Invalid edge");
            AdjacencyMatrix[from, to] = AdjacencyMatrix[to, from] = true;
        }
        public bool IsEdge(int from, int to)
        {
            return AdjacencyMatrix[from, to];
        }
        public Graph Subgraph(HashSet<int> vertices)
        {
            var subgraph = new Graph(vertices.Count);
            foreach (var v in vertices)
            {
                foreach (var w in vertices)
                {
                    if(IsEdge(v, w))
                        subgraph.AddEdge(v, w);
                }
            }
            return subgraph;
        }
        public bool IsCorrect()
        {
            if (AdjacencyMatrix.GetLength(0) != AdjacencyMatrix.GetLength(1))
                return false;
            for (var i = 0; i < AdjacencyMatrix.GetLength(0); i++)
                for (var j = i; j < AdjacencyMatrix.GetLength(1); j++)
                    if (j == i && AdjacencyMatrix[i, j]) // loops
                        return false;
                    else if (AdjacencyMatrix[i, j] != AdjacencyMatrix[j, i])
                        return false;
            return true;
        }
    }
}
