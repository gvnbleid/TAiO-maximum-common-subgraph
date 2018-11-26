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
