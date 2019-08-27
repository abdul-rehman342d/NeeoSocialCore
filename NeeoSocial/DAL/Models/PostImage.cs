using NewSocial.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class PostImage
    {
        public int PostImageID { get; set; }

        [ForeignKey("post")]
        public long PostID { get; set; }
        public string imagePath { get; set; }
        public Post post { get; set; }
    }
}
