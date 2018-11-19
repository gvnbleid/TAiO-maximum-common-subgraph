using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Priority_Queue;

namespace CliqueHeuristic
{
    internal class Graph
    {
        public static bool[,] ModularProduct(bool[,] g, bool[,] h)
        {
            if (!IsCorrect(g) || !IsCorrect(h))
                throw new ArgumentException("Invalid input graph");
            var gSize = g.GetLength(0);
            var hSize = h.GetLength(0);
            var dim = gSize * hSize;
            var product = new bool[dim, dim];
            for (var i = 0; i < dim; i++)
                for (var j = i; j < dim; j++)
                    if (i / hSize == j / hSize || i % gSize == j % gSize)
                        product[i, j] = false;
                    else
                        product[i, j] = product[j, i] = (g[i / hSize, j / hSize] && h[i % gSize, j % gSize]) ||
                                                        (!g[i / hSize, j / hSize] && !h[i % gSize, j % gSize]);
            return product;
        }
        private class CliqueNode : FastPriorityQueueNode
        {
            public int CliqueNumber { get; set; }
            public int VertexSmallestDegree { get; set; }
            public HashSet<int> Vertices = new HashSet<int>();
            public CliqueNode(int v, int vertexSmallestDegree)
            {
                CliqueNumber = v;
                VertexSmallestDegree = vertexSmallestDegree;
                Vertices.Add(v);
            }
        }
        public static int[] Clique(bool[,] g, bool modeVerticesOnly)
        {
            if (!IsCorrect(g))
                throw new ArgumentException("Invalid input graph");
            var gSize = g.GetLength(0);
            var vertexDegrees = new int[gSize];
            for (var i = 0; i < vertexDegrees.Length; i++)
                for (var j = 0; j < vertexDegrees.Length; j++)
                    if (g[i, j])
                        vertexDegrees[i]++;
            var queue = new FastPriorityQueue<CliqueNode>(gSize);
            for (var i = 0; i < vertexDegrees.Length; i++)
                queue.Enqueue(new CliqueNode(i, vertexDegrees[i]), vertexDegrees[i]);
            var unmergable = new bool[gSize, gSize];
            var unmergableCount = gSize; // how many true values in 'unmergable', initialized with diagonal
            while (unmergableCount < gSize * gSize)
            {
                CliqueNode node1 = queue.Dequeue(), node2 = null;
                var unmergableWithNode1 = new LinkedList<CliqueNode>();
                do
                {
                    if (node2 != null)
                        unmergableWithNode1.AddLast(node2);
                    node2 = queue.Dequeue();
                } while (unmergable[node1.CliqueNumber, node2.CliqueNumber]);
                foreach (var node in unmergableWithNode1)
                    queue.Enqueue(node, node.VertexSmallestDegree);
                var mergable = true;
                foreach (var node1Vertex in node1.Vertices)
                {
                    foreach (var node2Vertex in node2.Vertices)
                        if (!g[node1Vertex, node2Vertex])
                        {
                            unmergable[node1.CliqueNumber, node2.CliqueNumber] =
                                unmergable[node2.CliqueNumber, node1.CliqueNumber] = true;
                            unmergableCount += 2;
                            mergable = false;
                            break;
                        }
                    if (!mergable)
                        break;
                }
                if (mergable)
                {
                    node1.Vertices.UnionWith(node2.Vertices);
                    node1.VertexSmallestDegree = Math.Min(node1.VertexSmallestDegree, node2.VertexSmallestDegree);
                    queue.Enqueue(node1, node1.VertexSmallestDegree);
                }
            }
            HashSet<int> clique = null;
            if (modeVerticesOnly)
            {
                foreach (var cliqueNode in queue)
                    if (clique == null || cliqueNode.Vertices.Count > clique.Count)
                        clique = cliqueNode.Vertices;
            }
            else
            {
                // TODO: wersja dla sumy wierzcholkow i krawedzi
            }
            return clique?.ToArray();
        }
        public static bool IsCorrect(bool[,] g)
        {
            if (g.GetLength(0) != g.GetLength(1))
                return false;
            for (var i = 0; i < g.GetLength(0); i++)
                for (var j = i; j < g.GetLength(1); j++)
                    if (j == i && g[i, j]) // loops
                        return false;
                    else if (g[i, j] != g[j, i])
                        return false;
            return true;
        }
    }
}
