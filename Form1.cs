using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;


namespace CsRGBshow
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        byte[,] array_Binary; //二值化陣列
        byte[,] array_Outline; //輪廓線陣列
        byte[,] Tbin; //擇處理之二值化陣列
        int Gdim_width = 4;
        int Gdim_height = 4; //計算區域亮度區塊的寬與高
        int[,] Th; //每一區塊的平均亮度，二值化門檻值
       
        ArrayList C; //目標物件集合
        Bitmap Mb; //底圖副本
        FastPixel f = new FastPixel(); //宣告快速繪圖物件

        #region  OpenToolStripMenuItem_Click
        //開啟影像
        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Bitmap bmp = new Bitmap(openFileDialog1.FileName);
                f.image = bmp;
                f.Bmp2RGB(bmp); //讀取RGB亮度陣列
                f.array_Gray = f.array_Green;//灰階陣列為綠光陣列
                pictureBox1.Image = bmp;
            }
        }
        #endregion
        #region BinaryToolStripMenuItem_Click
        //二值化
        private void BinaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //array_Gray = Negative(array_Gray);
            Th = ThresholdBuild(f.array_Gray); //門檻值陣列建立
            int Offset = int.Parse(textBox1.Text);
            Gdim_width = int.Parse(textBox2.Text);
            Gdim_height = int.Parse(textBox2.Text);
            array_Binary = new byte[f.image_width, f.image_height];
            if (radioButton1.Checked)
            {
                for (int i = 1; i < f.image_width - 1; i++)
                {
                    int x = i / Gdim_width;
                    for (int j = 1; j < f.image_height - 1; j++)
                    {
                        int y = j / Gdim_height;
                        if (f.array_Green[i, j] < Th[x, y] + Offset)//Th[x, y]
                        {
                            array_Binary[i, j] = 1;
                        }
                    }
                }
            }
            if (radioButton2.Checked)
            {
                for (int i = 1; i < f.image_width - 1; i++)
                {
                    int x = i / Gdim_width;
                    for (int j = 1; j < f.image_height - 1; j++)
                    {
                        int y = j / Gdim_height;
                        if (f.array_Green[i, j] > Th[x, y] - Offset)//Th[x, y]
                        {
                            array_Binary[i, j] = 1;
                        }
                    }
                }
            }
            pictureBox1.Image = f.BWImg(array_Binary);
        }
        #endregion
        #region ThresholdBuild
        //門檻值陣列建立
        private int[,] ThresholdBuild(byte[,] b)
        {
            Gdim_width = int.Parse(textBox2.Text);
            Gdim_height = int.Parse(textBox2.Text);
            int kx = f.image_width / Gdim_width, ky = f.image_height / Gdim_height;
            Th = new int[kx, ky];
            //累計各區塊亮度值總和
            for (int i = 0; i < f.image_width; i++)
            {
                int x = i / Gdim_width;
                for (int j = 0; j < f.image_height; j++)
                {
                    int y = j / Gdim_height;
                    Th[x, y] += f.array_Green[i, j];
                }
            }
            for (int i = 0; i < kx; i++)
            {
                for (int j = 0; j < ky; j++)
                {
                    Th[i, j] /= Gdim_width * Gdim_height;
                }
            }
            return Th;
        }
        #endregion
        #region OutlineToolStripMenuItem_Click
        //輪廓線
        private void OutlineMenuItem_Click(object sender, EventArgs e)
        {
            //array_Outline = GetLine(array_Binary);
            array_Outline = GetScribeLine(f.array_Green);
            //array_Outline = GetScribeLine(HistogramEqualization(f.array_Green, getGrayHistogram(f.image)));
            pictureBox1.Image = f.BWImg(array_Outline);
        }
        #endregion
        #region GetLine
        //建立輪廓點陣列
        private byte[,] GetLine(byte[,] b)
        {
            byte[,] Q = new byte[f.image_width, f.image_height];
            for (int i = 2; i < f.image_width - 2; i++)
            {
                for (int j = 2; j < f.image_height - 2; j++)
                {
                    if (b[i, j] == 1 && b[i - 1, j] == 1 && b[i + 1, j] == 1 && b[i, j - 1] == 1 && b[i, j + 1] == 1 && b[i - 1, j - 1] == 1 && b[i - 1, j + 1] == 1 && b[i + 1, j - 1] == 1 && b[i + 1, j + 1] == 1
                        /*&& b[i - 2, j + 2] == 1 && b[i - 1, j + 2] == 1 && b[i, j + 2] == 1 && b[i + 1, j + 2] == 1 && b[i + 2, j + 2] == 1 && b[i - 2, j + 1] == 1 && b[i + 2, j + 1] == 1 && b[i - 2, j] == 1 && b[i + 2, j] == 1 && b[i - 2, j - 1] == 1 && b[i + 2, j - 1] == 1 && b[i - 2, j - 2] == 1 && b[i - 1, j - 2] == 1 && b[i, j - 2] == 1 && b[i + 1, j - 2] == 1 && b[i + 2, j - 2] == 1*/)
                    {
                        Q[i, j] = 1;
                    }
                }
            }
            return Q;
        }
        #endregion
        #region GetScribeLine
        //建立切割道陣列
        private byte[,] GetScribeLine(byte[,] b)
        {
            //Q:輪廓線陣列
            byte[,] Q = new byte[f.image_width, f.image_height];
            int offset = int.Parse(tbOffset.Text);
            for (int i = 1; i < f.image_width - 1; i++)
            {
                for (int j = 1; j < f.image_height - 1; j++)
                {
                    if (Math.Abs(b[i - 1, j] - b[i, j]) < offset
                        && Math.Abs(b[i - 1, j - 1] - b[i, j]) < offset
                        && Math.Abs(b[i - 1, j + 1] - b[i, j]) < offset
                        && Math.Abs(b[i, j - 1] - b[i, j]) < offset
                        && Math.Abs(b[i, j + 1] - b[i, j]) < offset
                        && Math.Abs(b[i + 1, j] - b[i, j]) < offset
                        && Math.Abs(b[i + 1, j - 1] - b[i, j]) < offset
                        && Math.Abs(b[i + 1, j + 1] - b[i, j]) < offset)
                    {
                        Q[i, j] = 1;
                    }
                }
            }
            return Q;
        }
        #endregion
        #region Outline
        private byte[,] Outline(byte[,] b)
        {
            byte[,] Q = new byte[f.image_width, f.image_height];
            for (int i = 1; i < f.image_width - 1; i++)
            {
                for (int j = 1; j < f.image_height - 1; j++)
                {
                    if (b[i, j] == 0) continue;
                    if (b[i - 1, j] == 0) { Q[i, j] = 1; continue; }
                    if (b[i + 1, j] == 0) { Q[i, j] = 1; continue; }
                    if (b[i, j - 1] == 0) { Q[i, j] = 1; continue; }
                    if (b[i, j + 1] == 0) { Q[i, j] = 1; }
                }
            }
            return Q;
        }
        #endregion
        #region equalToolStripMenuItem_Click
        private void equalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            byte[,] Q = new byte[f.image_width, f.image_height];
            Q = HistogramEqualization(f.array_Green, getGrayHistogram(f.image));
            pictureBox1.Image = f.GrayImg(Q);
        }
        #endregion
        #region GetGrayHistogram
        public double[] getGrayHistogram(Bitmap grayImage)
        {
            double[] numb = new double[256];
            for (int j = 0; j < grayImage.Height; j++)
            {
                for (int i = 0; i < grayImage.Width; i++)
                {
                    Color pixelRGB = grayImage.GetPixel(i, j);
                    int grayNumb = pixelRGB.G;
                    numb[grayNumb]++;
                }
            }
            for (int k = 0; k < 256; k++)
            {
                double value = numb[k];
                double rate = value / (grayImage.Height * grayImage.Width * 1.0);
                numb[k] = rate;
            }
            return numb;
        }
        #endregion
        #region HistogramEqualization
        private byte[,] HistogramEqualization(byte[,] b, double[] density)
        {
            byte[,] Q = new byte[f.image_width, f.image_height];
            for (int j = 0; j < f.image_height; j++)
            {
                for (int i = 0; i < f.image_width; i++)
                {
                    double densitySum = 0;
                    int value = b[i, j];
                    for (int k = 0; k <= value; k++)
                    {    //累積概率
                        densitySum += density[k];
                    }
                    byte s = (byte)Math.Round(255 * densitySum);
                    Q[i, j] = s;
                }
            }
            return Q;
        }
        #endregion
        #region TargetsToolStripMenuItem_Click
        //建立目標物件
        private void TargetsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ArrayList D = new ArrayList();
            C = getTargets(array_Outline); //建立目標物件集合
            listBox1.Items.Clear();

            #region 過濾長寬不夠之物件
            for (int k = 0; k < C.Count; k++)
            {
                TgInfo T = (TgInfo)C[k];
                if (T.height > f.maxHeight && T.width > f.maxWidth) D.Add(T);
                if (T.height > f.maxHeight && T.width < f.minWidth) D.Add(T);
                if (T.height < f.minHeight && T.width > f.maxWidth) D.Add(T);
            }
            
            C = D;
            //依長寬排序
            for (int i = 0; i < 10; i++)
            {
                for (int j = i + 1; j < C.Count; j++)
                {
                    TgInfo T = (TgInfo)C[i], G = (TgInfo)C[j];
                    if (T.width + T.height < G.width + G.height)
                    {
                        C[i] = G; C[j] = T;
                    }
                }
            }
            if (C.Count == 0)
            {
                MessageBox.Show("找不到切割道交點");
                return;
            }
            #endregion
            //繪製目標輪廓點
            Bitmap bmp = new Bitmap(f.image_width, f.image_height);
            for (int k = 0; k < C.Count; k++)
            {
                TgInfo T = (TgInfo)C[k];
                for (int m = 0; m < T.P.Count; m++)
                {
                    Point p = (Point)T.P[m];
                    bmp.SetPixel(p.X, p.Y, Color.Black);
                }
            }
            listBox1.Items.Add("物件數量" + C.Count.ToString());
            pictureBox1.Image = bmp;
        }
        #endregion
        #region getTargets
        //以輪廓點建立目標陣列，排除負目標
        private ArrayList getTargets(byte[,] q)
        {
            ArrayList A = new ArrayList();
            byte[,] b = (byte[,])q.Clone();//建立輪廓點陣列副本
            for (int i = 1; i < f.image_width - 1; i++)
            {
                for (int j = 1; j < f.image_height - 1; j++)
                {
                    if (b[i, j] == 0) continue;
                    TgInfo G = new TgInfo();
                    G.xmn = i; G.xmx = i; G.ymn = j; G.ymx = j; G.P = new List<Point>();
                    ArrayList nc = new ArrayList();//每一輪搜尋的起點集合
                    nc.Add(new Point(i, j));//輸入之搜尋起點
                    G.P.Add(new Point(i, j));
                    b[i, j] = 0;//清除此起點之輪廓點標記
                    do
                    {
                        ArrayList nb = (ArrayList)nc.Clone();//複製此輪之搜尋起點集合
                        nc = new ArrayList();// 清除準備蒐集下一輪搜尋起點之集合
                        for (int m = 0; m < nb.Count; m++)
                        {
                            Point p = (Point)nb[m];//搜尋起點
                            //在此點周邊3X3區域內找輪廓點
                            for (int ii = p.X - 1; ii <= p.X + 1; ii++)
                            {
                                for (int jj = p.Y - 1; jj <= p.Y + 1; jj++)
                                {
                                    //if (ii - 1 >= 0 && jj - 1 >= 0 &&ii+1<=f.ny&&jj+1<=f.nx) continue;
                                    //if (b[ii, jj] == 1 && b[ii - 1, jj] == 1 && b[ii + 1, jj] == 1 && b[ii, jj - 1] == 1 && b[ii, jj + 1] == 1&& b[i - 1, j-1] == 1&& b[i - 1, j+1] == 1&& b[i + 1, j-1] == 1&& b[i + 1, j+1] == 1) continue;//非輪廓點忽略
                                    if (b[ii, jj] == 0) continue;//非輪廓點忽略
                                    Point k = new Point(ii, jj);//建立點物件
                                    nc.Add(k);//本輪搜尋新增的輪廓點
                                    G.P.Add(k);
                                    G.np += 1;//點數累計
                                    if (ii < G.xmn) G.xmn = ii;
                                    if (ii > G.xmx) G.xmx = ii;
                                    if (jj < G.ymn) G.ymn = jj;
                                    if (jj > G.ymx) G.ymx = jj;
                                    b[ii, jj] = 0;//清除輪廓點點標記
                                }
                            }
                        }
                    } while (nc.Count > 0);//此輪搜尋有新發現輪廓點時繼續搜尋
                    G.width = G.xmx - G.xmn + 1;//寬度計算
                    G.height = G.ymx - G.ymn + 1;//高度計算
                    A.Add(G);//加入有效目標集合
                }
            }
            return A; //回傳目標物件集合
        }
        #endregion
        #region PictureBox1_MouseDown
        //點選目標顯示位置與屬性
        private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (Mb == null) return;
            if (e.Button == MouseButtons.Left)
            {
                int m = -1;
                for (int k = 0; k < C.Count; k++)
                {
                    TgInfo T = (TgInfo)C[k];
                    if (e.X < T.xmn) continue;
                    if (e.X > T.xmx) continue;
                    if (e.Y < T.ymn) continue;
                    if (e.Y > T.ymx) continue;
                    m = k; break;
                }
                if (m >= 0)
                {
                    Bitmap bmp = (Bitmap)Mb.Clone();
                    TgInfo T = (TgInfo)C[m];
                    for (int n = 0; n < T.P.Count; n++)
                    {
                        Point p = (Point)T.P[n];
                        bmp.SetPixel(p.X, p.Y, Color.Red);
                    }
                    pictureBox1.Image = bmp;
                    //指定目標的資訊
                    string S = "Width=" + T.width.ToString();
                    S += "\n\r" + "Height=" + T.height.ToString();
                    S += "\n\r" + "Contrast=" + T.pm.ToString();
                    S += "\n\r" + "Point=" + T.np.ToString();
                    MessageBox.Show(S);
                }
            }
        }
        #endregion


        #region FilterToolStripMenuItem_Click
        //依據目標大小篩選目標       
        private void FilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int top_x_add_all = 0;
            int left_y_add_all = 0;
            int right_y_add_all = 0;
            int down_x_add_all = 0;
            int counter = 0;
            List<List<Point>> Hor_Line = new List<List<Point>>();
            List<List<Point>> Ver_Line = new List<List<Point>>();

            List<List<Point>> centerpoint_list = new List<List<Point>>();
            List<float> angle_List = new List<float>();
            Point centerpoint = new Point();
            string show = null;
            float angle_Average = 0;
            listBox1.Items.Clear();
            for (int k = 0; k < C.Count; k++)
            {
                TgInfo T = (TgInfo)C[k];
                T.left_point_list = new List<Point>();
                T.right_point_list = new List<Point>();
                T.top_point_list = new List<Point>();
                T.down_point_list = new List<Point>();
                T.left_centerpoint_list = new List<Point>();
                T.right_centerpoint_list = new List<Point>();
                T.top_centerpoint_list = new List<Point>();
                T.down_centerpoint_list = new List<Point>();

                #region 輪廓點內補滿 
                byte[,] Tbin = Fill(T.P);
                //pictureBox1.Image = f.BWImg(Fill(T.P));//繪製二值化圖
                //return;
                #endregion
                #region 找出左右上下點集合
                for (int m = 0; m < T.P.Count; m++)
                {
                    Point p = (Point)T.P[m];
                    if (p.X == 1 || p.X == 2 )
                    {
                        T.left_point_list.Add(p);
                    }
                    if (p.X == f.image_width - 1 || p.X == f.image_width - 2)
                    {
                        T.right_point_list.Add(p);
                    }
                    if (p.Y == 1 || p.Y == 2 )
                    {
                        T.top_point_list.Add(p);
                    }
                    if (p.Y == f.image_height-1 || p.Y == f.image_height - 2)
                    {
                        T.down_point_list.Add(p);
                    }
                }
                T.left_point_list = T.left_point_list.OrderBy(p => p.Y).ToList();
                T.right_point_list = T.right_point_list.OrderBy(p => p.Y).ToList();
                T.top_point_list = T.top_point_list.OrderBy(p => p.X).ToList();
                T.down_point_list = T.down_point_list.OrderBy(p => p.X).ToList();
                #endregion
                #region 左右上下 center point list
                #region 左 center point list
                for (int l = 1; l < T.left_point_list.Count; l++)
                {
                    if (l == 1)
                    {
                        left_y_add_all = T.left_point_list[0].Y;
                        counter = 1;
                    }
                    if ((T.left_point_list[l].Y - T.left_point_list[l - 1].Y) < 9)
                    {
                        left_y_add_all = left_y_add_all + T.left_point_list[l].Y;
                        counter = counter + 1;
                    }
                    else
                    {
                        left_y_add_all = left_y_add_all / counter;
                        T.left_centerpoint_list.Add(new Point(T.xmn, left_y_add_all));
                        left_y_add_all = T.left_point_list[l].Y;
                        counter = 1;
                    }
                    if (l == T.left_point_list.Count - 1)
                    {
                        left_y_add_all = left_y_add_all / counter;
                        T.left_centerpoint_list.Add(new Point(T.xmn, left_y_add_all));
                        left_y_add_all = 0;
                        counter = 0;
                    }
                }
                #endregion
                #region 右 center point list
                for (int l = 1; l < T.right_point_list.Count; l++)
                {
                    if (l == 1)
                    {
                        right_y_add_all = T.right_point_list[0].Y;
                        counter = 1;
                    }
                    if ((T.right_point_list[l].Y - T.right_point_list[l - 1].Y) < 9)
                    {
                        right_y_add_all = right_y_add_all + T.right_point_list[l].Y;
                        counter = counter + 1;
                    }
                    else
                    {
                        right_y_add_all = right_y_add_all / counter;

                        T.right_centerpoint_list.Add(new Point(T.xmx, right_y_add_all));
                        right_y_add_all = T.right_point_list[l].Y;
                        counter = 1;
                    }
                    if (l == T.right_point_list.Count - 1)
                    {
                        right_y_add_all = right_y_add_all / counter;
                        T.right_centerpoint_list.Add(new Point(T.xmx, right_y_add_all));
                        right_y_add_all = 0;
                        counter = 0;
                    }
                }
                #endregion
                #region 上 center point list
                for (int l = 1; l < T.top_point_list.Count; l++)
                {
                    if (l == 1)
                    {
                        top_x_add_all = T.top_point_list[0].X;
                        counter = 1;
                    }
                    if ((T.top_point_list[l].X - T.top_point_list[l - 1].X) < 9)
                    {
                        top_x_add_all = top_x_add_all + T.top_point_list[l].X;
                        counter = counter + 1;
                    }
                    else
                    {
                        top_x_add_all = top_x_add_all / counter;
                        T.top_centerpoint_list.Add(new Point(top_x_add_all, T.ymn));
                        top_x_add_all = T.top_point_list[l].X;
                        counter = 1;
                    }
                    if (l == T.top_point_list.Count - 1)
                    {
                        top_x_add_all = top_x_add_all / counter;
                        T.top_centerpoint_list.Add(new Point(top_x_add_all, T.ymn));
                        top_x_add_all = 0;
                        counter = 0;
                    }
                }
                #endregion
                #region 下 center point list
                for (int l = 1; l < T.down_point_list.Count; l++)
                {
                    if (l == 1)
                    {
                        down_x_add_all = T.down_point_list[0].X;
                        counter = 1;
                    }
                    if ((T.down_point_list[l].X - T.down_point_list[l - 1].X) < 9)
                    {
                        down_x_add_all = down_x_add_all + T.down_point_list[l].X;
                        counter = counter + 1;
                    }
                    else
                    {

                        down_x_add_all = down_x_add_all / counter;
                        T.down_centerpoint_list.Add(new Point(down_x_add_all, T.ymx));
                        down_x_add_all = T.down_point_list[l].X;
                        counter = 1;
                    }
                    if (l == T.down_point_list.Count - 1)
                    {
                        down_x_add_all = down_x_add_all / counter;
                        T.down_centerpoint_list.Add(new Point(down_x_add_all, T.ymx));
                        down_x_add_all = 0;
                        counter = 0;
                    }
                }
                #endregion
                #endregion
                #region 刪除重複點
                for (int i = T.left_centerpoint_list.Count - 1; i > 0; i--)
                {
                    if (T.left_centerpoint_list[i].Y == T.left_centerpoint_list[i - 1].Y)
                        T.left_centerpoint_list.Remove(T.left_centerpoint_list[i]);
                }
                for (int i = T.right_centerpoint_list.Count - 1; i > 0; i--)
                {
                    if (T.right_centerpoint_list[i].Y == T.right_centerpoint_list[i - 1].Y)
                        T.right_centerpoint_list.Remove(T.right_centerpoint_list[i]);
                }
                for (int i = T.top_centerpoint_list.Count - 1; i > 0; i--)
                {
                    if (T.top_centerpoint_list[i].X == T.top_centerpoint_list[i - 1].X)
                        T.top_centerpoint_list.Remove(T.top_centerpoint_list[i]);
                }
                for (int i = T.down_centerpoint_list.Count - 1; i > 0; i--)
                {
                    if (T.down_centerpoint_list[i].X == T.down_centerpoint_list[i - 1].X)
                        T.down_centerpoint_list.Remove(T.down_centerpoint_list[i]);
                }
                #endregion
                //if ((T.left_point_list.Count * 1.2 <= T.top_point_list.Count || T.top_point_list.Count <= T.left_point_list.Count * 0.75 ||
                //    T.right_point_list.Count * 1.2 <= T.top_point_list.Count || T.top_point_list.Count <= T.right_point_list.Count * 0.75 ||
                //    T.down_point_list.Count * 1.2 <= T.top_point_list.Count || T.top_point_list.Count <= T.down_point_list.Count * 0.75) &&
                //    (T.top_point_list.Count > 20) && (T.left_point_list.Count > 20) && (T.right_point_list.Count > 20) && (T.down_point_list.Count > 20)
                //    )
                //{
                //    MessageBox.Show("切割道四邊長度不一致");
                //    return;
                //}


                #region 找水平與垂直線
                #region 找水平線
                for (int w = 0; w < T.left_centerpoint_list.Count; w++)
                {
                    for (int s = 0; s < T.right_centerpoint_list.Count; s++)
                    {
                        List<Point> Point_Set = new List<Point>();
                        int error_point = 0;
                        for (int r = T.xmn; r < T.xmx + 1; r++)
                        {
                            Point p_H = new Point();
                            p_H = Point.Round(GetHorizontalLineY(T.left_centerpoint_list[w], T.right_centerpoint_list[s], r));
                            if (Tbin[r, p_H.Y] != 1)
                            {
                                //PointInLine = false;
                                //break; 
                                error_point = error_point + 1;
                            }
                        }
                        if (error_point < f.minWidth)
                        {
                            Point_Set.Add(T.left_centerpoint_list[w]);
                            Point_Set.Add(T.right_centerpoint_list[s]);
                            Hor_Line.Add(Point_Set);
                        }
                    }
                }
                #endregion
                #region 找垂直線
                for (int w = 0; w < T.top_centerpoint_list.Count; w++)
                {
                    for (int s = 0; s < T.down_centerpoint_list.Count; s++)
                    {
                        List<Point> Point_Set = new List<Point>();
                        int error_point = 0;
                        for (int r = T.ymn; r < T.ymx + 1; r++)
                        {
                            Point p_H = new Point();
                            p_H = Point.Round(GetVerticalLineX(T.top_centerpoint_list[w], T.down_centerpoint_list[s], r));
                            if (Tbin[p_H.X, r] != 1)
                            {
                                //PointInLine = false;
                                //break;
                                error_point = error_point + 1;
                            }
                        }
                        if (error_point < f.minHeight)
                        {
                            Point_Set.Add(T.top_centerpoint_list[w]);
                            Point_Set.Add(T.down_centerpoint_list[s]);
                            Ver_Line.Add(Point_Set);
                        }
                    }
                }
                #endregion
                #endregion
                #region 只找到垂直切割道
                if (Ver_Line.Count != 0 && Hor_Line.Count == 0)
                {
                    for (int i = 0; i < Ver_Line.Count; i++)
                    {
                        angle_List.Add(Angle(Ver_Line[i][0], Ver_Line[i][1], new Point(Ver_Line[i][0].X, Ver_Line[i][0].Y + 1)));
                    }
                    listBox1.Items.Clear();
                    listBox1.Items.Add("只找到垂直切割道共" + Ver_Line.Count.ToString() + "條");
                    for (int i = 0; i < angle_List.Count; i++)
                    {
                        show = "垂直線" + ((i + 1).ToString()) + "角度=" + angle_List[i].ToString() + "上=" + Ver_Line[i][0].ToString() + "下=" + Ver_Line[i][1].ToString();
                        listBox1.Items.Add(show);
                        angle_Average = angle_Average + angle_List[i];
                    }
                    angle_Average = angle_Average / angle_List.Count;
                    show = "平均角度=" + angle_Average.ToString();
                    listBox1.Items.Add(show);

                    //繪製有效目標
                    Bitmap bmp = new Bitmap(f.image_width, f.image_height);
                    for (int m = 0; m < T.P.Count; m++)
                    {
                        Point p = (Point)T.P[m];
                        bmp.SetPixel(p.X, p.Y, Color.Black);
                    }
                    // Draw a line between the points.
                    Graphics g = Graphics.FromImage(bmp);
                    for (int i = 0; i < Ver_Line.Count; i++)
                    {
                        g.DrawLine(new Pen(Color.Red), Ver_Line[i][0], Ver_Line[i][1]);
                    }
                    pictureBox1.Image = bmp;
                    Mb = (Bitmap)bmp.Clone();
                    return;
                }
                #endregion
                #region 只找到水平切割道
                if (Ver_Line.Count == 0 && Hor_Line.Count != 0)
                {
                    for (int i = 0; i < Hor_Line.Count; i++)
                    {
                        angle_List.Add(Angle(Hor_Line[i][0], Hor_Line[i][1], new Point(Hor_Line[i][0].X + 1, Hor_Line[i][0].Y)));
                    }
                    listBox1.Items.Clear();
                    listBox1.Items.Add("只找到水平切割道" + Hor_Line.Count.ToString() + "條");
                    for (int i = 0; i < angle_List.Count; i++)
                    {
                        show = "水平線" + ((i + 1).ToString()) + "角度=" + angle_List[i].ToString() + "上=" + Hor_Line[i][0].ToString() + "下=" + Hor_Line[i][1].ToString();
                        listBox1.Items.Add(show);
                        angle_Average = angle_Average + angle_List[i];
                    }
                    angle_Average = angle_Average / angle_List.Count;
                    show = "平均角度=" + angle_Average.ToString();
                    listBox1.Items.Add(show);

                    //繪製有效目標
                    Bitmap bmp = new Bitmap(f.image_width, f.image_height);
                    for (int m = 0; m < T.P.Count; m++)
                    {
                        Point p = (Point)T.P[m];
                        bmp.SetPixel(p.X, p.Y, Color.Black);
                    }
                    // Draw a line between the points.
                    Graphics g = Graphics.FromImage(bmp);
                    for (int i = 0; i < Hor_Line.Count; i++)
                    {
                        g.DrawLine(new Pen(Color.Red), Hor_Line[i][0], Hor_Line[i][1]);
                    }
                    pictureBox1.Image = bmp;
                    Mb = (Bitmap)bmp.Clone();
                    return;
                }
                #endregion
                if (Ver_Line.Count == 0 && Hor_Line.Count == 0)
                {
                    MessageBox.Show("找不到水平或垂直切割道");
                    return;
                }
                #region 找直線交點
                if (Ver_Line.Count > 0 && Hor_Line.Count > 0)
                {
                    for (int h = 0; h < Hor_Line.Count; h++)
                    {
                        for (int s = 0; s < Ver_Line.Count; s++)
                        {
                            List<Point> centerPoint_Set = new List<Point>();
                            centerpoint = Point.Round(GetIntersection(Hor_Line[h][0], Hor_Line[h][1], Ver_Line[s][0], Ver_Line[s][1]));
                            angle_List.Add(Angle(centerpoint, Hor_Line[h][1], new Point(centerpoint.X + 1, centerpoint.Y)));
                            centerPoint_Set.Add(centerpoint);
                            centerPoint_Set.Add(Hor_Line[h][0]);
                            centerPoint_Set.Add(Hor_Line[h][1]);
                            centerPoint_Set.Add(Ver_Line[s][0]);
                            centerPoint_Set.Add(Ver_Line[s][1]);
                            centerpoint_list.Add(centerPoint_Set);
                        }
                    }
                    listBox1.Items.Clear();
                    listBox1.Items.Add("找到交點共" + centerpoint_list.Count.ToString() + "點");
                    for (int i = 0; i < angle_List.Count; i++)
                    {
                        show = "點" + ((i + 1).ToString()) + "角度=" + angle_List[i].ToString() + "中=" + centerpoint_list[i][0].ToString() + "左=" + centerpoint_list[i][1].ToString() + "右=" + centerpoint_list[i][2].ToString() + "上=" + centerpoint_list[i][3].ToString() + "下=" + centerpoint_list[i][4].ToString();
                        listBox1.Items.Add(show);
                        angle_Average = angle_Average + angle_List[i];
                    }
                    angle_Average = angle_Average / angle_List.Count;
                    show = "平均角度=" + angle_Average.ToString();
                    listBox1.Items.Add(show);
                    
                    show = "Width=" + FindDieSize(centerpoint_list, Hor_Line.Count, Ver_Line.Count,440,400)[0].ToString();
                    listBox1.Items.Add(show);
                    show = "Height=" + FindDieSize(centerpoint_list, Hor_Line.Count, Ver_Line.Count, 440, 400)[1].ToString();
                    show = "距離中心最近點X距離=" + GetNearestIntersectPointToCenterDistance(centerpoint_list)[0].ToString()
                        +",Y距離=" + GetNearestIntersectPointToCenterDistance(centerpoint_list)[1].ToString();
                    listBox1.Items.Add(show);
                    //繪製有效目標
                    Bitmap bmp = new Bitmap(f.image_width, f.image_height);
                    for (int m = 0; m < T.P.Count; m++)
                    {
                        Point p = (Point)T.P[m];
                        bmp.SetPixel(p.X, p.Y, Color.Black);
                    }
                    // Draw a line between the points.
                    Graphics g = Graphics.FromImage(bmp);
                    for (int i = 0; i < centerpoint_list.Count; i++)
                    {
                        g.DrawLine(new Pen(Color.Yellow, 5), centerpoint_list[i][0], new Point(centerpoint_list[i][0].X + 20, centerpoint_list[i][0].Y));
                        g.DrawLine(new Pen(Color.Yellow, 5), centerpoint_list[i][0], new Point(centerpoint_list[i][0].X - 20, centerpoint_list[i][0].Y));
                        g.DrawLine(new Pen(Color.Yellow, 5), centerpoint_list[i][0], new Point(centerpoint_list[i][0].X, centerpoint_list[i][0].Y + 20));
                        g.DrawLine(new Pen(Color.Yellow, 5), centerpoint_list[i][0], new Point(centerpoint_list[i][0].X, centerpoint_list[i][0].Y - 20));
                    }
                    pictureBox1.Image = bmp;
                    Mb = (Bitmap)bmp.Clone();
                    return;
                }
                #endregion

            }
        }
        #endregion

        #region GetIntersection
        public static PointF GetIntersection(PointF lineFirstStar, PointF lineFirstEnd, PointF lineSecondStar, PointF lineSecondEnd)
        {
            /*
             * L1，L2都存在斜率的情況：
             * 直線方程L1: ( y - y1 ) / ( y2 - y1 ) = ( x - x1 ) / ( x2 - x1 ) 
             * => y = [ ( y2 - y1 ) / ( x2 - x1 ) ]( x - x1 ) + y1
             * 令 a = ( y2 - y1 ) / ( x2 - x1 )
             * 有 y = a * x - a * x1 + y1   .........1
             * 直線方程L2: ( y - y3 ) / ( y4 - y3 ) = ( x - x3 ) / ( x4 - x3 )
             * 令 b = ( y4 - y3 ) / ( x4 - x3 )
             * 有 y = b * x - b * x3 + y3 ..........2
             * 
             * 如果 a = b，則兩直線平等，否則， 聯解方程 1,2，得:
             * x = ( a * x1 - b * x3 - y1 + y3 ) / ( a - b )
             * y = a * x - a * x1 + y1
             * 
             * L1存在斜率, L2平行Y軸的情況：
             * x = x3
             * y = a * x3 - a * x1 + y1
             * 
             * L1 平行Y軸，L2存在斜率的情況：
             * x = x1
             * y = b * x - b * x3 + y3
             * 
             * L1與L2都平行Y軸的情況：
             * 如果 x1 = x3，那麼L1與L2重合，否則平等
             * 
            */
            float a = 0, b = 0;
            int state = 0;
            if (lineFirstStar.X != lineFirstEnd.X)
            {
                a = (lineFirstEnd.Y - lineFirstStar.Y) / (lineFirstEnd.X - lineFirstStar.X);
                state |= 1;
            }
            if (lineSecondStar.X != lineSecondEnd.X)
            {
                b = (lineSecondEnd.Y - lineSecondStar.Y) / (lineSecondEnd.X - lineSecondStar.X);
                state |= 2;
            }
            switch (state)
            {
                case 0: //L1與L2都平行Y軸
                    {
                        if (lineFirstStar.X == lineSecondStar.X)
                        {
                            //throw new Exception("兩條直線互相重合，且平行於Y軸，無法計算交點。");
                            return new PointF(0, 0);
                        }
                        else
                        {
                            //throw new Exception("兩條直線互相平行，且平行於Y軸，無法計算交點。");
                            return new PointF(0, 0);
                        }
                    }
                case 1: //L1存在斜率, L2平行Y軸
                    {
                        float x = lineSecondStar.X;
                        float y = (lineFirstStar.X - x) * (-a) + lineFirstStar.Y;
                        return new PointF(x, y);
                    }
                case 2: //L1 平行Y軸，L2存在斜率
                    {
                        float x = lineFirstStar.X;
                        //網上有相似代碼的，這一處是錯誤的。你可以對比case 1 的邏輯 進行分析
                        //源code:lineSecondStar * x + lineSecondStar * lineSecondStar.X + p3.Y;
                        float y = (lineSecondStar.X - x) * (-b) + lineSecondStar.Y;
                        return new PointF(x, y);
                    }
                case 3: //L1，L2都存在斜率
                    {
                        if (a == b)
                        {
                            // throw new Exception("兩條直線平行或重合，無法計算交點。");
                            return new PointF(0, 0);
                        }
                        float x = (a * lineFirstStar.X - b * lineSecondStar.X - lineFirstStar.Y + lineSecondStar.Y) / (a - b);
                        float y = a * x - a * lineFirstStar.X + lineFirstStar.Y;
                        return new PointF(x, y);
                    }
            }
            // throw new Exception("不可能發生的情況");
            return new PointF(0, 0);
        }
        #endregion
        #region GetHorizontalLineY
        public static PointF GetHorizontalLineY(PointF Point1, PointF Point2, int x)
        {
            /*
               y=mx+b             
            */
            float m = 0, b = 0;
            m = (Point2.Y - Point1.Y) / (Point2.X - Point1.X);
            b = Point1.Y - m * Point1.X;
            if (float.IsInfinity(m))
            {
                return new PointF(0, 0);
            }
            return new PointF(x, m * x + b);
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }


        #endregion
        #region GetVerticalLineX
        public static PointF GetVerticalLineX(PointF Point1, PointF Point2, int y)
        {
            /*
               y=mx+b             
            */
            float m = 0, b = 0;
            if (Point2.X == Point1.X)
            {
                return new PointF(Point1.X, y);
            }
            m = (Point2.Y - Point1.Y) / (Point2.X - Point1.X);
            b = Point1.Y - m * Point1.X;
            if (float.IsInfinity(m))
            {
                return new PointF(0, 0);
            }
            return new PointF((y - b) / m, y);
        }

        private void laplaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Bitmap bmp = new Bitmap(openFileDialog1.FileName);
                //pictureBox1.Image = bmp;
                listBox1.Items.Add("Laplacian Score=" + f.Laplacian(bmp).ToString());
            }
        }
        #endregion
        #region Angle
        public static float Angle(Point cen, Point first, Point second)
        {
            float dx1, dx2, dy1, dy2;
            float angle;

            dx1 = first.X - cen.X;
            dy1 = first.Y - cen.Y;

            dx2 = second.X - cen.X;

            dy2 = second.Y - cen.Y;

            float c = (float)Math.Sqrt(dx1 * dx1 + dy1 * dy1) * (float)Math.Sqrt(dx2 * dx2 + dy2 * dy2);

            if (c == 0) return -1;

            angle = (float)((Math.Acos((dx1 * dx2 + dy1 * dy2) / c))*(180 / Math.PI));//radian To Degree


            return (second.Y > first.Y) ? angle : -angle;
        }
        #endregion
        #region FindDieSize
        private int[] FindDieSize(List<List<Point>> centerpoint, int HorLineNumber, int VerLineNumber, int DieWidth, int DieHeight)
        {
            double TotalAddWidth = 0;
            double TotalAddHeight = 0;
            double width = 0;
            double height = 0;
            int[] dieWidthHeight = new int[2];
            List<double> Width_List = new List<double>();
            List<double> Height_List = new List<double>();
            double DieSizeDifference = 0.1;

            //至少要有四邊才能形成一顆Die
            //沒有切割道DieSize回傳0
            if (HorLineNumber == 0 && VerLineNumber == 0)
            {
                dieWidthHeight[0] = 0;
                dieWidthHeight[1] = 0;
                return dieWidthHeight;
            }
            //水平或垂直切割道數量只有一條
            if (HorLineNumber == 1 || VerLineNumber == 1)
            {
                dieWidthHeight[0] = DieWidth;
                dieWidthHeight[1] = DieHeight;
                return dieWidthHeight;
            }

            #region FindWidth
            for (int k = 0; k < HorLineNumber; k++)
            {
                for (int i = 0+k*VerLineNumber ; i < (k+1)*VerLineNumber - 1; i++)//3條線算2個距離
                {
                    width = TwoPointFindDistance(centerpoint[i][0], centerpoint[i + 1][0]);
                    if (width < DieWidth * (1 + DieSizeDifference) && width > DieWidth * (1 - DieSizeDifference))//計算距離跟給的距離差距不大就視為正確
                    {
                        Width_List.Add(width);
                    }
                }
            }
            #endregion
            #region FindHeight
            for (int k = 0; k < VerLineNumber; k++)
            {
                for (int i = 0 + k; i < VerLineNumber * (HorLineNumber-1); i+=VerLineNumber)//3條線算2個距離
                {
                    height = TwoPointFindDistance(centerpoint[i][0], centerpoint[i + VerLineNumber][0]);
                    if (height < DieHeight * (1 + DieSizeDifference) && height > DieHeight * (1 - DieSizeDifference))//計算距離跟給的距離差距不大就視為正確
                    {
                        Height_List.Add(height);
                    }
                }
            }
            #endregion
            #region Calculate Average Width And Height
            for (int i = 0; i < Width_List.Count; i++)
            {
                TotalAddWidth = TotalAddWidth + Width_List[i];
            }
            for (int i = 0; i < Height_List.Count; i++)
            {
                TotalAddHeight = TotalAddHeight + Height_List[i];
            }
            dieWidthHeight[0] = (int)(TotalAddWidth / Width_List.Count);
            dieWidthHeight[1] = (int)(TotalAddHeight / Height_List.Count);
            #endregion
            return dieWidthHeight;
        }
        #endregion
        #region TwoPointFindDistance
        public double TwoPointFindDistance(Point FirstP, Point SecondP)
        {
            int x = Math.Abs(SecondP.X - FirstP.X);
            int y = Math.Abs(SecondP.Y - FirstP.Y);
            return Math.Sqrt(x * x + y * y);
        }
        #endregion

        #region Negative
        //負片
        private byte[,] Negative(byte[,] b)
        {
            for (int i = 0; i < f.image_width; i++)
            {
                for (int j = 0; j < f.image_height; j++)
                {
                    b[i, j] = (byte)(255 - b[i, j]);
                }
            }
            return b;
        }
        #endregion
        #region Fill
        private byte[,] Fill(List<Point> a)
        {
            Tbin = new byte[f.image_width, f.image_height];//選取目標的二值化陣列
            for (int n = 0; n < a.Count; n++)
            {
                Point p = (Point)a[n];
                Tbin[p.X, p.Y] = 1;//起點
                Tbin[p.X + 1, p.Y] = 1;
                Tbin[p.X - 1, p.Y] = 1;
                Tbin[p.X, p.Y + 1] = 1;
                Tbin[p.X, p.Y - 1] = 1;
                Tbin[p.X + 1, p.Y + 1] = 1;
                Tbin[p.X + 1, p.Y - 1] = 1;
                Tbin[p.X - 1, p.Y + 1] = 1;
                Tbin[p.X - 1, p.Y - 1] = 1;
            }
            return Tbin;
        }
        #endregion
        #region GetNearestIntersectPointToCenterDistance
        public int[] GetNearestIntersectPointToCenterDistance(List<List<Point>> intersect_list)
        {
            int[] PointToCenterXYinPixel = { 0, 0 };
            double PointToCenterDistance = Math.Sqrt(f.image_width * f.image_width + f.image_height * f.image_height);
            Point CenterPoint = new Point(f.image_width / 2, f.image_height / 2);
            Point NearestCenterPoint = CenterPoint;
            for (int i = 0; i < intersect_list.Count; i++)
            {
                if (TwoPointFindDistance(intersect_list[i][0], CenterPoint) < PointToCenterDistance)
                {
                    PointToCenterDistance = TwoPointFindDistance(intersect_list[i][0], CenterPoint);
                    NearestCenterPoint = intersect_list[i][0];
                }
            }
            PointToCenterXYinPixel[0] = CenterPoint.X - NearestCenterPoint.X;
            PointToCenterXYinPixel[1] = CenterPoint.Y - NearestCenterPoint.Y;
            return PointToCenterXYinPixel;
        }
        #endregion

        private void RegionMarkMenuItem_Click(object sender, EventArgs e)
        {
            ArrayList D = new ArrayList();
            C = getTargets(array_Outline); //建立目標物件集合
            listBox1.Items.Clear();

            #region 過濾長寬不夠之物件
            for (int k = 0; k < C.Count; k++)
            {
                TgInfo T = (TgInfo)C[k];
                if (T.height > f.maxHeight || T.width > f.maxWidth) D.Add(T);
            }
            if (D.Count == 0)
            {
                MessageBox.Show("找不到符合長度的切割道");
            }
            else
            {
                C = D;
            }
            #endregion
            //繪製目標輪廓點
            Bitmap bmp = new Bitmap(f.image_width, f.image_height);
            pictureBox1.Image = bmp;
            Color[] colors = { Color.Red, Color.Blue, Color.Green
                             , Color.Brown, Color.Orange, Color.SkyBlue
                             , Color.LightCoral, Color.MediumPurple };
            int color_idx = 0;
            for (int k = 0; k < C.Count; k++)
            {
                TgInfo T = (TgInfo)C[k];
                if (T.P.Count > 10000)
                {
                    if (color_idx >= colors.Length) color_idx = 0;
                    Console.WriteLine("[RegionMark]:C[" + k.ToString() + "]=" + T.P.Count.ToString());
                    for (int m = 0; m < T.P.Count; m++)
                    {
                        System.Drawing.Point p = (System.Drawing.Point)T.P[m];
                        bmp.SetPixel(p.X, p.Y, colors[color_idx]);
                    }
                    color_idx++;
                }
            }
            listBox1.Items.Add("物件數量" + C.Count.ToString());
            pictureBox1.Image = bmp;
        }
        readonly int _left = 0;
        readonly int _right = 1;
        readonly int _upper = 2;
        readonly int _lower = 3;
        readonly int boundary_num = 4;
        ProjSorter[] boundary;

        private void scribeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            boundary = new ProjSorter[boundary_num];
            boundary[_left] = new ProjSorter(1, 3);
            boundary[_right] = new ProjSorter(f.image_width - 4, f.image_width - 2);
            boundary[_upper] = new ProjSorter(1, 3);
            boundary[_lower] = new ProjSorter(f.image_height - 4, f.image_height - 2);

            bool[] need_set = { true, true, true, true };
            FindEndPoint(need_set);
            bool redo = false;
            for (int i=0; i<boundary_num; i++)
            {
                Console.WriteLine("Boundary " + i.ToString());
                boundary[i].Trim();
                if (boundary[i].NeedReAdd)
                {
                    redo = true;
                    boundary[i].EnlargeRange();
                }
                need_set[i] = boundary[i].NeedReAdd;
            }
            if (redo)
            {
                FindEndPoint(need_set);
                redo = false;
                for (int i = 0; i < boundary_num; i++)
                {
                    boundary[i].Trim();
                    if (boundary[i].NeedReAdd)
                        redo = true;
                }
            }
            if (redo)
            {
                MessageBox.Show("找不到適當的切割道");
                return;
            }
            for (int i = 0; i < 4; i+=2)
            {
                if (boundary[i].LineCount != boundary[i+1].LineCount)
                    boundary[i].Adjust(boundary[i+1]);
            }
            //繪製目標輪廓點
            Bitmap bmp = new Bitmap(f.image_width, f.image_height);
            Graphics g = Graphics.FromImage(bmp);
            for (int i = 0; i < boundary[_upper].LineCount; i++)
            {
                g.DrawLine(new Pen(Color.YellowGreen, 5)
                            ,0 , boundary[_upper].Lines[i].Mid
                            , f.image_width - 1, boundary[_lower].Lines[i].Mid);
            }
            for (int i = 0; i < boundary[_right].LineCount; i++)
            {
                g.DrawLine(new Pen(Color.YellowGreen, 5)
                            , boundary[_left].Lines[i].Mid, 0
                            , boundary[_right].Lines[i].Mid, f.image_height - 1);
            }
            pictureBox1.Image = bmp;
        }
        private void FindEndPoint(bool[] need_set)
        {
            for (int i=0; i<boundary_num; i++)
            {
                boundary[i].Clear();
            }
            for (int k = 0; k < C.Count; k++)
            {
                TgInfo T = (TgInfo)C[k];
                for (int m = 0; m < T.P.Count; m++)
                {
                    Point p = (Point)T.P[m];
                    for (int i = 0; i < 2; i++)
                    {
                        if (need_set[i]) boundary[i].SetInRange(p.Y, p.X);
                        if (need_set[i + 2]) boundary[i + 2].SetInRange(p.X, p.Y);
                    }
                }
            }

        }
    }

    class ProjSorter
    {
        public ProjSorter(int begin, int end)
        {
            AssoPosBegin = begin;
            AssoPosEnd = end;
        }
        List<int> values = new List<int>();
        List<LinePair> lines = new List<LinePair>();
        public List<LinePair> Lines { get { return lines; } }
        public int LineCount { get { return lines.Count; } }
        public int LineWidth { get; set; }
        private int DieWidth { get; set; }
        const int MinGap = 3;
        const int MaxLineWidth = 300;
        const int MinLineWidth = 10;

        public int AssoPosBegin { get; set; }
        public int AssoPosEnd { get; set; }
        public bool NeedReAdd { get; private set; }
        public void EnlargeRange()
        {
            if (AssoPosBegin <= MaxLineWidth)
            {
                AssoPosBegin += MaxLineWidth;
                AssoPosEnd += MaxLineWidth;
            }
            else
            {
                AssoPosBegin -= MaxLineWidth;
                AssoPosEnd -= MaxLineWidth;
            }
        }
        public void Clear()
        {
            values.Clear();
            lines.Clear();
        }
        public void SetInRange(int pos, int c)
        {
            if (c >= AssoPosBegin && c <= AssoPosEnd) 
                values.Add(pos);
        }
        public void CondenseLines()
        {
            lines.Clear();
            values.Sort();
            int start = 0;
            int width = 0;
            bool UnqualifiedlineFound = false;//用來記錄是否超過臨界值的點都不夠連續

            for (int i = 1; i < values.Count; i++)
            {
                if (values[i] - values[i - 1] > MinGap)
                {
                    if (values[i - 1] - values[start] > MinLineWidth)
                    {
                        Console.WriteLine(String.Format("Get Line [{0}-{1}]", values[start], values[i - 1]));
                        LinePair p = new LinePair(values[start], values[i - 1]);
                        lines.Add(p);

                        width += p.Second - p.First;
                    }
                    else
                    {
                        Console.WriteLine(String.Format("Line width too Short [{0}-{1}]", values[start], values[i-1]));
                        UnqualifiedlineFound = true;
                    }
                    start = i;
                }
                else
                {
                    //Console.WriteLine(String.Format("value[{0}].Pos={1}", i, values[i]));
                }
            }
            if (lines.Count == 0)
            {
                Console.WriteLine(String.Format("No Gap between Points [{0}-{1}]", values[0], values[values.Count - 1]));
                /*
                  * start==0 && UnqualifiedlineFound
                  * =>幾乎整條都超過threshold，設定單條line-
                  * =>會在LineTooWide地判斷中淘汰
                  * start>0
                  * =>影像中只有一條切割道
                  */
                if (start > 0 || !UnqualifiedlineFound)
                {
                    LinePair p = new LinePair(values[start], values[values.Count - 1]);
                    lines.Add(p);
                    width = p.Second - p.First;
                }
            }
            //將屬於同一個切割道的數條LINE合併
            int cur = 0;
            int width1 = 0;
            while (cur < lines.Count - 1)
            {
                if ((lines[cur + 1].First - lines[cur].Second) < MaxLineWidth)
                {
                    LinePair p = lines[cur + 1];
                    lines.RemoveAt(cur + 1);
                    lines[cur].Merge(p);
                }
                else
                {
                    width1 += lines[cur].Second - lines[cur].First;
                    cur++;
                }
            }
            if (width1 > 0) width = width1;
            LineWidth = (int)((float)width / lines.Count + 0.5);
            if (lines.Count > 2)
            {
                DieWidth = (lines[lines.Count - 1].First - lines[0].Second - (lines.Count - 2) * LineWidth)
                            / (lines.Count - 1);
            }
            else
            {
                DieWidth = lines.Count * 2;
            }
        }
        private bool LineTooWide()
        {
            foreach (var v in lines)
            {
                if ((v.Second - v.First) > MaxLineWidth)
                {
                    Console.WriteLine("切割道寬度過大:" + (v.Second - v.First).ToString());
                    NeedReAdd = true;
                    return true;
                }
            }
            NeedReAdd = false;
            return false;
        }
        public bool Trim()
        {
            CondenseLines();
            if (lines.Count == 0)
            {
                Console.WriteLine("找不到足夠寬度的切割道");
                return false;
            }
            if (!LineTooWide())
            {
                Console.WriteLine("找到切割道；切割道寬度為：" + LineWidth.ToString());
                return true;
            }
            Console.WriteLine("找到的切割道寬度過大或其他問題；切割道寬度為：" + LineWidth.ToString());
            return false;
        }
        public void Adjust(ProjSorter s)
        {
            int cnt = 0;
            int target = 0;
            int th = MaxLineWidth * 3;

            //移除本身多餘的線段
            while (cnt < LineCount)
            {
                while (target < s.LineCount)
                {
                    if (Math.Abs(lines[cnt].Mid - s.Lines[target].Mid) < th)
                    {
                        lines[cnt].MappingTo = target;
                        s.lines[target].MappingTo = cnt;
                        break;
                    }
                    if (cnt < lines.Count - 1
                        && s.Lines[target].Second > lines[cnt + 1].First) break;
                    target++;
                }
                if (lines[cnt].MappingTo == -1) //Not Found
                {
                    lines.RemoveAt(cnt);
                }
                else
                {
                    target = lines[cnt].MappingTo;
                    cnt++;
                }
            }
            //移除s多餘的線段
            cnt = 0;
            while (cnt < s.LineCount)
            {
                if (s.Lines[cnt].MappingTo == -1)
                {
                    s.Lines.RemoveAt(cnt);
                }
                else
                {
                    cnt++;
                }
            }
        }
    }
    class ProjValue
    {
        public int Pos { get; set; }
        public bool HasValue { get; set; }
        public ProjValue(int p)
        {
            HasValue = false;
            Pos = p;
        }
    }
    class LinePair
    {
        public int Mid { get; private set; }
        public int MappingTo { get; set; }
        public LinePair()
        {
            MappingTo = -1;
        }
        public LinePair(int first, int second)
        {
            First = first;
            Second = second;
            MappingTo = -1;
            Mid = (First + Second) / 2;
        }
        public void Merge(LinePair p)
        {
            First = Math.Min(First, p.First);
            Second = Math.Max(Second, p.Second);
            Mid = (First + Second) / 2;
        }
        public int First { get; set; }
        public int Second { get; set; }
    }
}
