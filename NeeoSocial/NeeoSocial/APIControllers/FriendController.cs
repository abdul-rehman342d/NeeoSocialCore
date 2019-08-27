using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NeeoSocial.Utility;
using NewSocial.Models;

namespace NeeoSocial.APIControllers
{
    [Route("FriendApi")]
    [ApiController]
    public class FriendController : ControllerBase
    {
        DbCalls db = new DbCalls();
        public IActionResult addFriend(Int64 FriendID)
        {
            string Message;
            int code;
            long UserID = Convert.ToInt64(Request.GetHeader("UserID"));
            var isUserExist = db.User.Where(u => u.UserID == UserID).FirstOrDefault();

            if (isUserExist != null && FriendID != 0)
            {
                var check = db.FriendRequest.Where(c => c.fromReq == UserID && c.toReq == FriendID).FirstOrDefault();
                var confirmCheck = db.FriendRequest.Where(c => c.fromReq == FriendID && c.toReq == UserID).FirstOrDefault();
                var isAlreadyFriend = db.Friend.Where(c => (c.UserID1 == FriendID && c.UserID2 == UserID) || (c.UserID1 == UserID && c.UserID2 == FriendID)).FirstOrDefault();
                if (check != null)
                {
                    code = 200;
                    Message = "Request Already Sent";
                    return Ok(new { code, Message });

                }
                if (confirmCheck == null && isAlreadyFriend == null)
                {
                    FriendRequest friendReq = new FriendRequest();
                    friendReq.fromReq = UserID;
                    friendReq.toReq = FriendID;
                    db.FriendRequest.Add(friendReq);
                    db.SaveChanges();
                    code = 200;
                    Message = "Request sent";
                    return Ok(new { code, Message });

                }
                else
                {
                    code = 400;
                    Message = "Unautherized Changing";
                    return BadRequest(new { code, Message });
                }
            }
            else
            {
                code = 401;
                Message = "login First";
                return BadRequest(new { code, Message });
            }
        }
        public IActionResult cancelSentRequest(Int64 FriendID)
        {
            string Message;
            int code;
            long UserID = Convert.ToInt64(Request.GetHeader("UserID"));
            var isUserExist = db.User.Where(u => u.UserID == UserID).FirstOrDefault();

            if (isUserExist != null && FriendID != 0)
            {
                
                var checkOfReq = db.FriendRequest.Where(c => c.fromReq == UserID && c.toReq == FriendID).FirstOrDefault();
                if (checkOfReq != null)
                {
                    db.FriendRequest.Remove(checkOfReq);
                    db.SaveChanges();
                    code = 200;
                    Message = "Request Cancelled";
                    return Ok(new { code, Message });
                }
                else
                {
                    code = 400;
                    Message = "Unauthorized changing";
                    return BadRequest(new { code, Message });
                }
            }
            else
            {
                code = 401;
                Message = "login First";
                return BadRequest(new { code, Message });
            }
        }
        public IActionResult checkRequestStatus(Int64 FriendID)
        {
            string Message;
            int code;
            long UserID = Convert.ToInt64(Request.GetHeader("UserID"));
            var isUserExist = db.User.Where(u => u.UserID == UserID).FirstOrDefault();

            if (isUserExist != null && FriendID != 0)
            {
               
                var check = db.FriendRequest.Where(c => c.fromReq == UserID && c.toReq == FriendID).FirstOrDefault();
                var Confirmcheck = db.FriendRequest.Where(c => c.fromReq == FriendID && c.toReq ==UserID).FirstOrDefault();
                var isAlreadyFriend = db.Friend.Where(c => (c.UserID1 == FriendID && c.UserID2 == UserID) || (c.UserID1 == UserID && c.UserID2 == FriendID)).FirstOrDefault();
                if (check != null)
                {
                    code = 200;
                    Message = "Request Already Sent";
                    return Ok(new { code, Message });

                }
                if (Confirmcheck != null)
                {
                    code = 200;
                    Message = "Request Available";
                    return Ok(new { code, Message });

                }
                if (isAlreadyFriend != null)
                {
                    code = 200;
                    Message = "Already Friend";
                    return Ok(new { code, Message });

                }

                else
                {
                    code = 200;
                    Message = "Add friend";
                    return Ok(new { code, Message });
                }
            }
            else
            {
                code = 401;
                Message = "login First";
                return BadRequest(new { code, Message });
            }
        }
        public IActionResult confirmFriend(Int64 FriendID)
        {
            string Message;
            int code;
            long UserID = Convert.ToInt64(Request.GetHeader("UserID"));
            var isUserExist = db.User.Where(u => u.UserID == UserID).FirstOrDefault();

            if (isUserExist != null && FriendID != 0)
            {
              
                var checkOfReq = db.FriendRequest.Where(c => c.fromReq == FriendID && c.toReq == UserID).FirstOrDefault();
                var isAlreadyFriend = db.Friend.Where(c => (c.UserID1 == FriendID && c.UserID2 == UserID) || (c.UserID1 == UserID && c.UserID2 == FriendID)).FirstOrDefault();
                if (checkOfReq != null && isAlreadyFriend == null)
                {
                    Friend newFriend = new Friend();
                    newFriend.UserID1 = UserID;
                    newFriend.UserID2 = FriendID;
                    db.Friend.Add(newFriend);
                    db.SaveChanges();

                    db.FriendRequest.Remove(checkOfReq);
                    db.SaveChanges();
                    code = 200;
                    Message = "Friend added";
                    return Ok(new { code, Message });
                }
                else
                {
                    code = 401;
                    Message = "Unautherized Changing";
                    return BadRequest(new { code, Message });
                }
            }
            else
            {
                code = 401;
                Message = "login First";
                return BadRequest(new { code, Message });
            }
        }
        public IActionResult rejectRequest(Int64 FriendID)
        {
            string Message;
            int code;
            long UserID = Convert.ToInt64(Request.GetHeader("UserID"));
            var isUserExist = db.User.Where(u => u.UserID == UserID).FirstOrDefault();
            if (isUserExist != null && FriendID != 0)
            {
               
                var checkOfReq = db.FriendRequest.Where(c => c.fromReq == FriendID && c.toReq == UserID).FirstOrDefault();
                if (checkOfReq != null)
                {
                    db.FriendRequest.Remove(checkOfReq);
                    db.SaveChanges();
                    code = 200;
                    Message = "Request Rejected";
                    return Ok(new { code, Message });
                }
                else
                {
                    code = 401;
                    Message = "Unauthorized changing";
                    return BadRequest(new { code, Message });
                }

            }
            else
            {
                code = 401;
                Message = "login First";
                return BadRequest(new { code, Message });
            }
        }
        public IActionResult pendingFriendRequests()
        {
            string Message;
            int code;
            long UserID = Convert.ToInt64(Request.GetHeader("UserID"));
            var isUserExist = db.User.Where(u => u.UserID == UserID).FirstOrDefault();

            if (isUserExist != null)
            {   var friendRequests = (from k in db.FriendRequest.Where(u => u.toReq == UserID).ToList()
                                      select new
                                      {
                                          k.FriendRequestID,
                                          User = (from j in db.User.Where(j => j.UserID == k.fromReq).ToList()
                                                  select new
                                                  {
                                                      j.UserID,
                                                      j.name,
                                                      userProfile = (from x in db.UserMedia.Where(u => u.UserID == j.UserID && u.type == 1).ToList()
                                                                     select new
                                                                     {
                                                                         x.ImageUrl
                                                                     }).LastOrDefault(),
                                                  }).ToList().FirstOrDefault(),
                                      }).ToList().OrderByDescending(u => u.FriendRequestID);
                code = 200;
                Message = "success";
                return Ok(new { code, Message, friendRequests });
            }
            else
            {
                code = 401;
                Message = "login First";
                return BadRequest(new { code, Message});
            }
        }
        public IActionResult currentUserFriendList()
        {
            string Message;
            int code;
            long UserID = Convert.ToInt64(Request.GetHeader("UserID"));
            var isUserExist = db.User.Where(u => u.UserID == UserID).FirstOrDefault();

            if (isUserExist != null)
            {
                var friendList = (from k in db.Friend.Where(u => u.UserID1 == UserID || u.UserID2 == UserID).ToList()
                                  select new
                                  {
                                      User1 = (from j in db.User.Where(j => j.UserID == k.UserID1).ToList()
                                               select new
                                               {
                                                   j.UserID,
                                                   j.name,
                                                   userProfile = (from x in db.UserMedia.Where(u => u.UserID == j.UserID && u.type == 1).ToList()
                                                                  select new
                                                                  {
                                                                      x.ImageUrl
                                                                  }).LastOrDefault(),
                                               }).ToList().FirstOrDefault(),
                                      User2 = (from j in db.User.Where(j => j.UserID == k.UserID2).ToList()
                                               select new
                                               {
                                                   j.UserID,
                                                   j.name,
                                                   userProfile = (from x in db.UserMedia.Where(u => u.UserID == j.UserID && u.type == 1).ToList()
                                                                  select new
                                                                  {
                                                                      x.ImageUrl
                                                                  }).LastOrDefault(),
                                               }).ToList().FirstOrDefault(),


                                  }).ToList();
                code = 200;
                Message = "success";
                return BadRequest(new { code, Message,friendList });
            }
            else
            {
                code = 401;
                Message = "login First";
                return BadRequest(new { code, Message });
            }
        }
        public IActionResult unfriend(Int64 FriendID)
        {
            string Message;
            int code;
            long UserID = Convert.ToInt64(Request.GetHeader("UserID"));
            var isUserExist = db.User.Where(u => u.UserID == UserID).FirstOrDefault();

            if (isUserExist != null)
            {
                 var friendForUnfriend = db.Friend.Where(u => (u.UserID1 == UserID && u.UserID2 == FriendID) || (u.UserID1 == FriendID && u.UserID2 == UserID)).FirstOrDefault();
                if (friendForUnfriend != null)
                {
                    db.Friend.Remove(friendForUnfriend);
                    db.SaveChanges();
                    code = 200;
                    Message = "success";
                     return Ok(new { code, Message});
                }
                else
                {
                    code = 400;
                    Message = "Unauthorized changing";
                    return BadRequest(new { code, Message });
                }

            }
            else
            {
                code = 401;
                Message = "login First";
                return BadRequest(new { code, Message });
            }
        }

        public IActionResult selectedUserFriendList(Int64 FriendID)
        {
            string Message;
            int code;
            long UserID = Convert.ToInt64(Request.GetHeader("UserID"));
            var isUserExist = db.User.Where(u => u.UserID == UserID).FirstOrDefault();

            if (isUserExist != null)
            {
                var friendList = (from k in db.Friend.Where(u => u.UserID1 == FriendID || u.UserID2 == FriendID).ToList()
                                  select new
                                  {
                                      User1 = (from j in db.User.Where(j => j.UserID == k.UserID1).ToList()
                                               select new
                                               {
                                                   j.UserID,
                                                   j.name,
                                                   userProfile = (from x in db.UserMedia.Where(u => u.UserID == j.UserID && u.type == 1).ToList()
                                                                  select new
                                                                  {
                                                                      x.ImageUrl
                                                                  }).LastOrDefault(),
                                               }).ToList().FirstOrDefault(),
                                      User2 = (from j in db.User.Where(j => j.UserID == k.UserID2).ToList()
                                               select new
                                               {
                                                   j.UserID,
                                                   j.name,
                                                   userProfile = (from x in db.UserMedia.Where(u => u.UserID == j.UserID && u.type == 1).ToList()
                                                                  select new
                                                                  {
                                                                      x.ImageUrl
                                                                  }).LastOrDefault(),
                                               }).ToList().FirstOrDefault(),


                                  }).ToList();
                code = 200;
                Message = "success";
                return Ok(new { code, Message,friendList });
            }
            else
            {
                code = 401;
                Message = "login First";
                return BadRequest(new { code, Message });
            }
        }

        public IActionResult viewUserFrnd(int UserID)
        {
            string Message;
            int code;
            long uID = Convert.ToInt64(Request.GetHeader("UserID"));
            var isUserExist = db.User.Where(u => u.UserID == UserID).FirstOrDefault();

            if (isUserExist != null)
            {

                //Session["ViewdUser"] = UserID;
                code = 200;
                Message = "User Set";
                return Ok(new { code, Message,UserID });
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