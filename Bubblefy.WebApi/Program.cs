using System.Text;
using System.Text.Json.Serialization;
using Bubblefy.Model;
using Bubblefy.Service;
using Microsoft.AspNetCore.Mvc;
var builder = WebApplication.CreateBuilder(args);
 
builder.Services.AddBubblefyServices();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
var jwtSettings = builder.Configuration.GetSection("JWTSettings");
var chaveSecreta = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"]);
var signingKey = TokenService.GetSigningKey(builder.Configuration);
 
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "JwtBearer";
    options.DefaultChallengeScheme = "JwtBearer";
}).AddJwtBearer("JwtBearer", options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(chaveSecreta),
        ClockSkew = TimeSpan.Zero
    };
});
 
builder.Services.AddAuthorization();
 
var app = builder.Build();
 
app.UseAuthentication();
app.UseAuthorization();
 
app.MapPost("/login/auth", ([FromBody] Usuario usuario) =>
{
    TokenService authService = new TokenService(builder.Configuration);
    return authService.GenerateToken(usuario);
});