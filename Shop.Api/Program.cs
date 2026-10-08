using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Shop.Api.Interfaces;
using Shop.Api.Middlewares;
using Shop.Api.Services;
using Shop.Application.Commands.Category;
using Shop.Application.Interfaces;
using Shop.Application.Interfaces.Repositories;
using Shop.Application.Interfaces.Repository;
using Shop.Application.Interfaces.Services;
using Shop.Application.Mapping;
using Shop.Application.Services;
using Shop.Application.Validators.Category;
using Shop.Infrastructure.Configuration;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Helpers;
using Shop.Infrastructure.RabbitMQ;
using Shop.Infrastructure.Repositories;
using Shop.Infrastructure.Services;
using StackExchange.Redis;
using System.Text;
namespace Shop.Api;
public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddDbContext<ShopDbContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConnection"));
        });

        builder.Services.AddAutoMapper(
            _ => { },
            typeof(CategoryProfile).Assembly,
            typeof(UserProfile).Assembly
        );
        builder.Services.AddMediatR(cfg =>
        cfg.RegisterServicesFromAssembly(
        typeof(CreateCategoryCommand).Assembly));

        var configuration = builder.Configuration;
        builder.Services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
        // ================= JWT Settings =================
        builder.Services.Configure<RabbitMqSettings>(configuration.GetSection("RabbitMq"));
        var jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>()
      ?? throw new Exception("JWT settings not configured.");
        // ================= CORS =================
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        // ================= Swagger + JWT =================
        builder.Services.AddSwaggerGen(options =>
        {

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {

                Type = SecuritySchemeType.Http,

                Scheme = "bearer",

                BearerFormat = "JWT",

                Name = "Authorization",

                In = ParameterLocation.Header,

                Description = "Enter JWT token"
            });


            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {

                [new OpenApiSecuritySchemeReference("Bearer", document)] = []

            });

        });
        //======================Redis=====================
        builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>

        {

            var config = builder.Configuration.GetConnectionString("RedisServerConnection");

            return ConnectionMultiplexer.Connect(config);

        });
        //builder.Services.AddSwaggerGen();

        // Add services to the container.
        //DI container
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        //--------------SERVICES
        //--------------SERVICES
        builder.Services.AddScoped<Interfaces.IProductService, Services.ProductService>();
        builder.Services.AddScoped<ICategoryService, CategoryService>();
        builder.Services.AddScoped<IImageService, ImageService>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IJWTService, JWTService>();
        builder.Services.AddScoped<IEmailService, EmailService>();
        builder.Services.AddScoped<IAdminService, AdminService>();
        //--------------HELPERS
        builder.Services.AddSingleton<IHashHelper, HashHelper>();
        //--------------REPOSITORIES
        //--------------REPOSITORIES
        builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
        builder.Services.AddScoped<IAuthRepository, AuthRepository>();
        builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();\n        builder.Services.AddScoped<IUserProviderRepository, UserProviderRepository>();
        builder.Services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
        builder.Services.AddScoped<IProductRepository, ProductRepository>();
        //VALIDATORS
        builder.Services.AddFluentValidationAutoValidation();
        builder.Services.AddValidatorsFromAssemblyContaining<CategoryCreateValidator>();

        builder.Services.AddScoped<
    IDeliveryAddressRepository,
    DeliveryAddressRepository>();

        builder.Services.AddScoped<DeliveryAddressService>();


        builder.Services.Configure<MongoDbSettings>(
        configuration.GetSection("MongoDb"));


        builder.Services.AddScoped<RabbitMQProducer>();


        builder.Services.AddMemoryCache();

        //builder.Services.AddScoped<ICachingService, MemoryCachingService>();
        builder.Services.AddScoped<ICachingService, RedisCachingService>();
        // ================= AUTHENTICATION (JWT + COOKIES + GOOGLE) =================
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })

     .AddJwtBearer(options =>

     {

         options.TokenValidationParameters = new TokenValidationParameters
         {

             ValidateIssuer = true,

             ValidateAudience = true,

             ValidateLifetime = true,

             ValidateIssuerSigningKey = true,


             ValidIssuer = jwtSettings.Issuer,

             ValidAudience = jwtSettings.Audience,


             IssuerSigningKey = new SymmetricSecurityKey(

                 Encoding.UTF8.GetBytes(jwtSettings.Key)

             ),


             ClockSkew = TimeSpan.Zero

         };

     })

     .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme).AddGoogle(options =>

     {

            options.ClientId = configuration["Authentication:Google:ClientId"]!;

            options.ClientSecret = configuration["Authentication:Google:ClientSecret"]!;

            options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

        });
        builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMq"));

        builder.Services.AddScoped<RabbitMQProducer>();
        //===============Cors
        builder.Services.AddCors(options =>

        {

            options.AddPolicy("AllowAll", policy =>

            {

                policy.AllowAnyOrigin()

                      .AllowAnyMethod()

                      .AllowAnyHeader();

            });

        });
        builder.Services.AddHostedService<OrderRabbitMqConsumer>();
        builder.Services.AddSingleton<IProductFeedbackService, MongoProductFeedbackService>();
        builder.Services.AddHostedService<RabbitMqReaderService>();
        builder.Services.AddSingleton<IQueueService, RabbitMqService>();
        builder.Services.AddAuthorization();
        var app = builder.Build();
        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider
                .GetRequiredService<ShopDbContext>();

            var hashHelper = scope.ServiceProvider
                .GetRequiredService<IHashHelper>();

            await ProviderSeeder.SeedAsync(context);\n            await AdminSeeder.SeedAsync(context, hashHelper);
        }
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseCors("AllowAll");

        app.UseAuthentication();

        app.UseAuthorization();
        app.UseMiddleware<RequestTimerMiddleware>();
        app.UseStaticFiles();
        app.MapControllers();



        app.Run();
    }
}
