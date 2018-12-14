using System;
using Microsoft.AspNetCore.Http;

namespace DTApp.API.DTO
{
    public class PhotosForUploadDto
    {
        public string Url {get; set;}
        public string Descrpition { get; set; }

        public IFormFile File {get; set;}
        
        public DateTime DateAdded { get; set; }

        public string PublicId { get; set; }

        public PhotosForUploadDto()
        {
            DateAdded = DateTime.Now;

        }
    }
}