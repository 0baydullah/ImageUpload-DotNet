using ImageUpload.Data;
using ImageUpload.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;



namespace ImageUpload.Controllers
{
    [Route("api/")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ImageController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/Image/base64
        [HttpPost("base64")]
        public async Task<IActionResult> UploadBase64([FromBody] UserBase64 user)
        {
            if (string.IsNullOrEmpty(user.ImageBase64))
                return BadRequest("Image is required.");

            _context.UserBase64.Add(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }

        // GET: api/Image/base64
        [HttpGet("base64")]
        public async Task<IActionResult> GetBase64Users()
        {
            var usersSet = _context.UserBase64;
            if (usersSet == null)
                return NotFound("UsersBase DbSet not found.");

            var users = await Task.Run(() => usersSet.ToList());
            return Ok(users);
        }

        // POST: api/Image/file
        [HttpPost("file")]
        public async Task<IActionResult> UploadImageFile([FromForm] string name, [FromForm] string email, [FromForm] IFormFile image)
        {
            if (image == null || image.Length == 0)
                return BadRequest("Image file is required.");

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            var user = new UserImageFile
            {
                Name = name,
                Email = email,
                ImagePath = $"/images/{fileName}"
            };

            _context.UserImageFile.Add(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }

        // GET: api/Image/file
        [HttpGet("file")]
        public async Task<IActionResult> GetImageFileUsers()
        {
            var usersSet = _context.UserImageFile;
            if (usersSet == null)
                return NotFound("UsersImage DbSet not found.");

            var users = await Task.Run(() => usersSet.ToList());
            return Ok(users);
        }
    }
}
