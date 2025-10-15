using FileRepository;
using RepositoryContracts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Add services to the container.
// Remove Swagger due to compatibility issues with .NET 10.0 preview

builder.Services.AddScoped<IPostRepository, PostInFileRepository>();
builder.Services.AddScoped<IUserRepository, UserInFileRepository>();
builder.Services.AddScoped<ICommentRepository, CommentInFileRepository>();

var app = builder.Build();

app.MapControllers();

// Configure the HTTP request pipeline.
// Remove Swagger code due to compatibility issues with .NET 10.0 preview

app.UseHttpsRedirection();

app.Run();
