using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreLibrary;
using McGregor;

namespace TAIO_MCGREGOR
{
    class SolutionV
    {

        public static void McGregor(State s, ref State max, int option = 0)
        {
            int count = 0;
            int v1 = Program.firstNeighbour(s);
            if (v1 != -1)
                foreach (var pair in Program.nextPair(s, v1))
                {
                    if (pair == null) break;
                    //v1 = pair.Item1;
                    //Console.Write("Try pair [{0},{1}]", pair.Item1, pair.Item2);
                    if (isFeasiblePair(s, pair, ref count)) //positive count guarantees cohesion
                    {
                        //Console.WriteLine(" correct! {0}", s.correspondingVerticles.Count - s.countOfNullNodes);
                        s.AddNewPair(pair.Item1, pair.Item2, count);
                        if (option == 1)
                        {
                            checkMaxVE(s, ref max);
                            if (!Program.LeafOfSearchTree(s))
                                McGregor(s, ref max, option);
                        }
                        else
                        {
                            checkMax(s, ref max);
                            if (!Program.LeafOfSearchTree(s) && !PruningCondition(s, max))
                                McGregor(s, ref max);
                        }
                            s.Backtrack(count);
                    }
                    count = 0;
                }
            //case with null node
            s.AddNewPair(v1, -1, 0);
            if (!Program.LeafOfSearchTree(s))
                if(option == 1 || !PruningCondition(s, max))
                McGregor(s, ref max, option);
            s.Backtrack(0);
        }
        private static bool isFeasiblePair(State s, Tuple<int, int> pair, ref int countOfEdges)
        {
            int count = 0;
            List<(Edge edge1, Edge edge2)> listOfEdges = new List<(Edge edge1, Edge edge2)>();
            foreach ((int v1, int v2) el in s.correspondingVerticles)
                if (el.Item2 != -1)
                {
                    if (s.G1[el.Item1, pair.Item1] != 0 ^ s.G2[el.Item2, pair.Item2] != 0)
                        return false;
                    else
                    {
                        if (s.G1[el.Item1, pair.Item1] == 1)
                        {
                            listOfEdges.Add((new Edge(el.Item1, pair.Item1), new Edge(el.Item2, pair.Item2)));
                            count++;
                        }
                    }
                }
            
            foreach (var el in listOfEdges)
                s.correspondingEdges.Add(el);
            countOfEdges = count;
            return true;
        }
        private static void checkMax(State s, ref State max)
        {
            if (s.correspondingVerticles.Count - s.countOfNullNodes > max.correspondingVerticles.Count - max.countOfNullNodes)
                max = s.Copy();
        }
        private static void checkMaxVE(State s, ref State max)
        {
            if (s.correspondingVerticles.Count - s.countOfNullNodes + s.correspondingEdges.Count > max.correspondingVerticles.Count - max.countOfNullNodes + max.correspondingEdges.Count)
                max = s.Copy();
        }
        private static bool PruningCondition(State s, State max)
        {
            int limit = s.G1.GetLength(0);
            return limit - s.countOfNullNodes <= max.correspondingVerticles.Count - max.countOfNullNodes;
        }
    }
}
