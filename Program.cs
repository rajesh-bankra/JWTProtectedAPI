using JWTProtectedAPI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add JWT authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = false,
            ValidIssuer = builder.Configuration["Jwt:Issuer"], // your issuer
            ValidAudience = builder.Configuration["Jwt:Audience"], // your audience
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])) // your secret key
        };
        // Optional: You can set an event handler to catch specific errors (like invalid token).
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                if (context.Exception is SecurityTokenExpiredException)
                {
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";
                    return context.Response.WriteAsync("Token has expired.");
                }
                else if (context.Exception is SecurityTokenInvalidSignatureException)
                {
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";
                    return context.Response.WriteAsync("Invalid token signature.");
                }
                else if (context.Exception is SecurityTokenArgumentException)
                {
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";
                    return context.Response.WriteAsync("Invalid token argument.");
                }


                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsync("Authentication failed.");
            }
        };
    });

// Add Swagger
builder.Services.AddSwaggerGen(options =>
{
    // Add Security definition for JWT Bearer
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


// Add controllers
builder.Services.AddControllers();

var app = builder.Build();
// Add Middleware to the pipeline
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<SanitizeInputMiddleware>();

// Add JwtLoggingMiddleware (make sure it's not interfering with authentication)
//app.UseMiddleware<JwtLoggingMiddleware>(); // Add this line before authentication middleware

// Use authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Use Swagger
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
