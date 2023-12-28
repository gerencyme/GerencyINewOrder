using ApiAuthentication.Token;
using AutoMapper;
using Domain.Interfaces.IGeneric;
using Domain.Utils;
using GerencyINewOrderApi.Config;
using GerencyIProductApi.Config;
using GerencylApi.TokenJWT;
using Infrastructure.Configuration;
using Infrastructure.Repository.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddControllersWithViews();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "GerencyI New Order", Version = "v1" });

    // Configuração para autenticação com Bearer Token
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        BearerFormat = "JWT",
        Description = "Insira o token JWT.",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    };

    c.AddSecurityDefinition("Bearer", securityScheme);

    var securityRequirement = new OpenApiSecurityRequirement
        {
            { securityScheme, new[] { "Bearer" } }
        };

    c.AddSecurityRequirement(securityRequirement);
});

//JWT
builder.Configuration.AddJsonFile("appsettings.json");
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
      .AddJwtBearer(option =>
      {
          var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
          option.TokenValidationParameters = new TokenValidationParameters
          {
              ValidateIssuer = false,
              ValidateAudience = false,
              ValidateLifetime = true,
              ValidateIssuerSigningKey = true,

              ValidIssuer = jwtSettings.Issuer,
              ValidAudience = jwtSettings.Audience,
              IssuerSigningKey = JwtSecurityKey.Create(jwtSettings.SecurityKey)
          };

          option.Events = new JwtBearerEvents
          {
              OnAuthenticationFailed = context =>
              {
                  Console.WriteLine("OnAuthenticationFailed: " + context.Exception.Message);
                  return Task.CompletedTask;
              },
              OnTokenValidated = async context =>
              {
                  var isRefreshTokenClaim = context.Principal.Claims.FirstOrDefault(c => c.Type == "refresh_token")?.Value;
                  if (!string.IsNullOrEmpty(isRefreshTokenClaim) && isRefreshTokenClaim.ToLower() == "true")
                  {
                      // Se é um Refresh Token, use a lógica da classe TokenJWTBuilder para gerar um novo Access Token
                      var jwtBuilder = new TokenJWTBuilder()
                          .AddSecurityKey(JwtSecurityKey.Create(jwtSettings.SecurityKey))
                          .AddIssuer(jwtSettings.Issuer)
                          .AddAudience(jwtSettings.Audience)
                          .AddSubject("novousuario@exemplo.com") // Substitua pelo sub do usuário real
                          .WithExpiration(jwtSettings.TokenExpirationMinutes)
                          .Builder();

                      // Acessa diretamente a propriedade Response para substituir o token na resposta
                      context.HttpContext.Response.Headers.Add("new-access-token", jwtBuilder.Value);
                  }

                  Console.WriteLine("OnTokenValidated: " + context.SecurityToken);
                  await Task.CompletedTask;
              },
          };

      });

var serviceConfig = new DIServices();
serviceConfig.MapDependencies(builder.Services);

var repositoryConfig = new DIRepository();
repositoryConfig.RegisterDependencies(builder.Services);

// Adicione a leitura das configurações do appsettings.json
builder.Configuration.AddJsonFile("appsettings.json");

// Configure as configurações do MongoDB
var mongoDbSettings = builder.Configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>();

// Registra as configurações como um serviço no DI (Dependency Injection)
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));

// Registra o IMongoClient e IMongoDatabase
builder.Services.AddSingleton<IMongoClient>(sp => new MongoClient(mongoDbSettings.ConnectionString));
builder.Services.AddScoped<IMongoDatabase>(sp => sp.GetRequiredService<IMongoClient>().GetDatabase(mongoDbSettings.DatabaseName));

// Registra o ContextMongoDb
builder.Services.AddScoped<ContextMongoDb>();

// Registra o RepositoryMongoDBGeneric como serviço
builder.Services.AddSingleton(typeof(IGenericMongoDb<>), typeof(RepositoryMongoDBGeneric<>));
//builder.Services.AddSingleton(typeof(IGeneric<>), typeof(RepositoryGeneric<>));

// Config Auto Mapping
IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "GerencyINewOrder V1");
    });

    //app.MapControllers().AllowAnonymous(); //method for disable authentication
}
else app.MapControllers();

app.MapGet("/", () => "Hello World!");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseSwaggerUI();

app.Run();
