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
    [Route("api/[controller]")]
    [ApiController]
    public class FriendController : ControllerBase
    {
        DbCalls db = new DbCalls();
        [Route("AddFriend")]
        [HttpPost]
        public IActionResult addFriend(Int64 FriendID)
        {
            string Message;
            int code;
            HttpContext.Request.Headers.TryGetValue("Authorization", out var UserID);
            var userid = Request.GetHeader("UserID");

            // if (Session["ApplicationUser"] != null && FriendID != 0)

            if (userid != null && FriendID != 0)
            {
                //var User = (Models.User)Session["ApplicationUser"];

                var check = db.FriendRequest.Where(c => c.fromReq == long.Parse(userid) /*User.UserID*/ && c.toReq == FriendID).FirstOrDefault();
                var confirmCheck = db.FriendRequest.Where(c => c.fromReq == FriendID && c.toReq == long.Parse(userid)/* User.UserID*/).FirstOrDefault();
                var isAlreadyFriend = db.Friend.Where(c => (c.UserID1 == FriendID && c.UserID2 == long.Parse(userid) /*User.UserID*/) || (c.UserID1 == long.Parse(userid)/* User.UserID*/ && c.UserID2 == FriendID)).FirstOrDefault();
                if (check != null)
                {
                    code = 200;
                    Message = "Request Already Sent";
                    return Ok(Message);

                    //return Json(new { code, Message }, JsonRequestBehavior.AllowGet);

                }
                if (confirmCheck == null && isAlreadyFriend == null)
                {
                    FriendRequest friendReq = new FriendRequest();
                    friendReq.fromReq = long.Parse(userid) /*User.UserID*/;
                    friendReq.toReq = FriendID;
                    db.FriendRequest.Add(friendReq);
                    db.SaveChanges();
                    code = 200;
                    Message = "Request sent";
                    return Ok(Message);
                   // return Json(new { code, Message }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    code = 400;
                    Message = "Unautherized Changing";
                    return BadRequest(Message);
                }
            }
            else
            {
                code = 401;
                Message = "login First";
                return BadRequest(Message);
            }
        }
        [Route("CancelFriendRequest")]
        [HttpPost]
        public IActionResult cancelSentRequest(Int64 FriendID)
        {
            string Message;
            int code;
            HttpContext.Request.Headers.TryGetValue("Authorization", out var UserID);
            var userid = Request.GetHeader("UserID");
            //if (Session["ApplicationUser"] != null && FriendID != 0)
            if (userid != null &&  FriendID != 0)
            {
               // var User = (Models.User)Session["ApplicationUser"];

                var checkOfReq = db.FriendRequest.Where(c => c.fromReq == long.Parse(userid) /*User.UserID */&& c.toReq == FriendID).FirstOrDefault();
                if (checkOfReq != null)
                {
                    db.FriendRequest.Remove(checkOfReq);
                    db.SaveChanges();
                    code = 200;
                    Message = "Request Cancelled";
                    return Ok(Message);
                }
                else
                {
                    code = 400;
                    Message = "Unauthorized changing";
                    return Unauthorized(Message);
                }

            }
            else
            {
                code = 401;
                Message = "login First";
                return BadRequest(Message);
            }
        }
        [Route("CheckRequestStatus")]
        [HttpPost]
        public IActionResult checkRequestStatus(Int64 FriendID)
        {
            string Message;
            int code;
            HttpContext.Request.Headers.TryGetValue("Authorization", out var UserID);
            var userid = Request.GetHeader("UserID");
            //if (Session["ApplicationUser"] != null && FriendID != 0)
            if (userid != null &&  FriendID != 0)
            {
              //  var User = (Models.User)Session["ApplicationUser"];
                var check = db.FriendRequest.Where(c => c.fromReq == long.Parse(userid) /*User.UserID*/ && c.toReq == FriendID).FirstOrDefault();
                var Confirmcheck = db.FriendRequest.Where(c => c.fromReq == FriendID && c.toReq == long.Parse(userid) /*User.UserID*/).FirstOrDefault();
                var isAlreadyFriend = db.Friend.Where(c => (c.UserID1 == FriendID && c.UserID2 == long.Parse(userid) /*User.UserID*/) || (c.UserID1 == long.Parse(userid) /*User.UserID*/ && c.UserID2 == FriendID)).FirstOrDefault();
                if (check != null)
                {
                    code = 200;
                    Message = "Request Already Sent";
                    return Ok(Message);
                }
                if (Confirmcheck != null)
                {
                    code = 200;
                    Message = "Request Available";
                    return Ok(Message);

                }
                if (isAlreadyFriend != null)
                {
                    code = 200;
                    Message = "Already Friend";
                    return Ok(Message);
                }

                else
                {
                    code = 200;
                    Message = "Add friend";
                    return Ok(Message);

                }
            }
            else
            {
                code = 401;
                Message = "login First";
                return Ok(Message);

            }
        }
        [Route("ConfirmFriend")]
        [HttpPost]
        public IActionResult confirmFriend(Int64 FriendID)
        {
            string Message;
            int code;
            HttpContext.Request.Headers.TryGetValue("Authorization", out var UserID);
            var userid = Request.GetHeader("UserID");
            //if (Session["ApplicationUser"] != null && FriendID != 0)
            if (userid != null && FriendID != 0)
            {
               // var User = (Models.User)Session["ApplicationUser"];

                var checkOfReq = db.FriendRequest.Where(c => c.fromReq == FriendID && c.toReq == long.Parse(userid) /*User.UserID*/).FirstOrDefault();
                var isAlreadyFriend = db.Friend.Where(c => (c.UserID1 == FriendID && c.UserID2 == long.Parse(userid) /*User.UserID*/) || (c.UserID1 == long.Parse(userid)/* User.UserID*/ && c.UserID2 == FriendID)).FirstOrDefault();
                if (checkOfReq != null && isAlreadyFriend == null)
                {
                    Friend newFriend = new Friend();
                    newFriend.UserID1 = long.Parse(userid)/*User.UserID*/;
                    newFriend.UserID2 = FriendID;
                    db.Friend.Add(newFriend);
                    db.SaveChanges();

                    db.FriendRequest.Remove(checkOfReq);
                    db.SaveChanges();
                    code = 200;
                    Message = "Friend added";
                    return Ok(Message);

                }
                else
                {
                    code = 401;
                    Message = "Unautherized Changing";
                    return Unauthorized(Message);
                }
            }
            else
            {
                code = 401;
                Message = "login First";
                return BadRequest(Message);
            }
        }
        [Route("RejectRequest")]
        [HttpPost]
        public IActionResult rejectRequest(Int64 FriendID)
        {
            string Message;
            int code;
            HttpContext.Request.Headers.TryGetValue("Authorization", out var UserID);
            var userid = Request.GetHeader("UserID");
            // if (Session["ApplicationUser"] != null && FriendID != 0)
            if (userid != null &&  FriendID != 0)
                {
               // var User = (Models.User)Session["ApplicationUser"];

                var checkOfReq = db.FriendRequest.Where(c => c.fromReq == FriendID && c.toReq == long.Parse(userid) /*User.UserID*/).FirstOrDefault();
                if (checkOfReq != null)
                {
                    db.FriendRequest.Remove(checkOfReq);
                    db.SaveChanges();
                    code = 200;
                    Message = "Request Rejected";
                    return Ok(Message);
                   // return Json(new { code, Message }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    code = 401;
                    Message = "Unauthorized changing";
                    return Unauthorized(Message);
                    //return Json(new { code, Message }, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                code = 401;
                Message = "login First";
                return BadRequest(Message);
                //return Json(new { code, Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [Route("PendingFriendRequests")]
        [HttpPost]
        public IActionResult pendingFriendRequests()
        {
            string Message;
            int code;
            HttpContext.Request.Headers.TryGetValue("Authorization", out var UserID);
            var userid = Request.GetHeader("UserID");
            //if (Session["ApplicationUser"] != null)
            if (userid != null)
            {
               // var User = (Models.User)Session["ApplicationUser"];
                var friendRequests = (from k in db.FriendRequest.Where(u => u.toReq == long.Parse(userid)/*User.UserID*/).ToList()
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
                return Ok(Message);
                //return Json(new { code, Message, friendRequests }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                code = 401;
                Message = "login First";
                return BadRequest(Message);
            }
        }
      
        [Route("CurrentUserFriendList")]
        [HttpGet]
        public IActionResult currentUserFriendList()
        {
            string Message;
            int code;
            HttpContext.Request.Headers.TryGetValue("Authorization", out var UserID);
            var userid = Request.GetHeader("UserID");

            if (userid != null)
            {
                // var User = (Models.User)Session["ApplicationUser"];
                var friendList = (from k in db.Friend.Where(u => u.UserID1 == long.Parse(userid)/*User.UserID */|| u.UserID2 == long.Parse(userid) /* User.UserID*/).ToList()
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
            return Ok(friendList);

                //return Json(new { code, Message, friendList }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                code = 401;
                Message = "login First";
                return BadRequest(Message);
            }
        }

        [Route("SelectedUserFriendList")]
        [HttpGet]
        public IActionResult selectedUserFriendList(Int64 FriendID)
        {
            string Message;
            int code;
            HttpContext.Request.Headers.TryGetValue("Authorization", out var UserID);
            var userid = Request.GetHeader("UserID");
            //if (Session["ApplicationUser"] != null)
            //{
            if (userid != null)
            {
                //  var User = (Models.User)Session["ApplicationUser"];
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
            return Ok(friendList);
              //  return Json(new { code, Message, friendList }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                code = 401;
                Message = "login First";
                return BadRequest(Message);
            }
        }

        [Route("UnFriend")]
        [HttpPost]
        public IActionResult unfriend(Int64 FriendID)
        {
            string Message;
            int code;
            HttpContext.Request.Headers.TryGetValue("Authorization", out var UserID);
            var userid = Request.GetHeader("UserID");
            if (userid != null)
            {
                //  var User = (Models.User)Session["ApplicationUser"];
                var friendForUnfriend = db.Friend.Where(u => (u.UserID1 == long.Parse(userid) /*User.UserID*/ && u.UserID2 == FriendID) || (u.UserID1 == FriendID && u.UserID2 == 1/*User.UserID*/)).FirstOrDefault();
                if (friendForUnfriend != null)
                {
                    db.Friend.Remove(friendForUnfriend);
                    db.SaveChanges();
                    code = 200;
                    Message = "success";
                return Ok(Message);
                    //eturn Json(new { code, Message }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    code = 400;
                    Message = "Unauthorized changing";
                return BadRequest(Message);
                   // return Json(new { code, Message }, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                code = 401;
                Message = "login First";
                return BadRequest(Message);
            }
        }

        // uncompelate  Web API
        public IActionResult viewUserFrnd(int UserID)
        {
            string Message;
            int code;

            //if (Session["ApplicationUser"] != null)
            //{

                //Session["ViewdUser"] = UserID;
                code = 200;
                Message = "User Set";
            return Ok(Message);
              //  return Json(new { code, Message }, JsonRequestBehavior.AllowGet);

            }
            //else
            //{
            //    code = 401;
            //    Message = "login first";
            //    return Json(new { code, Message }, JsonRequestBehavior.AllowGet);
            //}
       // }
    }
}