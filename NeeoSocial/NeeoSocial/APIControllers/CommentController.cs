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
    [Route("CommentApi")]
    [ApiController]
    public class CommentController : ControllerBase
    {

        DbCalls db = new DbCalls();
        [HttpPost]
        [Route("addComment")]
        
        public IActionResult addComment(Comment currentComment)
        {
            string Message;
            int code;
            long UserID = Convert.ToInt64(Request.GetHeader("UserID"));
            var isUserExist = db.User.Where(u => u.UserID == UserID).FirstOrDefault();

            if (isUserExist != null)
            {
                if (currentComment.commentText.Length <= 200)
                {
                    var isPostExist = db.Post.Where(u => u.PostID == currentComment.PostID).FirstOrDefault();
                    if (isPostExist != null)
                    {
                        currentComment.PostID = Convert.ToInt64(currentComment.PostID);
                        currentComment.UserID = UserID;
                        currentComment.commentText = currentComment.commentText;
                        currentComment.commentTime = DateTime.Now;
                        db.Comment.Add(currentComment);
                        db.SaveChanges();
                        code = 200;
                        Message = "comment Successfully added";
                        return Ok(new { code, Message });
                    }
                    else
                    {
                        code = 400;
                        Message = "Post Not exist";
                        return BadRequest(new { code, Message });
                    }

                  
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

    }
}