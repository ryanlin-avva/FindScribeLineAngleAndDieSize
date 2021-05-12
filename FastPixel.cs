using System;
using System.Collections;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Imaging;

namespace CsRGBshow
{
    //Fast RGB Processing Class
    class FastPixel
    {
        public int nx, ny; //影像寬與高
        public byte[,] array_Red, array_Green, array_Blue; //Red, Green & Blue 陣列
        public byte[,] array_Gray; //灰階陣列
        byte[] array_RGB; //影像的可存取副本資料陣列
        System.Drawing.Imaging.BitmapData D; //影像資料
        IntPtr ptr; //影像資料所在的記憶體指標(位置)
        int n, L, nB; //影像總位元組數，單行位元組數，單點位元組數
        public Bitmap image;
        int[] LaplacianMask ={
                    -1,-1,-1,
                    -1,8,-1,
                    -1,-1,-1,
            };



        //鎖定點陣圖(Bitmap)物件的記憶體位置，建立一個可操作的位元組陣列副本
        private void LockBMP(Bitmap bmp)
        {
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height); //矩形物件，定義影像範圍
            //鎖定影像區記憶體(暫時不接受作業系統的移動)
            D = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
            ptr = D.Scan0; //影像區塊的記憶體指標
            L = D.Stride; //每一影像列的長度(bytes)
            nB = (int)Math.Floor((double)L / (double)bmp.Width); //每一像素的位元組數(3或4)
            n = L * bmp.Height; //影像總位元組數
            array_RGB = new byte[n]; //宣告影像副本資料陣列
            System.Runtime.InteropServices.Marshal.Copy(ptr, array_RGB, 0, n); //拷貝點陣圖資料到副本陣列
        }

        //複製位元組陣列副本的處理結果到Bitmap物件，並解除其記憶體鎖定
        private void UnLockBMP(Bitmap bmp)
        {
            System.Runtime.InteropServices.Marshal.Copy(array_RGB, 0, ptr, n); //拷貝副本陣列到點陣圖位置
            bmp.UnlockBits(D); //解除鎖定
        }

        //取得RGB陣列
        public void Bmp2RGB(Bitmap bmp)
        {
            nx = bmp.Width; ny = bmp.Height; //影像寬高
            array_Red = new byte[nx, ny]; array_Green = new byte[nx, ny]; array_Blue = new byte[nx, ny]; //RGB
            LockBMP(bmp);
            if(bmp.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                for (int j = 0; j < ny; j++)
                {
                    int Lj = j * D.Stride;
                    for (int i = 0; i < nx; i++)
                    {
                        int k = Lj + i * nB;
                       
                        array_Green[i, j] = array_RGB[k]; //Green
                    }
                }

            }
            if (bmp.PixelFormat == PixelFormat.Format24bppRgb)
            {
                for (int j = 0; j < ny; j++)
                {
                    int Lj = j * D.Stride;
                    for (int i = 0; i < nx; i++)
                    {
                        int k = Lj + i * nB;
                        array_Red[i, j] = array_RGB[k + 2]; //Red
                        array_Green[i, j] = array_RGB[k + 1]; //Green
                        array_Blue[i, j] = array_RGB[k]; //Blue
                    }
                }

            }
            if (bmp.PixelFormat == PixelFormat.Format32bppRgb)
            {
                for (int j = 0; j < ny; j++)
                {
                    int Lj = j * D.Stride;
                    for (int i = 0; i < nx; i++)
                    {
                        int k = Lj + i * nB;
                        array_Red[i, j] = array_RGB[k + 2]; //Red
                        array_Green[i, j] = array_RGB[k + 1]; //Green
                        array_Blue[i, j] = array_RGB[k]; //Blue
                    }
                }

            }
            UnLockBMP(bmp);
        }

        //灰階圖
        public Bitmap GrayImg(byte[,] b)
        {
            Bitmap bmp = new Bitmap(b.GetLength(0), b.GetLength(1));
            LockBMP(bmp);
            for (int j = 0; j < b.GetLength(1); j++)
            {
                for (int i = 0; i < b.GetLength(0); i++)
                {
                    int k = j * L + i * nB;
                    byte c = b[i, j];
                    array_RGB[k] = c; array_RGB[k + 1] = c; array_RGB[k + 2] = c; //RGB一致
                    array_RGB[k + 3] = 255; //實心不透明
                }
            }
            UnLockBMP(bmp);
            return bmp;
        }
        //黑白圖
        public Bitmap BWImg(byte[,] b)
        {
            Bitmap bmp = new Bitmap(b.GetLength(0), b.GetLength(1));
            LockBMP(bmp);
            for (int j = 0; j < b.GetLength(1); j++)
            {
                for (int i = 0; i < b.GetLength(0); i++)
                {
                    int k = j * L + i * nB;
                    if (b[i, j] == 1)
                    {
                        array_RGB[k] = 0; array_RGB[k + 1] = 0; array_RGB[k + 2] = 0; //黑
                    }
                    else
                    {
                        array_RGB[k] = 255; array_RGB[k + 1] = 255; array_RGB[k + 2] = 255; //白
                    }
                    array_RGB[k + 3] = 255;
                }
            }
            UnLockBMP(bmp);
            return bmp;
        }


        public int Laplacian(Bitmap bitmap)
        {
            int width = bitmap.Width, height = bitmap.Height;
            int score_laplacian=0;

            Bmp2RGB(bitmap);
            LockBMP(bitmap);
            for (int y = 1; y < (height - 1); y++)
            {
                for (int x = 1; x < (width - 1); x++)
                {
                    score_laplacian += (array_Red[x, y] + array_Green[x, y] + array_Blue[x, y]) * LaplacianMask[4] +
                                       (array_Red[x-1, y-1] + array_Green[x - 1, y - 1] + array_Blue[x - 1, y - 1]) * LaplacianMask[0] +
                                       (array_Red[x, y-1] + array_Green[x, y - 1] + array_Blue[x, y - 1]) * LaplacianMask[1] +
                                       (array_Red[x+1, y-1] + array_Green[x + 1, y - 1] + array_Blue[x + 1, y - 1]) * LaplacianMask[2] +
                                       (array_Red[x-1, y] + array_Green[x - 1, y] + array_Blue[x - 1, y]) * LaplacianMask[3] +
                                       (array_Red[x+1, y] + array_Green[x + 1, y] + array_Blue[x + 1, y]) * LaplacianMask[5] +
                                       (array_Red[x-1, y+1] + array_Green[x - 1, y + 1] + array_Blue[x - 1, y + 1]) * LaplacianMask[6] +
                                       (array_Red[x, y+1] + array_Green[x, y + 1] + array_Blue[x, y + 1]) * LaplacianMask[7] +
                                       (array_Red[x+1, y+1] + array_Green[x + 1, y + 1] + array_Blue[x + 1, y + 1]) * LaplacianMask[8] ;

                    
                }
            }
            //score_laplacian = score_laplacian / 3 / 9 / width / height;
            UnLockBMP(bitmap);

            return score_laplacian;
        }

        #region PointPm
        //輪廓點與背景的對比度
        private int PointPm(Point p)
        {
            int x = p.X, y = p.Y, mx = array_Gray[x, y];
            if (mx < array_Gray[x - 1, y]) mx = array_Gray[x - 1, y];
            if (mx < array_Gray[x + 1, y]) mx = array_Gray[x + 1, y];
            if (mx < array_Gray[x, y - 1]) mx = array_Gray[x, y - 1];
            if (mx < array_Gray[x, y + 1]) mx = array_Gray[x, y + 1];
            return mx - array_Gray[x, y];
        }
        #endregion

    }
    //目標物件類別
    class TgInfo
    {
        public int np = 0; //目標點數
        public List<Point> P = null; //目標點的集合
        public int xmn = 0, xmx = 0, ymn = 0, ymx = 0; //四面座標極值
        public List<Point> left_point_list = null;
        public List<Point> top_point_list = null;
        public List<Point> right_point_list = null;
        public List<Point> down_point_list = null;
        public List<Point> left_centerpoint_list = null;
        public List<Point> top_centerpoint_list = null;
        public List<Point> right_centerpoint_list = null;
        public List<Point> down_centerpoint_list = null;
        //public List<Point> centerpoint_list = null;
        public int width = 0, height = 0; //寬與高
        public int pm = 0; //目標與背景的對比強度
        public int cx = 0, cy = 0; //目標中心點座標
        public int ID = 0; //依對比度排序的序號
        
    }


}
