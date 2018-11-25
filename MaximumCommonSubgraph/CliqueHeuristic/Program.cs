using System;
using System.Collections.Generic;
using System.Linq;
using CoreLibrary;

namespace CliqueHeuristic
{
    public class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 3 || (args[2] != "V" && args[2] != "E"))
            {
                throw new ArgumentException("Invalid arguments!");
            }

            var graphA = GraphLoader.LoadGraph(args[0]);
            var graphB = GraphLoader.LoadGraph(args[1]);
            
            //Console.WriteLine("Graph A edges: " + graphA);
            //Console.WriteLine("Graph B edges: " + graphB);
            Console.WriteLine();

            var modularGraph = new ModularGraph(graphA, graphB);
            var result = modularGraph.LargestCliqueHeuristic(args[2] == "1");
            var edgesA = graphA.Subgraph(result.Item1).Edges;
            var edgesB = graphB.Subgraph(result.Item2).Edges;
            GraphLoader.WriteSummary(graphA, graphB, edgesA, edgesB, result.Item1.Count);
            //Console.WriteLine("Vertices count maximum subgraph:");
            //Console.WriteLine(vertexModeResult.ConvertToString());
            //Console.WriteLine();
            //var sumModeResult = modularGraph.LargestCliqueHeuristic(false);
            //Console.WriteLine("Vertices and edges sum maximum subgraph:");
            //Console.WriteLine(sumModeResult.ConvertToString());
        }
    }
    public static class TupleExtender
    {
        public static string ConvertToString(this Tuple<HashSet<int>, HashSet<int>> tuple)
        {
            var result = "Selected graph A vertices:";
            foreach (var i in tuple.Item1)
                result += " " + i;
            result += "\nSelected graph B vertices:";
            foreach (var i in tuple.Item2)
                result += " " + i;
            return result;
        }
    }
}
