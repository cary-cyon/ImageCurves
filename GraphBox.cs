using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace image_lab2
{
    internal class GraphBox : Control
    {
        private ContentAlignment alignmentValue = ContentAlignment.MiddleLeft;
        //хранит точки кривой 
        private List<Point> pointList = new List<Point>();
        //чтобы добавть начало и конец холста к кривой 
        private bool isPaint = false;
        private int? controlPointIndex = null;
        protected override void OnPaint(PaintEventArgs e)
        {
            if (!isPaint)
            {
                pointList.Add(new Point(0, this.Width));
                pointList.Add(new Point(this.Height, 0));
            }
            base.OnPaint(e);
            e.Graphics.Clear(Color.White);
            //цикл обходит точки кривой 
            for (int i = 0; i < pointList.Count - 1; i++)
            {
                e.Graphics.DrawLine(Pens.Red, pointList[i], pointList[i + 1]);
                e.Graphics.FillEllipse(Brushes.Purple, pointList[i].X-3, pointList[i].Y-3, 7, 7);
                if(i == controlPointIndex)
                {
                    e.Graphics.FillEllipse(Brushes.Red, pointList[i].X - 5, pointList[i].Y - 5, 10, 10);
                }
            }
            e.Graphics.FillEllipse(Brushes.Purple, 
                                   pointList[pointList.Count - 1].X - 3, 
                                   pointList[pointList.Count - 1].Y - 3, 
                                   7, 
                                   7);
            isPaint = true;
        }
        //метод для добавления новой точки к кривой (точка в пользовательских координатах
        public void PointListAddPoint(Point point)
        {
            for(int i = 0; i < pointList.Count-1; i++)
            {
                if(pointList[i].X < point.X && pointList[i+1].X > point.X)
                {
                    pointList.Insert(i+1, point);
                    break;
                }
                
            }
            
        }
        public void PointListDeletePoint(Point point)
        {
            for(int i =1; i < pointList.Count-1; i++)
            {
                if(Math.Pow(point.X - pointList[i].X, 2) + Math.Pow(point.Y - pointList[i].Y, 2) <= 9)
                {
                    pointList.RemoveAt(i);
                }
            }
        }
        public void PointListSetControlPoint(Point point)
        {
            if(controlPointIndex== null)
            {
                for (int i = 0; i < pointList.Count; i++)
                {
                    if (Math.Pow(point.X - pointList[i].X, 2) + Math.Pow(point.Y - pointList[i].Y, 2) <= 9)
                    {
                        controlPointIndex = i;
                    }
                }
            }
        }
        public void PointListUnSetControlPoint()
        {
            controlPointIndex=null;
        }
        //плохо
        public void PointListMoveControlPoint(Point point)
        {
            if(controlPointIndex == null)
            {
                return;
            }
            pointList.RemoveAt((int)controlPointIndex);
            PointListAddPoint(point);
            
        }
        //метод возвращает массив для замены пикселей по кривой 
        private List<double[]> normPointList()
        {
            List<double[]> points = new List<double[]>();
            for(int i =0; i < pointList.Count; i++)
            {
                double X = pointList[i].X;
                double Y = pointList[i].Y;
                points.Add(new double[2] { X / Height, 1 - (Y / Width) });
            }
            return points;
        }
        public double[] GetMassByCurves()
        {
            double[] mass = new double[256];
            int end = 1;
            int start = 0;
            List<double[]> norm_list = normPointList();
            double b = 0;
            double y1 = norm_list[end][1];
            double x1 = norm_list[end][0];
            double y = norm_list[start][1];
            double x = norm_list[start][0];
            double k = (y1 - y) / (x1 - x);
            k = k / 256;

            for (int j=0; j<256; j++)
            {

                mass[j] = k * j + b;
                if (norm_list[end][0]<j*(1.0/256.0))
                {
                    start++;
                    end++;
                    y1 = norm_list[end][1];
                    x1 = norm_list[end][0];
                    y = norm_list[start][1];
                    x = norm_list[start][0];
                    k = (y1 - y) / (x1 - x);
                    k = k / 256;
                    b = norm_list[start][1] - k * j;
                }
            }

            return mass;

        }

    }
}
//for(int i = 0; i < norm_list.Count-1; i++)
//{
//    double k = (norm_list[i + 1][1] - norm_list[i][1]) / (norm_list[i + 1][0] - norm_list[i][0]);
//    k = k / 256;
//    double b = norm_list[i][1] - k * end;
//    for (int j = end; j <(int)256*norm_list[i+1][0]; j++)
//    {

//        mass[end + j] = k * j + b;
//    }
//    end = (int)(256 * norm_list[i + 1][0]);
//}