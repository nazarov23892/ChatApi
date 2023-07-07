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
using ChatApi.BLL.Services;

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
                        return Results.BadRequest(error: new ErrorResponseDto("empty or invalid request param value"));
                    }
                    var response = userService.CreateUser(createUserRequestDto);
                    if (userService.HasValidationProblems)
                    {
                        return Results.BadRequest(error: new ErrorResponseDto(userService.ValidationProblems.FirstOrDefault()));
                    }
                    return Results.Ok(value: response);
                });

            app.MapPost(
                pattern: "/api/chats",
                handler: ([FromBody] CreateChatRequestDto? createChatRequestDto, IChatService chatService) =>
                {
                    if (createChatRequestDto == null)
                    {
                        return Results.BadRequest(error: new ErrorResponseDto("empty or invalid request param value"));
                    }
                    var response = chatService.Create(createChatRequestDto);
                    if (chatService.HasValidationProblems)
                    {
                        return Results.BadRequest(new ErrorResponseDto(chatService.ValidationProblems.FirstOrDefault()));
                    }
                    return Results.Ok(value: response);
                });

            app.MapGet(
                pattern: "/api/chats",
                handler: ([FromBody] ChatsOfUserRequestDto? chatsOfUserRequestDto, IChatService chatService) =>
                {
                    if (chatsOfUserRequestDto == null)
                    {
                        return Results.BadRequest(error: new ErrorResponseDto("empty or invalid request param value"));
                    }
                    ChatsOfUserResponseDto? response = chatService.GetUserChats(chatsOfUserRequestDto);
                    if (chatService.HasValidationProblems)
                    {
                        var errorResonseDto = new ErrorResponseDto(chatService.ValidationProblems.FirstOrDefault());
                        return errorResonseDto.Error?.Contains("user not found") ?? false
                            ? Results.NotFound(errorResonseDto)
                            : Results.BadRequest(errorResonseDto);
                    }
                    return Results.Ok(value: response);
                });

            app.MapPost(
                pattern: "/api/messages/chat",
                handler: ([FromBody] ChatMessagesRequestDto? chatMessagesRequestDto, IChatService chatService) =>
                {
                    if (chatMessagesRequestDto == null)
                    {
                        return Results.BadRequest(error: new ErrorResponseDto("empty or invalid request param value"));
                    }
                    IEnumerable<ChatMessageItemDto>? chatMessagesResponse = chatService.GetChatMessages(chatMessagesRequestDto);
                    if (chatService.HasValidationProblems)
                    {
                        var errorResonseDto = new ErrorResponseDto(chatService.ValidationProblems.FirstOrDefault());
                        return errorResonseDto.Error?.Contains("chat not found") ?? false
                            ? Results.NotFound(errorResonseDto) 
                            : Results.BadRequest(errorResonseDto);
                    }
                    return Results.Ok(value: chatMessagesResponse);
                });

            app.MapPost(
                pattern: "/api/messages/add",
                handler: ([FromBody] PostMessageRequestDto? postMessageRequestDto, IChatService chatService) =>
                {
                    if (postMessageRequestDto == null)
                    {
                        return Results.BadRequest(error: new ErrorResponseDto("empty or invalid request param value"));
                    }
                    PostMessageResponse? response = chatService.PostMessage(postMessageRequestDto);
                    if (chatService.HasValidationProblems)
                    {
                        var errorResonseDto = new ErrorResponseDto(chatService.ValidationProblems.FirstOrDefault());
                        return Results.BadRequest(errorResonseDto);
                    }
                    return Results.Ok(value: response);
                });

            app.Run();
        }
    }
}