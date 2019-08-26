using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DAL.Models;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using MimeKit;

namespace NeeoSocial.Controllers
{
    public class AuthenticationController : Controller
    {
        //DbCalls db = new DbCalls();
        //class JwtOptions
        //{
        //    public string SecretKey { get; set; }
        //    public int ExpiryMinutes { get; set; }
        //    public string Issuer { get; set; }
        //}


        //private readonly IDistributedCache _cache;
        //private readonly IHttpContextAccessor _httpContextAccessor;
        //private readonly IOptions<JwtOptions> _jwtOptions;


        public IActionResult Index()
        {
            return View();
        }

        ///// <summary>
        /////  Generate Token   encrpty  metadata
        ///// </summary>
        ///// <param name="username"></param>
        ///// <returns></returns>
        //private string GenerateToken(string username)
        //{
        //    var claims = new Claim[]
        //    {
        //        new Claim(ClaimTypes.Name, username),
        //        new Claim(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString()),
        //        new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(DateTime.Now.AddDays(1)).ToUnixTimeSeconds().ToString()),
        //    };

        //    var token = new JwtSecurityToken(
        //        new JwtHeader(new SigningCredentials(
        //            new SymmetricSecurityKey(Encoding.UTF8.GetBytes("the secret that needs to be at least 16 characeters long for HmacSha256")),
        //                                     SecurityAlgorithms.HmacSha256)),
        //        new JwtPayload(claims));

        //    return new JwtSecurityTokenHandler().WriteToken(token);
        //}

        ///// <summary>
        ///// GetCurrentToken metadata
        ///// </summary>
        ///// <returns></returns>
        //public string GetCurrentToken()
        //{
        //    var authorizationHeader = _httpContextAccessor
        //        .HttpContext.Request.Headers["authorization"];

        //    return authorizationHeader == StringValues.Empty
        //        ? string.Empty
        //        : authorizationHeader.Single().Split(" ").Last();
        //}

        ///// <summary>
        ///// GetCurrentToken Deactivate or remove  metadata
        ///// </summary>
        ///// <returns></returns>
        //public async Task DeactivateAsync()
        //  => await _cache.SetStringAsync(GetCurrentToken(),
        //      " ", new DistributedCacheEntryOptions
        //      {

        //          AbsoluteExpirationRelativeToNow =
        //              TimeSpan.FromMinutes(_jwtOptions.Value.ExpiryMinutes)
        //      });
        ///// <summary>
        ///// password encrpty  metadata 
        ///// </summary>
        ///// <param name="text"></param>
        ///// <returns></returns>
        //private static string encode(string text)
        //{
        //    byte[] mybyte = System.Text.Encoding.UTF8.GetBytes(text);
        //    string returntext = System.Convert.ToBase64String(mybyte);
        //    return returntext;
        //}
        //private static string decode(string text)
        //{
        //    byte[] mybyte = System.Convert.FromBase64String(text);
        //    string returntext = System.Text.Encoding.UTF8.GetString(mybyte);
        //    return returntext;
        //}

        ///// <summary>
        /////  email is validtaion  encrpty  metadata
        ///// </summary>
        ///// <param name="email"></param>
        ///// <returns></returns>
        //private static bool isEmailValid(string email)
        //{
        //    try
        //    {
        //        var currentEmail = new System.Net.Mail.MailAddress(email);
        //        return currentEmail.Address == email;
        //    }
        //    catch
        //    {
        //        return false;
        //    }

        //}

        ///// <summary>
        /////  Send Email Metadata
        ///// </summary>
        ///// <param name="email"></param>
        //private static void sendEmail(string email)
        //{
        //    var message = new MimeMessage();
        //    message.From.Add(new MailboxAddress("NeeoSocial Verification", "cybill.netsol@gmail.com"));
        //    message.To.Add(new MailboxAddress("NeeoSocial Verification", email));
        //    message.Subject = "Neeo Social Email Verification";
        //    //message.Body = new TextPart("plain")
        //    //{
        //    //    Text= " Hi Sir/Madam We just need to verify your Email address for complete your registration!"
        //    //};


        //    var bodyBuilder = new BodyBuilder();

        //    bodyBuilder.HtmlBody = "<p>Hi Sir/Madam We just need to verify your Email address for complete your registration!</p></br> <a href='http://social.neeoapp.com/Authentication'>My Timeline</a>";

        //    message.Body = bodyBuilder.ToMessageBody();


        //    using (var client = new SmtpClient())
        //    {
        //        client.Connect("smtp.gmail.com", 587, false);
        //        client.Authenticate("cybill.netsol@gmail.com", "328782395");
        //        client.Send(message);
        //        client.Disconnect(true);
        //    }
        //}
        //[HttpPost]
        //public IActionResult login(string email, string password)
        //{
        //    try
        //    {
        //        var user = (from k in db.User.Where(u => u.email == email && u.password == encode("Al-SHAFQAT" + password)).ToList()
        //                    select new
        //                    {
        //                        k.UserID,
        //                        k.name,
        //                        k.isVerified
        //                    }).FirstOrDefault();


        //        if (user != null && email != null && password != null && password.Length > 5 && isEmailValid(email) != false)
        //        {


        //            string token = GenerateToken(user.name);
        //            Token currentTokken = new Token();
        //            currentTokken.UserID = user.UserID;
        //            currentTokken.tokenString = token;
        //            db.Token.Add(currentTokken);
        //            return Json(new { code = HttpStatusCode.OK, message = "logged in", token });
        //        }
        //        else
        //        {
        //            return Json(new { code = HttpStatusCode.BadRequest, message = "Invalid User" });
        //        }
        //    }
        //    catch (ApplicationException)
        //    {
        //        return Json(new { code = HttpStatusCode.BadRequest, message = "Invalid User" });
        //    }
        //    catch (Exception)
        //    {
        //        return Json(new { code = HttpStatusCode.InternalServerError, message = "Invalid User" });
        //    }
        //}
        //[HttpPost]
        //public IActionResult registration(string name, string email, string password, string dateOfBirth, int gender, string city, string country)
        //{
        //    try
        //    {
        //        if (name != "" && email != "" && isEmailValid(email) != false && password != "" && !(password.Length <= 5) && dateOfBirth != "" && (gender != 1 || gender != 0) && city != "" && (country != "" && country.Length <= 3))
        //        {

        //            if (db.User.Where(u => u.email == email).FirstOrDefault() == null)
        //            {
        //                var split = dateOfBirth.Split('-');
        //                string date = split[0];
        //                string Month = split[1];
        //                string years = split[2];
        //                dateOfBirth = years + "-" + Month + "-" + date;

        //                User currentUser = new User();
        //                currentUser.name = name;
        //                currentUser.email = email;
        //                currentUser.password = encode("Al-SHAFQAT" + password);
        //                currentUser.gender = gender;
        //                currentUser.dateOfBirth = Convert.ToDateTime(dateOfBirth);
        //                currentUser.city = city;
        //                currentUser.country = country;
        //                currentUser.registrationDate = DateTime.UtcNow;
        //                currentUser.isVerified = false;
        //                db.User.Add(currentUser);
        //                db.SaveChanges();

        //                string token = GenerateToken(currentUser.name);
        //                Token currentTokken = new Token();
        //                currentTokken.UserID = currentUser.UserID;
        //                currentTokken.tokenString = token;
        //                db.Token.Add(currentTokken);
        //                db.SaveChanges();

        //                sendEmail(email);

        //                return Json(new { code = HttpStatusCode.OK, message = "User registered", token });
        //            }
        //            else
        //            {
        //                return Json(new { code = HttpStatusCode.BadRequest, message = "Email already exist" });
        //            }
        //        }
        //        else
        //        {
        //            return Json(new { code = HttpStatusCode.BadRequest, message = "Unauthorized Changes" });
        //        }
        //    }
        //    catch (ApplicationException)
        //    {
        //        return Json(new { code = HttpStatusCode.InternalServerError, message = "Server Error" });
        //    }
        //    catch (Exception)
        //    {
        //        return Json(new { code = HttpStatusCode.InternalServerError, message = "Server Error" });
        //    }
        //}
        //[HttpPost]
        //public IActionResult logout()
        //{
        //    try
        //    {
        //        DeactivateAsync();
        //        return Json(new { code = HttpStatusCode.OK, message = "Logged out" });
        //    }
        //    catch (ApplicationException)
        //    {
        //        return Json(new { code = HttpStatusCode.BadRequest, message = "Invalid User" });
        //    }
        //    catch (Exception)
        //    {
        //        return Json(new { code = HttpStatusCode.InternalServerError, message = "Invalid User" });
        //    }
        //}
    }
}