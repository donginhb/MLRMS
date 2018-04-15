using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ESDLL
{
    public enum ENUM_MODE
    {
        MODE_ENROLL,                                // 注册模式
        MODE_MATCH                                  // 识别模式
    };


    public enum ES_MATCH_TYPE
    {
        ES_MATCH_ONE_TO_ONE = 0,     ///< 1:1
        ES_MATCH_ONE_TO_MANY = 1      ///< 1:n
    };


    [StructLayout(LayoutKind.Sequential)]
    public struct ES_PREVIEW_FEEDBACK
    {
        public double dIrisArea;            ///< 虹膜面积
        public double dIrisRadius;          ///< 虹膜半径
        public int nImageScore;          ///< 图像打分
    };



    [StructLayout(LayoutKind.Sequential)]
    public struct ES_DEBUG_INFO
    {
        public ushort nEyeNum;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public int[] nResultSeg;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public int[] nResultBlur;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public int[] nResultMotion;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public double[] dMatchScore;

    };

    [StructLayout(LayoutKind.Sequential)]
    public struct ES_DEBUG_BUFF
    {
        public IntPtr pEyeBuff;         ///< 眼图存储区（眼图大小*2）（注册）（识别）

        public IntPtr pEyeSegBuff;      ///< 分割存储区（眼图大小*2）（注册）（识别）

        public IntPtr pNormalBuff;      ///< 归一化存储区（512*48*2）（注册）（识别）

        public IntPtr pTempBuff;        ///< 模板存储区（模板大小*2）（注册）（识别）

        public IntPtr pMatchResult;     ///< 全部比较结果（1:N）             （识别）

    };

    ///反馈信息
    /// 
    [StructLayout(LayoutKind.Sequential)]
    public struct ES_FEEDBACK_PARA
    {

        public ushort nIrisRad;         ///< 虹膜半径         （注册）（识别）
        public ushort nEnrollProgressL; ///< 左眼注册进度0-100（注册）
        public ushort nEnrollProgressR; ///< 右眼注册进度0-100（注册）
        public ES_DEBUG_INFO strDebugInfo;     ///< 调试信息
        public ES_DEBUG_BUFF strDebugBuff;     ///< 调试buff

    };

    public enum ES_ENROLL_STATE
    {
        ES_ENROLL_FLG_START = 0,     ///< 开始注册，输入标志，由用户设定
        ES_ENROLL_FLG_RUNING = 1,     ///< 正在注册，输出标志
        ES_ENROLL_FLG_FINISHED = 2      ///< 完成注册，输出标志
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct ES_IMAGE_PARAM
    {
        public int nEyeWidth;            ///< 眼图宽
        public int nEyeHeight;           ///< 眼图高
        public int nTempLength;          ///< 模板长
        public int nTempExtendNum;       ///< 扩展的模板数

        public int nTemplateNum;        ///< 注册使用模板数
        public int nClusterNum;         ///< 注册期望获得模板数

        public double dEnrollMinIrisR;   ///< 注册最小半径
        public double dEnrollMaxIrisR;   ///< 注册最大半径
        public double dMatchMinIrisR;    ///< 识别最小半径
        public double dMatchMaxIrisR;    ///< 识别最大半径
    };


    ///活体结果
    public enum ES_LIVENESS_RESULT
    {
        ES_IRIS_REAL = 0,        ///< 活体
        ES_IRIS_FAKE = 1,        ///< 假体
        ES_IRIS_UNKNOWN = 2      ///< 未知
    }
   public class ESDLL
    {
        public static byte[] EyeSmartIrisApiLibForEyeSquare2;
        public static ES_IMAGE_PARAM ImageParamForEyeSquare2;

        [DllImport("EyeSmartIrisAPI.dll", EntryPoint = "ESInitialize")]
        public static extern int ESInitialize(byte[] pParambuf);

        [DllImport("EyeSmartIrisAPI.dll", EntryPoint = "ESRelease")]
        public static extern void ESRelease();

        [DllImport("EyeSmartIrisAPI.dll", EntryPoint = "ESProcessCameraFrame")]
        public static extern int ESProcessCameraFrame(IntPtr pPreviewFrame, ushort nWidth, ushort nHeight, byte[] pParaBuffer, char nEyeNum, IntPtr pstFeedBack, byte[] pEyeImage0, byte[] pEyeImage1);

        [DllImport("EyeSmartIrisAPI.dll", EntryPoint = "ESLivenessDetect", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ESLivenessDetect(byte[] pParaBuffer, int width, int height, byte[] paramlib, ref int pResult);

        [DllImport("EyeSmartIrisAPI.dll", EntryPoint = "ESIrisEnroll")]
        public static extern int ESIrisEnroll(byte[] pPreview,
                                 ushort nPreviewWidth,
                                 ushort nPreviewHeigh,
                                 byte[] pParamLib,
                                 ref int pStartedFlag,
                                 byte[] pEnrollTemplate,
                                 ushort[] pTemplateNum,
                                 byte[] pEnrollEyeImg,
                                 ref ES_FEEDBACK_PARA pFeedback);

        [DllImport("EyeSmartIrisAPI.dll", EntryPoint = "ESIrisMatch")]
        public static extern int ESIrisMatch(byte[] pPreview,
                                ushort nPreviewWidth,
                                ushort nPreviewHeigh,
                                byte[] pParamLib,
                                ES_MATCH_TYPE emMatchType,
                                byte[] pEnrollTemplateL,
                                byte[] pEnrollTemplateR,
                                ushort[] pTemplateNum,
                                ref int pResult,
                                ref ES_FEEDBACK_PARA pFeedback);

        [DllImport("EyeSmartIrisAPI.dll", EntryPoint = "ESLoadDataFile", CharSet = CharSet.Ansi)]
        public static extern int ESLoadDataFile(string pFilePath, int nLength, byte[] pParamLib);

        [DllImport("EyeSmartIrisAPI.dll", EntryPoint = "ESGetImageParam")]
        public static extern int ESGetImageParam(byte[] pParaBuffer, ref ES_IMAGE_PARAM pImgParam);


        public static void EnvInit()
        {
            FileInfo fi = new FileInfo(@".\EyeMatrix_eyesqure2.dat");
            long size = fi.Length;
            EyeSmartIrisApiLibForEyeSquare2 = new byte[size];

            int errorCode = ESLoadDataFile(@".\EyeMatrix_eyesqure2.dat", (int)size, EyeSmartIrisApiLibForEyeSquare2);

            if (errorCode == 0)
            {
                ESGetImageParam(EyeSmartIrisApiLibForEyeSquare2, ref ImageParamForEyeSquare2);
                IrisEnrollBuffer.EnrollBufferStart(ImageParamForEyeSquare2.nClusterNum, ImageParamForEyeSquare2.nEyeWidth, ImageParamForEyeSquare2.nEyeHeight);
            }
            else
            {
                throw new Exception("Reading camera configuration file error.");
            }
            ESInitialize(EyeSmartIrisApiLibForEyeSquare2);
        }


        public static void EnvRelease()
        {
            ESRelease();
        }

        [DllImport("ESImageProcess.dll")]
        public static extern int ESRgbToGray(byte[] pRgbBuf, int nWidth, int nHeight, byte[] pGrayBuf);

        [DllImport("ESImageProcess.dll")]
        public static extern int ESSaveImgToBMP(byte[] pImgData, int nWidth, int nHeight, IMG_TYPE emType, string pFileName);

        public enum IMG_TYPE
        {
            IMAGE_RGB24 = 0,    ///RGB图像
            IMAGE_GRAY = 1     ///8位灰度图像
        };

    }
}
