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
        public IActionResult addComment([FromBody]int postid ,string commenttext)
        {
            string Message;
            HttpContext.Request.Headers.TryGetValue("Authorization", out var UserID);
            var userid = Request.GetHeader("UserID");


            //if (Session["ApplicationUser"] != null)
            //{
            if (userid != null)
                {
                if (commenttext.Length <= 200)
                {
                    //var User = (Models.User)Session["ApplicationUser"];
                    Comment currentComment = new Comment();
                    currentComment.PostID = Convert.ToInt64(postid);
                    currentComment.UserID = 1; /*User.UserID;*/
                    currentComment.commentText = commenttext;
                    currentComment.commentTime = DateTime.Now;
                    db.Comment.Add(currentComment);
                    db.SaveChanges();
                   
                    Message = "comment Successfully added";
                return Ok(Message);
                //var result = new OkObjectResult(new { message = "200 OK", currentDate = DateTime.Now, Message });
                //return result;

            }
                else
                {
                   
                    Message = "Unauthorized changing";
                return BadRequest(Message);
                //var result = new BadRequestObjectResult(new { message = "400 Bad Request", currentDate = DateTime.Now, Message });
                //return result;
            }
            }
            else
            {
               
                Message = "login First";
                return BadRequest(Message);
            }
        }

        [Route("AddSubComment")]
        [HttpPost]
        public IActionResult addSubComment(int CommentID, string commentText)
        {
            string Message;
            int code;
            HttpContext.Request.Headers.TryGetValue("Authorization", out var UserID);
            var userid = Request.GetHeader("UserID");
            //if (Session["ApplicationUser"] != null)
            //{
            if (userid != null)
                {
                if (commentText.Length <= 200)
                {
                    //var User = (Models.User)Session["ApplicationUser"];
                    SubComment currentComment = new SubComment();
                    currentComment.CommentID = CommentID;
                    currentComment.UserID = 1; //User.UserID;
                    currentComment.commentText = commentText;
                    currentComment.commentTime = DateTime.Now;
                    db.SubComment.Add(currentComment);
                    db.SaveChanges();
                    code = 200;
                    Message = "Subcomment Successfully added";
                return Ok(Message);
                }
                else
                {
                    code = 400;
                    Message = "Unauthorized changing";
                return BadRequest(Message);
                }
            }
            else
            {
                
                Message = "login First";
                return BadRequest(Message);
            }
        }


    }
}