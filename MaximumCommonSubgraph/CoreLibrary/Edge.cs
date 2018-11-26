using System;

namespace CoreLibrary
{
    public class Edge
    {
        public readonly int v1, v2;

        public Edge(int v1, int v2)
        {
            this.v1 = v1;
            this.v2 = v2;
        }
        public override string ToString()
        {
            return $"<{v1}, {v2}>";
        }
        public override bool Equals(object obj)
        {
            return ((obj as Edge)?.v1 == v1 && (obj as Edge).v2 == v2) || ((obj as Edge)?.v1 == v2 && (obj as Edge).v2 == v1);
        }
    }
}