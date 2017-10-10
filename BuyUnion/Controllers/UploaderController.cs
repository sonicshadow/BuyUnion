using BuyUnion.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BuyUnion.Controllers
{
    public class UploaderController : Controller
    {
        // GET: Uploader
        [HttpPost]
        [AllowCrossSiteJson]
        public ActionResult Upload(FileUpload model)
        {
            ICollection<string> filename = null;
            if (string.IsNullOrWhiteSpace(model.FilePath))
            {
                filename = this.UploadFile(isResetName: model.IsResetName);
            }
            else
            {
                filename = this.UploadFile(path: model.FilePath, isResetName: model.IsResetName);
            }
            if (filename == null || filename.Count <= 0)
            {
                return Json(Comm.ToJsonResult("Error", "上传失败"));
            }
            return Json(Comm.ToJsonResult("Success", "成功", new
            {
                FileUrls = filename,
                FileFullUrls = filename.Select(s => Url.ContentFull(s))
            }));
        }

        public ActionResult DeleteFile(string file)
        {
            try
            {
                DeleteSeverFile(file);
                return Json(Comm.ToJsonResult("Success", "删除成功"));
            }
            catch (Exception ex)
            {
                return Json(Comm.ToJsonResult("Error", "删除失败"));
            }
        }

        private void DeleteSeverFile(string file)
        {
            var fileName = file.ToLower();
            file = Server.MapPath(file);
            try
            {
                FileInfo fileInfo = new FileInfo(file);
                if (!fileInfo.Exists)
                {
                    throw new DirectoryNotFoundException("文件不存在");
                }
                System.IO.File.Delete(file);
            }
            catch (System.IO.IOException e)
            {
                throw e;
            }
        }

        [HttpPost]
        public ActionResult ForCkEditor()
        {
            var filename = this.UploadFile().ToList();
            if (filename == null || filename.Count <= 0)
            {
                return Json(new { State = "Error", Message = "未找到上传文件" });
            }
            ViewBag.CKEditorFuncNum = Request["CKEditorFuncNum"];
            var name = $"{filename[0]}?404=default";
            ViewBag.File = Url.ContentNullEmpty(name);
            if (Request["responseType"] == "json")
            {
                return Json(new { fileName = new System.IO.FileInfo(filename[0]).Name, uploaded = 1, url = Url.Content(name) });
            }
            else
            {
                return View();
            }
        }

    }
}