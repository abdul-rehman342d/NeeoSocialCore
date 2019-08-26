using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewSocial.Models;

namespace NeeoSocial.APIControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        DbCalls db = new DbCalls();
        [Route("SaveProfile")]
        [HttpPost]
        public IActionResult SaveProfile(string image)
        {
            string Message;
            int code;
            //if (Session["ApplicationUser"] != null)
            //{
                //var User = (Models.User)Session["ApplicationUser"];
                code = 200;
                Message = "Saved";


                byte[] contents = convertIntoByte(image);
                string subpath = "~/images/userProfiles/";
                string fileName = 1 /*User.UserID*/ + "_Profile_" + Guid.NewGuid() + ".jpg";
               var uploadPath =  "NOT Define "/*HttpContext.Server.MapPath(subpath)*/;
                var path = Path.Combine(uploadPath, Path.GetFileName(fileName));

                System.IO.File.WriteAllBytes(path, contents);



                UserMedia currentUerMedia = new UserMedia();
                currentUerMedia.UserID = 1 /*User.UserID*/;
                currentUerMedia.type = 1;
                currentUerMedia.ImageUrl = "/images/userProfiles/" + fileName;
                db.UserMedia.Add(currentUerMedia);
                db.SaveChanges();
                return Ok(Message);
               
           // }


        //else
        //{
        //    code = 401;
        //    Message = "Login First";
        //    return Json(new { code, Message }, JsonRequestBehavior.AllowGet);
        //}

         }

        public byte[] convertIntoByte(string byte_array)
        {
            byte[] bytes = System.Convert.FromBase64String((byte_array.Split(',') as string[])[1]);
            return bytes;
        }
    }
}