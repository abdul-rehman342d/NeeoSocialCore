using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NeeoSocial.Utility;
using NewSocial.Models;

namespace NeeoSocial.APIControllers
{

  
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {


        DbCalls db = new DbCalls();
        public IActionResult SaveProfile(string image)
        {
            string Message;
            int code;
            //if (Session["ApplicationUser"] != null)
            //{
               // var User = (Models.User)Session["ApplicationUser"];
                code = 200;
                Message = "Saved";
                UserMedia currentUerMedia = new UserMedia();
                currentUerMedia.UserID = 1/* User.UserID*/;
                db.UserMedia.Add(currentUerMedia);
                db.SaveChanges();
            return Ok(Message);
                //return Json(new { code, Message }, JsonRequestBehavior.AllowGet);
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
            byte[] bytes;
            if (byte_array != "")
            {

                bytes = System.Convert.FromBase64String((byte_array.Split(',') as string[])[1]);
                return bytes;
            }
            else
            {
                bytes = null;
            }
            return bytes;
        }
        public string convertByteIntostring(byte[] currentImage)
        {
            string image;
            if (currentImage == null)
            {
                image = "Not Available";
            }
            else
            {
                image = "data:image/jpeg;base64," + Convert.ToBase64String(currentImage);
            }

            return image;
        }

        public IActionResult addPost(string text, string image, string video)
        {
            string Message;
            int code;

            HttpContext.Request.Headers.TryGetValue("Authorization", out var UserID);
            var userid = Request.GetHeader("UserID");
            //if (Session["ApplicationUser"] != null)
            if (userid != null)
            {
            if ((text != "" || image != "" || video != "") && text.Length <= 500)
                {
                // var User = (Models.User)Session["ApplicationUser"];
        

                Post currentPost = new Post();
                    if (image != "" && image.Length >= 50)
                    {
                        byte[] contents = convertIntoByte(image);
                        string subpath = "~/images/userPosts/";
                        string fileName = long.Parse(userid) /*User.UserID*/ + "_Post_" + Guid.NewGuid() + ".jpg";
                        var uploadPath = "adsds"/* HttpContext.Server.MapPath(subpath)*/;
                        var path = Path.Combine(uploadPath, Path.GetFileName(fileName));
                        System.IO.File.WriteAllBytes(path, contents);

                        currentPost.imageURL = "/images/userPosts/" + fileName;
                    }
                    currentPost.UserID = long.Parse(userid) /*User.UserID*/;
                    currentPost.text = text;
                    currentPost.postTime = DateTime.UtcNow;
                    currentPost.updateTime = DateTime.UtcNow;
                    db.Post.Add(currentPost);
                    db.SaveChanges();
                    code = 200;
                    Message = "Post Successfully added";
                return Ok(Message);
                    //return Json(new { code, Message }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    code = 400;
                    Message = "UnAuthorized Changing";
                return BadRequest(Message);
                    //return Json(new { code, Message }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                code = 401;
                Message = "Login First";
                return BadRequest(Message);
            }


        }

        #region POSTLIST ........ LEFT

        public IActionResult postList()
        {
            string Message;
            int code;
            HttpContext.Request.Headers.TryGetValue("Authorization", out var UserID);
           var userid = Request.GetHeader("UserID");
            //if (Session["ApplicationUser"] != null)
                if (userid != null)
                {
                //var serializer = new JavaScriptSerializer();
                //serializer.MaxJsonLength = Int32.MaxValue;
                //var User = (Models.User)Session["ApplicationUser"];




                var friendlist1 = (from f in db.Friend.Where(u => u.UserID1 == long.Parse(userid)/* User.UserID*/).ToList()
                                   select new
                                   {
                                       f.UserID2,
                                   }).ToList();
                var friendlist2 = (from f in db.Friend.Where(u => u.UserID2 == long.Parse(userid) /* User.UserID*/).ToList()
                                   select new
                                   {
                                       f.UserID1,
                                   }).ToList();
                List<long> friendIds = new List<long>();
                for (int i = 0; i < friendlist1.Count; i++)
                {
                    friendIds.Add(friendlist1[i].UserID2);
                }
                for (int i = 0; i < friendlist2.Count; i++)
                {
                    friendIds.Add(friendlist2[i].UserID1);
                }
                friendIds.Add(long.Parse(userid) /*User.UserID*/);
                var postList = (from ep in friendIds
                                join i in db.Post.Include("Comments").Include("Reactions").ToList()
                                 on ep equals i.UserID
                                select new
                                {
                                    i.PostID,
                                    i.UserID,

                                    userProfile = (from k in db.UserMedia.Where(u => u.UserID == i.UserID && u.type == 1).ToList()
                                                   select new
                                                   {
                                                       k.ImageUrl
                                                   }).LastOrDefault(),
                                    userName = (from j in db.User.Where(u => u.UserID == i.UserID).ToList()
                                                select new
                                                {
                                                    j.name
                                                }).FirstOrDefault(),
                                    i.text,
                                    imageURl = "../../" + i.imageURL,
                                    i.video,
                                    postTime = TimeZone.CurrentTimeZone.ToLocalTime(i.postTime),
                                    agreeCount = i.Reactions.Where(u => u.reactionType == 1).ToList().Count(),
                                    agree = (from j in i.Reactions.Where(u => u.reactionType == 1).ToList()
                                             select new
                                             {
                                                 userName = (from x in db.User.Where(x => x.UserID == j.UserID)
                                                             select new
                                                             {
                                                                 x.name,
                                                                 x.UserID
                                                             }
                                                             ).FirstOrDefault(),
                                             }).ToList(),
                                    disagreeCount = i.Reactions.Where(u => u.reactionType == 0).ToList().Count(),
                                    disAgree = (from j in i.Reactions.Where(u => u.reactionType == 0).ToList()
                                                select new
                                                {
                                                    userName = (from x in db.User.Where(x => x.UserID == j.UserID)
                                                                select new
                                                                {
                                                                    x.name,
                                                                    x.UserID
                                                                }
                                                                ).FirstOrDefault(),
                                                }).ToList(),
                                    Comments = (from j in i.Comments
                                                select new
                                                {
                                                    userProfile = (from k in db.UserMedia.Where(u => u.UserID == j.UserID && u.type == 1).ToList()
                                                                   select new
                                                                   {
                                                                       k.ImageUrl
                                                                   }).LastOrDefault(),

                                                    userName = (from x in db.User.Where(x => x.UserID == j.UserID)
                                                                select new
                                                                {
                                                                    x.name,
                                                                    x.UserID
                                                                }
                                                                ).FirstOrDefault(),
                                                    j.CommentID,
                                                    j.commentText,
                                                    commentTime = j.commentTime.ToString("dd-MMM-yy hh:mm tt"),


                                                    subAgreeCount = db.SubReaction.Where(u => u.reactionType == 1 && u.CommentID == j.CommentID).ToList().Count(),
                                                    subDisAgreeCount = db.SubReaction.Where(u => u.reactionType == 0 && u.CommentID == j.CommentID).ToList().Count(),
                                                }).ToList(),
                                }).ToList().OrderByDescending(u => u.postTime).Take(30);
                code = 200;
                Message = "Post List Available";
                return Ok(postList);
                //return  Json(new { code, Message, postList }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                code = 401;
                Message = "Login First";
                return BadRequest(Message);
                //return Json(new { code, Message }, JsonRequestBehavior.AllowGet);

            }
        }
        #endregion

        #region postListofCurrentUser LEFT
        public IActionResult postListofCurrentUser()
        {
            string Message;
            int code;
            HttpContext.Request.Headers.TryGetValue("Authorization", out var UserID);
            var userid = Request.GetHeader("UserID");
            //if (Session["ApplicationUser"] != null)
            if (userid  != null)

                {


             //   var User = (Models.User)Session["ApplicationUser"];
                var postList = (from i in db.Post.Include("Comments").Include("Reactions").Where(u => u.UserID == long.Parse(userid) /*User.UserID*/).ToList()
                                select new
                                {
                                    i.PostID,
                                    i.UserID,

                                    userProfile = (from k in db.UserMedia.Where(u => u.UserID == i.UserID && u.type == 1).ToList()
                                                   select new
                                                   {
                                                       k.ImageUrl
                                                   }).LastOrDefault(),
                                    userName = (from j in db.User.Where(u => u.UserID == i.UserID).ToList()
                                                select new
                                                {
                                                    j.name
                                                }).FirstOrDefault(),
                                    i.text,
                                    imageURl = "../../" + i.imageURL,
                                    //i.video,
                                    postTime = TimeZone.CurrentTimeZone.ToLocalTime(i.postTime),
                                    agreeCount = i.Reactions.Where(u => u.reactionType == 1).ToList().Count(),
                                    //agree = i.Reactions.Where(u => u.reactionType == 1).ToList(),
                                    disagreeCount = i.Reactions.Where(u => u.reactionType == 0).ToList().Count(),
                                    //disAgree = i.Reactions.Where(u => u.reactionType == 0).ToList(),
                                    Comments = (from j in i.Comments
                                                select new
                                                {
                                                    userProfile = (from k in db.UserMedia.Where(u => u.UserID == j.UserID && u.type == 1).ToList()
                                                                   select new
                                                                   {
                                                                       k.ImageUrl
                                                                   }).LastOrDefault(),

                                                    userName = (from x in db.User.Where(x => x.UserID == j.UserID)
                                                                select new
                                                                {
                                                                    x.name,
                                                                    x.UserID
                                                                }
                                                                ).FirstOrDefault(),
                                                    j.CommentID,
                                                    j.commentText,
                                                    commentTime = j.commentTime.ToString("dd-MMM-yy hh:mm tt")
                                                }).ToList(),
                                }).ToList().OrderByDescending(u => u.PostID).Take(30);
                code = 200;
                Message = "Post List Available";
                return Ok(Message);
                //return Json(new { code, Message, postList }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                code = 401;
                Message = "Login First";
                return BadRequest(Message);
               // return Json(new { code, Message }, JsonRequestBehavior.AllowGet);

            }
        }
        #endregion

        #region postListofSelectedUser ...LEFT

        public IActionResult postListofSelectedUser(Int64 FriendID)
        {
            string Message;
            int code;
            HttpContext.Request.Headers.TryGetValue("Authorization", out var UserID);
            var userid = Request.GetHeader("UserID");
           // if (Session["ApplicationUser"] != null)
           if(userid != null)
            {
                // if (Session["ViewdUser"] != null)
                if (userid != null)
                {
                    var postList = (from i in db.Post.Include("Comments").Include("Reactions").Where(u => u.UserID == FriendID).ToList()
                                    select new
                                    {
                                        i.PostID,
                                        i.UserID,

                                        userProfile = (from k in db.UserMedia.Where(u => u.UserID == i.UserID && u.type == 1).ToList()
                                                       select new
                                                       {
                                                           k.ImageUrl
                                                       }).LastOrDefault(),
                                        userName = (from j in db.User.Where(u => u.UserID == i.UserID).ToList()
                                                    select new
                                                    {
                                                        j.name
                                                    }).FirstOrDefault(),
                                        i.text,
                                        imageURl = "../../" + i.imageURL,
                                        //i.video,
                                        postTime = TimeZone.CurrentTimeZone.ToLocalTime(i.postTime),
                                        agreeCount = i.Reactions.Where(u => u.reactionType == 1).ToList().Count(),
                                        //agree = i.Reactions.Where(u => u.reactionType == 1).ToList(),
                                        disagreeCount = i.Reactions.Where(u => u.reactionType == 0).ToList().Count(),
                                        //disAgree = i.Reactions.Where(u => u.reactionType == 0).ToList(),
                                        Comments = (from j in i.Comments
                                                    select new
                                                    {
                                                        userProfile = (from k in db.UserMedia.Where(u => u.UserID == j.UserID && u.type == 1).ToList()
                                                                       select new
                                                                       {
                                                                           k.ImageUrl
                                                                       }).LastOrDefault(),

                                                        userName = (from x in db.User.Where(x => x.UserID == j.UserID)
                                                                    select new
                                                                    {
                                                                        x.name,
                                                                        x.UserID
                                                                    }
                                                                    ).FirstOrDefault(),
                                                        j.CommentID,
                                                        j.commentText,
                                                        commentTime = j.commentTime.ToString("dd-MMM-yy hh:mm tt"),
                                                        sucomments = (from s in db.SubComment.Where(x => x.CommentID == j.CommentID)
                                                                      select new
                                                                      {
                                                                          userProfile = (from sp in db.UserMedia.Where(u => u.UserID == s.UserID && u.type == 1).ToList()
                                                                                         select new
                                                                                         {
                                                                                             sp.ImageUrl
                                                                                         }).LastOrDefault(),

                                                                          userName = (from sn in db.User.Where(x => x.UserID == s.UserID)
                                                                                      select new
                                                                                      {
                                                                                          sn.name,
                                                                                          sn.UserID
                                                                                      }
                                                                      ).FirstOrDefault(),
                                                                      }
                                                                    ).ToList(),

                                                    }).ToList(),
                                    }).ToList().OrderByDescending(u => u.PostID).Take(30);
                    code = 200;
                    Message = "Post List Available";
                    return Ok(Message);
                   // return Json(new { code, Message, postList }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    code = 400;
                    Message = "Try Again";
                    return BadRequest(Message);
                   // return Json(new { code, Message }, JsonRequestBehavior.AllowGet);

                }


            }
            else
            {
                code = 401;
                Message = "Login First";
                return BadRequest(Message);

            }
        }

        #endregion

        #region deletePost
        //public JsonResult deletePost(int currentPostID)
        //{
        //    string Message;
        //    int code;
        //    if (Session["ApplicationUser"] != null)
        //    {
        //        var User = (Models.User)Session["ApplicationUser"];
        //        var currentPost = db.Post.Where(u => u.PostID == currentPostID && (u.UserID == User.UserID || User.UserID == 1)).FirstOrDefault();
        //        if (currentPost != null)
        //        {
        //            var comments = db.Comment.Where(u => u.PostID == currentPost.PostID).ToList();
        //            var reactions = db.Reaction.Where(u => u.PostID == currentPost.PostID).ToList();

        //            foreach (var comment in comments)
        //            {
        //                db.Comment.Remove(comment);
        //            }
        //            foreach (var reaction in reactions)
        //            {
        //                db.Reaction.Remove(reaction);
        //            }
        //            db.Post.Remove(currentPost);
        //            db.SaveChanges();
        //            code = 200;
        //            Message = "Post Deleted";
        //            return Json(new { code, Message }, JsonRequestBehavior.AllowGet);
        //        }
        //        else
        //        {
        //            code = 400;
        //            Message = "UnAuthorized Changing";
        //            return Json(new { code, Message }, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    else
        //    {
        //        code = 401;
        //        Message = "Login First";
        //        return Json(new { code, Message }, JsonRequestBehavior.AllowGet);
        //    }


        //}
        #endregion

    }
}