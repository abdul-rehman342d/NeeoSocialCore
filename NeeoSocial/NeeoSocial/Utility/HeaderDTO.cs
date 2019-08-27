using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace NeeoSocial.Utility
{
   
    public static class HeaderDTO
    {
        
            public static string GetHeader(this HttpRequest request, string key)
            {
                return request.Headers.FirstOrDefault(x => x.Key == key).Value.FirstOrDefault();
            }

        public static string GetFilename(this IFormFile file)
        {
            return ContentDispositionHeaderValue.Parse(
                            file.ContentDisposition).FileName.ToString().Trim('"');
        }

        public class FileInputModel
        {
            public List<IFormFile> FileToUpload { get; set; }
        }


    }
}
