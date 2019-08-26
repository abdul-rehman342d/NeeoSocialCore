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
  
    [Route("Authentication")]
    [ApiController]
    public class AuthenticationApiController : ControllerBase
    {

        

        User user = new User();
          DbCalls db = new DbCalls();

        // GET: Authentication
        //const string SessionName = "ApplicationUser"; 
        //const string SessionAge = "_Age";

        /// <summary>
        /// User Register 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [Route("UserRegister")]
        [HttpPost]
        // public IActionResult UserRegisteration([FromRoute]string name, string email, string password, string dateOfBirth, int gender, string city, string country)
        public IActionResult UserRegisteration([FromBody] User user)
        {
            
            string Message;
            // int code;
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
                   
                    //var split = dateOfBirth.Split('-');
                    //string date = split[0];
                    //string Month = split[1];
                    //string years = split[2];
                    //user.dateOfBirth = years + "-" + Month + "-" + date;
                    currentUser.dateOfBirth = DateTime.Now; /*Convert.ToDateTime(dateOfBirth)*/;
                    currentUser.city = user.city;
                    currentUser.country = user.country;
                    currentUser.isVerified = false;
                 

                    db.User.Add(currentUser);
                    db.SaveChanges();

                    var key = "my-key";

                    var str = JsonConvert.SerializeObject(currentUser);
                    HttpContext.Session.SetString(key, str);


                    //var gh = HttpContext.Session.GetString(key);
                    //var obj = JsonConvert.DeserializeObject(str);
                    Message = "User registered";
                //  HttpContext.Session.SetString(user.name, "ApplicationUser");
                //var  gh = HttpContext.Session.GetString("ApplicationUser");

                   // HttpContext.Session[""];
                   //  Session.Timeout = 525600;
                    var result = new OkObjectResult(new { message = "200 OK", currentDate = DateTime.Now, Message });
                    return result;
                  
                   
                }
                else
                {
                    Message = "Email Already Exist";
                    var result = new BadRequestObjectResult(new { message = "400 Bad Request", currentDate = DateTime.Now , Message });
                    return result;
                    //return BadRequest(Message);
                    
                }
            }
            else
            {
                Message = "Unauthorized Changes";
                var result = new NotFoundObjectResult(new { message = "404 Not Found", currentDate = DateTime.Now , Message});
                return result;

               // return BadRequest(Message);
              
            }
        }
        /// <summary>
        /// User Login the account
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
      [Route("Login")]
        [HttpPost]
        public IActionResult Login([FromBody] User users)

        {
            string Message;
            //  int code;
            if (db.User.Any())
            {
                 user = db.User.Where(u => u.email == users.email && u.password == users.password).FirstOrDefault();
                if (user != null)
                {
                  
                    Message = "successfully logged in";

                    //Session["ApplicationUser"] = user;

              
                    HttpContext.Session.GetString(user.name);

                    var key = "my-key";
                    var str = JsonConvert.SerializeObject(user);
                    HttpContext.Session.SetString(key, str);

                    //Session.Timeout = 525600;
                    return Ok(Message);
                }
                else
                {
                    // code = 400;
                    Message = "Invalid username or password!";
                    return BadRequest(Message);
                    //return Json(new { code, Message }, JsonRequestBehavior.AllowGet);
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