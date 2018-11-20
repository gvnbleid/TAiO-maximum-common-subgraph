using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace CliqueHeuristic
{
    public class GraphReader
    {
        public static bool tryReadArgs(string[] args, out bool[,] G1, out bool[,] G2)
        {
            G1 = null;
            G2 = null;

            if (args.Length != 2)
            {
                Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: Please specify no more and no less than 2 paths for input graphs");
                return false;
            }

            if (File.Exists(args[0]) && File.Exists(args[1]))
            {
                if (!tryLoadGraph(args[0], out G1) || !tryLoadGraph(args[1], out G2))
                {
                    return false;
                }
                makeG1Smaller(ref G1, ref G2);
                return true;
            }

            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: At least one of the paths is not valid");
            return false;
        }
        private static bool tryLoadGraph(string path, out bool[,] G)
        {
            G = null;
            try
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
                            G[i, j] = list[i][j] == "1";
                    }
                    else
                        throw new Exception($"{MethodBase.GetCurrentMethod().Name}: File is empty");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

            return true;
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
