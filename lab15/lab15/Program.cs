using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Construction_15_1
{
    public static class additionFunctions
    {
        public static double degToRad(double deg)
        {
            return deg * Math.PI / 180;
        }

        public static double radToDeg(double rad)
        {
            return rad / Math.PI * 180;
        }

        public static double lengthdir_x(double len, double dir)
        {
            return len * Math.Cos(degToRad(dir));
        }

        public static double lengthdir_y(double len, double dir)
        {
            return len * Math.Sin(degToRad(dir)) * (-1);
        }

        public static double point_direction(int x1, int y1, int x2, int y2)
        {
            return 180 - radToDeg(Math.Atan2(y1 - y2, x1 - x2));
        }

        public static double point_distance(int x1, int y1, int x2, int y2)
        {
            return Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
        }
    }



    public class Graph
    {
        public class Node
        {
            public int id;
            public int active;
            public int x;
            public int y;
            public string stringId;
            public int mark = 0;
            public List<int> edges;
            public SolidBrush nodeColor = new SolidBrush(Color.Black);

            public void addEdge(int id)
            {
                if (!edges.Contains(id)) edges.Add(id);
            }

            public void removeEdge(int id)
            {
                edges.Remove(id);
            }
        };

        public List<Node> nodes = new List<Node>();
        public List<bool> used = new List<bool>();
        private int maxId = 0;
        public int x = 0;
        public int y = 0;
        public bool inverted = false;
        public void addNode()
        {
            bool find = false;
            int id = 0;
            for (int i = 0; i < maxId; i++)
            {
                bool exist = false;
                foreach (Node nd in nodes)
                {
                    if (nd.id == i)
                    {
                        exist = true;
                        break;
                    }
                }
                if (!exist)
                {
                    id = i;
                    find = true;
                    break;
                }
            }
            if (!find)
            {
                id = maxId;
                maxId++;
            }
            Node n = new Node();
            n.id = id;
            n.active = 0;
            n.x = x;
            n.y = y;
            n.stringId = id.ToString();
            n.mark = 0;
            n.edges = new List<int>();
            nodes.Add(n);

            used.Add(false);
            nodes.Sort((x, y) => x.id.CompareTo(y.id));
        }

        public void RemoveNode(int id)
        {
            Node n = null;
            foreach (Node nd in nodes)
            {
                nd.edges.Remove(id);
                if (nd.id == id)
                {
                    n = nd;
                }
            }
            nodes.Remove(n);
        }

        public void LoadNode(int id, int x, int y, string name, List<int> e)
        {
            Node n = new Node();
            if (maxId <= id)
                maxId = id + 1;
            n.id = id;
            n.active = 0;
            n.x = x;
            n.y = y;
            n.stringId = id.ToString();
            n.edges = e;
            nodes.Add(n);
            used.Add(false);
            n.mark = 0;
        }

        public void InvertEdges()
        {
            inverted = !inverted;
            int[,] matrix = new int[this.nodes.Count, this.nodes.Count];
            for (int i = 0; i < this.nodes.Count; i++)
                for (int j = 0; j < this.nodes.Count; j++)
                    matrix[i, j] = 0;

            foreach (Node n in this.nodes)
            {
                for (int i = 0; i < n.edges.Count; i++)
                    matrix[n.id, n.edges[i]] = 1;
            }

            foreach (Node n in this.nodes)
            {
                n.edges.Clear();
                n.edges.Capacity = nodes.Count;
            }
            foreach (Node n in nodes)
            {
                for (int j = 0; j < nodes.Count; j++)
                {
                    if (matrix[j, n.id] == 1) n.edges.Add(j);
                }
            }

        }

        public Node findUnusedVertex(List<Node> list)
        {
            foreach (Node n in list)
            {
                if (!used[list.IndexOf(n)])
                    return n;
            }
            return null;
        }

        public void dfs1(Node v, ref int timer, ref List<Node> order)
        {
            used[nodes.IndexOf(v)] = true;
            for (int i = 0; i < v.edges.Count; i++)
            {
                if (!used[v.edges[i]])
                {
                    dfs1(nodes[v.edges[i]], ref timer, ref order);
                }
            }
            order.Add(v);
            timer++;
            v.mark = timer;
        }


        public void dfs2(Node v, List<Node> components)
        {
            used[nodes.IndexOf(v)] = true;
            components.Add(v);
            for (int i = 0; i < v.edges.Count; i++)
            {
                if (!used[v.edges[i]])
                {
                    dfs2(nodes[v.edges[i]], components);
                }
            }
        }

    }

    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
