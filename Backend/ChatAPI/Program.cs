using ChatAPI.DataService;
using ChatAPI.Hubs;
using ChatAPI.Repositories;
using ChatAPI.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("reactApp", builder =>
    {
        builder.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Configuração do MongoDB
var connectionString = builder.Configuration.GetConnectionString("MongoDb");
var client = new MongoClient(connectionString);
var database = client.GetDatabase("NodeJSTest"); 
builder.Services.AddSingleton(database);
builder.Services.AddSingleton<IChatRepository, MongoChatRepository>();
builder.Services.AddScoped<ChatService>();

builder.Services.AddSingleton<OllamaService>();

builder.Services.AddHttpClient<OllamaService>();
builder.Services.AddSingleton<OllamaService>();

builder.Services.AddSingleton<ShareDb>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHub<ChatHub>("/Chat");

app.UseCors("reactApp");

app.Run();
