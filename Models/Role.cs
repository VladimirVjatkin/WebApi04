namespace WebApi04.Models
{
    public enum RoleId
    {
        Admin = 0,
        User = 1
    }
    public class Role
    {
        public string Name { get; set; }
        public RoleId RoleId { get; set; }
        public virtual List<User> Users { get; set; }
    }
}
