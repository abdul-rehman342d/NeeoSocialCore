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
    public class CommentController : ControllerBase
    {

        DbCalls db = new DbCalls();
        [Route("AddComment")]
        [HttpPost]
        public IActionResult addComment(int PostID, string commentText)
        {
            string Message;
            int code;
            long UserID = Convert.ToInt64(Request.GetHeader("UserID"));
            var isUserExist = db.User.Where(u => u.UserID == UserID).FirstOrDefault();

            if (isUserExist != null)
            {
                if (commentText.Length <= 200)
                {
                  
                    Comment currentComment = new Comment();
                    currentComment.PostID = Convert.ToInt64(PostID);
                    currentComment.UserID =UserID;
                    currentComment.commentText = commentText;
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