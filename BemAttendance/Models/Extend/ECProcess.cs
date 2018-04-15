using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;

namespace BEMAttendance.Models.Extend
{
    public class ECProcess
    {
        public static string ECIPaddress { get; set; }
        public static int ECPort { get; set; }

        static Queue<ECItem> _tasks = new Queue<ECItem>();
        readonly static object _locker = new object();

        static EventWaitHandle _wh = new AutoResetEvent(false);

        static Thread _worker;

        static bool boolThreadStart { get; set; }

        static void DoWork()
        {
            while(true)
            {
                ECItem work = null;
                lock(_locker)
                {
                    if(_tasks.Count>0)
                    {
                        work = _tasks.Dequeue();
                        if(work==null)         //退出机制，当遇到一个null任务时，代表任务的结束
                        {
                            return;
                        }
                    }
                }
                if(work!=null)
                {
                    UploadEyeImage(work.eyeFileName, work.eyeImgWidth, work.eyeImgHeight);
                    UploadFaceImage(work.faceFileName,work.id,work.dateTime);
                }
                else
                {
                    _wh.WaitOne();  //如果没有任务了，等待信号
                }
            }
        }
        
        public static void EnqueueTask(ECItem task)
        {
            lock(_locker)
            {
                if (boolThreadStart == false)
                {
                    _worker = new Thread(DoWork);
                    _worker.Start();
                }
                if(_tasks.Count<5000)           //缓冲队列5000条
                {
                    _tasks.Enqueue(task);
                }
            }
            _wh.Set();   //给工作线程发开始信号
        
        }
        static void UploadEyeImage(string fileName,Int64 width,Int64 height)
        {
            if(string.IsNullOrEmpty(fileName))
            {
                return;
            }
            if (!File.Exists(fileName))
            {
                return;
            }
            string url = string.Format("http://{0}:{1}/eyeCloud/ecc/bis/Biology/EyeImage", ECIPaddress, ECPort);
            try
            {
                byte[] bytes = null;

                using (FileStream fs = File.Open(fileName,FileMode.Open))
                {
                        bytes = new byte[fs.Length];
                        fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
                }
                if(bytes!=null)
                {
                    ECEyeImgJson eyeImg = new ECEyeImgJson();
                    eyeImg.uuid = Guid.NewGuid().ToString();
                    eyeImg.eyeImageData = new List<ECEyeImageData>();
                    ECEyeImageData imgData = new ECEyeImageData();
                    imgData.eyeImageDes = 0;
                    imgData.imageData = Convert.ToBase64String(bytes);
                    imgData.nWidth = width;
                    imgData.nHeight = height;
                    eyeImg.eyeImageData.Add(imgData);

                    string imgJson = JSONHelper.ObjectToJson<ECEyeImgJson>(eyeImg, Encoding.UTF8);
                    Post(url, imgJson);  //上传眼图
                }
         
            }
            catch(Exception ex)
            {
                LogHelper.Error("上传眼图失败", ex);
            }
            
        }
        static void UploadFaceImage(string fileName,long id,DateTime dt)
        {
            string url = string.Format("http://{0}:{1}/eyeCloud/ecc/bis/Biology/FaceImage", ECIPaddress, ECPort);
            if(string.IsNullOrEmpty(fileName))
            {
                return;
            }
            if(!File.Exists(fileName))
            {
                return;
            }
            try
            {
                byte[] bytes = null;
                int width = 0;
                int height = 0;

                using (Image img = Image.FromFile(fileName))
                {
                    width = img.Width;
                    height = img.Height;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        Bitmap bmp = new Bitmap(img);
                        bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);  //将图像以指定的格式存入缓存内存流
                        bytes = new byte[ms.Length];
                        ms.Position = 0;
                        ms.Read(bytes, 0, Convert.ToInt32(ms.Length));
                    }
                }
                if (bytes != null)
                {
                    ECFaceImgJson faceImg = new ECFaceImgJson();
                    faceImg.uuid = Guid.NewGuid().ToString();
                    faceImg.faceImageData = new List<ECFaceImageData>();
                    ECFaceImageData imgData = new ECFaceImageData();
                    imgData.imageType = 1;
                    imgData.imageData = Convert.ToBase64String(bytes);
                    imgData.nWidth = width;
                    imgData.nHeight = height;
                    imgData.imageLength = bytes.Length;
                    faceImg.faceImageData.Add(imgData);

                    string imgJson = JSONHelper.ObjectToJson<ECFaceImgJson>(faceImg, Encoding.UTF8);
                    Post(url, imgJson);  //上传脸图

                }
            }
            catch(Exception ex )
            {
                LogHelper.Error("上传脸图失败", ex);
            }

        }
        public void RegFace()
        {

        }
        private static string Post(string url,string jsonParas)
        {
            Stream writer;
            HttpWebRequest request;
            string postContent = string.Empty;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                byte[] payload = System.Text.Encoding.UTF8.GetBytes(jsonParas);
                request.ContentLength = payload.Length;
                writer = request.GetRequestStream();
                writer.Write(payload, 0, payload.Length);
                writer.Close();
            }
            catch(Exception ex)
            {
                writer = null;
                LogHelper.Error("连接EC服务器失败",ex);
                return postContent;
            }
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch(WebException ex)
            {
                response = ex.Response as HttpWebResponse;
            }
            try
            {
                Stream s = response.GetResponseStream();
                StreamReader sRead = new StreamReader(s);
                postContent = sRead.ReadToEnd();
            }
            catch(Exception ex)
            {
                LogHelper.Error("提交数据给EC获取响应失败", ex);
            }
            return postContent;      
        }
    }
    public class ECItem
    {
        public string faceFileName { get; set; }
        public string eyeFileName {get;set;}
        public DateTime dateTime { get; set; }
        public long id { get; set; }
        public Int64 eyeImgDes { get; set; }
        public Int64 eyeImgWidth { get; set; }
        public Int64 eyeImgHeight { get; set; }
    }
}