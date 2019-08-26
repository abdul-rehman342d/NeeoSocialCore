using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLayer.Models
{
    public class User
    {
        public long UserID { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public DateTime dateOfBirth { get; set; }
        public int gender { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public DateTime registrationDate { get; set; }
        public bool isVerified { get; set; }
    }
}
