using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using Employee = BEMAttendance.employee;
namespace BEMAttendance.Models
{
    public class IrisCenter
    {
        BemEntities db = new BemEntities();
        const int templateSize = 512 * 2;
        byte[] EnrollTemplateL;
        byte[] EnrollTemplateR;
        static readonly ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();
        Dictionary<int, Employee> DicEmployees;
        int LeftTemplateNum;
        int RightTemplateNum;

        byte[] EyeSmartIrisApiLibForEyeSquare2;
        public void Init()
        {
            try
            {
                ESDLL.ESDLL.EnvInit();
            }
            catch(Exception ex)
            {
                LogHelper.Error("初始化失败", ex);
            }
        }
        public void ReadAllTemplates()
        {
            try
            {
                rwLock.EnterWriteLock();
                var allIrisInfo = db.employee.ToList();
                int leftNumber = 0;
                int rightNumber = 0;
                int personCount = 0;
                int leftNumStartIndex = 0;
                int rightNumStartIndex = 0;
                int leftEyeDicIndex = 0;
                int rightEyeDicIndex = 0;
                foreach(var iris in allIrisInfo)
                {
                    if(iris.Template==null||iris.Template.Length%templateSize!=0)
                    {
                        continue;
                    }
                    personCount++;
                    leftNumber += iris.LeftNum.HasValue ? (int)iris.LeftNum : 0;
                    rightNumber += iris.RightNum.HasValue ? (int)iris.RightNum : 0;
                }
                EnrollTemplateL = new byte[leftNumber * templateSize];
                EnrollTemplateR = new byte[rightNumber * templateSize];
                rightEyeDicIndex = leftNumber;
                foreach(var iris in allIrisInfo)
                {
                    int leftEyeNum = iris.LeftNum.HasValue ? (int)iris.LeftNum : 0;
                    int rightEyeNum = iris.RightNum.HasValue ? (int)iris.RightNum : 0;
                    if (iris.Template == null || iris.Template.Length % templateSize != 0)
                    {
                        continue;
                    }
                    Array.Copy(iris.Template, 0, EnrollTemplateL, leftNumStartIndex, templateSize * (int)iris.LeftNum);
                    for(int i=0;i< leftEyeNum;i++)
                    {
                        DicEmployees.Add(leftEyeDicIndex + i, new Employee { EmpCode=iris.EmpCode,EmpName=iris.EmpName});
                    }
                    leftEyeDicIndex += leftEyeNum;
                    leftNumStartIndex += templateSize * leftNumber;
                    int middleOfBlob = iris.Template.Length / 2;
                    Array.Copy(iris.Template, middleOfBlob, EnrollTemplateR, rightNumStartIndex, templateSize * rightEyeNum);
                    for(int i=0;i<rightEyeNum;i++)
                    {
                        DicEmployees.Add(rightEyeDicIndex + i, new Employee { EmpCode = iris.EmpCode, EmpName = iris.EmpName });
                    }
                    rightEyeDicIndex += rightEyeNum;
                    rightNumStartIndex += templateSize * rightEyeNum;
                }
                LeftTemplateNum = leftNumber;
                RightTemplateNum = rightNumber;
                rwLock.ExitWriteLock();
        
            }
            catch(Exception ex)
            {
                LogHelper.Error("ReadAllTemplates Error:", ex);
            }
        }
        //public bool GetMatch(byte[] grayImage,out Employee employee)
        //{
        //    try
        //    {
        //        IAsyncResult
        //    }
        //    catch(Exception ex)
        //    {
        //        LogHelper.Error("Iris Match Error:", ex);
        //    }
        //}
    }
}