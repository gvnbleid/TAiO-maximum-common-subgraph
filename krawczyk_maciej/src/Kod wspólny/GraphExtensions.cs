using System;
using System.Collections.Generic;

namespace CoreLib
{
    public static class GraphExtensions
    {
        public static void PrintToConsole(this Graph g, List<Edge> matching)
        {
            if (g.Size <= 10)
            {
                g.WriteMatrix(matching);
            }
            else
            {
                Console.WriteLine(g);
                Console.WriteLine($"Number of matching edges: {matching.Count}");
                if (matching.Count <= 30)
                {
                    Console.WriteLine("List of matching edges:");
                    foreach (var edge in matching)
                    {
                        Console.WriteLine($"<{edge.v1},{edge.v2}>");
                    }
                }
            }
        }

        private static void WriteMatrix(this Graph g, List<Edge> matching)
        {
            int[,] AdjacencyMatrixCopy = new int[g.AdjacencyMatrix.GetLength(0), g.AdjacencyMatrix.GetLength(0)];
            for (int i = 0; i < g.AdjacencyMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < g.AdjacencyMatrix.GetLength(1); j++)
                {
                    AdjacencyMatrixCopy[i, j] = g.AdjacencyMatrix[i, j];
                }
            }
            foreach (var el in matching)
            {
                AdjacencyMatrixCopy[el.v1, el.v2] = 2;
                AdjacencyMatrixCopy[el.v2, el.v1] = 2;
            }

            for (int i = 0; i < g.Size; i++)
            {
                for (int j = 0; j < g.Size; j++)
                {
                    if (AdjacencyMatrixCopy[i, j] == 2)
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.Write("1");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.Write(" ");
                    }
                    else
                        Console.Write(AdjacencyMatrixCopy[i, j] + " ");
                }
                Console.Write("\n");
            }
            Console.Write("\n");
        }

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

        public static int[,] DeleteVertex(int[,] matrix, int index)
        {
            var oldSize = matrix.GetLength(0);
            var newSize = oldSize - 1;
            var newMatrix = new int[newSize, newSize];
            int iPrim = 0, jPrim = 0;
            for (var i = 0; i < oldSize; i++)
            {
                if (i == index) continue;
                for (var j = 0; j < oldSize; j++)
                {
                    if (j == index) continue;
                    newMatrix[iPrim, jPrim] = matrix[i, j];
                    jPrim++;
                }

                jPrim = 0;
                iPrim++;
            }
            return newMatrix;
        }
    }
}
