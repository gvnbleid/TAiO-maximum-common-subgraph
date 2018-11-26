using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace CoreLibrary
{
    public class Graph
    {
        #region public_fields
        public bool this[int i, int j] => _adjacencyMatrix[i, j];
        public int Size => _adjacencyMatrix.GetLength(0);
        #endregion
        #region protected_fields
        #endregion
        #region private_fields
        private bool[,] _adjacencyMatrix;
        #endregion

        #region public_methods
        public Graph(bool[,] matrix)
        {
            if (!IsAdjacencyMatrixCorrect(matrix))
                throw new ArgumentException("Invalid input matrix");
            _adjacencyMatrix = matrix;
        }
        public Graph(int size)
        {
            _adjacencyMatrix = new bool[size, size];
        }
        public Graph Clone()
        {
            var matrix = _adjacencyMatrix.Clone() as bool[,];
            return new Graph(matrix);
        }
        public int CountEdges()
        {
            var count = 0;
            for (var i = 0; i < Size; i++)
                for (var j = 0; j < i; j++)
                    if (_adjacencyMatrix[i, j]) count++;
            return count;
        }
        public bool IsEdge(int i, int j)
        {
            return this[i, j];
        }
        #endregion
        #region protected_methods
        #endregion
        #region private_methods
        private void DeleteVertex(int index)
        {
            var newSize = Size - 1;
            var newMatrix = new bool[newSize, newSize];
            int iPrim = 0, jPrim = 0;
            for (var i = 0; i < Size; i++)
            {
                if (i == index) continue;
                for (var j = 0; j < Size; j++)
                {
                    if (j == index) continue;
                    newMatrix[iPrim, jPrim] = _adjacencyMatrix[i, j];
                    jPrim++;
                }

                jPrim = 0;
                iPrim++;
            }
            _adjacencyMatrix = newMatrix;
        }
        #endregion
        #region public_static_methods
        public static bool IsAdjacencyMatrixCorrect(bool[,] matrix)
        {
            if (matrix.GetLength(0) != matrix.GetLength(1))
                return false;
            for (var i = 0; i < matrix.GetLength(0); i++)
                for (var j = i; j < matrix.GetLength(1); j++)
                    if (j == i && matrix[i, j]) // loops
                        return false;
                    else if (matrix[i, j] != matrix[j, i])
                        return false;
            return true;
        }
        public static Graph CreateRandomGraph(int maxSize)
        {
            var size = GoodRandom.Next(maxSize) + 1;
            var matrix = new bool[size, size];
            for (var i = 0; i < size; i++)
                for (var j = 0; j < i; j++)
                    if (GoodRandom.Bool())
                        matrix[i, j] = matrix[j, i] = true;
            return new Graph(matrix);
        }
        #endregion


        /*public int NumberOfUnconnectedSubgraphs
        {
            get
            {
                var edges = new List<>(int from, int to)>();
                for (var i = 0; i < Size; i++)
                {
                    for (var j = 0; j < Size; j++)
                    {
                        if (_adjacencyMatrix[i, j] == 1 && !edges.Contains((j, i)))
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
        }*/

        /*public void PrintToConsole(List<Edge> matching)
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
            int[,] AdjacencyMatrixCopy = new int[_adjacencyMatrix.GetLength(0), _adjacencyMatrix.GetLength(0)];
            for (int i = 0; i < _adjacencyMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < _adjacencyMatrix.GetLength(1); j++)
                {
                    AdjacencyMatrixCopy[i, j] = _adjacencyMatrix[i, j];
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
        }*/

        /*public int NumberOfUnconnectedSubgraphsInMatching(Graph match)
        {
            if (match.Size > Size) return int.MaxValue;
            if (!(_adjacencyMatrix.Clone() is int[,] matrixWithoutMatch))
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
            for (var i = 0; i < Size; i++)
            {
                for (var j = 0; j < Size; j++)
                {
                    if (matrixWithoutMatch[i, j] == 1 && !edges.Contains((j, i)))
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
        }*/
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
                        stringBuilder.Append($"{_adjacencyMatrix[i, j]} ");
                    }

                    stringBuilder.AppendLine();
                }

                stringBuilder.AppendLine();
            }

            var edgeCount = CountEdges();
            stringBuilder.AppendLine($"Number of vertices in graph: {Size}");
            stringBuilder.AppendLine($"Number of edges in graph: {edgeCount}");
            if (edgeCount <= 20)
            {
                stringBuilder.AppendLine($"List of edges: ");
                foreach (var edge in Edges)
                {
                    stringBuilder.AppendLine($"<{edge.v1},{edge.v2}>");
                }
            }

            return stringBuilder.ToString();
        }


        #endregion

        #region HelperMethods_AddingAndRemovingEdgesAndVertices

        
        
        private int[] SortVerticesBasedOnDegree()
        {
            var vertices = new List<>(int vertex, int degree)>();
            for (var i = 0; i < Size; i++)
            {
                vertices.Add((i, GetVertexDegree(i)));
            }

            vertices.Sort((tuple1, tuple2) => tuple1.degree.CompareTo(tuple2.degree));
            return vertices
                .ConvertAll(tuple => tuple.vertex)
                .ToArray();
        }
        private List<>(int from, int to)> GetEdges()
        {
            var edges = new List<>(int, int)>();
            for (var i = 0; i < Size; i++)
            {
                for (var j = 0; j < Size; j++)
                {
                    if (_adjacencyMatrix[i, j] == 1 && !edges.Contains((j, i)))
                    {
                        edges.Add((i, j));
                    }
                }
            }

            return edges;
        }
        private List<>(int from, int to)> GetPossibleEdges()
        {
            var emptyPairs = new List<>(int, int)>();
            for (var i = 0; i < Size; i++)
            {
                for (var j = 0; j < Size; j++)
                {
                    if (_adjacencyMatrix[i, j] == 0 && i != j && !emptyPairs.Contains((j, i)))
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
                if (_adjacencyMatrix[index, i] == 1) degree++;
            }
            return degree;
        }
        private int GetValueRound(int i, int j)
        {
            return _adjacencyMatrix[i % Size, j % Size];
        }

        #endregion
    }
}