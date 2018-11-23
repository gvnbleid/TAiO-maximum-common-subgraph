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
            if (args.Length != 2)
            {
                throw new ArgumentException("Wrong number of arguments! Please specify two paths for the first and second input graphs");
            }

            var graphA = GraphLoader.LoadGraph(args[0]);
            var graphB = GraphLoader.LoadGraph(args[1]);
            
            Console.WriteLine("Graph A edges: " + graphA);
            Console.WriteLine("Graph B edges: " + graphB);
            Console.WriteLine();

            var modularGraph = new ModularGraph(graphA, graphB);
            var vertexModeResult = modularGraph.LargestCliqueHeuristic(true);
            Console.WriteLine("Vertices count maximum subgraph:");
            Console.WriteLine(vertexModeResult.ConvertToString());
            Console.WriteLine();
            var sumModeResult = modularGraph.LargestCliqueHeuristic(false);
            Console.WriteLine("Vertices and edges sum maximum subgraph:");
            Console.WriteLine(sumModeResult.ConvertToString());
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
