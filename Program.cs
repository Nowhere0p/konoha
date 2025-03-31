using System.Net;
using System.Text;
using Konoha.DbCore;
using Konoha.Middleware;
// using Konoha.Middleware;
using Konoha.Models;
using Konoha.Services;
using Konoha.Services.EmailHelper;
using Konoha.Services.OtpVerificationService;
using Konoha.Services.Products;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Dependency Injection
builder.Services.AddScoped<IAuthService, AuthService>();

// Email Service
var emailConfigs = builder.Configuration.GetSection("EmailSettings");
builder.Services.AddTransient<ISmtpClient>(provider =>
{
    var smtpClient = new SmtpClient();
    smtpClient.Connect(
        emailConfigs.GetSection("Host").Value,
        int.Parse(emailConfigs.GetSection("Port").Value),
        MailKit.Security.SecureSocketOptions.StartTls // Use appropriate security options
    );
    smtpClient.Authenticate(
        emailConfigs.GetSection("Username").Value,
        emailConfigs.GetSection("Password").Value
    );

    return smtpClient;
});

// JWT Authentication
builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "")
            ),
        };
    });

builder.Services.AddAuthorization();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        }
    );
});

// Add controllers before other middleware configurations
builder.Services.AddControllers();

var userDbService = await InitializeMongoClientAsync<UserDetails>(
    builder.Configuration.GetSection("UserDetailsDb")
);
builder.Services.AddSingleton<IMongoDbService<UserDetails>>(userDbService);

var otpDbService = await InitializeMongoClientAsync<OtpVerification>(
    builder.Configuration.GetSection("OtpVerificationDb")
);
builder.Services.AddSingleton<IMongoDbService<OtpVerification>>(otpDbService);

//Dependency Injections
builder.Services.AddSingleton<IUserClient, UserClient>();
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddSingleton<IOtpVerificationService, OtpVerificationService>();
builder.Services.AddSingleton<IProductClient, ProductsClient>();
builder.Services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));

// Build the app
var app = builder.Build();

// Use the error handling middleware
app.UseMiddleware<ErrorHandlingMiddleware>();

// Configure the HTTP request pipeline
app.MapControllers();

// Add CORS, authentication and authorization to the middleware pipeline
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

// Configure Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();

async Task<MongoDbService<T>> InitializeMongoClientAsync<T>(
    IConfigurationSection configurationSection
)
    where T : IMongoDbRecord
{
    var mongoSettings = MongoClientSettings.FromConnectionString(
        configurationSection.GetSection("ConnectionString").Value
    );
    mongoSettings.ServerApi = new ServerApi(ServerApiVersion.V1);
    mongoSettings.RetryWrites = true;
    mongoSettings.RetryReads = true;
    var databaseName = configurationSection.GetSection("DatabaseName").Value;
    var collectionName = configurationSection.GetSection("CollectionName").Value;
    var client = new MongoClient(mongoSettings);
    return new MongoDbService<T>(client, databaseName, collectionName);
}
