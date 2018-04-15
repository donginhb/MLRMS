using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;

namespace BEMAttendance.Models.SDK
{
    public class ESSdk
    {
        [DllImport("ESImageProcess.dll",CallingConvention =CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern int ESSaveImgToBMP(byte[] pImgData,int nWidth,int nHeight, ES_IMG_TYPE emType, string pFileName);

        public enum ES_IMG_TYPE
        {
            ES_IMAGE_RGB24 = 0,    ///< RGB图像
            ES_IMAGE_GRAY = 1     ///< 8位灰度图像
        }
    }
}