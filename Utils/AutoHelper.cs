using Emgu.CV;
using Emgu.CV.Bioinspired;
using Emgu.CV.CvEnum;
using Emgu.CV.Ocl;
using Emgu.CV.Structure;
using IronOcr;
using KAutoHelper;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using Tesseract;
using WhereMyTreasure.Utils;
using ImreadModes = Emgu.CV.CvEnum.ImreadModes;
using Point = System.Drawing.Point;
//using Tesseract.Interop;

namespace WhereMyTreasure.Utils
{
    public enum ActionType
    {
        Tap = 0,
        Swipe = 1,

    }
    internal class AutoHelper
    {

        bool _isStop = false;
        public int[] _delayTimeRange = { 1, 2 };
        public int _maxWaitTime = 30;
        public int _waitTimeCount = 5;
        public float _detectPercent = 0.7f;
        #region DATA
        Bitmap PLAYER_POST_BMP;
        Bitmap GLOBAL_POST_BMP;

        public string deviceID;

        private Bitmap SCREENSHOT;
        public int _multi = 2;
        public int _threshold = 100;
        public Vector2 GlobalPos = new Vector2();
        private Utils _util = new Utils();

        public void LoadData()
        {
            PLAYER_POST_BMP = (Bitmap)Bitmap.FromFile("TemplateImage//CurrentPost.png");
            GLOBAL_POST_BMP = (Bitmap)Bitmap.FromFile("TemplateImage//GlobalPos.png");
        }
        #endregion
        public AutoHelper(string _deviceID)
        {
            deviceID = _deviceID;
            LoadData();
        }
        public void Login()
        {
            try
            {
                KAutoHelper.ADBHelper.FindImageAndClick(deviceID, "TemplateImage//btn_dang_nhap.png", 500, 1);
            }
            catch
            {
                return;
            }
        }
        public void CheckForRate()
        {
            try
            {
                KAutoHelper.ADBHelper.FindImageAndClick(deviceID, "TemplateImage//btn_dont_rate.png", 500, 1);
            }
            catch
            {
                return;
            }
        }

        public bool Move(Point targetPos, int duration)
        {

            int radius = 20;
            Point dragPoint = new Point();
            Point dragStartPoint = new Point(80,300);
            GetCurrentPosition();
            float x = targetPos.X - GlobalPos.X;
            float y = targetPos.Y - GlobalPos.Y;
            dragPoint.Y = (int)(radius * (y / Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2))));
            dragPoint.X = (int)Math.Sqrt((Math.Pow(radius, 2) - Math.Pow(dragPoint.Y, 2)));
            KAutoHelper.ADBHelper.Swipe(deviceID, dragStartPoint.X, dragStartPoint.Y, dragStartPoint.X+ dragPoint.X, dragStartPoint.Y+ dragPoint.Y, duration);
            return true;
        }
        public void GetCurrentPosition()
        {
            Task t = new Task(() =>
            {
                while (true)
                {
                    if (_isStop)
                    {
                        return;
                    }
                    TakeScreenShot();
                    var posImageFind = FindImageInScreen(GLOBAL_POST_BMP);
                    if (posImageFind != null)
                    {
                        string[] result = GetPositionFromIMage(SCREENSHOT, (System.Drawing.Point)posImageFind, new Point(PLAYER_POST_BMP.Width, PLAYER_POST_BMP.Height));

                        GlobalPos.X = int.Parse(result[1]);
                        GlobalPos.Y = int.Parse(result[2]);

                    }

                    Random rd = new Random();
                    Delay(rd.Next(_delayTimeRange[0], _delayTimeRange[1]));
                }
            });
            t.Start();
        }
        public string[] GetPositionFromIMage(Bitmap screen, Point boundingBoxPos, Point boundingBoxRange)
        {
            using (var engine = new TesseractEngine("tessdata", "train", EngineMode.Default))
            {
                engine.SetVariable("tessedit_char_whitelist", "-0123456789,.");
                engine.DefaultPageSegMode = PageSegMode.SingleLine;

                var regionOfInterest = new Rectangle((int)boundingBoxPos.X + GLOBAL_POST_BMP.Width, (int)boundingBoxPos.Y, (int)(boundingBoxRange.X - GLOBAL_POST_BMP.Width), (int)boundingBoxRange.Y);

                string point = "null";
                var red_linear = new Image<Gray, byte>(new Bitmap(screen.Clone(regionOfInterest, screen.PixelFormat), new System.Drawing.Size(screen.Width * _multi, screen.Height * _multi)));

                CvInvoke.Threshold(red_linear, red_linear, _threshold, 255, ThresholdType.Binary);

                using (var img = PixConverter.ToPix(red_linear.Bitmap))
                {
                    using (var page = engine.Process(img))
                    {
                        point = page.GetText();
                    }
                }
                return point.Split(".");
            }
        }
        public Point? FindImageInScreen(Bitmap image)
        {
            return KAutoHelper.ImageScanOpenCV.FindOutPoint(SCREENSHOT, image, _detectPercent);
        }
        public void TakeScreenShot()
        {
            SCREENSHOT = KAutoHelper.ADBHelper.ScreenShoot(deviceID);

        }

        public void Stop()
        {
            _isStop = true;
        }
        public void Delay(int delay)
        {
            while (!_isStop)
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                delay--;
                if (_isStop)
                {
                    break;
                }
            }
        }
    }
}
