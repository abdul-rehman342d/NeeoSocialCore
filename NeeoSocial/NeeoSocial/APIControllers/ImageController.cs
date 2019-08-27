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
    [ApiController]
    public class ImageController : ControllerBase
    {
        DbCalls db = new DbCalls();
        //[Route("SaveProfile")]
        //[HttpPost]
        //public IActionResult SaveProfile(string image)
        //{
        //    string Message;
        //    int code;
        //    long UserID = Convert.ToInt64(Request.GetHeader("UserID"));
        //    var isUserExist = db.User.Where(u => u.UserID == UserID).FirstOrDefault();

        //    if (isUserExist != null)
        //    {
             
        //        code = 200;
        //        Message = "Saved";


        //        byte[] contents = convertIntoByte(image);
        //        string subpath = "~/images/userProfiles/";
        //        string fileName = isUserExist + "_Profile_" + Guid.NewGuid() + ".jpg";
        //        var uploadPath = HttpContext.Server.MapPath(subpath);
        //        var path = Path.Combine(uploadPath, Path.GetFileName(fileName));

        //        System.IO.File.WriteAllBytes(path, contents);



        //        UserMedia currentUerMedia = new UserMedia();
        //        currentUerMedia.UserID = UserID;
        //        currentUerMedia.type = 1;
        //        currentUerMedia.ImageUrl = "/images/userProfiles/" + fileName;
        //        db.UserMedia.Add(currentUerMedia);
        //        db.SaveChanges();
        //        return Ok(new { code, Message });
        //    }
        //    else
        //    {
        //        code = 401;
        //        Message = "Login First";
        //        return BadRequest(new { code, Message });
        //    }

        //}


        public class FileDetails
        {
            public string Name { get; set; }
            public string Path { get; set; }
        }




        [Route("Post")]
        [HttpPost]
        public async Task<IActionResult> UploadFiles(List<IFormFile> files)
        {
            //List<IFormFile> files = new List<IFormFile>();
            if (files == null || files.Count == 0)
                return Content("files not selected");

            foreach (var file in files)
            {
                var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot/userPosts/",
                        file.GetFilename());

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }

            return Ok("ImageUpload");
        }







        public async Task<IActionResult> SaveProfile(FileInputModel model)
        {
            string Message;
            int code;
            long UserID = Convert.ToInt64(Request.GetHeader("UserID"));
            var isUserExist = db.User.Where(u => u.UserID == UserID).FirstOrDefault();
            if (isUserExist != null)
            {
                code = 200;
                Message = "Saved";
                string subpath = "~/images/userProfiles/";
                string fileName = UserID + "_Profile_" + Guid.NewGuid() + ".jpg";

                if (model.FileToUpload != null && model.FileToUpload.Count > 0)
                {

                    foreach (IFormFile file in model.FileToUpload)
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
                        currentUerMedia.ImageUrl = "wwwroot/images/userProfiles/" + fileName;
                        db.UserMedia.Add(currentUerMedia);
                        db.SaveChanges();

                    }

                }

             


                return Ok(new { code, Message });
            }
            else
            {
                code = 401;
                Message = "Login First";
                return Ok(new { code, Message });
            }

        }

        //[HttpPost]
        //public IActionResult UploadImages()
        //{
        //    string Message;
        //    int code;

        //    try
        //    {

        //        var httpContext = System.Web.HttpContext.Current;
        //        List<string> images = new List<string>();
        //        // Check for any uploaded file  
        //        if (httpContext.Request.Files.Count > 0)
        //        {
        //            //Loop through uploaded files  
        //            for (int i = 0; i < httpContext.Request.Files.Count; i++)
        //            {
        //                HttpPostedFile httpPostedFile = httpContext.Request.Files[i];
        //                if (httpPostedFile != null /*&& httpPostedFile.ContentType==*/)
        //                {
        //                    // Construct file save path
        //                    string newfileName = httpPostedFile.FileName.Remove(httpPostedFile.FileName.Length - 4, 4) + "_u_" + Guid.NewGuid() + ".jpg";
        //                    string subpath = "/images/userPosts/";
        //                    var uploadPath = HttpContext.Server.MapPath("~" + subpath);
        //                    var fileSavePath = Path.Combine(uploadPath, newfileName);
        //                    var returnpath = subpath + newfileName;
        //                    // Save the uploaded file  
        //                    httpPostedFile.SaveAs(fileSavePath);

        //                    FileDetails currentImage = new FileDetails();
        //                    currentImage.Name = httpPostedFile.FileName;
        //                    currentImage.Path = returnpath;
        //                    images.Add("../.." + returnpath);
        //                }
        //            }
        //            code = 200;
        //            return BadRequest(new { code, images });
        //        }
        //        Message = "Length Required";
        //        return BadRequest(new { Message });

        //    }
        //    catch (ApplicationException applicationException)
        //    {
        //        return BadRequest(new { HttpStatusCode.InternalServerError });
        //    }
        //    catch (Exception exception)
        //    {
        //        return Json(new { HttpStatusCode.InternalServerError }, JsonRequestBehavior.AllowGet);

        //    }
        //}
        private byte[] convertIntoByte(string byte_array)
        {
            byte[] bytes = System.Convert.FromBase64String((byte_array.Split(',') as string[])[1]);
            return bytes;
        }

    }
}