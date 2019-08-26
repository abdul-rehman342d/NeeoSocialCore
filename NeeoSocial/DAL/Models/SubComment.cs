using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NewSocial.Models
{
    public class SubComment
    {
        [Key]
        public int SubCommentID { get; set; }

        [ForeignKey("comment")]
        public int CommentID { get; set; }
        public long UserID { get; set; }
        public string commentText { get; set; }
        public DateTime commentTime { get; set; }
        public Comment comment { get; set; }
        public ICollection<SubReaction> SubReactions { get; set; }
    }
}