
using Marketplace.Api.Services;
using Marketplace.Api.Services.Interfaces;
using Marketplace.Application.Interfaces.Services;
using Marketplace.Application.Mappings;
using Marketplace.Application.Services;
using Marketplace.Domain.Interfaces.Repositories;
using Marketplace.Infrastructure.Data;
using Marketplace.Infrastructure.Models;
using Marketplace.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {

            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("logs/marketplace-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                Log.Information("Starting up the application");

                var builder = WebApplication.CreateBuilder(args);

                builder.Host.UseSerilog();
                // Add services to the container.

                builder.Services.AddControllers();
                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Title = "Marketplace API",
                        Version = "v1",
                        Description = "API for Marketplace application",
                        
                    });

                    // Add JWT Authentication support in Swagger
                    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer"
                    });

                    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                                Array.Empty<string>()
                            }
                        });
                });



                // Add services to the container
                builder.Services.AddControllersWithViews();

                // Configure DbContext
                builder.Services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


                builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<AppDbContext>()
                    .AddDefaultTokenProviders();


                builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer= true,
                        ValidateAudience= true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "Mahmodramadanali3343klasdflaf:;fasdfa;sfas;afsdfadsb!"))
                    };
                });


                // Register Repositories
                builder.Services.AddScoped<IProductRepository, ProductRepository>();
                builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
                builder.Services.AddScoped<IOrderRepository, OrderRepository>();
                builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
                builder.Services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();


                builder.Services.AddAutoMapper(typeof(MappingProfile));

                builder.Services.AddScoped<IProductService, ProductService>();
                builder.Services.AddScoped<ICategoryService, CategoryService>();
                builder.Services.AddScoped<IOrderService, OrderService>();
                builder.Services.AddScoped<IReviewService, ReviewService>();
                builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();
                builder.Services.AddScoped<IJwtService, JwtService>();


                builder.Services.AddScoped<IAdminService, AdminService>();
                builder.Services.AddScoped<IStatisticsService, StatisticsService>();
                builder.Services.AddScoped<IFileUploadService, FileUploadService>();
                builder.Services.AddScoped<IEmailService, EmailService>();
                builder.Services.AddScoped<IUserProfileService, UserProfileService>();
                builder.Services.AddScoped<IWishlistService, WishlistService>();

                // Add CORS
                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("AllowAll",
                        policy =>
                        {
                            policy.AllowAnyOrigin()
                                  .AllowAnyMethod()
                                  .AllowAnyHeader();
                        });

                    options.AddPolicy("AllowSpecific",
                        policy =>
                        {
                            policy.WithOrigins("http://localhost:3000", "http://localhost:4200")
                                  .AllowAnyMethod()
                                  .AllowAnyHeader();
                        });
                });

                var app = builder.Build();



                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                    
                }

                app.UseHttpsRedirection();

                app.UseAuthentication();
                app.UseAuthorization();


                app.MapControllers();

                using (var scope = app.Services.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    await dbContext.Database.MigrateAsync();

                    // Seed roles and default users
                    await DbInitializer.InitializeAsync(scope.ServiceProvider);
                }

                app.Run();

            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application failed to start");
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
