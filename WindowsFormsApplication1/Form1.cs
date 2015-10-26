using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public class Point
    {
        double x, y;
        public Point(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
        public Point()
        {
            this.x = 0;
            this.y = 0;
        }
        public double getX()
        {
            return this.x;
        }
        public double getY()
        {
            return this.y;
        }
        public void setX(double x)
        {
            this.x = x;
        }
        public void setY(double y)
        {
            this.y = y;
        }
    }
    public partial class Form1 : Form
    {
        int numClicks;
        int[,] map = new int[256, 256];
        int[,] parx = new int[256, 256];
        int[,] pary = new int[256, 256];
        List<Point> dlaPoints;

        public Form1()
        {
            InitializeComponent();
            numClicks = 0;
            dlaPoints = new List<Point>();
        }

        //Random noise generator
        private void button1_Click(object sender, EventArgs e)
        {
            Bitmap b = new Bitmap(256, 256);
            Random r = new Random();
            for (int i = 0; i <= 255; i++)
            {
                for(int j = 0; j <= 255; j++)
                {
                    int v = r.Next(0, 255);
                    b.SetPixel(i, j, Color.FromArgb(v, v, v));
                }
            }
            pictureBox1.Image = (Image)b;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Bitmap b = new Bitmap(pictureBox1.Image);
            Bitmap nb = new Bitmap(256, 256);
            for(int i=0; i<=255; i++)
            {
                for(int j=0; j<=255; j++)
                {
                    int s = 0, n = 0;
                    if (i - 1 >= 0 && j - 1 >= 0) { s += b.GetPixel(i - 1, j - 1).R; n++; }
                    if (i - 1 >= 0) { s += b.GetPixel(i - 1, j).R; n++; }
                    if (i - 1 >= 0 && j + 1 <= 255) { s += b.GetPixel(i - 1, j + 1).R; n++; }
                    if (j - 1 >= 0) { s += b.GetPixel(i, j - 1).R; n++; }
                    if (j + 1 <= 255) { s += b.GetPixel(i, j + 1).R; n++; }
                    if (i + 1 <= 255 && j - 1 >= 0) { s += b.GetPixel(i + 1, j - 1).R; n++; }
                    if (i + 1 <= 255) { s += b.GetPixel(i + 1, j).R; n++; }
                    if (i + 1 <= 255 && j + 1 <= 255) { s += b.GetPixel(i + 1, j + 1).R; n++; }
                    int nv = (int)s/n;
                    nb.SetPixel(i, j, Color.FromArgb(nv, nv, nv));
                }
            }
            pictureBox1.Image = (Image)nb;
        }

        public double interpolate(double a, double b, double x)
        {
            double f = (1 - Math.Cos(x * 3.1415927)) * 0.5;
            return (double)(a*(1-f) + b*f);
        }

        //Perlin noise generator
        private void button3_Click(object sender, EventArgs e)
        {
            Bitmap b = new Bitmap(256, 256);
            for(int i=0; i<=255; i++)
                for(int j=0; j<=255; j++)
                    b.SetPixel(i,j,Color.FromArgb(128,128,128));
            Random r = new Random();
            for (int amp = 128; amp >= 4; amp /= 2)
            {
                Bitmap tb = new Bitmap(257, 257);
                for (int i = 0; i <= 256; i += amp/2)
                {
                    for (int j = 0; j <= 256; j += amp/2)
                    {
                        int v = r.Next(128 - amp / 2, 128 + amp / 2);
                        tb.SetPixel(i, j, Color.FromArgb(v, v, v));
                    }
                }
                int lastX=0, lastY=0;
                for(int i = 0; i <= 255; i++)
                {
                    if(i == lastX + amp/2) lastX=i;
                    lastY = 0;
                    for(int j = 0; j <= 255; j++)
                    {
                        if(j == lastY + amp/2) lastY=j;
                        double i1 = interpolate(tb.GetPixel(lastX,lastY).R, tb.GetPixel(lastX+amp/2,lastY).R, (double)(i-lastX)*2/amp);
                        double i2 = interpolate(tb.GetPixel(lastX,lastY+amp/2).R, tb.GetPixel(lastX+amp/2,lastY+amp/2).R, (double)(i-lastX)*2/amp);
                        int i3 = (int)interpolate(i1, i2, (double)(j-lastY)*2/amp);
                        tb.SetPixel(i,j,Color.FromArgb(i3,i3,i3));
                    }
                }
                for(int i=0; i<=255; i++)
                    for(int j=0; j<=255; j++)
                    {
                        int c = b.GetPixel(i,j).R;
                        int tc = tb.GetPixel(i,j).R;
                        int rc = c + tc - 128;
                        b.SetPixel(i,j,Color.FromArgb(rc,rc,rc));
                    }
            }
            pictureBox1.Image = (Image)b;
        }

        //save dialog
        private void button1_Click_1(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName != "")
            {
                System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog1.OpenFile();
                pictureBox1.Image.Save(fs, System.Drawing.Imaging.ImageFormat.Bmp);
            }
        }

        Bitmap b;

        private void simulateBrownianMotion(double x, double y)
        {
            //MessageBox.Show(x + " " + y);
            double step = 3;
            double stick = 3;       // should be >= step
            bool found = false;
            Pen pen = new Pen(Color.Black, 1);
            var graphics = Graphics.FromImage(b);
            Random r = new Random();
            while(!found)
            {
                double d = r.NextDouble()*2*Math.PI;
                double dx = Math.Cos(d)*step;
                double dy = Math.Sin(d)*step;
                x += dx;
                y += dy;
                if (x < 0) x = -x; // x = 255 + x;
                if (y < 0) y = -y; // y = 255 + y;
                if (x > 255) x = 510 - x; // x -= 255;
                if (y > 255) y = 510 - y; // y -= 255;
                foreach(Point p in dlaPoints)
                {
                    double dist = (x-p.getX())*(x-p.getX())+(y-p.getY())*(y-p.getY());
                    if(dist < stick*stick)
                    {
                        dlaPoints.Add(new Point(x,y));
                        b.SetPixel((int)x,(int)y,Color.Black);
                        graphics.DrawLine(pen, (int)x, (int)y, (int)p.getX(), (int)p.getY());
                        found = true;
                        break;
                    }
                }
            }
            pictureBox1.Image = (Image)b;
        }

        //DLA simulation generator -- NOT WORKING!!
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                b = new Bitmap(pictureBox1.Image);
            }
            catch
            {
                b = new Bitmap(256, 256);
                for (int i = 0; i <= 255; i++)
                    for (int j = 0; j <= 255; j++)
                        b.SetPixel(i, j, Color.White);
            }
            if (dlaPoints.Count < 3)
            {
                dlaPoints.Add(new Point(e.Location.X,e.Location.Y));
                b.SetPixel(e.Location.X, e.Location.Y, Color.Black);
                pictureBox1.Image = (Image)b;
                return;
            }
            else MessageBox.Show("Points added. Beginning rendering...");
            int numParticles = 3000;
            int sizeField = 128;
            Random r = new Random();
            Random rx = new Random();
            Random ry = new Random();
            dlaPoints.Add(new Point(e.Location.X, e.Location.Y));
            b.SetPixel(e.Location.X, e.Location.Y, Color.Black);
            for (int i = 1; i <= numParticles; i++)
            {
                double px = 0, py = 0;
                px = r.NextDouble() * 256;
                py = r.NextDouble() * 256;
                /*int d;
                px = r.NextDouble() * 256;
                d = r.Next(0, 4);
                switch (d)
                {
                    case 0: py = 0; break;
                    case 1: py = 255; break;
                    case 2: py = px; px = 0; break;
                    case 3: py = px; px = 255; break;
                }*/
               // if(i>=2) MessageBox.Show(i.ToString());
                simulateBrownianMotion(px, py);
                //MessageBox.Show("stop brownian");
            }
            /*int numPart = 10000;
            int sizeField = 128;
            int x = e.Location.X;
            int y = e.Location.Y;
            Random r = new Random();
            Random rx = new Random();
            Random ry = new Random();
            b.SetPixel(x, y, Color.Black);
            for(int i=1; i<=numPart; i++)
            {
                int px, py;
                px = r.Next(Math.Max(x - sizeField, 0), Math.Min(x + sizeField, 255));
                py = r.Next(Math.Max(y - sizeField, 0), Math.Min(y + sizeField, 255));
                bool found = false;
                while(!found)
                {
                    //MessageBox.Show(x + " " + y + "\n" + px + " " + py + "\n" + b.GetPixel(x,y) + "\n" + b.GetPixel(px+1,py));
                    if (px + 1 <= Math.Min(255, x + sizeField) && b.GetPixel(px + 1, py) == b.GetPixel(x,y)) { b.SetPixel(px, py, Color.Black); found = true; break; }
                    if (px - 1 >= Math.Max(0, x - sizeField) && b.GetPixel(px - 1, py) == b.GetPixel(x, y)) { b.SetPixel(px, py, Color.Black); found = true; break; }
                    if (py + 1 <= Math.Min(255, y + sizeField) && b.GetPixel(px, py + 1) == b.GetPixel(x, y)) { b.SetPixel(px, py, Color.Black); found = true; break; }
                    if (py - 1 >= Math.Max(0, y - sizeField) && b.GetPixel(px, py - 1) == b.GetPixel(x, y)) { b.SetPixel(px, py, Color.Black); found = true; break; }
                    int d = r.Next(0, 4);
                    if (d == 0 && px - 1 > Math.Max(0, x - sizeField)) { px--; }
                    if (d == 1 && px + 1 < Math.Min(255, x + sizeField)) { px++; }
                    if (d == 2 && py - 1 > Math.Max(0, y - sizeField)) { py--; }
                    if (d == 3 && py + 1 < Math.Min(255, y + sizeField)) { py++; }
                    //MessageBox.Show(d.ToString());
                }
            }
            //MessageBox.Show("1");
            Bitmap pic = (Bitmap)pictureBox1.Image;
            for (int i = 0; i <= 255; i++)
                for (int j = 0; j <= 255; j++)
                    if (b.GetPixel(i, j) == Color.Black)
                        pic.SetPixel(i, j, Color.Black);
            pictureBox1.Image = (Image)b;
            MessageBox.Show("Image rendered.");*/
        }

        private void visit(int x, int y)
        {
            map[x, y]++;
            if(parx[x,y]!=-1)
            {
                visit(parx[x, y], pary[x, y]);
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            Random r = new Random();
            int[,] used = new int[256, 256];
            int numPoints = (1 << 15);
            for(int i=0; i<=255; i++)
                for(int j=0; j<=255; j++)
                {
                    parx[i, j] = -1;
                    pary[i, j] = -1;
                    map[i, j] = 0;
                    used[i, j] = 0;
                }
            int total = 0;
            for(int i = 1; i <= 5; i++)
            {
                int y = r.Next(0,256);
                int x = r.Next(0,256);
                used[x,y]=1;
                total++;
            }
            while(total<numPoints)
            {
                int x = r.Next(0, 256);
                int y = r.Next(0, 256);
                if (used[x, y] != 0) continue;
                bool hit = false;
                while(!hit)
                {
                    int lx = x;
                    int ly = y;
                    int d = r.Next(0, 4);
                    if (d == 0) { x++; }
                    else if (d == 1) { x--; }
                    else if (d == 2) { y++; }
                    else { y--; }
                    if (x < 0 || x > 255 || y < 0 || y > 255)
                        break;
                    if(used[x,y]>0)
                    {
                        hit = true;
                        total++;
                        parx[lx,ly] = x;
                        pary[lx,ly] = y;
                        used[lx,ly] = 1;
                        visit(lx,ly);
                    }
                }
            }
            Bitmap b = new Bitmap(256, 256);
            for(int i=0; i<=255; i++)
            {
                for(int j=0; j<=255; j++)
                {
                    int c;
                    c = Math.Min(map[i, j], 255);
                    b.SetPixel(i,j,Color.FromArgb(c,c,c));
                }
            }
            pictureBox1.Image = (Image)b;
        }

    }
}
