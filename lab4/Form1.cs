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
            polygonPoints = new List<Point>();
            polygon = false;
            wasPolygon = false;
            line = false;
            wasLine = false;
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
        bool polygon = false;
        bool segment = false;
        private void button2_Click(object sender, EventArgs e)
        {
            pensil = false;
            line = true;
            polygon = false;
            segment = false;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            pensil = true;
            line = false;
            polygon = false;
            segment = false;
        }

        int line_st_x, line_st_y;
        private Point first, last;
        private bool wasPolygon = false, wasLine = false;
        private Point segmentFrom, segmentTo, point;
        List<Point> polygonPoints = new List<Point>();
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (line || wasLine)
                DrawLine(e);
            
            if (polygon || wasPolygon)
                DrawPolygon(e);

            if (segment)
                DrawSegment(e);

            if (!segment)
            {
                segmentFrom = Point.Empty;
                segmentTo = Point.Empty;
                point = Point.Empty;
            }
        }

        void DrawLine(MouseEventArgs e)
        {
            if (line)
            {
                drawLineBresenham(pen, line_st_x, line_st_y, e.X, e.Y);
                wasLine = true;
                line = false;
            }
            else if (wasLine)
            {
                line_st_x = e.X;
                line_st_y = e.Y;
                wasLine = false;
                line = true;
            }
        }

        void DrawPolygon(MouseEventArgs e)
        {
            if (polygon)
            {   
                wasPolygon = true;
                polygonPoints.Add(new Point(e.X, e.Y));
            }
            if (!polygon && wasPolygon)
            {
                for (int i = 0; i < polygonPoints.Count - 1; i++)
                {
                    graphics.DrawLine(pen, polygonPoints[i], polygonPoints[i + 1]);
                }
                graphics.DrawLine(pen, polygonPoints[0], polygonPoints[polygonPoints.Count - 1]);
                wasPolygon = false;
                
                var image = (Bitmap)pictureBox1.Image;
                image.SetPixel(e.X, e.Y, pen.Color);
                pictureBox1.Invalidate();
                BelongsToPolygon(e.X, e.Y);
            }
        }

        void DrawSegment(MouseEventArgs e)
        {
            if (segmentFrom.IsEmpty)
            {
                segmentFrom.X = e.X;
                segmentFrom.Y = e.Y;
            }
            else if (segmentTo.IsEmpty)
            {
                segmentTo.X = e.X;
                segmentTo.Y = e.Y;
                graphics.DrawLine(pen, segmentFrom, segmentTo);
            }
            else
            {
                point.X = e.X;
                point.Y = e.Y;
                var image = (Bitmap)pictureBox1.Image;
                image.SetPixel( e.X, e.Y, pen.Color);
                pictureBox1.Invalidate();
                ClassifyPointPosition(segmentFrom, segmentTo, point);
                segmentFrom = Point.Empty;
                segmentTo = Point.Empty;
            }
            
        }

        void ClassifyPointPosition(Point segmentFrom, Point segmentTo, Point point)
        {
            var b = new Point(point.X - segmentFrom.X, segmentFrom.Y - point.Y);
            var a = new Point(segmentTo.X - segmentFrom.X, segmentFrom.Y - segmentTo.Y);
            var left = b.Y * a.X - b.X * a.Y > 0;
            graphics.DrawString(left ? "Точка слева" : "Точка справа", new Font(FontFamily.GenericMonospace, 8),
                Brushes.DarkRed, new PointF(point.X + 5, point.Y - 15));
        }

        double Angle(Point point, Point ray1, Point ray2)
        {
            var vector1 = new Point(ray1.X - point.X, ray1.Y - point.Y);
            var vector2 = new Point(ray2.X - point.X, ray2.Y - point.Y);
            var numerator = vector1.X * vector2.X + vector1.Y * vector2.Y;
            var lengthToFirst = Math.Sqrt(Math.Pow((ray1.X - point.X), 2) + Math.Pow(ray1.Y - point.Y, 2));
            var lengthToSecond = Math.Sqrt(Math.Pow((ray2.X - point.X), 2) + Math.Pow(ray2.Y - point.Y, 2));
            var det = vector1.X * vector2.Y - vector1.Y * vector2.X;
            return Math.Acos(numerator / (lengthToFirst * lengthToSecond)) * Math.Sign(det);
        }

        void BelongsToPolygon(int x, int y)
        {
            var sum = 0.0;
            sum += Angle(new Point(x, y), polygonPoints.Last(), polygonPoints.First());
            for (var i = 0; i < polygonPoints.Count - 1; i++)
            {
                sum += Angle(new Point(x, y), polygonPoints[i], polygonPoints[i + 1]);
            }

            var outside = Math.Abs(sum) < 0.1;
            graphics.DrawString(outside ? "Точка снаружи" : "Точка внутри", new Font(FontFamily.GenericMonospace, 8),
                Brushes.DarkRed, new PointF(x + 5, y - 15));
        }

        private void button2_MouseClick(object sender, MouseEventArgs e)
        {
        }

        private void button4_Click(object sender, EventArgs e)
        {
            pensil = false;
            line = false;
            polygon = !polygon;
            segment = false;
        }


        private void button5_Click(object sender, EventArgs e)
        {
            pensil = false;
            line = false;
            polygon = false;
            segment = true;
        }
    }
}
