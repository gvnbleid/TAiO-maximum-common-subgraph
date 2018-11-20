using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAIO_MCGREGOR
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime dt = DateTime.Now;
            int[,] G1 = null, G2 = null;
            int option = 1;
            if (args.Any())
                GraphReader.readArgs(args, out G1, out G2, ref option);
            else
            {
                G1 = new int[,] {
                    { 0,1,0,1,0,1,0,0 },
                    { 1,0,1,1,0,1,1,1 },
                    { 0,1,0,1,0,0,0,0 },
                    { 1,1,1,0,1,1,0,1 },
                    { 0,0,0,1,0,1,0,0 },
                    { 1,1,0,1,1,0,1,0 },
                    { 0,1,0,0,0,1,0,0 },
                    { 0,1,0,1,0,0,0,0 }};
                G2 = new int[,] {
                    { 0,1,0,1,0,0,0,0 },
                    { 1,0,1,1,0,1,1,1 },
                    { 0,1,0,1,0,0,0,0 },
                    { 1,1,1,0,1,1,0,1 },
                    { 0,0,0,1,0,1,0,0 },
                    { 0,1,0,1,1,0,1,0 },
                    { 0,1,0,0,0,1,0,0 },
                    { 0,1,0,1,0,0,0,0 } };
            }
            Console.Write(Graph.convertFromMatrix(G1));
            Console.Write(Graph.convertFromMatrix(G2));
            State s = new State();
            if(option == 1)
                SolutionV.McGregor(new State(), G1, G2, ref s);
            else if(option == 2)
                SolutionE.McGregor(new State(), G1, G2, ref s);
            Console.Write(s);

        }

        public static bool LeafOfSearchTree(State s, int limit)
        {
            return s.correspondingVerticles.Count >= limit;
        }
        
        public static int firstNeighbour(State s, int[,] G1)
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
                    for (int i = 0; i < G1.GetLength(0); i++)
                    {
                        if (G1[i, el.Item1] == 1)
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

                for (int i = 0; i < G1.GetLength(0); i++)
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
        public static IEnumerable<Tuple<int,int>> nextPair(State s, int v1, int[,] G2)
        {
            
            bool used = false;
            for(int i=0;i<G2.GetLength(0);i++)
            {
                foreach (Tuple<int, int> el in s.correspondingVerticles)
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
