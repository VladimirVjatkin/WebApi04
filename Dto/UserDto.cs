namespace WebApi04.Dto
{
    public class UserDto
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public UserRoleDto Role { get; set; }
    }
}
