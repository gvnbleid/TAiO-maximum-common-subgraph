using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace GraphGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            Random rand = new Random();
            for (int d = 4; d <= 50; d += 1)
            {
                for (int k = 0; k < 2; k++)
                {
                    //if (!int.TryParse(args[0], out int graphSize) || !double.TryParse(args[1], out double density) ||
                    //    args.Length != 3)
                    //{
                    //    throw new ArgumentException(
                    //        "Please specify args in order: [graphSize (int)] [graphDensity (decimal number from range 0-1)] [outputFileName]");
                    //}
                    int graphSize = d;
                    double density = 0.5;
                    double minDensity = (double) (graphSize - 1) / (graphSize * graphSize);
                    //if (density < minDensity || density > 1)
                    //{
                    //    throw new ArgumentException($"For given graph size density has to be between {minDensity} and 1");
                    //}

                    int[,] graphMatrix = new int[graphSize, graphSize];

                    List<int> vertices = new List<int>();

                    int firstVertex = rand.Next(graphSize);
                    int secondVertex = rand.Next(graphSize);

                    graphMatrix[firstVertex, secondVertex] = 1;
                    graphMatrix[secondVertex, firstVertex] = 1;

                    vertices.Add(firstVertex);
                    vertices.Add(secondVertex);

                    int desiredNumberOfEdges = (int) ((graphSize - 1) * graphSize / 2 * density);
                    int currentNumberOfEdges = 1;

                    for (int i = 0; i < graphSize; i++)
                    {
                        if (vertices.Contains(i))
                        {
                            continue;
                        }

                        int oldVertex = vertices[rand.Next(vertices.Count)];
                        graphMatrix[i, oldVertex] = 1;
                        graphMatrix[oldVertex, i] = 1;
                        vertices.Add(i);

                        currentNumberOfEdges++;
                    }

                    while (currentNumberOfEdges < desiredNumberOfEdges)
                    {
                        int oldVertex = vertices[rand.Next(vertices.Count)];
                        int newVertex = rand.Next(graphSize);

                        if (oldVertex == newVertex || graphMatrix[oldVertex, newVertex] == 1)
                        {
                            continue;
                        }

                        vertices.Add(newVertex);

                        graphMatrix[oldVertex, newVertex] = 1;
                        graphMatrix[newVertex, oldVertex] = 1;

                        currentNumberOfEdges++;
                    }

                    var sb = new StringBuilder();

                    for (int i = 0; i < graphSize; i++)
                    {
                        for (int j = 0; j < graphSize; j++)
                        {
                            sb.Append(graphMatrix[i, j]);
                            if (j < graphSize - 1)
                            {
                                sb.Append(",");
                            }
                        }

                        sb.AppendLine();
                    }

                    File.WriteAllText($"graf_{d}_0,5_{k}.csv", sb.ToString());
                    Console.WriteLine($"graf_{d}_0,5_{k}.csv created");
                }
            }
        }
    }
}
