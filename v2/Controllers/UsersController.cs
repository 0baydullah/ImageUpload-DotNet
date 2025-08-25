using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using v2.Data;
using v2.Entities;

namespace v2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public UsersController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: api/users
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_context.users.ToList());
        }

        // POST: api/users
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] UserDto userDto)
        {
            if (userDto.image == null || userDto.image.Length == 0)
                return BadRequest("Image is required");

            var uploads = Path.Combine("wwwroot", "images");
            if (!Directory.Exists(uploads))
                Directory.CreateDirectory(uploads);

            var fileName = $"dp_{userDto.empId}";
            var filePath = Path.Combine(uploads, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await userDto.image.CopyToAsync(stream);
            }

            var user = new User
            {
                empId = userDto.empId,
                name = userDto.name,
                image = "/images/" + fileName
            };

            _context.users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }
    }
}
