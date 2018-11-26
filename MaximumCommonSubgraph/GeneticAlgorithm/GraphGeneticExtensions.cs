using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreLibrary;

namespace GeneticAlgorithm
{
    public static class GraphGeneticExtensions
    {
        public static void Mutate(this Graph g)
        {
            var vertexProbability = CoreLibrary.GoodRandom.Next(100);
            var edgeProbability = CoreLibrary.GoodRandom.Next(100);
            var signDecision = CoreLibrary.GoodRandom.Bool();
            var sign = signDecision ? -1 : 1;
            var vertexChange = g.GetChange(vertexProbability);
            var edgeChange = g.GetChange(edgeProbability);
            vertexChange *= sign;
            edgeChange *= sign;
            if (vertexChange != 0)
            {
                var newSize = g.Size + vertexChange;
                if (newSize > 0)
                {
                    if (newSize > g.Size)
                    {
                        g.AddRandomVertex(vertexChange);
                    }
                    else
                    {
                        g.RemoveRandomVertex(-vertexChange);
                    }

                }
            }

            if (edgeChange != 0)
            {
                if (edgeChange > 0)
                {
                    g.AddRandomEdges(edgeChange);
                }
                else
                {
                    g.RemoveRandomEdges(-edgeChange);
                }
            }
        }
        public static int NumberOfUnconnectedSubgraphsInMatching(this Graph g, Graph match)
        {
            if (match.Size > g.Size) return int.MaxValue;
            if (!(g.AdjacencyMatrix.Clone() is int[,] matrixWithoutMatch))
            {
                throw new InvalidCastException();
            }
            for (var i = 0; i < match.Size; i++)
            {
                for (var j = 0; j < match.Size; j++)
                {
                    if (match[i, j] == 1) matrixWithoutMatch[i, j] = 0;
                }
            }

            var edges = new List<(int from, int to)>();
            for (var i = 0; i < g.Size; i++)
            {
                for (var j = 0; j < g.Size; j++)
                {
                    if (matrixWithoutMatch[i, j] == 1 && !edges.Contains((j, i)))
                    {
                        edges.Add((i, j));
                    }
                }
            }


            if (edges.Count == 0) return 0;
            var visited = new bool[g.Size];
            var queue = new Queue<int>();
            var subgraphsCount = 0;
            while (visited.Contains(false))
            {
                for (var i = 0; i < g.Size; i++)
                {
                    if (visited[i] == false)
                    {
                        queue.Enqueue(i);
                        break;
                    }
                }
                subgraphsCount++;
                while (queue.Count > 0)
                {
                    var p = queue.Dequeue();
                    if (!visited[p])
                    {
                        visited[p] = true;
                        foreach (var edge in edges.FindAll(e => e.from == p || e.to == p))
                        {
                            if (edge.from == p && !visited[edge.to])
                            {
                                queue.Enqueue(edge.to);
                            }
                            else if (!visited[edge.from])
                            {
                                queue.Enqueue(edge.from);
                            }
                        }
                    }
                }


            }

            return subgraphsCount;
        }

        private static void AddRandomEdges(this Graph g, int numberOfEdgesToAdd)
        {
            var possibleEdges = g.GetPossibleEdges();
            if (possibleEdges.Count > 0)
            {
                for (var i = 0; i < numberOfEdgesToAdd; i++)
                {
                    var edge = possibleEdges[CoreLibrary.GoodRandom.Next(possibleEdges.Count)];
                    g.AdjacencyMatrix[edge.from, edge.to] = 1;
                    g.AdjacencyMatrix[edge.to, edge.from] = 1;
                    possibleEdges.Remove(edge);
                    if (possibleEdges.Count <= 0) break;
                }
            }
        }
        private static void RemoveRandomEdges(this Graph g, int numberOfEdgesToRemove)
        {
            var edges = g.GetEdges();
            if (edges.Count > 0)
            {
                for (var i = 0; i < numberOfEdgesToRemove; i++)
                {
                    var edge = edges[CoreLibrary.GoodRandom.Next(edges.Count)];
                    g.AdjacencyMatrix[edge.from, edge.to] = 0;
                    g.AdjacencyMatrix[edge.to, edge.from] = 0;
                    edges.Remove(edge);
                    if (edges.Count <= 0) break;
                }
            }
        }
        private static Graph AddRandomVertex(this Graph g, int numberOfVerticesToAdd)
        {
            var newSize = g.Size + numberOfVerticesToAdd;
            var newMatrix = new int[newSize, newSize];
            for (var i = 0; i < g.Size; i++)
            {
                for (var j = 0; j < g.Size; j++)
                {
                    newMatrix[i, j] = g.AdjacencyMatrix[i, j];
                }
            }

            return new Graph(newMatrix);
        }
        private static Graph RemoveRandomVertex(this Graph g, int numberOfVerticesToDelete)
        {
            int[,] newMatrix = null;
            var sortedVertices = g.SortVerticesBasedOnDegree();
            var verticesToDelete = sortedVertices
                .Take(numberOfVerticesToDelete).ToList();
            verticesToDelete.Sort((v1, v2) => -v1.CompareTo(v2));
            foreach (var vertex in verticesToDelete)
            {
                newMatrix = GraphExtensions.DeleteVertex(g.AdjacencyMatrix, vertex);
            }
            return new Graph(newMatrix);
        }
    }
}
