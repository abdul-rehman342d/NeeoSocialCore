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
    [Route("ReactionApi")]
    [ApiController]
    public class ReactionController : ControllerBase
    {
        DbCalls db = new DbCalls();
        [HttpPost]
        [Route("addReaction")]
        public IActionResult addReaction(Reaction currentReaction)
        {
            string Message;
            int code;
            long UserID = Convert.ToInt64(Request.GetHeader("UserID"));
            var isUserExist = db.User.Where(u => u.UserID == UserID).FirstOrDefault();
            if (isUserExist != null && (currentReaction.reactionType == 1 || currentReaction.reactionType == 0))
            {
                var isReactionExist = db.Reaction.Where(c => c.UserID == UserID && c.PostID == currentReaction.PostID).FirstOrDefault();
                if (isReactionExist == null)
                {

                    currentReaction.reactionTime = DateTime.Now;
                    currentReaction.UserID = UserID;
                    db.Reaction.Add(currentReaction);
                    db.SaveChanges();
                }
                else if (isReactionExist != null && currentReaction.PostID == isReactionExist.PostID && currentReaction.UserID == isReactionExist.UserID && currentReaction.reactionType == isReactionExist.reactionType)
                {
                    db.Reaction.RemoveRange(db.Reaction.Where(c => c.UserID == UserID && c.PostID == currentReaction.PostID));
                    db.SaveChanges();

                }
                else
                {
                    db.Reaction.RemoveRange(db.Reaction.Where(c => c.UserID ==UserID && c.PostID == currentReaction.PostID));
                    db.SaveChanges();
                    currentReaction.reactionTime = DateTime.Now;
                    currentReaction.UserID = UserID;
                    db.Reaction.Add(currentReaction);
                    db.SaveChanges();
                }
                code = 200;
                Message = "Reaction Successfully added";
                return Ok(new { code, Message });
            }
            else
            {
                code = 400;
                Message = "login First";
                return BadRequest(new { code, Message });
            }
        }
        [HttpPost]
        [Route("addSubReaction")]
        public IActionResult addSubReaction(SubReaction currentReaction)
        {
            string Message;
            int code;
            long UserID = Convert.ToInt64(Request.GetHeader("UserID"));
            var isUserExist = db.User.Where(u => u.UserID == UserID).FirstOrDefault();

            if (isUserExist != null && (currentReaction.reactionType == 1 || currentReaction.reactionType == 0))
            {
                currentReaction.UserID = UserID;
                var isReactionExist = db.SubReaction.Where(c => c.UserID == UserID && c.CommentID == currentReaction.CommentID).FirstOrDefault();
                if (isReactionExist == null)
                {
                    currentReaction.reactionTime = DateTime.Now;
                    
                    db.SubReaction.Add(currentReaction);
                    db.SaveChanges();
                }
                else if (isReactionExist != null && currentReaction.CommentID == isReactionExist.CommentID && UserID == isReactionExist.UserID && currentReaction.reactionType == isReactionExist.reactionType)
                {
                    db.SubReaction.RemoveRange(db.SubReaction.Where(c => c.UserID == UserID && c.CommentID == currentReaction.CommentID));
                    db.SaveChanges();

                }
                else
                {
                    db.SubReaction.RemoveRange(db.SubReaction.Where(c => c.UserID == UserID && c.CommentID == currentReaction.CommentID));
                    db.SaveChanges();
                    currentReaction.reactionTime = DateTime.Now;
                    currentReaction.UserID =UserID;
                    db.SubReaction.Add(currentReaction);
                    db.SaveChanges();
                }
                code = 200;
                Message = "Sub Reaction Successfully added";
                return Ok(new { code, Message });
            }
            else
            {
                code = 400;
                Message = "login First";
                return BadRequest(new { code, Message });
            }
        }
    }
}