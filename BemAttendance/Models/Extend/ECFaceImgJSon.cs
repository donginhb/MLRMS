using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEMAttendance.Models.Extend
{
    public class ECFaceImgJson
    {
        public string uuid { get; set; }
        public List<ECFaceImageData> faceImageData { get; set; }
    }
    public class ECFaceImageData
    {
        public Int64 imageType { get; set; }
        public Int64 nHeight { get; set; }
        public Int64 nWidth { get; set; }
        public Int64 imageLength { get; set; }
        public string imageData { get; set; }
    }
    public class ResponseJson
    {
        public bool result { get; set; }
        public Int64 errorCode { get; set; }
        public string errorMessage { get; set; }
        public object data { get; set; }
    }
}