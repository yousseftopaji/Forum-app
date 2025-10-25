using FileRepository;
using RepositoryContracts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IPostRepository, PostInFileRepository>();
builder.Services.AddScoped<IUserRepository, UserInFileRepository>();
builder.Services.AddScoped<ICommentRepository, CommentInFileRepository>();

var app = builder.Build();

app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
