using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NeeoSocial.Utility;

namespace NeeoSocial.APIControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
       
        DbCalls db = new DbCalls();
        public IActionResult UserDeatile()
        {
            string Message;
            int code;
            HttpContext.Request.Headers.TryGetValue("Authorization", out var UserID);
            var userid = Request.GetHeader("UserID");
            // if (Session["ApplicationUser"] != null)

             
            if (userid != null)
             {
            var User = long.Parse(userid) ; //(Models.User)Session["ApplicationUser"];
                var currentUser = (from k in db.User.Where(u => u.UserID == User/* User.UserID*/).ToList()
                                   select new
                                   {
                                       k.UserID,
                                       k.name,
                                       k.email
                                   }).FirstOrDefault();
                var currentUserProfile = (from k in db.UserMedia.Where(u => u.UserID == User /*User.UserID*/ && u.type == 1).ToList()
                                          select new
                                          {
                                              k.ImageUrl
                                          }).LastOrDefault();

                int numberOffriend = db.Friend.Where(u => u.UserID1 == User  /*User.UserID*/ || u.UserID2 == User /*User.UserID*/).ToList().Count;
                int numberOfPendingReq = db.FriendRequest.Where(u => u.toReq == User /*User.UserID*/).ToList().Count;
                int totalFollowers = numberOffriend + numberOfPendingReq;


                code = 200;
                Message = "User Detail available";
            var result = new OkObjectResult(new { message = "200 OK", currentDate = DateTime.Now , currentUser, currentUserProfile , totalFollowers });
            return result;
           // return Ok(Message);
           // return Json(new { code, Message, currentUser, currentUserProfile, totalFollowers }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //code = 401;
                //Message = "login first";
             var result = new OkObjectResult(new { message = "200 OK", currentDate = DateTime.Now });
            return result;
                //return Json(new { code, Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}