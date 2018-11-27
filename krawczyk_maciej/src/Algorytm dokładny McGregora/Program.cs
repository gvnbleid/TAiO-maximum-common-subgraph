using System;
using System.Collections.Generic;
using System.Linq;
using CoreLib;
using TAIO_MCGREGOR;

namespace McGregor
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime dt = DateTime.Now;
            if (args.Length != 3 || (args[2] != "V" && args[2] != "E"))
            {
                throw new ArgumentException("Invalid arguments!");
            }

            var G1 = GraphLoader.LoadGraph(args[0]);
            var G2 = GraphLoader.LoadGraph(args[1]);
            State s = new State(G1.AdjacencyMatrix, G2.AdjacencyMatrix);
            
            if (args[2] == "V")
            {
                Console.Write("V Solution\n");
                SolutionV.McGregor(new State(G1.AdjacencyMatrix, G2.AdjacencyMatrix), ref s);              
            }
            else
            {
                Console.Write("V+E Solution\n");
                SolutionV.McGregor(new State(G1.AdjacencyMatrix, G2.AdjacencyMatrix), ref s, 1);
            }

            //List<Edge> edges = s.correspondingEdges.Select(x => x.Item1).ToList();
            GraphLoader.WriteSummary(G1, G2, s.correspondingEdges,
                s.correspondingVerticles.Count(x => x.v1 != -1 && x.v2 != -1));

        }

        public static bool LeafOfSearchTree(State s)
        {
            int limit = s.G1.GetLength(0);
            return s.correspondingVerticles.Count >= limit;
        }
        
        public static int firstNeighbour(State s)
        {
            int v1 = -1;
            bool selected = false;
            bool contains = false;
            if (s.correspondingVerticles.Count - s.countOfNullNodes != 0)
            {
                //wez sasiada pierwszego lepszego
                foreach (var el in s.correspondingVerticles)
                {
                    if (el.Item2 == -1) continue;
                    for (int i = 0; i < s.G1.GetLength(0); i++)
                    {
                        if (s.G1[i, el.Item1] == 1)
                        {
                            foreach (var el2 in s.correspondingVerticles)
                                if (el2.Item1 == i)
                                {
                                    contains = true;
                                    break;
                                }
                            if (!contains)
                            {
                                selected = true;
                            }
                            contains = false;
                        }
                        if (selected)
                        {
                            v1 = i;
                            break;
                        }
                    }
                    if (selected)
                        break;
                }
            }
            else
            {

                for (int i = 0; i < s.G1.GetLength(0); i++)
                {
                    foreach (var el in s.correspondingVerticles)
                        if (el.Item1 == i)
                        {
                            contains = true;
                            break;
                        }
                    if(!contains)
                    {
                        v1 = i;
                        break;
                    }
                    contains = false;
                }
            }
            return v1;
        }
        public static IEnumerable<Tuple<int,int>> nextPair(State s, int v1)
        {
            
            bool used = false;
            for(int i=0;i<s.G2.GetLength(0);i++)
            {
                foreach (var el in s.correspondingVerticles)
                        if (el.Item2 == i) //used
                        {
                            used = true;
                            break;
                        }
                    if (!used)
                    {
                        yield return new Tuple<int, int>(v1, i);
                    }
                used = false;
                
            }
            yield return null;
        }
        
        private static int getMaxScore(int [,] G1, int [,] G2)
        {
            int count = 0, count2 = 0;
            for(int i=1;i<G1.GetLength(0);i++)
                for(int j=0;j<i;j++)
                {
                    if (G1[i, j] == 1)
                        count++;
                }
            for (int i = 1; i < G2.GetLength(0); i++)
                for (int j = 0; j < i; j++)
                {
                    if (G2[i, j] == 1)
                        count2++;
                }
            return Math.Max(count + G1.GetLength(0), count2 + G2.GetLength(0));
        }
        
    }
}
