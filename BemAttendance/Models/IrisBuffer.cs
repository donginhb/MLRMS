using ESDLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESDLL
{
    public static class IrisEnrollBuffer
    {
        public static int EnrollProcess = 0;

        public static byte[] EntrollTempalte;
        public static ushort[] TemplateNumber = new ushort[2];
        public static byte[] EntrollEyeImg;


        public static byte[] EyeBuff;
        public static byte[] EyeSegBuff;

        public static int EnrollWidth;
        public static int EnrollHeight;


        public static ES_FEEDBACK_PARA feedback = new ES_FEEDBACK_PARA();

        public static void EnrollBufferStart(int eyeTemplateNumber, int width, int height)
        {
            EnrollWidth = width;
            EnrollHeight = height;
            EntrollTempalte = new byte[512 * 2 * eyeTemplateNumber * 2];
            EntrollEyeImg = new byte[width * height * eyeTemplateNumber * 2];

            EyeBuff = new byte[width * height * 2];
            EyeSegBuff = new byte[width * height * 6];

            feedback.strDebugBuff = new ES_DEBUG_BUFF();
            feedback.strDebugBuff.pEyeBuff = IntPtr.Zero;
            feedback.strDebugBuff.pTempBuff = IntPtr.Zero;
            feedback.strDebugBuff.pEyeSegBuff = IntPtr.Zero;
            feedback.strDebugBuff.pNormalBuff = IntPtr.Zero;
            feedback.strDebugBuff.pMatchResult = IntPtr.Zero;
            feedback.strDebugInfo = new ES_DEBUG_INFO();
            feedback.strDebugInfo.nResultMotion = new int[2];
            feedback.strDebugInfo.nResultBlur = new int[2];
            feedback.strDebugInfo.nResultSeg = new int[2];
            feedback.strDebugInfo.dMatchScore = new double[2];
        }


    }

    public static class IrisMatchBuffer
    {
        public static byte[] EnrollTemplateL;
        public static byte[] EnrollTemplateR;
        public static ushort[] TemplateNumber = new ushort[2]; //生成个数
   
        public static ES_FEEDBACK_PARA feedback = new ES_FEEDBACK_PARA();



        static IrisMatchBuffer()
        {
            feedback.strDebugBuff = new ES_DEBUG_BUFF();
            feedback.strDebugBuff.pEyeBuff = IntPtr.Zero;
            feedback.strDebugBuff.pTempBuff = IntPtr.Zero;
            feedback.strDebugBuff.pEyeSegBuff = IntPtr.Zero;
            feedback.strDebugBuff.pNormalBuff = IntPtr.Zero;
            feedback.strDebugBuff.pMatchResult = IntPtr.Zero;
            feedback.strDebugInfo = new ES_DEBUG_INFO();
            feedback.strDebugInfo.nResultMotion = new int[2];
            feedback.strDebugInfo.nResultBlur = new int[2];
            feedback.strDebugInfo.nResultSeg = new int[2];
            feedback.strDebugInfo.dMatchScore = new double[2];
        }

        public static void SetEnrollTemplate(byte[] templateL, byte[] templateR, int LeftNumber, int rightNumber)
        {
            EnrollTemplateL = templateL;
            EnrollTemplateR = templateR;
           TemplateNumber[0] = (ushort)LeftNumber;
           TemplateNumber[1] = (ushort)rightNumber;
        }
    }
}
