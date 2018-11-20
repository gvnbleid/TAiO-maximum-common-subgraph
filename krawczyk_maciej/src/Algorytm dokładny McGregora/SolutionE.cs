using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAIO_MCGREGOR
{
    class SolutionE
    {   
        public static void McGregor(State s, int[,] G1, int[,] G2, ref State max)
        {
            int count = 0;
            int v1 = Program.firstNeighbour(s, G1);
            if(v1 != -1)
                foreach (var pair in Program.nextPair(s, v1, G2))
                {
                    if (pair == null) break;
                    v1 = pair.Item1;
                    //Console.Write("Try pair [{0},{1}]", pair.Item1, pair.Item2);
                    if (isFeasiblePair(s, pair, G1, G2, ref count)) //positive count guarantees cohesion
                    {
                        //Console.WriteLine(" correct! {0}", s.correspondingVerticles.Count - s.countOfNullNodes);
                        s.AddNewPair(pair.Item1, pair.Item2, count);
                        checkMax(s, ref max);
                        if (!Program.LeafOfSearchTree(s, G1.GetLength(0)) /*&& !PruningCondition(s, max, maxScore)*/)
                            McGregor(s, G1, G2, ref max);
                        s.Backtrack(count);
                    }
                    else
                        Console.WriteLine("");
                    count = 0;
                }
            //case with null node
            s.AddNewPair(v1, -1, 0);
            if (!Program.LeafOfSearchTree(s, G1.GetLength(0)))
                McGregor(s, G1, G2, ref max);
            s.Backtrack(0);
        }
        private static bool isFeasiblePair(State s, Tuple<int, int> pair, int[,] G1, int[,] G2, ref int countOfEdges)
        {
            int count = 0;
            List<Tuple<Edge, Edge>> listOfEdges = new List<Tuple<Edge, Edge>>();
            //if (pair.Item2 != -1)
            //{
            foreach (Tuple<int, int> el in s.correspondingVerticles)
                if (el.Item2 != -1)
                {
                    if (G1[el.Item1, pair.Item1] != 0 ^ G2[el.Item2, pair.Item2] != 0)
                        continue; //do not return false, try to add all edges
                    else
                    {
                        if (G1[el.Item1, pair.Item1] == 1)
                        {
                            //we can immediately add to State s
                            s.correspondingEdges.Add(new Tuple<Edge, Edge>(new Edge(el.Item1, pair.Item1), new Edge(el.Item2, pair.Item2)));
                            count++;
                        }
                    }
                    s.countOfTrackedEdges++;
                }
            countOfEdges = count;
            return true;
        }
        private static void checkMax(State s, ref State max)
        {
            if (s.correspondingVerticles.Count - s.countOfNullNodes + s.correspondingEdges.Count 
                > max.correspondingVerticles.Count - max.countOfNullNodes + max.correspondingEdges.Count)
                max = s.Copy();
        }
    }
}
