using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAIO_MCGREGOR
{
    class State
    {
        public int countOfNullNodes;
        public List<Tuple<int, int>> correspondingVerticles = new List<Tuple<int, int>>();
        public List<Tuple<Edge, Edge>> correspondingEdges = new List<Tuple<Edge, Edge>>();
        public int countOfEdges;
        public int countOfTrackedEdges = 0;
        public void Backtrack(int count)
        {
            if(correspondingVerticles.Count > 0)
            {
                if (correspondingVerticles.Last().Item2 == -1)
                    countOfNullNodes--;
                else
                {
                    if(correspondingEdges.Count > 0)
                    for (int i = 0; i < count; i++)
                        correspondingEdges.Remove(correspondingEdges.Last());
                }
                correspondingVerticles.Remove(correspondingVerticles.Last());
                countOfEdges -= count;
            }
        }
        public State Copy()
        {
            State s = new State();
            s.correspondingVerticles = new List<Tuple<int, int>>(correspondingVerticles);
            s.countOfEdges = countOfEdges;
            s.countOfNullNodes = countOfNullNodes;
            s.correspondingEdges = new List<Tuple<Edge, Edge>>(correspondingEdges);
            s.countOfTrackedEdges = countOfTrackedEdges;
            return s;
        }
        public void AddNewPair(int n1, int n2, int edgesToAdd)
        {
            if (n2 == -1) countOfNullNodes++;
            countOfEdges += edgesToAdd;
            correspondingVerticles.Add(new Tuple<int, int>(n1, n2));
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(String.Format("Count of Verticles = {0}", correspondingVerticles.Count - countOfNullNodes));
            sb.AppendLine(String.Format("Count of Edges = {0}", countOfEdges));
            foreach (var el in correspondingVerticles)
                sb.Append(String.Format("[{0}, {1}]", el.Item1, el.Item2));
            sb.AppendLine("");
            foreach (var el in correspondingEdges)
                sb.AppendLine(el.Item1 + " - " + el.Item2);
            return sb.ToString();
        }
    }
}
