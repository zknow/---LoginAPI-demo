using LoginAPI.DB;
using LoginAPI.Domain;
using LoginAPI.Repository;
using LoginAPI.Service;
using LoginAPI.Controllers;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using LoginAPI.Utility;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        SetupBuilder(builder);

        var app = builder.Build();

        SetupApp(app);

        app.Run();
    }

    private static void SetupBuilder(WebApplicationBuilder builder)
    {
        // Load Config
        var config = new ConfigurationBuilder()
          .SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile("appsettings.json", optional: false, true)
          .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, true)
          .Build();

        // MySQL Setting
        var connectionString = config.GetConnectionString("SqlConnection");
        builder.Services.AddDbContext<DbCtx>(options =>
              options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

        // Redis Setting
        var redisConnectStr = config.GetConnectionString("RedisConnection");
        var redis = ConnectionMultiplexer.Connect(redisConnectStr);
        if (!redis.IsConnected)
        {
            Console.WriteLine($"Redis Conn Faile");
            Environment.Exit(1);
        }
        // 將Redis DI
        builder.Services.AddSingleton(redis);

        // Service、Repo DI
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddControllers();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        // jwt Auth Imp
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(JwtUtility.JwtAuthenticationOption);
    }

    private static void SetupApp(WebApplication app)
    {
        // jwt
        app.UseAuthentication();
        app.UseAuthorization();

        // Initialize the database
        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<DbCtx>();
            context.Database.EnsureCreated();
            context.SetDefaultData();
        }

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
                c.RoutePrefix = "";
            });
        }

        // 方便測試先關掉 TLS
        // app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();
    }
}