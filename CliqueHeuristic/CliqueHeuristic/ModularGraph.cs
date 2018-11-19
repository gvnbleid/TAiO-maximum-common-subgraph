using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Priority_Queue;

namespace CliqueHeuristic
{
    public class ModularGraph : Graph
    {
        private readonly Graph _g, _h;
        public ModularGraph(Graph g, Graph h) : base(g.VertexCount * h.VertexCount)
        {
            _g = g;
            _h = h;
            for (var i = 0; i < VertexCount; i++)
                for (var j = i; j < VertexCount; j++)
                    if (FirstGraphVertex(i) != FirstGraphVertex(j) &&
                        SecondGraphVertex(i) != SecondGraphVertex(j) &&
                        ((g.IsEdge(FirstGraphVertex(i), FirstGraphVertex(j)) && h.IsEdge(SecondGraphVertex(i), SecondGraphVertex(j))) ||
                         (!g.IsEdge(FirstGraphVertex(i), FirstGraphVertex(j)) && !h.IsEdge(SecondGraphVertex(i), SecondGraphVertex(j)))))
                    {
                        AddEdge(i, j);
                    }
            // TODO: remove unnecessary check
            if (!IsCorrect())
                throw new Exception("Invalid graph state");
        }
        private int FirstGraphVertex(int v)
        {
            return v/_h.VertexCount;
        }
        private int SecondGraphVertex(int v)
        {
            return v%_h.VertexCount;
        }

        private int[] VertexDegrees()
        {
            var vertexDegrees = new int[VertexCount];
            for (var i = 0; i < vertexDegrees.Length; i++)
                for (var j = 0; j < vertexDegrees.Length; j++)
                    if (IsEdge(i, j))
                        vertexDegrees[i]++;
            return vertexDegrees;
        }
        private class CliqueNode : FastPriorityQueueNode
        {
            public int CliqueNumber { get; }
            public int VertexSmallestDegree { get; set; }
            public HashSet<int> Vertices = new HashSet<int>();
            public CliqueNode(int v, int vertexSmallestDegree)
            {
                CliqueNumber = v;
                VertexSmallestDegree = vertexSmallestDegree;
                Vertices.Add(v);
            }
        }
        public Tuple<HashSet<int>, HashSet<int>> LargestCliqueHeuristic(bool modeVerticesOnly)
        {
            var vertexDegrees = VertexDegrees();
            var list = new LinkedList<CliqueNode>();
            var queue = new FastPriorityQueue<CliqueNode>(VertexCount);
            for (var i = 0; i < vertexDegrees.Length; i++)
            {
                var cliqueNode = new CliqueNode(i, vertexDegrees[i]);
                list.AddLast(cliqueNode);
                queue.Enqueue(cliqueNode, cliqueNode.VertexSmallestDegree);
            }
            var unmergable = new bool[VertexCount, VertexCount];
            while (true)
            {
                CliqueNode node1 = queue.Dequeue(), node2 = null;
                var unmergableWithNode1 = new LinkedList<CliqueNode>();
                do
                {
                    if (node2 != null)
                        unmergableWithNode1.AddLast(node2);
                    if (queue.Count <= 0)
                    {
                        // node1 is no more mergable with any of the elements in the queue
                        foreach (var node in unmergableWithNode1)
                            queue.Enqueue(node, node.VertexSmallestDegree);
                        node1 = queue.Dequeue();
                    }
                    node2 = queue.Dequeue();
                } while (unmergable[node1.CliqueNumber, node2.CliqueNumber]);
                foreach (var node in unmergableWithNode1)
                    queue.Enqueue(node, node.VertexSmallestDegree);
                var mergable = true;
                foreach (var node1Vertex in node1.Vertices)
                {
                    foreach (var node2Vertex in node2.Vertices)
                        if (!IsEdge(node1Vertex, node2Vertex))
                        {
                            queue.Enqueue(node1, node1.VertexSmallestDegree);
                            queue.Enqueue(node2, node2.VertexSmallestDegree);
                            unmergable[node1.CliqueNumber, node2.CliqueNumber] =
                                unmergable[node2.CliqueNumber, node1.CliqueNumber] = true;
                            mergable = false;
                            break;
                        }
                    if (!mergable)
                        break;
                }
                if (mergable)
                {
                    node1.Vertices.UnionWith(node2.Vertices);
                    node1.VertexSmallestDegree = Math.Min(node1.VertexSmallestDegree, node2.VertexSmallestDegree);
                    for(var i = 0; i < VertexCount; i++)
                        unmergable[node1.CliqueNumber, i] |= unmergable[node2.CliqueNumber, i];
                    queue.Enqueue(node1, node1.VertexSmallestDegree);
                }
                break;
            }
            HashSet<int> clique = null;
            if (modeVerticesOnly)
            {
                foreach (var cliqueNode in list)
                    if (clique == null || cliqueNode.Vertices.Count > clique.Count)
                        clique = cliqueNode.Vertices;
            }
            else
            {
                var gEdgeCount = 0;
                foreach (var cliqueNode in list)
                    if (clique == null)
                        clique = cliqueNode.Vertices;
                    else
                    {
                        var newGVertices = new HashSet<int>(cliqueNode.Vertices.Select(FirstGraphVertex));
                        var newGEdgeCount = _g.Subgraph(newGVertices).EdgeCount;
                        if (newGVertices.Count + newGEdgeCount > clique.Count + gEdgeCount)
                        {
                            clique = cliqueNode.Vertices;
                            gEdgeCount = newGEdgeCount;
                        }
                    }
            }
            var gVertices = new HashSet<int>(clique?.Select(FirstGraphVertex));
            var hVertices = new HashSet<int>(clique?.Select(SecondGraphVertex));
            return new Tuple<HashSet<int>, HashSet<int>>(gVertices, hVertices);
        }
    }
}
