using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab4
{
    public partial class Form1 : Form
    {

        Pen pen;
        Graphics graphics;

        public Form1()
        {
            InitializeComponent();
            pen = new Pen(Color.Black, 1);
            pictureBox1.Image  = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            graphics = Graphics.FromImage(pictureBox1.Image);
            pictureBox1.Invalidate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            graphics.Clear(Color.White);
            pictureBox1.Invalidate();
        }


        int start_x;
        int start_y;
        bool drawing = false;
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            start_x = e.X;
            start_y = e.Y;
            drawing = true;
            Bitmap image = (Bitmap)pictureBox1.Image;
            image.SetPixel( e.X, e.Y, pen.Color);
            pictureBox1.Invalidate();
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            drawing = false;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (drawing)
            {
                graphics.DrawLine(pen, start_x, start_y, e.X, e.Y);
                start_x = e.X;
                start_y = e.Y;
                pictureBox1.Invalidate();
            }
        }
        void drawLineBresenham(Pen pen, int x1, int y1, int x2, int y2)
        {
            Bitmap image = (Bitmap)pictureBox1.Image;
            int deltaX = Math.Abs(x2 - x1);
            int deltaY = Math.Abs(y2 - y1);
            int signX = x1 < x2 ? 1 : -1;
            int signY = y1 < y2 ? 1 : -1;
            int error = deltaX - deltaY;

            image.SetPixel(x2, y2, pen.Color);
            pictureBox1.Invalidate();
            while (x1 != x2 || y1 != y2)
            {
                image.SetPixel(x1, y1, pen.Color);
                pictureBox1.Invalidate();
                int error2 = error * 2;
                if (error2 > -deltaY)
                {
                    error -= deltaY;
                    x1 += signX;
                }
                if (error2 < deltaX)
                {
                    error += deltaX;
                    y1 += signY;
                }
            }

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        bool pensil = false;
        bool line = false;
        private void button2_Click(object sender, EventArgs e)
        {
            pensil = false;
            line = true;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            pensil = true;
            line = false;
        }

        int line_st_x, line_st_y;

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (line)
            {
                drawLineBresenham(pen, line_st_x, line_st_y, e.X, e.Y);
            }
            else
            {
                line_st_x = e.X;
                line_st_y = e.Y;
            }
            line = !line;
        }

        private void button2_MouseClick(object sender, MouseEventArgs e)
        {
        }
    }
}
