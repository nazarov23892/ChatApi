using ChatApi.BLL.Repositories;
using ChatApi.BLL.Services.Users.Concrete;
using ChatApi.BLL.Services.Users;
using ChatApi.DAL.Repositories.Concrete;
using ChatApi.BLL.Services.Users.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ChatApi.WEB
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddSingleton<IUserRepository, UserRepository>();
            builder.Services.AddTransient<IUserService, UserService>();

            var app = builder.Build();

            app.MapGet("/", () => "Hello World!");

            app.MapPost(
                pattern: "/api/users",
                handler: ([FromBody] CreateUserRequestDto? createUserRequestDto, IUserService userService) =>
                {
                    if (createUserRequestDto == null)
                    {
                        return Results.ValidationProblem(
                            errors: new Dictionary<string, string[]>
                            {
                                ["empty or ivalid request param value"] = Array.Empty<string>()
                            });
                    }
                    var response = userService.CreateUser(createUserRequestDto);
                    if (userService.HasValidationProblems)
                    {
                        return Results.ValidationProblem(userService.ValidationProblems);
                    }
                    return Results.Ok(value: response);
                });

            app.Run();
        }
    }
}