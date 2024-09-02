using System.Reflection;
using System.Text;
using System.Threading.RateLimiting;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using API.Config.Swagger;
using API.Database.Seeds;
using APP;
using APP.Mapper;
using APP.Middlewares;
using DOMAIN.Entities.Roles;
using DOMAIN.Entities.Users;
using INFRASTRUCTURE.Context;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

//integrate sentry
var sentryDsn = Environment.GetEnvironmentVariable("SENTRY_DSN");

// builder.WebHost.UseSentry(o =>
// {
//     o.Dsn = sentryDsn;
//     // When configuring for the first time, to see what the SDK is doing:
//     o.Debug = true;
//     // Set TracesSampleRate to 1.0 to capture 100%
//     // of transactions for performance monitoring.
//     // We recommend adjusting this value in production
//     o.TracesSampleRate = 1.0;
// });


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
    options.DescribeAllParametersInCamelCase();

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description =
            "JWT Authorization Header using Bearer Security Scheme. \r\r\r\r Enter Bearer [space] and then the security token to authenticate",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },

            []
        }
    });
    options.OperationFilter<ReApplyOptionalParameterFilter>();
    
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

//Add Cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("default",
        policyBuilder =>
        {
            policyBuilder
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

//validate model state
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .Select(x => new
                {
                    Code = x.Key,
                    Description = x.Value.Errors.Select(e => e.ErrorMessage).FirstOrDefault()
                })
                .ToArray();

            var problemDetails = new ProblemDetails
            {
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1", 
                Title = "One or more validation errors occurred.",
                Status = StatusCodes.Status422UnprocessableEntity,
                Extensions = { ["errors"] = errors }
            };

            return new ObjectResult(problemDetails)
            {
                StatusCode = StatusCodes.Status422UnprocessableEntity
            };
        };
    });
//add http context accessor
builder.Services.AddHttpContextAccessor();

// Configure rate limiting
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.PermitLimit = 10; 
        opt.Window = TimeSpan.FromMinutes(1); 
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 2; 
    });
});

//add automapper
builder.Services.AddAutoMapper(typeof(OryxMapper));

//configure database
var defaultDbConnectionString = Environment.GetEnvironmentVariable("ConnectionString");

builder.Services.AddDbContext<ApplicationDbContext>(o =>
    o.UseNpgsql(defaultDbConnectionString)
);

builder.Services.AddIdentityCore<User>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = false;
    })
    .AddRoles<Role>()
    .AddUserManager<UserManager<User>>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

//add authentication
var jwtKey = builder.Configuration["JwtSettings:Key"];
var keyBytes = Encoding.ASCII.GetBytes(jwtKey ?? string.Empty);

TokenValidationParameters tokenValidation = new()
{
    IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
    ValidateLifetime = true,
    ValidateAudience = false,
    ValidateIssuer = false,
    ClockSkew = TimeSpan.Zero
};

builder.Services.AddSingleton(tokenValidation);

builder.Services.AddAuthentication(authOptions =>
    {
        authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(jwtOptions =>
    {
        jwtOptions.TokenValidationParameters = tokenValidation;
    });

builder.Services.AddAuthorization();
builder.Services.AddTransientServices();
builder.Services.AddScopedServices();
builder.Services.AddSingletonServices();

//add api versioning
builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1);
        options.ApiVersionReader = new UrlSegmentApiVersionReader();
    })
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'V";
        options.SubstituteApiVersionInUrl = true;
    });

builder.Services.ConfigureOptions<ConfigureSwaggerGenOptions>();

var app = builder.Build();

app.SeedData();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    var descriptions = app.DescribeApiVersions();
    foreach (var description in descriptions)
    {
        var url = $"/swagger/{description.GroupName}/swagger.json";
        var name = description.GroupName.ToUpperInvariant();
            
        options.SwaggerEndpoint(url, name);
    }        
    options.RoutePrefix = "";
    options.DefaultModelsExpandDepth(-1);
});

//app.UseMiddleware<SentryPerformanceMiddleware>();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.SeedData();

app.UseRouting();

// Apply rate limiting middleware
//app.UseRateLimiter();

//use CORS
app.UseCors("default");

app.UseAuthentication();

app.UseMiddleware<JwtMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
