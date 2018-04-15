using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEMAttendance.Models.Extend
{
    public class ECEyeImgJson
    {
        public string uuid { get; set; }
        public List<ECEyeImageData> eyeImageData { get; set; }
    }
    public class ECEyeImageData
    {
        public Int64 eyeImageDes { get; set; }
        public Int64 nHeight { get; set; }
        public Int64 nWidth { get; set; }
        public string imageData { get; set; }
    }
   
}