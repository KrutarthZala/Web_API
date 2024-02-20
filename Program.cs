using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Product_CRUD_Web_API.Implementation;
using Product_CRUD_Web_API.Models;
using Product_CRUD_Web_API.Services;
using System.Text;

namespace Product_CRUD_Web_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Create a new web application builder
            var builder = WebApplication.CreateBuilder(args);

            // Retrieve the connection string from the configuration
            var connectionStr = builder.Configuration.GetConnectionString("connectionString");
            
            // Add DbContext to the dependency injection container
            builder.Services.AddDbContextPool<ProductApiContext>(option =>
            option.UseSqlServer(connectionStr)
            );


            // Add services to the dependency injection 
            builder.Services.AddControllers();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(option =>
            {
                option.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!))
                };
            });

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Configure Swagger/OpenAPI documentation
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Build the web application
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                // Enable Swagger UI in development environment
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Redirect HTTP requests to HTTPS
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseAuthentication();
            // Enable authorization middleware
            app.UseAuthorization();

            // Map controllers to endpoints
            app.MapControllers();
            
            // Start the application
            app.Run();
        }
    }
}
