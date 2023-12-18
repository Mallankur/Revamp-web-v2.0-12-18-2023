using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Revamp_Ank_App.Ankur.Revamp.Infrastructure.Repositories;
using Revamp_Ank_App.DomainEntites.Repositores.Entites;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "RevampApp", Version = "v1@ankMall" });
    // Configure XML comments for your controllers and models here
    // c.IncludeXmlComments("your-xml-comments-file.xml");
});
builder.Services.Configure<SQLConnectorOptions>(builder.Configuration.GetSection("SQLConnector"));
builder.Services.Configure<MongoScoket>(builder.Configuration.GetSection("MongoObject"));

builder.Services.AddMvc().AddXmlSerializerFormatters();

builder.Services.AddScoped<ISQLconnecterAnkur, RevampRepositoryAnkurServises>();
builder.Services.AddScoped<ISQLconnecterAnkurMall228, SQLConnetter>();
builder.Services.AddScoped<SQLConnetter>();


builder.Services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
}));


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

app.Run();
