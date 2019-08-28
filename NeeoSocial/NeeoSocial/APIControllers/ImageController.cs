using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NeeoSocial.Utility;
using NewSocial.Models;
using static NeeoSocial.Utility.HeaderDTO;

namespace NeeoSocial.APIControllers
{
    [Route("ImageApi")]
    
    public class ImageController : ControllerBase
    {
        DbCalls db = new DbCalls();
      
        public class FileDetails
        {
            public string Name { get; set; }
            public string Path { get; set; }
        }

        [Route("UploadImages")]
        [HttpPost]
        public async Task<IActionResult> UploadFiles(List<IFormFile> files)
        {
            string Message;
            int code;
            long UserID = Convert.ToInt64(Request.GetHeader("UserID"));
            var isUserExist = db.User.Where(u => u.UserID == UserID).FirstOrDefault();
            if (isUserExist != null)
            {
                //List<IFormFile> files = new List<IFormFile>();
                if (files == null || files.Count == 0)
                    return Content("files not selected");
                string fileName;
                string returnpath;
                List<string> images = new List<string>();
                string subpath = "/images/userPosts/";
                foreach (var file in files)
                {
                    if (file.ContentType.StartsWith("image"))
                    {
                        fileName = UserID + "_Post_" + Guid.NewGuid() + ".jpg";
                        var path = Path.Combine(
                                Directory.GetCurrentDirectory(), "wwwroot/images/userPosts/",
                                fileName);

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        returnpath = subpath + fileName;
                        FileDetails currentImage = new FileDetails();
                        currentImage.Name = file.FileName;
                        currentImage.Path = returnpath;
                        images.Add(returnpath);
                    }
                   
                }
                return Ok(new { code = 200,Message="Files Uploaded", images });
            }
            else
            {
                code = 401;
                Message = "Login First";
                return Ok(new { code, Message });

            }
             
        }



       




        [Route("SaveProfile")]
        [HttpPost]
        public async Task<IActionResult> SaveProfile(FileInputModel model)
        {
            string Message;
            int code;
            long UserID = Convert.ToInt64(Request.GetHeader("UserID"));
            var isUserExist = db.User.Where(u => u.UserID == UserID).FirstOrDefault();
            if (isUserExist != null)
            {
                code = 200;
                Message = "Fraz";
                //+model.FileToUpload[0].ContentType
                string fileName = UserID + "_Profile_" + Guid.NewGuid() + ".jpg";
                string fileSavedpath = "";
                if (model.FileToUpload != null && model.FileToUpload.Count > 0)
                {

                    foreach (IFormFile file in model.FileToUpload)
                    {
                        if (file.ContentType.StartsWith("image"))
                        {
                            var path = Path.Combine(
                                                           Directory.GetCurrentDirectory(), "wwwroot/images/userProfiles/",
                                                           fileName);

                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }


                            UserMedia currentUerMedia = new UserMedia();
                            currentUerMedia.UserID = UserID;
                            currentUerMedia.type = 1;
                            currentUerMedia.ImageUrl = "/images/userProfiles/" + fileName;
                            db.UserMedia.Add(currentUerMedia);
                            db.SaveChanges();
                            code = 200;
                            Message = "Saved";
                            fileSavedpath = currentUerMedia.ImageUrl;
                        }
                        else
                        {
                            code = 200;
                            Message = "Unsupported Media Type";
                        }
                    }

                }

                return Ok(new { code, Message , fileSavedpath });
            }
            else
            {
                code = 401;
                Message = "Login First";
                return Ok(new { code, Message });
            }

        }

      

    }
}