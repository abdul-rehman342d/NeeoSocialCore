using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NeeoSocial.Utility;
using NewSocial.Models;
using Newtonsoft.Json;

namespace NeeoSocial.APIControllers
{
  
    [Route("AuthenticationApi")]
    [ApiController]
    public class AuthenticationApiController : ControllerBase
    {
        DbCalls db = new DbCalls();
        /// <summary>
        /// User Register 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [Route("UserRegister")]
        [HttpPost]
        public IActionResult UserRegisteration([FromBody] User user)
        {
            
            string Message;
            if (user.name != "" && user.email != "" && user.password != "" && !(user.password.Length <= 5) && user.dateOfBirth != null /*&& (gender != 1 || gender != 0)*/ && user.city != "" && (user.country != "" && user.country.Length >= 3))
            {
                var emailExist = db.User.Where(u => u.email == user.email).FirstOrDefault();
                if (emailExist == null)
                {
                   User currentUser = new User();
                    currentUser.name = user.name;
                    currentUser.email = user.email;
                    currentUser.password = user.password;
                    currentUser.gender = user.gender;
                    currentUser.dateOfBirth = user.dateOfBirth;
                    currentUser.city = user.city;
                    currentUser.country = user.country;
                    currentUser.isVerified = false;
                    db.User.Add(currentUser);
                    db.SaveChanges();
                    Message = "User registered";
                    return Ok(new { code = 200, message = Message,userID= currentUser.UserID});
                }
                else
                {
                    Message = "Email Already Exist";
                    return Ok(new { code = 400, Message = Message });
                }
            }
            else
            {
                Message = "Unauthorized Changes";
               
                return BadRequest(new { code = 400, Message = Message });
             
            }
        }
        /// <summary>
        /// User Login the account
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        [Route("Login")]
        [HttpPost]
        public IActionResult Login(User user)
        {
            string Message;
            if (db.User.Any())
            {
                 var User = db.User.Where(u => u.email == user.email && u.password == user.password).FirstOrDefault();
                if (User != null)
                {
                    Message = "successfully logged in";
                    return Ok(new { code = "200", Message = Message, userID = User.UserID });
                }
                else
                {
                    Message = "Incorrect Email or Password";
                    return Ok(new { code = "400", message = Message});
                }
            }
            else
            {
                // code = 400;
                Message = "No User Exist";
                return BadRequest(Message);

            }
        }

        //Logout
        [Route("Logout")]
        [HttpPost]
        public IActionResult logout()
        {

            

              HttpContext.Request.Headers.TryGetValue("Authorization",  out var UserID);
             var userid = Request.GetHeader("UserID");

            string Message;
         
            if (userid != null)
            {
               

            
                Message = "Successfully logged out";
              
                return Ok(Message);
            }
            else
            {
               
                Message = "Login First";
                return BadRequest(Message);
                
            }


       
    }


      
    }
}