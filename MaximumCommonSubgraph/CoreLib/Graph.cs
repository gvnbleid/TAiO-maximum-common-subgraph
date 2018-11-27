using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CoreLib
{
    public class Graph
    {
        public int[,] AdjacencyMatrix { get; internal set; }

        public List<int> Vertices = new List<int>();
        public List<Edge> Edges = new List<Edge>();

        public int[] Matching1 { get; set; }
        public int[] Matching2 { get; set; }

        public Graph(int[,] graph)
        {
            AdjacencyMatrix = graph.Clone() as int[,];

            int size = graph.GetLength(0);
            if (size > 0)
            {
                Vertices.Add(0);
                for (int i = 1; i < graph.GetLength(0); i++)
                {
                    Vertices.Add(i);
                    for (int j = 0; j < i; j++)
                    {
                        if (graph[i, j] != 0)
                        {
                            Edges.Add(new Edge(i, j));
                        }
                    }
                }
            }

            if (!this.IsCorrect())
                throw new ArgumentException("Invalid input graph");
        }

        public Graph(int size)
        {
            AdjacencyMatrix = new int[size, size];
        }

        #region PropertiesWithoutLogic
        public int Score { get; set; }
        public int NormalizedScore { get; set; }

        #endregion

        #region PropertiesWithLogic

        public int Size => AdjacencyMatrix.GetLength(0);
        public int EdgesCount
        {
            get
            {
                var count = 0;
                for (var i = 0; i < Size; i++)
                {
                    for (var j = 0; j < i; j++)
                    {
                        if (AdjacencyMatrix[i, j] == 1) count++;
                    }
                }
                return count;
            }
        }
        public int this[int i, int j] => AdjacencyMatrix[i, j];
        #endregion

        #region PublicMethods

        public void AddEdge(int from, int to)
        {
            if (from < 0 || from >= Size || to < 0 || to >= Size)
                throw new ArgumentException("Invalid edge");
            if (AdjacencyMatrix[from, to] == 0)
            {
                AdjacencyMatrix[from, to] = 1;
                AdjacencyMatrix[to, from] = 1;
                Edges.Add(new Edge(from, to));
            }
            Debug.WriteLine("Edge already in graph");
        }
        public bool IsEdge(int from, int to)
        {
            return AdjacencyMatrix[from, to] == 1;
        }
        public Graph Subgraph(IEnumerable<int> vertices)
        {
            var verticesArray = vertices.ToArray();
            var mapping = new int[Size];
            for (var i = 0; i < verticesArray.Length; i++)
                mapping[verticesArray[i]] = i;

            var subgraph = new Graph(vertices.Count());
            foreach (var v in vertices)
            {
                foreach (var w in vertices)
                {
                    if (IsEdge(v, w))
                        subgraph.AddEdge(mapping[v], mapping[w]);
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
                if (j == i && AdjacencyMatrix[i, j] == 1) // loops
                    return false;
                else if (AdjacencyMatrix[i, j] != AdjacencyMatrix[j, i])
                    return false;
            return true;
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            if (Size <= 10)
            {
                stringBuilder.AppendLine("Adjacency matrix:");
                for (var i = 0; i < Size; i++)
                {
                    for (var j = 0; j < Size; j++)
                    {
                        stringBuilder.Append($"{AdjacencyMatrix[i, j]} ");
                    }

                    stringBuilder.AppendLine();
                }

                stringBuilder.AppendLine();
            }

            stringBuilder.AppendLine($"Number of vertices in graph: {Size}");
            stringBuilder.AppendLine($"Number of edges in graph: {EdgesCount}");
            if (EdgesCount <= 20)
            {
                stringBuilder.AppendLine($"List of edges: ");
                foreach (var edge in Edges)
                {
                    stringBuilder.AppendLine($"<{edge.v1},{edge.v2}>");
                }
            }

            return stringBuilder.ToString();
        }

        public Graph Clone()
        {
            return new Graph(AdjacencyMatrix);
        }

        #endregion

        #region HelperMethods_AddingAndRemovingEdgesAndVertices

        
       
        public int[] SortVerticesBasedOnDegree()
        {
            var vertices = new List<(int vertex, int degree)>();
            for (var i = 0; i < Size; i++)
            {
                vertices.Add((i, GetVertexDegree(i)));
            }

            vertices.Sort((tuple1, tuple2) => tuple1.degree.CompareTo(tuple2.degree));
            return vertices
                .ConvertAll(tuple => tuple.vertex)
                .ToArray();
        }
        //private List<(int from, int to)> GetEdges()
        //{
        //    var edges = new List<(int, int)>();
        //    for (var i = 0; i < Size; i++)
        //    {
        //        for (var j = 0; j < Size; j++)
        //        {
        //            if (AdjacencyMatrix[i, j] == 1 && !edges.Contains((j, i)))
        //            {
        //                edges.Add((i, j));
        //            }
        //        }
        //    }

        //    return edges;
        //}
        private int GetVertexDegree(int index)
        {
            var degree = 0;
            for (var i = 0; i < Size; i++)
            {
                if (AdjacencyMatrix[index, i] == 1) degree++;
            }
            return degree;
        }


        #endregion


        public void CreateRandomMatching1(Graph g)
        {
            Matching1 = new int[Size];
            var usedVertices = new List<int>();
            var matchedVerticesCount = 0;
            while (matchedVerticesCount < Size)
            {
                var selectedVertex = GoodRandom.Next(g.Size);
                while (usedVertices.Contains(selectedVertex))
                {
                    selectedVertex = ++selectedVertex % g.Size;
                }
                usedVertices.Add(selectedVertex);
                Matching1[matchedVerticesCount++] = selectedVertex;
            }
        }

        public void CreateRandomMatching2(Graph g)
        {
            Matching2 = new int[Size];
            var usedVertices = new List<int>();
            var matchedVerticesCount = 0;
            while (matchedVerticesCount < Size)
            {
                var selectedVertex = GoodRandom.Next(g.Size);
                while (usedVertices.Contains(selectedVertex))
                {
                    selectedVertex = ++selectedVertex % g.Size;
                }
                usedVertices.Add(selectedVertex);
                Matching2[matchedVerticesCount++] = selectedVertex;
            }
        }

        public int NumberOfUnconnectedSubgraphsInMatching1(Graph target, out bool unableToCalculate)
        {
            unableToCalculate = false;
            var isolatedVertices = new List<int>();
            if (Size > target.Size)
            {
                unableToCalculate = true;
                return int.MaxValue;
            }
            if (!(target.AdjacencyMatrix.Clone() is int[,] matrixWithoutMatch))
            {
                throw new InvalidCastException();
            }

            for (var i = 0; i < Size; i++)
            {
                for (var j = 0; j < Size; j++)
                {
                    if (AdjacencyMatrix[i, j] != 1) continue;
                    try
                    {
                        if (target[Matching1[i], Matching1[j]] == 0)
                        {
                            unableToCalculate = true;
                            return int.MaxValue;
                        }
                        matrixWithoutMatch[Matching1[i], Matching1[j]] = 0;
                    }
                    catch (Exception)
                    {
                        // ignored
                    }


                }
            }

            var edges = new List<(int from, int to)>();
            for (var i = 0; i < target.Size; i++)
            {
                isolatedVertices.Add(i);
                for (var j = 0; j < target.Size; j++)
                {
                    if (matrixWithoutMatch[i, j] == 1 && !edges.Contains((j, i)))
                    {
                        edges.Add((i, j));
                    }
                }
            }

            if (edges.Count == 0) return 0;
            var visited = new bool[target.Size];
            var queue = new Queue<int>();
            var subgraphsCount = 0;
            while (visited.Contains(false))
            {
                for (var i = 0; i < target.Size; i++)
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
            foreach (var edge in edges)
            {
                isolatedVertices.Remove(edge.from);
                isolatedVertices.Remove(edge.to);
            }

            return subgraphsCount - isolatedVertices.Count;
        }

        public int NumberOfUnconnectedSubgraphsInMatching2(Graph target, out bool unableToCalculate)
        {
            unableToCalculate = false;
            var isolatedVertices = new List<int>();
            if (Size > target.Size)
            {
                unableToCalculate = true;
                return int.MaxValue;
            }
            if (!(target.AdjacencyMatrix.Clone() is int[,] matrixWithoutMatch))
            {
                throw new InvalidCastException();
            }

            for (var i = 0; i < Size; i++)
            {
                for (var j = 0; j < Size; j++)
                {
                    if (AdjacencyMatrix[i, j] != 1) continue;
                    try
                    {
                        if (target[Matching2[i], Matching2[j]] == 0)
                        {
                            unableToCalculate = true;
                            return int.MaxValue;
                        }
                        matrixWithoutMatch[Matching2[i], Matching2[j]] = 0;
                    }
                    catch (Exception)
                    {
                        // ignored
                    }


                }
            }

            var edges = new List<(int from, int to)>();
            for (var i = 0; i < target.Size; i++)
            {
                isolatedVertices.Add(i);
                for (var j = 0; j < target.Size; j++)
                {
                    if (matrixWithoutMatch[i, j] == 1 && !edges.Contains((j, i)))
                    {
                        edges.Add((i, j));
                    }
                }
            }

            if (edges.Count == 0) return 0;
            var visited = new bool[target.Size];
            var queue = new Queue<int>();
            var subgraphsCount = 0;
            while (visited.Contains(false))
            {
                for (var i = 0; i < target.Size; i++)
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
            foreach (var edge in edges)
            {
                isolatedVertices.Remove(edge.from);
                isolatedVertices.Remove(edge.to);
            }

            return subgraphsCount - isolatedVertices.Count;
        }

    }
}
