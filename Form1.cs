using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace image_lab2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        //изображения выводимое в pictureBox
        private Bitmap pic = null;
        //массив храниящий значение изображения pic
        private int[,,] image = null;
        private double[] normCurveArray = null;
        private int[] forSeries = null;
        private void graphBox2_DoubleClick(object sender, EventArgs e)
        {
            graphBox2.PointListAddPoint(graphBox2.PointToClient(Control.MousePosition));
            normCurveArray =  graphBox2.GetMassByCurves();
            UseCurve();
            UpdateChart(forSeries);

            graphBox2.Refresh();


        }
        private void UseCurve()
        {
            for(int i = 0; i < image.GetLength(0); i++)
            {
                for(int j =0; j<image.GetLength(1);j++)
                {
                    image[i,j,0] = (int)(normCurveArray[image[i, j, 0]]*256);
                    image[i,j,1] = (int)(normCurveArray[image[i,j,1]]*256);
                    image[i, j, 2] = (int)(normCurveArray[image[i, j, 2]] * 256);
                }   
            }
            Bitmap bitmap = new Bitmap(image.GetLength(0), image.GetLength(1));
            for (int i = 0; i < image.GetLength(0); i++)
            {
                for (int j = 0; j < image.GetLength(1); j++)
                {
                    bitmap.SetPixel(i, j, Color.FromArgb(image[i, j, 0], image[i, j, 1], image[i, j, 2]));
                }
            }
            pictureBox1.Image = bitmap;
            forSeries = GetNumOfPixels(image);
            pictureBox1.Refresh();
        }
        //возвращает матрицу пикселей от pic
        private int[,,] GetMatOfPixels(Bitmap pic)
        {
            int[,,] res = new int[pic.Width, pic.Height, 3];
            for(int i =0; i < pic.Width; i++)
                for(int j =0; j < pic.Height; j++)
                {
                    Color col = pic.GetPixel(i, j);
                    res[i, j, 0] = col.R;
                    res[i, j, 1] = col.G;
                    res[i, j, 2] = col.B;
                }
            return res;
        }
        //возвращает массив с количеством пикселей каждой ярости;
        private int[] GetNumOfPixels(int [,,] pic)
        {
            int[] nums = new int[256];
            for (int i = 0; i < pic.GetLength(0); i++)
            {
                for (int j = 0; j < pic.GetLength(1); j++)
                {
                    double arg = (pic[i, j, 0] + pic[i, j, 1] + pic[i, j, 2]) / 3;
                    nums[(int)Math.Round(arg)]++;
                }
            }
            return nums;
        }
        private void graphBox2_Click(object sender, EventArgs e)
        {
            graphBox2.PointListDeletePoint(graphBox2.PointToClient(Control.MousePosition));
            chart1.Series[0].Points.Clear();
            UpdateChart(forSeries);
            graphBox2.Refresh();
            
        }
        //плохо
        private void graphBox2_MouseDown(object sender, MouseEventArgs e)
        {
            //graphBox2.PointListSetControlPoint(graphBox2.PointToClient(Control.MousePosition));
            //graphBox2.Refresh();
        }

        private void graphBox2_MouseUp(object sender, MouseEventArgs e)
        {
            //graphBox2.PointListMoveControlPoint(graphBox2.PointToClient(Control.MousePosition));
            //graphBox2.PointListUnSetControlPoint();
            //graphBox2.Refresh();
        }
        private int[] InitialaztionImage(string path)
        {
            pic = new Bitmap(path);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.Image = pic;
            image = GetMatOfPixels(pic);
            return  GetNumOfPixels(image);
        }
        private void UpdateChart(int[] forSeries)
        {
            
            for (int i = 0; i < 256; i++)
            {
                chart1.Series[0].Points.AddY(forSeries[i]);
            }
            chart1.Update();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            forSeries = InitialaztionImage("..\\..\\in1.jpg");
            UpdateChart(forSeries);

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if(pic != null)
                pic.Dispose();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files(*.jpg)|*.jpg;";
            //int[] forSeries = null;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    forSeries = InitialaztionImage(ofd.FileName);
                }
                catch
                {
                    MessageBox.Show("Vse Ploho");
                }
                chart1.Series[0].Points.Clear();
                UpdateChart(forSeries);
            }
        }
    }
}
