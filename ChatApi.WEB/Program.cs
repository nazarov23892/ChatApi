using ChatApi.BLL.Repositories;
using ChatApi.BLL.Services.Users.Concrete;
using ChatApi.BLL.Services.Users;
using ChatApi.DAL.Repositories.Concrete;
using ChatApi.BLL.Services.Users.DTOs;
using Microsoft.AspNetCore.Mvc;
using ChatApi.DAL.DataContexts;
using Microsoft.EntityFrameworkCore;
using ChatApi.BLL.Services.Chats.Concrete;
using ChatApi.BLL.Services.Chats;
using ChatApi.BLL.Services.Chats.DTOs;
using ChatApi.DAL.SeedData;

namespace ChatApi.WEB
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<AppDataContext>(o =>
            {
                o.UseInMemoryDatabase(databaseName: "chat-db");
            });
            builder.Services.AddTransient<IUserRepository, UserRepository>();
            builder.Services.AddTransient<IChatRepository, ChatRepository>();
            builder.Services.AddTransient<IUserService, UserService>();
            builder.Services.AddTransient<IChatService, ChatService>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                using (var scope = app.Services.CreateScope())
                {
                    var efDbContext = scope.ServiceProvider.GetRequiredService<AppDataContext>();
                    SeedDataTool.SeedData(efDbContext);
                }
            }

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

            app.MapPost(
                pattern: "/api/chats",
                handler: ([FromBody] CreateChatRequestDto? createChatRequestDto, IChatService chatService) =>
                {
                    if (createChatRequestDto == null)
                    {
                        return Results.ValidationProblem(
                            errors: new Dictionary<string, string[]>
                            {
                                ["empty or ivalid request param value"] = Array.Empty<string>()
                            });
                    }
                    var response = chatService.Create(createChatRequestDto);
                    if (chatService.HasValidationProblems)
                    {
                        return Results.ValidationProblem(chatService.ValidationProblems);
                    }
                    return Results.Ok(value: response);
                });

            app.MapGet(
                pattern: "/api/chats",
                handler: ([FromBody] ChatsOfUserRequestDto? chatsOfUserRequestDto, IChatService chatService) =>
                {
                    if (chatsOfUserRequestDto == null)
                    {
                        return Results.ValidationProblem(
                            errors: new Dictionary<string, string[]>
                            {
                                ["empty or ivalid request param value"] = Array.Empty<string>()
                            });
                    }
                    ChatsOfUserResponseDto? response = chatService.GetUserChats(chatsOfUserRequestDto);
                    if (chatService.HasValidationProblems)
                    {
                        return Results.ValidationProblem(chatService.ValidationProblems);
                    }
                    return Results.Ok(value: response);
                });

            app.MapPost(
                pattern: "/api/messages/chat",
                handler: ([FromBody] ChatMessagesRequestDto? chatMessagesRequestDto, IChatService chatService) =>
                {
                    if (chatMessagesRequestDto == null)
                    {
                        return Results.ValidationProblem(
                            errors: new Dictionary<string, string[]>
                            {
                                ["empty or ivalid request param value"] = Array.Empty<string>()
                            });
                    }
                    IEnumerable<ChatMessageItemDto>? chatMessagesResponse = chatService.GetChatMessages(chatMessagesRequestDto);
                    if (chatService.HasValidationProblems)
                    {
                        return Results.ValidationProblem(chatService.ValidationProblems);
                    }
                    return Results.Ok(value: chatMessagesResponse);
                });
            app.Run();
        }
    }
}