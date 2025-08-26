using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using v2.Data;
using v2.Entities;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace v2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/users
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_context.users.ToList());
        }

        // POST: api/users
        [HttpPost]
        public async Task<IActionResult> SaveOrUpdate([FromForm] UserDto userDto)
        {
            if (userDto.image == null || userDto.image.Length == 0)
                return BadRequest("Image is required");

            var uploads = Path.Combine("wwwroot", "images");
            if (!Directory.Exists(uploads))
                Directory.CreateDirectory(uploads);

            // extension manage [.jpg & .png]
            var extention = Path.GetExtension(userDto.image.FileName);
            if (!extention.AllowedExtension())
            {
                return BadRequest("Only .png & .jpg file will accepted");
            }

            // image size manage
            if (userDto.image.Length > Helper.AcceptedImageSizeBytes)
            {
                return BadRequest("Image size must not exceed 5 MB");
            }
                
            // file naming
            var fileName = $"dp_{userDto.empId}"+ extention;
            var filePath = Path.Combine(uploads, fileName);

            // update user
            if(userDto.id > 0)
            {
                var usr = _context.users.FirstOrDefault(x => x.id == userDto.id);
                if (usr == null)
                {
                    return NotFound($"User with Id : {userDto.id} not found!");
                }

                // delete old photo
                if (!string.IsNullOrEmpty(usr.image))
                {
                    var oldImagePath = Path.Combine("wwwroot","images", usr.image.Substring(8));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                usr.empId = userDto.empId;
                usr.name = userDto.name;
                usr.image = "/images/" + fileName;

                _context.users.Update(usr);

                // file creation and resize
                using (var image = await Image.LoadAsync(userDto.image.OpenReadStream()))
                {
                    image.Mutate(x => x.Resize(Helper.DpWidth, Helper.DpHeight));                 
                    var encoder = new JpegEncoder { Quality = 75 };
                    await image.SaveAsync(filePath, encoder);
                }

                return Ok(usr);
            }

            // add user
            var user = new User
            {
                id = userDto.id ?? 0,
                empId = userDto.empId,
                name = userDto.name,
                image = "/images/" + fileName
            };

            _context.users.Add(user);
            await _context.SaveChangesAsync();

            // file creation and resize
            using (var image = await Image.LoadAsync(userDto.image.OpenReadStream()))
            {
                image.Mutate(x => x.Resize(Helper.DpWidth, Helper.DpHeight));
                var encoder = new JpegEncoder { Quality = 75 };
                await image.SaveAsync(filePath, encoder);
            }

            return Ok(user);
        }
    }
}
