using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DTApp.API.Data;
using DTApp.API.DTO;
using DTApp.API.Helper;
using DTApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DTApp.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/users/{userId}/[controller]")]
    public class PhotosController : ControllerBase
    {
        private readonly IDatingRepository _repository;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;

        private Cloudinary _cloudinary ;

        public PhotosController(IDatingRepository repository, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            this._cloudinaryConfig = cloudinaryConfig;

            this._repository = repository;
            this._mapper = mapper;

            Account acc = new Account(_cloudinaryConfig.Value.CloudName, _cloudinaryConfig.Value.ApiKey, _cloudinaryConfig.Value.ApiSecret);
            _cloudinary = new Cloudinary(acc);
        }

        [HttpGet("{id}", Name="GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            // if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            // {
            //     return Unauthorized();
            // }
            var photoFromRepo = await _repository.GetPhoto(id);
            var photo = _mapper.Map<PhotoForReturnDto>(photoFromRepo);
            return Ok(photo);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int userId, int id)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var user = await _repository.GetUser(userId);

            if(!user.Photos.Any(p => p.Id == id))
            {
                return Unauthorized();
            }
            var photoFromRepo = await _repository.GetPhoto(id);

            if(photoFromRepo.IsMain)
            {
                return BadRequest("Can not delete Main photo");
            }

             if(!string.IsNullOrEmpty(photoFromRepo.PublicId))
             {
                var result = _cloudinary.Destroy( new DeletionParams(photoFromRepo.PublicId));

                if(result.Result == "ok")
                {
                    _repository.Delete(photoFromRepo);
                }
             }
             else
             {
                 _repository.Delete(photoFromRepo);
             }

            

            
            if(await _repository.SaveAll())
            {
                return Ok(); 
            }
            return BadRequest("Can't Delete the Photo");
        }

        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int userId, int id)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var user = await _repository.GetUser(userId);

            if(!user.Photos.Any(p => p.Id == id))
            {
                return Unauthorized();
            }

            var photoFromRepo = await _repository.GetPhoto(id);
            if(photoFromRepo.IsMain)
            {
                return BadRequest("Current photo is main photo");
            }

            var mainPhotoBase = user.Photos.FirstOrDefault(p => p.IsMain);
            var mainPhotoFromRepo = await _repository.GetPhoto(mainPhotoBase.Id);

            mainPhotoFromRepo.IsMain = false;
            photoFromRepo.IsMain = true;

            if(await _repository.SaveAll())
            {
                return NoContent();
            }
            return BadRequest("Can not set photo as main photo");
            
        }


        [HttpPost]
        public async Task<IActionResult> AddPhotosForUser(int userId, [FromForm]PhotosForUploadDto photosDto)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }
            var user = await _repository.GetUser(userId);
            var file = photosDto.File;
            var uploadResult = new ImageUploadResult();
            if(file.Length > 0)
            {
                using(var stream = file.OpenReadStream())
                {
                    ImageUploadParams uploadParam = new ImageUploadParams 
                    {
                      File = new FileDescription(file.Name, stream),
                      Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    };
                    uploadResult = _cloudinary.Upload(uploadParam);                    
                }
            }
            photosDto.Url = uploadResult.Uri.ToString();
            photosDto.PublicId = uploadResult.PublicId;

            var photo = _mapper.Map<Photo>(photosDto);

            if(!user.Photos.Any(p => p.IsMain))
                photo.IsMain = true;

            user.Photos.Add(photo);


            if(await _repository.SaveAll())
            {
                var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo);
                return CreatedAtRoute("GetPhoto", new { id = photo.Id}, photoToReturn );                
            }
            return BadRequest("Couldn't add Photo");
        }
        
    }
}