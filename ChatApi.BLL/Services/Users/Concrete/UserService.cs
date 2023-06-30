﻿using ChatApi.BLL.Entities;
using ChatApi.BLL.Repositories;
using ChatApi.BLL.Services.Users.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApi.BLL.Services.Users.Concrete
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public CreateUserResponseDto? CreateUser(CreateUserRequestDto createUserDto)
        {
            if (string.IsNullOrEmpty(createUserDto.UserName))
            {
                return null;
            }
            User user = new User
            {
                UserId = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                UserName = createUserDto.UserName
            };
            _userRepository.Add(user);
            return new CreateUserResponseDto { UserId = user.UserId };
        }
    }
}
