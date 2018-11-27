using System;
using System.Collections.Generic;
using System.Linq;
using CoreLib;

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
            
            Console.WriteLine();

            var modularGraph = new ModularGraph(graphA, graphB);
            var result = modularGraph.LargestCliqueHeuristic(args[2] == "1");
            var edgesA = graphA.Subgraph(result.Item1).Edges;
            var edgesB = graphB.Subgraph(result.Item2).Edges;
            GraphLoader.WriteSummary(graphA, graphB, edgesA, edgesB, result.Item1.Count);
        }
    }
}
