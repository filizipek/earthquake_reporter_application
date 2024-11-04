using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using EarthquakeReporter.Configuration;
using EarthquakeReporter.Models;
using EarthquakeReporter.Repositories;
using EarthquakeReporter.Services;
using Hangfire;
using Hangfire.Storage.SQLite;
using KafkaFlow;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using EarthquakeReporter.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Bind Jwt options from configuration
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

// Configure application settings
builder.Services.Configure<AppSettings>(builder.Configuration);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure KafkaFlow
builder.Services.ConfigureKafkaFlow();

// Register Hangfire with SQLite storage
builder.Services.AddHangfire(config =>
{
    config.UseSQLiteStorage("Data Source=hangfire.db");
});
builder.Services.AddHangfireServer();

// Register HttpClient service
builder.Services.AddHttpClient<IHttpClientService<List<EarthquakeEvent>>, HttpClientService<List<EarthquakeEvent>>>();

// Register application services
builder.Services.AddScoped<IEarthquakeDataService, EarthquakeDataService>();
builder.Services.AddScoped<IKafkaProducerService, KafkaProducerService>();
builder.Services.AddScoped<IHangfireJobs, HangfireJobs>();
builder.Services.AddScoped<IDapperContext, DapperContext>();
builder.Services.AddScoped<IEarthquakeEventRepository, EarthquakeEventRepository>();
builder.Services.AddScoped<ILoginRepository, LoginRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IFamilyMemberRepository, FamilyMemberRepository>();

// Add controllers
builder.Services.AddControllers();

// Configure JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var issuer = builder.Configuration["Jwt:Issuer"];
    var audience = builder.Configuration["Jwt:Audience"];
    var key = builder.Configuration["Jwt:Key"];

    if (string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience) || string.IsNullOrEmpty(key))
    {
        throw new InvalidOperationException("JWT configuration values are not properly set.");
    }

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
    };
});

// Add authorization services
builder.Services.AddAuthorization();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost",
        corsBuilder => corsBuilder
            .WithOrigins("http://localhost:3000")
            .WithMethods("GET", "POST", "PUT", "DELETE")
            .WithHeaders("Content-Type", "Authorization")
            .AllowCredentials());
});

var app = builder.Build();

// Start KafkaBus immediately
var kafkaBus = app.Services.CreateKafkaBus();
await kafkaBus.StartAsync();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Enable HTTPS redirection in production
app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();

// Ensure CORS is used before authentication and authorization
app.UseCors("AllowLocalhost");

app.UseAuthentication();
app.UseAuthorization();

// Configure Hangfire dashboard
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    DashboardTitle = "Hangfire Dashboard"
});

// Map controllers
app.MapControllers();

// Schedule Hangfire jobs
app.Lifetime.ApplicationStarted.Register(() =>
{
    using (var scope = app.Services.CreateScope())
    {
        var hangfireJobs = scope.ServiceProvider.GetRequiredService<IHangfireJobs>();
        
        // Schedule a job to fetch and log daily earthquake data
        RecurringJob.AddOrUpdate(
            "fetch-daily-earthquake",
            () => hangfireJobs.FetchAndLogDailyEarthquakeData(DateTime.UtcNow.Date),
            Cron.Hourly);
    }
});

await app.RunAsync();
