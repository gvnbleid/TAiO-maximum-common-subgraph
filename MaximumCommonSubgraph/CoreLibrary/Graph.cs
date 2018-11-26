using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace CoreLibrary
{
    public class Graph
    {
        public int[,] AdjacencyMatrix { get; internal set; }

        public List<int> Vertices = new List<int>();
        public List<Edge> Edges = new List<Edge>();

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
        public int NumberOfUnconnectedSubgraphs
        {
            get
            {
                var edges = new List<(int from, int to)>();
                for (var i = 0; i < Size; i++)
                {
                    for (var j = 0; j < Size; j++)
                    {
                        if (AdjacencyMatrix[i, j] == 1 && !edges.Contains((j, i)))
                        {
                            edges.Add((i, j));
                        }
                    }
                }


                if (edges.Count == 0) return 0;
                var visited = new bool[Size];
                var queue = new Queue<int>();
                var subgraphsCount = 0;
                while (visited.Contains(false))
                {
                    for (var i = 0; i < Size; i++)
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
        }

        #endregion

        #region PublicStaticMethods

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

        #endregion

        #region PublicMethods

        public void AddEdge(int from, int to)
        {
            if (from < 0 || from >= Size || to < 0 || to >= Size)
                throw new ArgumentException("Invalid edge");
            AdjacencyMatrix[from, to] = 1;
            AdjacencyMatrix[to, from] = 1;
            Edges.Add(new Edge(from, to));
        }
        public bool IsEdge(int from, int to)
        {
            return AdjacencyMatrix[from, to] == 1;
        }
        public Graph Subgraph(HashSet<int> vertices)
        {
            var verticesArray = vertices.ToArray();
            var mapping = new int[Size];
            for (var i = 0; i < verticesArray.Length; i++)
                mapping[verticesArray[i]] = i;

            var subgraph = new Graph(vertices.Count);
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

        public void PrintToConsole(List<Edge> matching)
        {
            if (Size <= 10)
            {
                WriteMatrix(matching);
            }
            else
            {
                Console.WriteLine(this);
                Console.WriteLine($"Number of matching edges: {matching.Count}");
                if(matching.Count <= 20)
                {
                    Console.WriteLine("List of matching edges:");
                    foreach (var edge in matching)
                    {
                        Console.WriteLine($"<{edge.v1},{edge.v2}>");
                    }
                }
            }
        }

        private void WriteMatrix(List<Edge> matching)
        {
            int[,] AdjacencyMatrixCopy = new int[AdjacencyMatrix.GetLength(0), AdjacencyMatrix.GetLength(0)];
            for (int i = 0; i < AdjacencyMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < AdjacencyMatrix.GetLength(1); j++)
                {
                    AdjacencyMatrixCopy[i, j] = AdjacencyMatrix[i, j];
                }
            }
            foreach (var el in matching)
            {
                AdjacencyMatrixCopy[el.v1, el.v2] = 2;
                AdjacencyMatrixCopy[el.v2, el.v1] = 2;
            }

            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
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

        
       
        private int[] SortVerticesBasedOnDegree()
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
        private List<(int from, int to)> GetEdges()
        {
            var edges = new List<(int, int)>();
            for (var i = 0; i < Size; i++)
            {
                for (var j = 0; j < Size; j++)
                {
                    if (AdjacencyMatrix[i, j] == 1 && !edges.Contains((j, i)))
                    {
                        edges.Add((i, j));
                    }
                }
            }

            return edges;
        }
        private List<(int from, int to)> GetPossibleEdges()
        {
            var emptyPairs = new List<(int, int)>();
            for (var i = 0; i < Size; i++)
            {
                for (var j = 0; j < Size; j++)
                {
                    if (AdjacencyMatrix[i, j] == 0 && i != j && !emptyPairs.Contains((j, i)))
                    {
                        emptyPairs.Add((i, j));
                    }
                }
            }

            return emptyPairs;
        }
        private int GetVertexDegree(int index)
        {
            var degree = 0;
            for (var i = 0; i < Size; i++)
            {
                if (AdjacencyMatrix[index, i] == 1) degree++;
            }
            return degree;
        }
        private int GetValueRound(int i, int j)
        {
            return AdjacencyMatrix[i % Size, j % Size];
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

        #endregion
        
    }
}
