using System;
using System.Collections.Generic;
using System.IO;

namespace CliqueHeuristic
{
    public class GraphReader
    {
        public static void readArgs(string[] args, out bool[,] G1, out bool[,] G2)
        {
            if (args.Length == 2)
            {
                if (File.Exists(args[0]) && File.Exists(args[1]))
                {
                    loadGraph(args[0], out G1);
                    loadGraph(args[1], out G2);
                    makeG1Smaller(ref G1, ref G2);
                    return;
                }
            }
            throw new Exception();
        }
        private static void loadGraph(string path, out bool[,] G)
        {
            using (var reader = new StreamReader(@path))
            {
                List<string[]> list = new List<string[]>();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    list.Add(values);
                }
                if (list.Count > 0)
                {
                    G = new bool[list.Count, list.Count];
                    for (int i = 0; i < list.Count; i++)
                        for (int j = 0; j < list.Count; j++)
                            if (!bool.TryParse(list[i][j], out G[i, j]))
                                throw new Exception();
                }
                else
                    throw new Exception();
            }
        }

        private static void makeG1Smaller(ref bool[,] G1, ref bool[,] G2)
        {
            if (G1.Length <= G2.Length) return;
            int[,] tmp = G2.Clone() as int[,];
            G2 = G1.Clone() as bool[,];
            G1 = tmp.Clone() as bool[,];
        }
    }
}
