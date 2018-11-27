using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GraphVizWrapper;
using GraphVizWrapper.Commands;
using GraphVizWrapper.Queries;

namespace CoreLib
{
    public static class GraphLoader
    {
        public static Graph LoadGraph(string pathToFile)
        {
            if (!File.Exists(pathToFile))
            {
                throw new FileNotFoundException(pathToFile);
            }
            using (var reader = new StreamReader(pathToFile))
            {
                var lines = new List<string[]>();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    lines.Add(values);
                }
                if (lines.Count > 0)
                {
                    var size = lines.Count;
                    var matrix = new int[size, size];
                    for (var i = 0; i < size; i++)
                    {
                        for (var j = 0; j < size; j++)
                        {
                            if (int.TryParse(lines[i][j], out var value))
                            {
                                if (value == 0 || value == 1)
                                {
                                    matrix[i, j] = value;
                                }
                                else
                                {
                                    throw new ArgumentOutOfRangeException("Possible values are 0 or 1");
                                }
                            }
                            else
                            {
                                throw new InvalidDataException($"Could not parse cell [{i},{j}]");
                            }

                        }
                    }
                    return new Graph(matrix);
                }
                else
                {
                    throw new Exception("Empty file");
                }
            }
        }

        public static void WriteSummary(Graph gA, Graph gB, List<(Edge edge1, Edge edge2)> matching, int matchingSize)
        {
            List<Edge> edgesFromFirstGraph = matching.Select(x => x.edge1).ToList();
            List<Edge> edgesFromSecondGraph = matching.Select(x => x.edge2).ToList();
            Graph foundGraph = GraphExtensions.ConstructGraphFromEdges(matchingSize, edgesFromFirstGraph);
            Console.WriteLine("***** First input graph *****");
            gA.PrintToConsole(edgesFromFirstGraph);
            Console.WriteLine("***** Second input graph *****:");
            gB.PrintToConsole(edgesFromSecondGraph);
            Console.WriteLine("***** Found maximum common subgraph *****");
            Console.WriteLine(foundGraph);

            var wrapper = InitializeGraphGeneration();
            SaveGraphAsImage(wrapper, gA, "firstGraph", edgesFromFirstGraph);
            SaveGraphAsImage(wrapper, gB, "secondGraph", edgesFromSecondGraph);
            SaveGraphAsImage(wrapper, foundGraph, "result");
        }

        public static void WriteSummary(Graph gA, Graph gB, List<Edge> edgesA, List<Edge> edgesB, int matchingSize)
        {
            if (edgesA.Count != edgesB.Count)
            {
                throw new ArgumentException();
            }

            var zippedLists = edgesA.Zip(edgesB, (x, y) => (x, y)).ToList();
            WriteSummary(gA, gB, zippedLists, matchingSize);
        }

        private static GraphGeneration InitializeGraphGeneration()
        {
            var getStartProcessQuery = new GetStartProcessQuery();
            var getProcessStartInfoQuery = new GetProcessStartInfoQuery();
            var registerLayoutPluginCommand = new RegisterLayoutPluginCommand(getProcessStartInfoQuery, getStartProcessQuery);

            return new GraphGeneration(getStartProcessQuery,
                getProcessStartInfoQuery,
                registerLayoutPluginCommand);
        }

        private static void SaveGraphAsImage(GraphGeneration wrapper,  Graph g, string name, List<Edge> edgesToBeMarked = null)
        {
            string dotLanguage = ConvertToDotLanguage(g, name, edgesToBeMarked);

            byte[] output = wrapper.GenerateGraph(dotLanguage, Enums.GraphReturnType.Png);

            File.WriteAllBytes($"{name}.png", output);
        }

        private static string ConvertToDotLanguage(Graph g, string name, List<Edge> edgesToBeMarked = null)
        {
            var sb = new StringBuilder();
            sb.Append("graph");
            sb.Append("{");
            foreach (var edge in g.Edges)
            {
                if (edgesToBeMarked != null && edgesToBeMarked.Contains(edge))
                {
                    sb.Append($"{edge.v1} -- {edge.v2} [color = \"red\"]; ");
                }
                else
                {
                    sb.Append($"{edge.v1} -- {edge.v2}; ");
                }
            }

            sb.Append("}");

            return sb.ToString();
        }

    }
}
