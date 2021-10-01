using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab31
{
    public partial class Form1 : Form
    {
        Pen pen;
        Pen fill;
        Graphics graphics;
        public Form1()
        {
            InitializeComponent();
            pen = new Pen(Color.Black, 1);
            fill = new Pen(Color.Black, 1);
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox2.BackColor = Color.Black;
            graphics = Graphics.FromImage(pictureBox1.Image);
            pictureBox1.Invalidate();
        }



        //------------------------------- #1 -----------------------------------------------

        private Color area_color;
        bool fill_flag = false;
        private void button5_Click(object sender, EventArgs e)
        {
            fill_flag = true;
        }

        private bool equalColors(Color c1, Color c2)
        {
            return c1.R == c2.R && c1.G == c2.G && c1.B == c2.B;
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (fill_flag == false)
            {
                if (line)
                {
                    if (bresenham)
                    {
                        drawLineBresenham(pen, line_st_x, line_st_y, e.X, e.Y);
                    }
                    else if (wu)
                    {
                        drawLineWu(pen, line_st_x, line_st_y, e.X, e.Y);
                    }
                }
                else
                {
                    line_st_x = e.X;
                    line_st_y = e.Y;
                }
                line = !line;
            }
            else if (fill_flag)
            {
                area_color = (pictureBox1.Image as Bitmap).GetPixel(e.X, e.Y);
                if (!equalColors(area_color, fill.Color)) 
                        rec_fill(e.X, e.Y);
            }
        }

        private void rec_fill(int x, int y)
        {
            Bitmap bitmap = pictureBox1.Image as Bitmap;
            if (0 <= x && x < bitmap.Width && 0 <= y && y < bitmap.Height && !equalColors(area_color, fill.Color))
            {
                Point leftBound = new Point(x, y);
                Point rightBound = new Point(x, y);
                Color currentColor = area_color;

                while (0 < leftBound.X && equalColors(bitmap.GetPixel(leftBound.X, y),area_color))
                {
                    leftBound.X -= 1;
                }

                while (rightBound.X < pictureBox1.Width - 1 && equalColors(bitmap.GetPixel(leftBound.X, y), area_color))
                {
                    rightBound.X += 1;
                }

                drawLineBresenham(fill, leftBound.X, leftBound.Y, rightBound.X, rightBound.Y);

                for (int i = leftBound.X; i < rightBound.X + 1; ++i)
                {
                    rec_fill(i, y + 1);
                }
                for (int i = leftBound.X; i < rightBound.X + 1; ++i)
                {
                    if (y > 0)
                    {
                        rec_fill(i, y - 1);
                    }
                }

            }
        }


        //------------------------------- #2 -----------------------------------------------
        bool bresenham = false;
        bool wu = false;
        bool line = false;
        int line_st_x;
        int line_st_y;

        private void button2_Click(object sender, EventArgs e)
        {
            bresenham = true;
            wu = false;
            fill_flag = false;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            bresenham = false;
            wu = true;
            fill_flag = false;
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
        void drawLineWu(Pen pen, int x1, int y1, int x2, int y2)
        {
            var image = (pictureBox1.Image as Bitmap);
            double dx = x2 - x1; 
            double dy = y2 - y1;
            if (Math.Abs(dx) > Math.Abs(dy))
            {
                var gradient = dy / dx;
                if (x2 < x1)
                {
                    var c = x1;
                    x1 = x2;
                    x2 = c;

                    c = y1;
                    y1 = y2;
                    y2 = c;
                }
                var intery = y1 + gradient;
                var xpxl1 = x1;
                var xpxl2 = x2;
                for (var x = xpxl1 + 1; x <= xpxl2 - 1; x++)
                {
                    image.SetPixel(x, (int)intery, Color.FromArgb((int)((1 - (intery - (int)intery)) * 255), pen.Color.R, pen.Color.G, pen.Color.B));
                    image.SetPixel(x, (int)intery + 1, Color.FromArgb((int)((intery - (int)intery) * 255), pen.Color.R, pen.Color.G, pen.Color.B));
                    pictureBox1.Invalidate();
                    intery = intery + gradient;
                }
            }
            else
            {
                if (y2 < y1)
                {
                    var c = x1;
                    x1 = x2;
                    x2 = c;

                    c = y1;
                    y1 = y2;
                    y2 = c;
                }

                var gradient = dx / dy;
                var interx = x1 + gradient;
                var ypxl1 = y1;
                var ypxl2 = y2;
                for (var y = ypxl1 + 1; y <= ypxl2 - 1; y++)
                {
                    image.SetPixel((int)interx, y, Color.FromArgb((int)((1 - (interx - (int)interx)) * 255), pen.Color.R, pen.Color.G, pen.Color.B));
                    image.SetPixel((int)interx + 1, y, Color.FromArgb((int)((interx - (int)interx) * 255), pen.Color.R, pen.Color.G, pen.Color.B));
                    pictureBox1.Invalidate();
                    interx = interx + gradient;
                }
            }
        }


        //------------------------------- clear ----------------------------------------------

        private void button4_Click(object sender, EventArgs e)
        {
            graphics.Clear(Color.White);
            pictureBox1.Invalidate();
            fill_flag = false;
        }

        //------------------------------- color ----------------------------------------------
        private void button1_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                pen.Color = colorDialog1.Color;
                fill.Color = colorDialog1.Color;
                pictureBox2.BackColor = colorDialog1.Color;
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            button1_Click(sender, e);
        }

        //---------------------------- pen --------------------------------------------------

        int start_x;
        int start_y;
        bool drawing = false;
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            start_x = e.X;
            start_y = e.Y;
            drawing = true;

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

        //-----------------------------------------------------------------------------------
        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

    }
}
