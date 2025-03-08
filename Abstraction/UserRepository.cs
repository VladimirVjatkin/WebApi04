using WebApi04.DB;
using WebApi04.Dto;
using WebApi04.Models;
using System.Text;
using System.Security.Cryptography;

namespace WebApi04.Abstraction
{
    public class UserRepository : IUserRepository
    {
        public int AddUser(UserDto user)
        {
            using (var context = new UserContext())
            {
                if (context.Users.Any(x => x.Name == user.Name))
                    throw new Exception("User is alredy exist!");


                if (user.Role == UserRoleDto.Admin)
                    if (context.Users.Any(u => u.RoleId == RoleId.Admin))
                        throw new Exception("Admin is alredy exists!");
      
                var entity = new User {Name = user.Name, RoleId = (RoleId)user.Role};
                
                entity.Salt = new byte[16];
                new Random().NextBytes(entity.Salt);
                var data = Encoding.UTF8.GetBytes(user.Password).Concat(entity.Salt).ToArray();
                
                entity.Password = SHA512.HashData(data); //было new SHA512Managed().ComputeHash(data)
                //entity.RoleId = (RoleId)Enum.Parse(typeof(RoleId), user.Role.ToString());
                entity.RoleId = (RoleId)user.Role;

                context.Users.Add(entity);
                context.SaveChanges();

                return entity.Id;
            }
        }

        public UserDto Authenticate(LoginDto loginDTO)
        {
            if (loginDTO.Name == "admin" && loginDTO.Password == "admin")
            {
                return new UserDto { Name = loginDTO.Name, Password = loginDTO.Password, Role = UserRoleDto.Admin };
            }
            if (loginDTO.Name == "user" && loginDTO.Password == "user")
            {
                return new UserDto { Name = loginDTO.Name, Password = loginDTO.Password, Role = UserRoleDto.User };
            }
            return null;
        }

        public RoleId CheckUser(LoginDto login)
        {
            using (var context = new UserContext())
            {
               var user =  context.Users.FirstOrDefault(x => x.Name == login.Name);

               if (user == null) throw new Exception("No user like this!");
                
               var data = Encoding.UTF8.GetBytes(login.Password).Concat(user.Salt).ToArray();
               var hash = SHA512.HashData(data); //было new SHA512Managed().ComputeHash(data)

                if (user.Password == hash)
                    return user.RoleId;
                else throw new Exception("Wrong password!");

               

            };
        }
    }
}
