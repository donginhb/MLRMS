using BEM.Models;
using BEMAttendance.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BEMAttendance.Controllers
{
    [RoutePrefix("bem/v1/video")]
    public class VideoController : Controller
    {
        VisitorRole role;
        string department1Code = string.Empty;
        string loginName;
        // GET: Video
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
                    if(db.softwareupdate.Where(m=>m.type==1).OrderByDescending(m=>m.uploadTime).Count()>0)
                    {
                        updateInfo = db.softwareupdate.Where(m => m.type == 1).OrderByDescending(m => m.uploadTime).First();
                    }
                    package.updateInfo = updateInfo;
                    var devList = db.clientdevice;
                    foreach(var dev in devList)
                    {
                        package.devices.Add(dev);
                    }
                }
            }
            catch(Exception ex)
            {
                LogHelper.Error("获取视频信息出错", ex);
            }
            return View(package);
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult Index(string devData)
        {
            try
            {
                devData = devData.TrimEnd(',');
                string[] splitDev = devData.Split(',');
                foreach (var itm in splitDev)
                {
                    TaskManager.SaveVideoUpdate(itm);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("视频信息下发失败", ex);
                return Content("视频信息下发失败", "text/html");
            }
            LogHelper.Info("视频信息下发成功" + "时间:" + DateTime.Now);
            return Content("OK", "text/html");
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
                if (extension.ToLower() != ".mp4")
                {
                    return Content("上传文件格式错误，当前只支持mp4文件", "text/html");
                }

                string fileName = Path.GetFileName(path);
                string savePath = Server.MapPath("~/Content");
                string folderPath = savePath + "\\video";
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                List<string> prePackagePath = new List<string>();
                if (new DirectoryInfo(folderPath).GetFiles().Where(m => Path.GetExtension(m.FullName).ToLower() == ".mp4").Count() > 0)
                {
                    List<FileInfo> files = new DirectoryInfo(folderPath).GetFiles().Where(m => Path.GetExtension(m.FullName).ToLower() == ".mp4").ToList();
                    foreach (var item in files)
                    {
                        prePackagePath.Add(item.FullName);   //把其他的文件保存，以便新文件进来后，把老文件删除
                    }
                }
                string newFileName = Path.GetFileNameWithoutExtension(file.FileName);
                if (prePackagePath.Where(m => Path.GetFileName(m) == fileName).Count() > 0)
                {
                    newFileName = newFileName + "(1)";
                }
                await Task.Run(delegate ()
                {
                    using (FileStream fileToSave = new FileStream(folderPath + "\\" + newFileName + ".mp4", FileMode.Create, FileAccess.Write))
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
                            LogHelper.Error("视频文件删除失败", ex);
                        }
                    }
                    softwareupdate updatesOne = new softwareupdate();
                    updatesOne.version = newFileName + ".mp4";
                    updatesOne.uploadTime = DateTime.Now;
                    updatesOne.type = 1;
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
                LogHelper.Error("上传视频文件失败", ex);
                return Content("上传视频文件失败，请检查", "text/html");
            }
        }
    }
}