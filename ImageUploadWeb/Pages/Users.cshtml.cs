using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace ImageUploadWeb.Pages
{
    public class UsersModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public UsersModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [BindProperty]
        public UserPostDto User { get; set; } = new();

        public List<UserGetDto> Users { get; set; } = new();

        public async Task OnGetAsync()
        {
            var client = _clientFactory.CreateClient("Api");
            var response = await client.GetAsync("api/users");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Users = JsonSerializer.Deserialize<List<UserGetDto>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<UserGetDto>();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var client = _clientFactory.CreateClient("Api");

            using var form = new MultipartFormDataContent();
            form.Add(new StringContent(User.id?.ToString() ?? string.Empty), "id");
            form.Add(new StringContent(User.empId.ToString()), "empId");
            form.Add(new StringContent(User.name), "name");

            if (User.image != null)
            {
                var streamContent = new StreamContent(User.image.OpenReadStream());
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(User.image.ContentType);
                form.Add(streamContent, "image", User.image.FileName);
            }

            var response = await client.PostAsync("api/users", form);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage(); // Refresh page after success
            }

            ModelState.AddModelError(string.Empty, "Something went wrong.");
            await OnGetAsync(); // reload users
            return Page();
        }
    }

    // For POST (form upload)
    public class UserPostDto
    {
        public int? id { get; set; }
        public int empId { get; set; }
        public string name { get; set; }
        public IFormFile? image { get; set; }
    }

    // For GET (deserialization only)
    public class UserGetDto
    {
        public int id { get; set; }
        public int empId { get; set; }
        public string name { get; set; }
        public string image { get; set; }  // image URL like "/images/dp_123.jpg"
    }
}
