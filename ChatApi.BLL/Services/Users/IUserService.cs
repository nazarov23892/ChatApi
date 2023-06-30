using ChatApi.BLL.Services.Users.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApi.BLL.Services.Users
{
    public interface IUserService
    {
        CreateUserResponseDto? CreateUser(CreateUserRequestDto createUserDto);
    }
}
