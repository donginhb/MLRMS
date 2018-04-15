using BEM.Models;
using BEMAttendance.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BEMAttendance.Controllers
{
    public class SoftwareUpdateController : Controller
    {
        VisitorRole role;
        string department1Code = string.Empty;
        string loginName;

        // GET: SoftwareUpdate
        public ActionResult Index()
        {
            string name, error;
            if (CookieHelper.HasCookie(out name, out error) == false)
            {
                return RedirectToAction("", "LoginUI");
            }
            else
            {
                new RoleHelper().GetRoles(name, out role, out department1Code, out loginName);
                ViewData["VisitorRole"] = role;
                ViewData["username"] = loginName;
            }

            UpdatesPackage package = new UpdatesPackage();
            softwareupdate updateInfo = null;

            try
            {
                package.devices = new List<clientdevice>();
                using (BemEntities db = new BemEntities())
                {
                    if(db.softwareupdate.Where(m=>m.type==0).OrderByDescending(m => m.uploadTime).Count()>0)
                    {
                        updateInfo = db.softwareupdate.Where(m => m.type == 0).OrderByDescending(m => m.uploadTime).First();
                    }
                    package.updateInfo = updateInfo;
                    var devlist = db.clientdevice;
                    foreach(var dev in devlist)
                    {
                        package.devices.Add(dev);
                    }
                }
            }
            catch(Exception ex)
            {
                LogHelper.Error("获取软件升级信息出错", ex);
            }
           return View(package);
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult Index(string devData)
        {
            //List<string> failList = new List<string>();
            try
            {
                devData = devData.TrimEnd(',');
                string[] splitDev = devData.Split(',');
                //softwareupdate updateInfo = null;
                //using (BemEntities db = new BemEntities())
                //{
                //    if (db.softwareupdate.Where(m => m.type == 0).OrderByDescending(m => m.uploadTime).Count() > 0)
                //    {
                //        updateInfo = db.softwareupdate.Where(m => m.type == 0).OrderByDescending(m => m.uploadTime).First();
                //    }
                //}
                //string version = updateInfo.version.Substring(updateInfo.version.IndexOf('#'), updateInfo.version.LastIndexOf('#') - updateInfo.version.IndexOf('#'));
                //Dictionary<string, string> devVersion = new Dictionary<string, string>();
                //List<string> devList = new List<string>();
           
                //foreach (var item in splitDev)
                //{
                //    string[] splitVer = item.Split(':');
                //    devVersion.Add(splitVer[0], splitVer[1]);
                //}
                //foreach(KeyValuePair<string,string> itm in devVersion)
                //{
                //    if(CompareVersion(itm.Value,version))
                //    {
                //        devList.Add(itm.Key);  
                //    }
                //    else
                //    {
                //        failList.Add(itm.Key);
                //    }
                //}

                foreach(var itm in splitDev)
                {
                    TaskManager.SavePackUpdate(itm);
                }
            }
            catch(Exception ex)
            {
                LogHelper.Error("软件包更新下发失败", ex);
                return Content("软件升级包下发失败","text/html");
            }
            LogHelper.Info("软件升级包下发成功,操作者:"+loginName+"时间:"+DateTime.Now);
                return Content("OK", "text/html");
        }
        private bool CompareVersion(string oldVersion,string newVersion)
        {

            Version vs1 = new Version(oldVersion);
            Version vs2 = new Version(newVersion);
            if(vs2>vs1)
            {
                return true;
            }
            return false;
        }
        [System.Web.Mvc.HttpGet]
        public ActionResult Upload()
        {
            string name, error, loginName;
            if (CookieHelper.HasCookie(out name, out error) == false)
            {
                return RedirectToAction("", "LoginUI");
            }
            else
            {
                new RoleHelper().GetRoles(name, out role, out department1Code, out loginName);
                ViewData["VisitorRole"] = role;
                ViewData["username"] = loginName;
            }
            Uri url = Request.UrlReferrer;
            if (url == null)
            {
                //如果是直接加载地址的，则需要重新加载js库
                ViewData["reloadJS"] = true;
            }
            else
            {
                ViewData["reloadJS"] = false;
            }
            return View();
        }

        [System.Web.Mvc.HttpPost]
        public async Task<ActionResult> Upload(HttpPostedFileBase file)
        {
            try
            {
                if (file == null)
                {
                    return Content("上传为空，请重新选择", "text/html");
                }
                string path = file.FileName;
                string content = string.Empty;
                string extension = Path.GetExtension(path);
                if (extension.ToLower() != ".apk")
                {
                    return Content("上传文件格式错误，当前只支持apk文件", "text/html");
                }
                string fileName = Path.GetFileName(path);
                string savePath = Server.MapPath("~/Content");
                string folderPath = savePath + "\\apk";
                if(!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                List<string> prePackagePath = new List<string>();
                if(new DirectoryInfo(folderPath).GetFiles().Where(m=>Path.GetExtension(m.FullName).ToLower()==".apk").Count()>0)
                {
                    List<FileInfo> files = new DirectoryInfo(folderPath).GetFiles().Where(m => Path.GetExtension(m.FullName).ToLower() == ".apk").ToList();
                    foreach(var item in files)
                    {
                        prePackagePath.Add(item.FullName);   //把其他的文件保存，以便新文件进来后，把老文件删除
                    }
                }
                string newFileName = Path.GetFileNameWithoutExtension(file.FileName);
                if(prePackagePath.Where(m=>Path.GetFileName(m)==fileName).Count()>0)
                {
                    newFileName = newFileName + "(1)";
                }
                await Task.Run(delegate ()
                {
                    using (FileStream fileToSave = new FileStream(folderPath+"\\"+newFileName + ".apk", FileMode.Create, FileAccess.Write))
                    {
                        byte[] bytes = new byte[file.InputStream.Length];
                        file.InputStream.Read(bytes, 0, (int)file.InputStream.Length);
                        fileToSave.Write(bytes, 0, bytes.Length);
                        fileToSave.Flush();
                    }
                    foreach (var item in prePackagePath)
                    {
                        try
                        {
                            System.IO.File.Delete(item);
                        }
                        catch (Exception ex)
                        {
                            LogHelper.Error("apk文件删除失败", ex);
                        }
                    }
                    softwareupdate updatesOne = new softwareupdate();
                    updatesOne.version = newFileName+".apk";
                    updatesOne.uploadTime = DateTime.Now;
                    updatesOne.type = 0;
                    using (BemEntities db = new BemEntities())
                    {
                        db.softwareupdate.Add(updatesOne);
                        db.SaveChanges();
                    }
                });
                
               return Content("OK", "text/html");
            }
            catch (Exception ex)
            {
                //日志记录
                LogHelper.Error("上传升级包失败", ex);
                return Content("上传升级包失败，请检查", "text/html");
            }
        }
    }
}