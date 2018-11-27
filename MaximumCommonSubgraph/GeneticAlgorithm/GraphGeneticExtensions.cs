using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreLib;

namespace GeneticAlgorithm
{
    public static class GraphGeneticExtensions
    {
        public static void Mutate(this Graph g)
        {
            var vertexProbability = GoodRandom.Next(100);
            var edgeProbability = GoodRandom.Next(100);
            var signDecision = GoodRandom.Bool();
            var sign = signDecision ? -1 : 1;
            var vertexChange = GetChange(vertexProbability);
            var edgeChange = GetChange(edgeProbability);
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
                    var edge = possibleEdges[GoodRandom.Next(possibleEdges.Count)];
                    g.AdjacencyMatrix[edge.v1, edge.v2] = 1;
                    g.AdjacencyMatrix[edge.v2, edge.v1] = 1;
                    possibleEdges.Remove(edge);
                    if (possibleEdges.Count <= 0) break;
                }
            }
        }
        private static void RemoveRandomEdges(this Graph g, int numberOfEdgesToRemove)
        {
            var edges = g.Edges;
            if (edges.Count > 0)
            {
                for (var i = 0; i < numberOfEdgesToRemove; i++)
                {
                    var edge = edges[GoodRandom.Next(edges.Count)];
                    g.AdjacencyMatrix[edge.v1, edge.v2] = 0;
                    g.AdjacencyMatrix[edge.v2, edge.v1] = 0;
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

        public static int CalculateNumberOfUnconnectedSubgraphs(this Graph g)
        {
            var edges = new List<(int from, int to)>();
            for (var i = 0; i < g.Size; i++)
            {
                for (var j = 0; j < g.Size; j++)
                {
                    if (g.AdjacencyMatrix[i, j] == 1 && !edges.Contains((j, i)))
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
        public static Graph CreateRandomGraph(int maxSize)
        {
            var size = GoodRandom.Next(maxSize) + 1;
            var matrix = new int[size, size];
            for (var i = 0; i < size; i++)
            {
                for (var j = 0; j < i; j++)
                {
                    if (GoodRandom.Bool())
                    {
                        matrix[i, j] = 1;
                        matrix[j, i] = 1;
                    }
                }
            }

            return new Graph(matrix);
        }

        public static Graph CreateChild(Graph mother, Graph father)
        {
            var childSize = (mother.Size + father.Size) / 2;
            if ((mother.Size + father.Size) % 2 == 1 && GoodRandom.Bool()) childSize++;
            var child = new int[childSize, childSize];
            var decision = false;
            for (var i = 0; i < childSize; i++)
            {
                for (var j = 0; j < i; j++)
                {
                    var edgeExistance = mother.GetValueRound(i, j) + father.GetValueRound(i, j);
                    switch (edgeExistance)
                    {
                        case 0:
                            decision = GoodRandom.Next(100) < 1;
                            if (decision)
                            {
                                child[i, j] = 1;
                                child[j, i] = 1;
                            }
                            break;
                        case 1:
                            decision = GoodRandom.Next(100) < 49;
                            if (decision)
                            {
                                child[i, j] = 1;
                                child[j, i] = 1;
                            }
                            break;
                        case 2:
                            decision = GoodRandom.Next(100) < 98;
                            if (decision)
                            {
                                child[i, j] = 1;
                                child[j, i] = 1;
                            }
                            break;
                        default:
                            throw new ArgumentException("Parent graphs are invalid, values other than 0 or 1 found");
                    }
                }
            }
            return new Graph(child);
        }

        private static int GetValueRound(this Graph g, int i, int j)
        {
            return g.AdjacencyMatrix[i % g.Size, j % g.Size];
        }
        private static int GetChange(int probability)
        {
            //gaussian probability should go here
            int change;
            switch (probability)
            {
                case 95:
                case 96:
                case 97:
                    change = 1;
                    break;
                case 98:
                    change = 2;
                    break;
                case 99:
                    change = 3;
                    break;
                default:
                    change = 0;
                    break;
            }

            return change;
        }

        private static List<Edge> GetPossibleEdges(this Graph g)
        {
            var emptyPairs = new List<Edge>();
            for (var i = 0; i < g.Size; i++)
            {
                for (var j = 0; j < i; j++)
                {
                    if (g.AdjacencyMatrix[i, j] == 0)
                    {
                        emptyPairs.Add(new Edge(i, j));
                    }
                }
            }

            return emptyPairs;
        }

    }
}
