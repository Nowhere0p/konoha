using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Konoha.Services;
using MongoDB.Driver;
using Konoha.DbCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//Dependency Injection
builder.Services.AddScoped<IAuthService, AuthService>();
// Remove generic registration since it needs to be registered per concrete type
// builder.Services.AddScoped<IMongoDbRecord, MongoDbRecord>(); 

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? ""))
        };
    });

builder.Services.AddAuthorization();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// Add controllers before other middleware configurations
builder.Services.AddControllers();

// MongoDB configuration
var mongoSettings = MongoClientSettings.FromConnectionString(
    builder.Configuration.GetSection("MongoDb").Get<MongoDbSettings>()?.ConnectionString
);
mongoSettings.ServerApi = new ServerApi(ServerApiVersion.V1);
mongoSettings.RetryWrites = true;
mongoSettings.RetryReads = true;

// Register MongoDB services
builder.Services.AddSingleton<IMongoClient>(sp => 
    new MongoClient(mongoSettings));

builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(builder.Configuration.GetSection("MongoDb").Get<MongoDbSettings>()?.DatabaseName);
});

var app = builder.Build();

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
