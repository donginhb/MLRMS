using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;

namespace BEMAttendance.Models
{
    public class EyecloudDLL
    {
        const string dllPath = "DataBaseCtl.dll";

        [DllImport(dllPath, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static void SetSvrIPAndPort(string szIPAddr, int iPort);

        [DllImport(dllPath,CallingConvention =CallingConvention.StdCall,CharSet =CharSet.Ansi)]
        public extern static int TransferUserAndTempInfoBySvr2(string szIdentityID, string szSrcDepartmentID, string szDesDepartmentID);

        [DllImport(dllPath, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static int DownloadInfoBySvr2(string szDepartmentID);

    }
}