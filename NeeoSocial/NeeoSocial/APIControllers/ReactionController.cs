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
    public class ReactionController : ControllerBase
    {
        DbCalls db = new DbCalls();
        [Route("AddReaction")]
        [HttpPost]
        public IActionResult addReaction(Reaction currentReaction)
        {
            string Message;
            HttpContext.Request.Headers.TryGetValue("Authorization", out var UserID);
            var userid = Request.GetHeader("UserID");
            //int code;
            //if (Session["ApplicationUser"] != null && (currentReaction.reactionType == 1 || currentReaction.reactionType == 0))
            if (userid != null && currentReaction.reactionType == 1 || currentReaction.reactionType == 0)
            {
               // var User = (Models.User)Session["ApplicationUser"];
                db.Reaction.RemoveRange(db.Reaction.Where(c => c.UserID == 1 /*User.UserID*/ && c.PostID == currentReaction.PostID));
                db.SaveChanges();
                currentReaction.reactionTime = DateTime.Now;
                currentReaction.UserID = long.Parse(userid) /*User.UserID*/;
                db.Reaction.Add(currentReaction);
                db.SaveChanges();
                //code = 200;
                Message = "Reaction Successfully added";
                return Ok(Message);
                //return Json(new { code, Message }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //code = 400;
                Message = "login First";
                return Unauthorized(Message);
                //return Json(new { code, Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [Route("AddSubReaction")]
        [HttpPost]
        public IActionResult addSubReaction(SubReaction currentReaction)
        {
            string Message;
            int code;
            //if (Session["ApplicationUser"] != null && (currentReaction.reactionType == 1 || currentReaction.reactionType == 0))
            if (currentReaction.reactionType == 1 || currentReaction.reactionType == 0)
            {
               // var User = (Models.User)Session["ApplicationUser"];
                var isReactionExist = db.SubReaction.Where(c => c.UserID ==  1 /*User.UserID*/ && c.CommentID == currentReaction.CommentID).FirstOrDefault();
                if (isReactionExist == null)
                {
                    currentReaction.reactionTime = DateTime.Now;
                    currentReaction.UserID = 1/*User.UserID*/;
                    db.SubReaction.Add(currentReaction);
                    db.SaveChanges();
                }
                else if (isReactionExist != null && currentReaction.CommentID == isReactionExist.CommentID && 1 /* User.UserID*/ == isReactionExist.UserID && currentReaction.reactionType == isReactionExist.reactionType)
                {
                    db.SubReaction.RemoveRange(db.SubReaction.Where(c => c.UserID == 1/*User.UserID*/ && c.CommentID == currentReaction.CommentID));
                    db.SaveChanges();

                }
                else
                {
                    db.SubReaction.RemoveRange(db.SubReaction.Where(c => c.UserID == 1/* User.UserID*/ && c.CommentID == currentReaction.CommentID));
                    db.SaveChanges();
                    currentReaction.reactionTime = DateTime.Now;
                    currentReaction.UserID = 1 /*User.UserID*/;
                    db.SubReaction.Add(currentReaction);
                    db.SaveChanges();
                }
                code = 200;
                Message = "Sub Reaction Successfully added";
                return Ok(Message);
            }
            else
            {
                code = 400;
                Message = "login First";
                return BadRequest(Message);
            }
        }
    }
}