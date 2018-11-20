using System;
using System.Collections.Generic;
using System.Linq;

namespace CliqueHeuristic
{
    public class Program
    {
        static void Main(string[] args)
        {
            bool[,] gA, gB;
            if (args.Any())
            {
                if (!GraphReader.tryReadArgs(args, out gA, out gB))
                {
                    return;
                }
            }
            else
            {
                gA = new[,]
                {
                    {false, true, false, false, true},
                    {true, false, true, false, false},
                    {false, true, false, true, false},
                    {false, false, true, false, true},
                    {true, false, false, true, false}
                };
                gB = new[,]
                {
                    {false, true, false, false, true},
                    {true, false, true, false, false},
                    {false, true, false, true, false},
                    {false, false, true, false, true},
                    {true, false, false, true, false}
                };
            }

            Graph graphA = new Graph(gA), graphB = new Graph(gB);
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
