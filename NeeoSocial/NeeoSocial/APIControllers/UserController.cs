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
    [Route("UserApi")]
    [ApiController]
    public class UserController : ControllerBase
    {
        DbCalls db = new DbCalls();
        long ViewdUser;
        [Route("UserDeatile")]
        public IActionResult UserDeatile()
        {
            string Message;
            int code;
            long UserID = Convert.ToInt64(Request.GetHeader("UserID"));
            var isUserExist = db.User.Where(u => u.UserID == UserID).FirstOrDefault();
            if(isUserExist != null)
            {
               
                var currentUser = (from k in db.User.Where(u => u.UserID == UserID).ToList()
                                   select new
                                   {
                                       k.UserID,
                                       k.name,
                                       k.email
                                   }).FirstOrDefault();
                var currentUserProfile = (from k in db.UserMedia.Where(u => u.UserID ==UserID && u.type == 1).ToList()
                                          select new
                                          {
                                              k.ImageUrl
                                          }).LastOrDefault();

                int numberOffriend = db.Friend.Where(u => u.UserID1 == UserID || u.UserID2 == UserID).ToList().Count;
                int numberOfPendingReq = db.FriendRequest.Where(u => u.toReq == UserID).ToList().Count;
                int totalFollowers = numberOffriend + numberOfPendingReq;


                code = 200;
                Message = "User Detail available";
                return Ok(new { code = code, Message= Message, currentUser=currentUser, currentUserProfile= currentUserProfile, totalFollowers=totalFollowers });
            }
            else
            {
                code = 401;
                Message = "login first";
                return BadRequest(new { code, Message });
            }
        }
        public IActionResult UserDeatileForOther()
        {
            string Message;
            int code;
            long UserID = Convert.ToInt64(Request.GetHeader("UserID"));
            var isUserExist = db.User.Where(u => u.UserID == UserID).FirstOrDefault();

            if (isUserExist != null)
            {
                    var currentUser = (from k in db.User.Where(u => u.UserID == UserID).ToList()
                                       select new
                                       {
                                           k.UserID,
                                           k.name,
                                           k.email
                                       }).FirstOrDefault();
                    var currentUserProfile = (from k in db.UserMedia.Where(u => u.UserID == UserID && u.type == 1).ToList()
                                              select new
                                              {
                                                  k.ImageUrl
                                              }).LastOrDefault();

                    int numberOffriend = db.Friend.Where(u => u.UserID1 == UserID || u.UserID2 == UserID).ToList().Count;
                    int numberOfPendingReq = db.FriendRequest.Where(u => u.toReq == UserID).ToList().Count;
                    int totalFollowers = numberOffriend + numberOfPendingReq;


                    code = 200;
                    Message = "User Detail available";
                    return Ok(new { code, Message, currentUser, currentUserProfile, totalFollowers });
            }
            else
            {
                code = 401;
                Message = "login first";
                return BadRequest(new { code, Message });
            }
        }
        public IActionResult viewUser(int UserID)
        {
            string Message;
            int code;
            long uID = Convert.ToInt64(Request.GetHeader("UserID"));
            var isUserExist = db.User.Where(u => u.UserID == UserID).FirstOrDefault();

            if (isUserExist != null)
            {
               
                if (uID == UserID)
                {
                    code = 200;
                    Message = "Same User";
                    return Ok(new { code, Message });
                }
                else
                {
                    ViewdUser = UserID;
                    code = 200;
                    Message = "User Set";
                    return Ok(new { code, Message, ViewdUser});
                }
            }
            else
            {
                code = 401;
                Message = "login first";
                return BadRequest(new { code, Message });
            }
        }
    }
}