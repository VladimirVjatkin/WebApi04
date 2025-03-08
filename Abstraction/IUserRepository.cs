using WebApi04.Dto;
using WebApi04.Models;

namespace WebApi04.Abstraction
{
    public interface IUserRepository
    {
        int AddUser(UserDto user);
        RoleId CheckUser(LoginDto login);
        UserDto Authenticate(LoginDto loginDTO);

    }
}
