using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NeeoSocial.Utility;
using NewSocial.Models;
using Newtonsoft.Json;

namespace NeeoSocial.APIControllers
{
  
    [Route("PostApi")]
    [ApiController]
    public class PostApiController : ControllerBase
    {

        DbCalls db = new DbCalls();
        public IActionResult SaveProfile(string image)
        {
            string Message;
            int code;
            long UserID = Convert.ToInt64(Request.GetHeader("UserID"));
            var isUserExist = db.User.Where(u => u.UserID == UserID).FirstOrDefault();

            if (isUserExist != null)
            {
                code = 200;
                Message = "Saved";
                UserMedia currentUerMedia = new UserMedia();
                currentUerMedia.UserID = UserID;
                db.UserMedia.Add(currentUerMedia);
                db.SaveChanges();
                return Ok(new { code, Message });
            }
            else
            {
                code = 401;
                Message = "Login First";
                return BadRequest(new { code, Message });
            }


        }
        private byte[] convertIntoByte(string byte_array)
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
        private string convertByteIntostring(byte[] currentImage)
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

        public class PostInParam
        {
            public string text;
            public List<string> images;
        }

        [HttpPost]
        [Route("addPost")]
        public IActionResult addPost(PostInParam post)
        {
            string Message;
            int code;
            long UserID = Convert.ToInt64(Request.GetHeader("UserID"));
            var isUserExist = db.User.Where(u => u.UserID == UserID).FirstOrDefault();

            if (isUserExist != null)
            {
                if ((post.text != "" || post.images.Count != 0) && post.text.Length <= 500)
                {
                    Post currentPost = new Post();
                    currentPost.UserID = UserID;
                    currentPost.text = post.text;
                    currentPost.postTime = DateTime.UtcNow;
                    currentPost.updateTime = DateTime.UtcNow;
                    db.Post.Add(currentPost);
                    db.SaveChanges();
                    PostImage currentImage = new PostImage();
                    if (post.images != null)
                    {
                        for (int i = 0; i < post.images.Count; i++)
                        {
                            currentImage.PostID = currentPost.PostID;
                            currentImage.imagePath = post.images[i];
                            db.PostImage.Add(currentImage);
                            db.SaveChanges();
                        }
                    }
                    code = 200;
                    Message = "Post Successfully added";
                    return Ok(new { code, Message });
                }
                else
                {
                    code = 400;
                    Message = "UnAuthorized Changing";
                    return BadRequest(new { code, Message });
                }
            }
            else
            {
                code = 401;
                Message = "Login First";
                return BadRequest(new { code, Message });
            }


        }


        public class SharePostInParam
        {
            public long PostID;
            public string text;
            
        }

        [HttpPost]
        [Route("sharePost")]
        public IActionResult sharePost(SharePostInParam sharePost)
        {

            string Message;
            int code;
            long UserID = Convert.ToInt64(Request.GetHeader("UserID"));
            var isUserExist = db.User.Where(u => u.UserID == UserID).FirstOrDefault();
            if (isUserExist != null)
            {
                var isPostExist = db.Post.Where(u => u.PostID == sharePost.PostID).FirstOrDefault();
                if (isPostExist != null)
                {
                    SharePost currentSharePost = new SharePost();
                    currentSharePost.PostID = sharePost.PostID;
                    currentSharePost.UserID = UserID;
                    currentSharePost.text = sharePost.text;
                    currentSharePost.shareTime = DateTime.UtcNow;
                    currentSharePost.updateTime = DateTime.UtcNow;
                    db.SharePost.Add(currentSharePost);
                    db.SaveChanges();
                    code = 200;
                    Message = "Post Successfully shared";
                    return Ok(new { code, Message });
                }
                else
                {
                    code = 400;
                    Message = "UnAuthorized Changing";
                    return BadRequest(new { code, Message });
                }
            }
            else
            {
                code = 401;
                Message = "Login First";
                return BadRequest(new { code, Message });
            }
        }


        [HttpGet]
        [Route("postList")]
        
        public IActionResult postList()
        {
            string Message;
            int code;
            long UserID = Convert.ToInt64(Request.GetHeader("UserID"));
            var isUserExist = db.User.Where(u => u.UserID == UserID).FirstOrDefault();

            if (isUserExist != null)
            {
                var friendlist1 = (from f in db.Friend.Where(u => u.UserID1 == UserID).ToList()
                                   select new
                                   {
                                       f.UserID2,
                                   }).ToList();
                var friendlist2 = (from f in db.Friend.Where(u => u.UserID2 == UserID).ToList()
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
                friendIds.Add(UserID);
                var postList = (from ep in friendIds
                                join i in db.Post.Include("Comments").Include("Reactions").Include("PostImages").ToList()
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
                                    images = (from img in i.PostImages.Where(u => u.PostID == i.PostID).ToList()
                                              select new
                                              {
                                                  path = "../.." + img.imagePath
                                              }).ToList(),
                                    //i.video,
                                    postTime = i.postTime.ToString("MM/dd/yyyy hh:mm tt"),
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
                return Ok(new { code, Message, postList });

            }
            else
            {
                code = 401;
                Message = "Login First";
                return BadRequest(new { code, Message });

            }
        }
        public IActionResult postListofCurrentUser()
        {
            string Message;
            int code;
            long UserID = Convert.ToInt64(Request.GetHeader("UserID"));
            var isUserExist = db.User.Where(u => u.UserID == UserID).FirstOrDefault();

            if (isUserExist != null)
            {
                var postList = (from i in db.Post.Include("Comments").Include("Reactions").Where(u => u.UserID == UserID).ToList()
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
                return Ok(new { code, Message, postList });
            }
            else
            {
                code = 401;
                Message = "Login First";
                return BadRequest(new { code, Message });

            }
        }
        public IActionResult postListofSelectedUser(Int64 FriendID)
        {
            string Message;
            int code;
            long UserID = Convert.ToInt64(Request.GetHeader("UserID"));
            var isUserExist = db.User.Where(u => u.UserID == UserID).FirstOrDefault();
            if (isUserExist != null)
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
                                                    commentTime = j.commentTime.ToString("dd-MMM-yy hh:mm tt")
                                                }).ToList(),
                                }).ToList().OrderByDescending(u => u.PostID).Take(30);
                code = 200;
                Message = "Post List Available";
                return Ok(new { code, Message, postList });
            }
            else
            {
                code = 401;
                Message = "Login First";
                return BadRequest(new { code, Message });

            }
        }

        [HttpDelete]
        [Route("deletePost/{currentPostID}")]
        public IActionResult deletePost(int currentPostID)
        {
            string Message;
            int code;
            long UserID = Convert.ToInt64(Request.GetHeader("UserID"));
            var isUserExist = db.User.Where(u => u.UserID == UserID).FirstOrDefault();
            if (isUserExist != null)
            {
                var currentPost = db.Post.Where(u => u.PostID == currentPostID && (u.UserID == UserID || UserID == 1)).FirstOrDefault();
                if (currentPost != null)
                {
                    var comments = db.Comment.Where(u => u.PostID == currentPost.PostID).ToList();
                    var reactions = db.Reaction.Where(u => u.PostID == currentPost.PostID).ToList();
                    var sharePosts = db.SharePost.Where(u => u.PostID == currentPost.PostID).ToList();

                    foreach (var comment in comments)
                    {
                        db.Comment.Remove(comment);
                    }
                    foreach (var reaction in reactions)
                    {
                        db.Reaction.Remove(reaction);
                    }
                    foreach (var sharePost in sharePosts)
                    {
                        db.SharePost.Remove(sharePost);
                    }
                    db.Post.Remove(currentPost);
                    db.SaveChanges();
                    code = 200;
                    Message = "Post Deleted";
                    return Ok(new { code, Message });
                }
                else
                {
                    code = 400;
                    Message = "UnAuthorized Changing";
                    return BadRequest(new { code, Message });
                }
            }
            else
            {
                code = 401;
                Message = "Login First";
                return BadRequest(new { code, Message });
            }


        }
       

        [Route("postList1")]
        public IActionResult sharePostList()
        {
            string Message;
            int code;
            long UserID = Convert.ToInt64(Request.GetHeader("UserID"));
            var isUserExist = db.User.Where(u => u.UserID == UserID).FirstOrDefault();
            
            if (isUserExist != null)
            {
                var friendlist1 = (from f in db.Friend.Where(u => u.UserID1 == UserID).ToList()
                                   select new
                                   {
                                       f.UserID2,
                                   }).ToList();
                var friendlist2 = (from f in db.Friend.Where(u => u.UserID2 == UserID).ToList()
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
                friendIds.Add(UserID);



                //var result = (from ep in friendIds
                //              join sp in db.SharePost.ToList()
                //               on ep equals sp.UserID
                //              select new
                //              {
                //                  sp.PostID,
                //                  sp.UserID

                //              }).ToList();


                var sharedPostList = (from ep in friendIds
                                      join sp in db.SharePost.ToList()
                                      on ep equals sp.UserID
                                      join i in db.Post.Include("Comments").Include("Reactions").Include("PostImages").ToList()
                                       on sp.PostID equals i.PostID
                                      select new
                                      {
                                          i.PostID,
                                          i.UserID,


                                          sharedUserProfile = (from k in db.UserMedia.Where(u => u.UserID == sp.UserID && u.type == 1).ToList()
                                                               select new
                                                               {
                                                                   k.ImageUrl
                                                               }).LastOrDefault(),
                                          sharedUserName = (from j in db.User.Where(u => u.UserID == sp.UserID).ToList()
                                                            select new
                                                            {
                                                                j.name
                                                            }).FirstOrDefault(),



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
                                          images = (from img in i.PostImages.Where(u => u.PostID == i.PostID).ToList()
                                                    select new
                                                    {
                                                        path = "../.." + img.imagePath
                                                    }).ToList(),
                                          //i.video,
                                          postTime = TimeZone.CurrentTimeZone.ToLocalTime(sp.shareTime),
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

                var postList = (from ep in friendIds
                                join i in db.Post.Include("Comments").Include("Reactions").Include("PostImages").ToList()
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
                                    images = (from img in i.PostImages.Where(u => u.PostID == i.PostID).ToList()
                                              select new
                                              {
                                                  path = "../.." + img.imagePath
                                              }).ToList(),
                                    //i.video,
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

                Dictionary<dynamic, DateTime> bothTimePosts = new Dictionary<dynamic, DateTime>();

                for (int i = 0; i < postList.Count(); i++)
                {

                    bothTimePosts.Add(postList.ElementAt(i), postList.ElementAt(i).postTime);

                }

                for (int i = 0; i < sharedPostList.Count(); i++)
                {

                    bothTimePosts.Add(sharedPostList.ElementAt(i), postList.ElementAt(i).postTime);

                }

                //var final = tenmp.OrderBy(x => tenmp.Values);
                var result = bothTimePosts.OrderByDescending(v => Convert.ToDateTime(v.Value)).Select(v => v.Key);
                code = 200;
                Message = "Post List Available";
                return Ok(new { code, Message, result });
            }
            else
            {
                code = 401;
                Message = "Login First";
                return BadRequest(new { code, Message });

            }
        }



    }
}