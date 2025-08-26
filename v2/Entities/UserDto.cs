namespace v2.Entities
{
    public class UserDto
    {
        public int? id { get; set; } = 0;
        public int empId { get; set; }
        public string name { get; set; }
        public IFormFile image { get; set; }
    }
}
