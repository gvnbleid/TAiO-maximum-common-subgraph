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
        public int[] Matching1 { get; set; }
        public int[] Matching2 { get; set; }

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
            var (childMatching1, childMatching2) = InheritMatching(mother, father,childSize); 
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

            return new Graph(child)
            {
                Matching1 = childMatching1,
                Matching2 = childMatching2
            };
        }

        private static (int[] matching1, int[] matching2) InheritMatching(Graph mother, Graph father, int childSize)
        {
            var smallerParentSize = mother.Size > father.Size ? father.Size : mother.Size;
            var matching1 = new int[childSize];
            var matching2 = new int[childSize];
            for (var i = 0; i < smallerParentSize; i++)
            {
                if (mother.Matching1[i] == father.Matching1[i])
                {
                    matching1[i] = mother.Matching1[i];
                }
                else
                {
                    matching1[i] = GoodRandom.Bool() ? mother.Matching1[i] : father.Matching1[i];
                }

                if (mother.Matching2[i] == father.Matching2[i])
                {
                    matching2[i] = mother.Matching2[i];
                }
                else
                {
                    matching2[i] = GoodRandom.Bool() ? mother.Matching2[i] : father.Matching2[i];
                }
            }

            for (var i = smallerParentSize; i < childSize; i++)
            {
                if (mother.Size > father.Size)
                {
                    matching1[i] = mother.Matching1[i];
                    matching2[i] = mother.Matching2[i];
                }
                else
                {
                    matching1[i] = father.Matching1[i];
                    matching2[i] = father.Matching2[i];
                }
            }

            return (matching1, matching2);
        }

        #endregion

        #region PublicMethods

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

        public void Mutate()
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
                var newSize = Size + vertexChange;
                if (newSize > 0)
                {
                    if (newSize > Size)
                    {
                        AddRandomVertex(vertexChange);
                    }
                    else
                    {
                        RemoveRandomVertex(-vertexChange);
                    }

                }
            }

            if (edgeChange != 0)
            {
                if (edgeChange > 0)
                {
                    AddRandomEdges(edgeChange);
                }
                else
                {
                    RemoveRandomEdges(-edgeChange);
                }
            }
        }

        public int NumberOfUnconnectedSubgraphsInMatching1(Graph target,out bool unableToCalculate)
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

        public int NumberOfUnconnectedSubgraphsInMatching2(Graph target,out bool unableToCalculate)
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
                    try{
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

        //public int NumberOfUnconnectedSubgraphsInMatching(Graph match)
        //{
        //    var isolatedVertices = new List<int>();
        //    if (match.Size > Size) return int.MaxValue;
        //    if (!(AdjacencyMatrix.Clone() is int[,] matrixWithoutMatch))
        //    {
        //        throw new InvalidCastException();
        //    }
        //    for (var i = 0; i < match.Size; i++)
        //    {
        //        for (var j = 0; j < match.Size; j++)
        //        {
        //            if (match[i, j] == 1) matrixWithoutMatch[i, j] = 0;
        //        }
        //    }

        //    var edges = new List<(int from, int to)>();
        //    for (var i = 0; i < Size; i++)
        //    {
        //        isolatedVertices.Add(i);
        //        for (var j = 0; j < Size; j++)
        //        {
        //            if (matrixWithoutMatch[i, j] == 1 && !edges.Contains((j, i)))
        //            {
        //                edges.Add((i, j));
        //            }
        //        }
        //    }


        //    if (edges.Count == 0) return 0;
        //    var visited = new bool[Size];
        //    var queue = new Queue<int>();
        //    var subgraphsCount = 0;
        //    while (visited.Contains(false))
        //    {
        //        for (var i = 0; i < Size; i++)
        //        {
        //            if (visited[i] == false)
        //            {
        //                queue.Enqueue(i);
        //                break;
        //            }
        //        }
        //        subgraphsCount++;
        //        while (queue.Count > 0)
        //        {
        //            var p = queue.Dequeue();
        //            if (!visited[p])
        //            {
        //                visited[p] = true;
        //                foreach (var edge in edges.FindAll(e => e.from == p || e.to == p))
        //                {
        //                    if (edge.from == p && !visited[edge.to])
        //                    {
        //                        queue.Enqueue(edge.to);
        //                    }
        //                    else if (!visited[edge.from])
        //                    {
        //                        queue.Enqueue(edge.from);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    foreach (var edge in edges)
        //    {
        //        isolatedVertices.Remove(edge.from);
        //        isolatedVertices.Remove(edge.to);
        //    }

        //    return subgraphsCount-isolatedVertices.Count;
        //}

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

        private void AddRandomEdges(int numberOfEdgesToAdd)
        {
            var possibleEdges = GetPossibleEdges();
            if (possibleEdges.Count > 0)
            {
                for (var i = 0; i < numberOfEdgesToAdd; i++)
                {
                    var edge = possibleEdges[GoodRandom.Next(possibleEdges.Count)];
                    AdjacencyMatrix[edge.from, edge.to] = 1;
                    AdjacencyMatrix[edge.to, edge.from] = 1;
                    possibleEdges.Remove(edge);
                    if (possibleEdges.Count <= 0) break;
                }
            }
        }
        private void RemoveRandomEdges(int numberOfEdgesToRemove)
        {
            var edges = GetEdges();
            if (edges.Count > 0)
            {
                for (var i = 0; i < numberOfEdgesToRemove; i++)
                {
                    var edge = edges[GoodRandom.Next(edges.Count)];
                    AdjacencyMatrix[edge.from, edge.to] = 0;
                    AdjacencyMatrix[edge.to, edge.from] = 0;
                    edges.Remove(edge);
                    if (edges.Count <= 0) break;
                }
            }
        }
        private void AddRandomVertex(int numberOfVerticesToAdd)
        {
            var newSize = Size + numberOfVerticesToAdd;
            var newMatrix = new int[newSize, newSize];
            var newMatching1 = new int[newSize];
            var newMatching2 = new int[newSize];
            for (var i = 0; i < Size; i++)
            {
                newMatching1[i] = Matching1[i];
                newMatching2[i] = Matching2[i];
                for (var j = 0; j < Size; j++)
                {
                    newMatrix[i, j] = AdjacencyMatrix[i, j];
                }
            }

            Matching1 = newMatching1;
            Matching2 = newMatching2;
            AdjacencyMatrix = newMatrix;
        }
        private void RemoveRandomVertex(int numberOfVerticesToDelete)
        {
            int[,] newMatrix = null;
            var newSize = Size - numberOfVerticesToDelete;
            var newMatching1 = new int[newSize];
            var newMatching2 = new int[newSize];
            var sortedVertices = SortVerticesBasedOnDegree();
            var verticesToDelete = sortedVertices
                .Take(numberOfVerticesToDelete).ToList();
            verticesToDelete.Sort((v1, v2) => -v1.CompareTo(v2));
            foreach (var vertex in verticesToDelete)
            {
                newMatrix = DeleteVertex(AdjacencyMatrix, vertex);
            }

            for (var i = 0; i < newSize; i++)
            {
                newMatching1[i] = Matching1[i];
                newMatching2[i] = Matching2[i];
            }

            Matching1 = newMatching1;
            Matching2 = newMatching2;
            AdjacencyMatrix = newMatrix;
        }
        private static int[,] DeleteVertex(int[,] matrix, int index)
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
