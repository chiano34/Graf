using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Construction_15_1
{
    public partial class Form1 : Form
    {
        public Graph g = new Graph();

        public int dragVertex = -1;
        public int dragEdge = -1;

        Random random = new Random();

        public int x1 = 0;
        public int y1 = 0;
        public int x2 = 0;
        public int y2 = 0;
        int counter = 0;
        List<Graph.Node> order = new List<Graph.Node>();
        List<List<Graph.Node>> componentsList = new List<List<Graph.Node>>();
        int rad = 30;
        public bool isWorking = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            g.x = pictureBox1.Width / 2;
            g.y = pictureBox1.Height / 2;

            Bitmap buffer = new Bitmap(Width, Height);
            Graphics gfx = Graphics.FromImage(buffer);

            SolidBrush textBrush = new SolidBrush(Color.White);
            SolidBrush animBrush = new SolidBrush(Color.Black);
            Pen penNode = new Pen(Color.SteelBlue);
            Pen penEdge = new Pen(Color.Green);
            penEdge.Width = 3;

            gfx.Clear(Color.White);
            penEdge.Color = Color.Green;
            animBrush.Color = Color.Green;
            foreach (Graph.Node node in g.nodes)
            {
                foreach (int eg in node.edges)
                {
                    foreach (Graph.Node node1 in g.nodes)
                    {
                        if (g.inverted)
                        {
                            penEdge.Color = Color.Gray;
                            animBrush.Color = Color.Red;
                        }
                        else
                        {
                            penEdge.Color = Color.Gray;
                            animBrush.Color = Color.Gray;
                        }
                        if (node1.id == eg)
                        {
                            double dir = additionFunctions.point_direction(node.x, node.y, node1.x, node1.y);
                            double dist = additionFunctions.point_distance(node.x, node.y, node1.x, node1.y);
                            gfx.DrawLine(penEdge,
                                new Point(node.x + (int)additionFunctions.lengthdir_x(rad / 2, dir), node.y + (int)additionFunctions.lengthdir_y(rad / 2, dir)),
                                new Point(node.x + (int)additionFunctions.lengthdir_x(dist - (rad / 2), dir),
                                node.y + (int)additionFunctions.lengthdir_y(dist - (rad / 2), dir)));
                            gfx.FillEllipse(animBrush,
                                new Rectangle(node.x + (int)additionFunctions.lengthdir_x(dist - rad / 2, dir) - 4,
                                node.y + (int)additionFunctions.lengthdir_y(dist - (rad / 2), dir) - 4, 8, 8));
                        }
                    }
                }
            }
            foreach (Graph.Node n in g.nodes)
            {
                animBrush.Color = n.nodeColor.Color;

                gfx.FillEllipse(animBrush, new Rectangle(n.x - rad / 2, n.y - rad / 2, rad, rad));
                gfx.DrawEllipse(penNode, new Rectangle(n.x - rad / 2, n.y - rad / 2, rad, rad));
                gfx.DrawString(n.stringId, new Font("Arial", 12, FontStyle.Bold), textBrush, new PointF(n.x - rad / 3, n.y - 10));
            }
            if (dragEdge != -1)
            {
                animBrush.Color = Color.Green;
                if (checkBox2.Checked)
                {
                    penEdge.Color = Color.Red;
                    animBrush.Color = Color.Red;
                }
                double dir1 = additionFunctions.point_direction(x1, y1, x2, y2);
                double dist1 = additionFunctions.point_distance(x1, y1, x2, y2);
                gfx.DrawLine(penEdge,
                    new Point(x1 + (int)additionFunctions.lengthdir_x(rad / 2, dir1), y1 + (int)additionFunctions.lengthdir_y(rad / 2, dir1)),
                    new Point(x1 + (int)additionFunctions.lengthdir_x(dist1, dir1), y1 + (int)additionFunctions.lengthdir_y(dist1, dir1)));
                gfx.FillEllipse(animBrush,
                    new Rectangle(x1 + (int)additionFunctions.lengthdir_x(dist1, dir1) - 4, y1 + (int)additionFunctions.lengthdir_y(dist1, dir1) - 4, 8, 8));
            }

            pictureBox1.Image = buffer;
            textBrush.Dispose();
            animBrush.Dispose();
            penNode.Dispose();
            penEdge.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!isWorking)
                g.addNode();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (g.nodes.Count == 0 || isWorking)
                button3.Enabled = false;
            else button3.Enabled = true;

            if (isWorking)
            {
                button1.Enabled = false;
                button4.Enabled = false;
            }
            else
            {
                button1.Enabled = true;
                button4.Enabled = true;
            }

            for (int i = 0; i < g.nodes.Count; i++)
            {
                for (int j = 0; j < g.nodes.Count; j++)
                {
                    if (i != j)
                    {
                        double dist = additionFunctions.point_distance(g.nodes[i].x, g.nodes[i].y, g.nodes[j].x, g.nodes[j].y);
                        int sz_in = 10;
                        if (dist <= (rad + sz_in))
                        {
                            var rand = new Random();
                            if (g.nodes[i].x == g.nodes[j].x)
                            {
                                if (rand.Next(2) == 1)
                                    g.nodes[i].x += 1;
                                else
                                    g.nodes[i].x -= 1;
                            }
                            if (g.nodes[i].y == g.nodes[j].y)
                            {
                                if (rand.Next(2) == 1)
                                    g.nodes[i].y += 1;
                                else
                                    g.nodes[i].y -= 1;
                            }
                            if (g.nodes[i].x < g.nodes[j].x)
                            {
                                g.nodes[i].x -= (int)(rad + sz_in - dist);
                                g.nodes[j].x += (int)(rad + sz_in - dist);
                            }
                            else
                            {
                                g.nodes[i].x += (int)(rad + sz_in - dist);
                                g.nodes[j].x -= (int)(rad + sz_in - dist);
                            }
                            if (g.nodes[i].y < g.nodes[j].y)
                            {
                                g.nodes[i].y -= (int)(rad + sz_in - dist);
                                g.nodes[j].y += (int)(rad + sz_in - dist);
                            }
                            else
                            {
                                g.nodes[i].y += (int)(rad + sz_in - dist);
                                g.nodes[j].y -= (int)(rad + sz_in - dist);
                            }
                        }
                    }
                }
                if (g.nodes[i].x - rad / 2 < 0) g.nodes[i].x = rad / 2;
                if (g.nodes[i].y - rad / 2 < 0) g.nodes[i].y = rad / 2;
                if (g.nodes[i].x + rad / 2 > pictureBox1.Width) g.nodes[i].x = pictureBox1.Width - rad / 2 - 1;
                if (g.nodes[i].y + rad / 2 > pictureBox1.Height) g.nodes[i].y = pictureBox1.Height - rad / 2 - 1;
            }

            Refresh();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragVertex != -1)
            {
                foreach (Graph.Node n in g.nodes)
                {
                    if (dragVertex == n.id)
                    {
                        n.x = e.X;
                        n.y = e.Y;
                        break;
                    }
                }
            }
            if (dragEdge != -1)
            {
                foreach (Graph.Node n in g.nodes)
                {
                    if (dragEdge == n.id)
                    {
                        x1 = n.x;
                        y1 = n.y;
                        x2 = e.X;
                        y2 = e.Y;
                        break;
                    }
                }
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                dragEdge = -1;
                if (dragVertex == -1)
                {
                    foreach (Graph.Node n in g.nodes)
                    {
                        if (additionFunctions.point_distance(n.x, n.y, e.X, e.Y) < rad / 2)
                        {
                            dragVertex = n.id;
                            n.x = e.X;
                            n.y = e.Y;
                            break;
                        }
                    }
                }
            }
            if (!isWorking)
            {
                if (e.Button == MouseButtons.Middle)
                {
                    dragVertex = -1;
                    dragEdge = -1;
                    foreach (Graph.Node n in g.nodes)
                    {
                        if (additionFunctions.point_distance(n.x, n.y, e.X, e.Y) < rad / 2)
                        {
                            g.RemoveNode(n.id);
                            break;
                        }
                    }
                }
                if (e.Button == MouseButtons.Right)
                {
                    dragVertex = -1;
                    x1 = 0;
                    y1 = 0;
                    x2 = 0;
                    y2 = 0;
                    foreach (Graph.Node n in g.nodes)
                    {
                        if (additionFunctions.point_distance(n.x, n.y, e.X, e.Y) < rad / 2)
                        {
                            dragEdge = n.id;
                            x1 = n.x;
                            y1 = n.y;
                            x2 = e.X;
                            y2 = e.Y;
                            break;
                        }
                    }
                }
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                dragVertex = -1;
            if (e.Button == MouseButtons.Right)
            {
                if (dragEdge != -1)
                {
                    foreach (Graph.Node n in g.nodes)
                    {
                        if (additionFunctions.point_distance(n.x, n.y, e.X, e.Y) < rad / 2)
                        {
                            if (n.id != dragEdge)
                            {
                                foreach (Graph.Node m in g.nodes)
                                {
                                    if (m.id == dragEdge)
                                    {
                                        if (checkBox2.Checked)
                                        {
                                            m.removeEdge(n.id);
                                            if (!checkBox1.Checked)
                                                n.removeEdge(m.id);
                                        }
                                        else
                                        {
                                            m.addEdge(n.id);
                                            if (!checkBox1.Checked)
                                                n.addEdge(m.id);
                                        }
                                        break;
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
                dragEdge = -1;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!timer2.Enabled && !isWorking)
            {
                if (g.nodes.Count > 0)
                {
                    button2.Enabled = false;
                    button2.Text = "Выполняется...";
                    for (int i = 0; i < g.used.Count; i++)
                    {
                        g.used[i] = false;
                    }
                    counter = 0;
                    order.Clear();
                    order = new List<Graph.Node>(g.nodes.Count);
                    while (g.findUnusedVertex(g.nodes) != null)
                    {
                        Graph.Node node = g.findUnusedVertex(g.nodes);
                        int k = 0;
                        g.dfs1(node, ref k, ref order);
                    }
                    g.InvertEdges();
                    order.Reverse();
                    for (int i = 0; i < g.used.Count; i++)
                    {
                        g.used[i] = false;
                    }
                    foreach (Graph.Node n in order)
                    {
                        List<Graph.Node> components1 = new List<Graph.Node>();
                        if (!g.used[n.id])
                            g.dfs2(n, components1);
                        if (components1.Count != 0)
                            componentsList.Add(components1);

                        for (int i = 0; i < components1.Count; i++)
                        {
                            textBox2.Text = textBox2.Text + components1[i].stringId;
                        }
                        textBox2.Text = textBox2.Text;

                        timer2.Start();
                        isWorking = true;
                    }
                }
            }
            else
            {
                timer2.Stop();
                isWorking = false;
            }


        }

        private void timer2_Tick(object sender, EventArgs e)
        {

            timer2.Interval = 750;
            if (counter < componentsList.Count)
            {
                SolidBrush tmp = new SolidBrush(Color.FromArgb(random.Next(256), random.Next(256), random.Next(256)));
                for (int i = 0; i < componentsList[counter].Count; i++)
                {
                    componentsList[counter][i].nodeColor.Color = tmp.Color;
                }
                counter++;
            }
            else
            {
                
                button2.Enabled = true;
                button2.Text = "Запустить алгоритм Косарайю";
                textBox2.Text = componentsList.Count.ToString() + " компоненты сильной связности" + "  ";
                componentsList.Clear();
                g.InvertEdges();
                timer2.Stop();
                isWorking = false;
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (g.nodes.Count != 0)
                saveFileDialog1.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            if (File.Exists(saveFileDialog1.FileName))
                File.Delete(saveFileDialog1.FileName);
            FileStream fstream = File.OpenWrite(saveFileDialog1.FileName);
            foreach (Graph.Node n in g.nodes)
            {
                string str = "";
                str += n.id.ToString() + " ";
                str += n.x.ToString() + " ";
                str += n.y.ToString() + " ";
                if (n.edges.Count != 0)
                {
                    foreach (int edge in n.edges)
                    {
                        str += edge.ToString() + ";";
                    }
                    str = str.Remove(str.Length - 1, 1);
                }
                str += " " + n.stringId + "\n";
                byte[] info = new UTF8Encoding(true).GetBytes(str);
                fstream.Write(info, 0, info.Length);
            }
            fstream.SetLength(fstream.Length - 1);
            fstream.Close();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            g.nodes.Clear();
            StreamReader sr = File.OpenText(openFileDialog1.FileName);
            while (!sr.EndOfStream)
            {
                string str = sr.ReadLine();
                string[] stringSplit = str.Split(' ');
                List<int> adjency_list = new List<int>();
                if (stringSplit[3] != "")
                {
                    string[] SSE = stringSplit[3].Split(';');
                    foreach (string eg in SSE)
                        adjency_list.Add(int.Parse(eg));
                }
                g.LoadNode(int.Parse(stringSplit[0]), int.Parse(stringSplit[1]), int.Parse(stringSplit[2]), stringSplit[4], adjency_list);
            }
            sr.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}