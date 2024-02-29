using CourseEnrollmentApp_API.Models;
using CourseEnrollmentApp_API.Services;
using CourseEnrollmentApp_API.Services.Bug;
using CourseEnrollmentApp_API.Services.Setting;
using CourseEnrollmentApp_API.Services.TempProc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

Setting setting = new();
builder.Configuration.Bind(nameof(setting), setting);

AddServices();

AddIdentity();

AddAuthentication();

ConfigureApp();


void AddServices()
{
    builder.Services.AddControllers(config =>
    {
        config.Filters.Add<HttpResponseExceptionFilter>();
    }).AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
    });

    builder.Services.AddSwaggerGen(c =>
    {
        c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
        c.IgnoreObsoleteActions();
        c.IgnoreObsoleteProperties();
        c.CustomSchemaIds(type => type.FullName);
    });

    builder.Services.AddSingleton(setting);
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddTransient<IErrorLogService, ErrorLogService>();
    var MyAllowAllHeadersPolicy = "MyAllowAllHeadersPolicy";

    builder.Services.AddCors(options =>
    {
        options.AddPolicy(name: MyAllowAllHeadersPolicy,
                          policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
    });

    builder.Services.AddTransient<ITmpProcService, TmpProc>();
}

void AddAuthentication()
{
    var tokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        //ValidateActor=true,
        ValidIssuer = setting.JwtSetting.Issuer,
        ValidAudience = setting.JwtSetting.Audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(setting.JwtSetting.IssuerSigningKey))
    };

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

    }).AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = tokenValidationParameters;
    });
}

void AddIdentity()
{
    //builder.Services.AddDbContext<DBContext>(options => options.UseSqlServer(setting.ConnectionString));
    builder.Services.AddDbContext<DBContext>(options => options.UseInMemoryDatabase("CourseEnrollmentApp_API"));

    builder.Services.AddIdentity<User, IdentityRole>(opts =>
    {
        opts.User.RequireUniqueEmail = setting.IsProduction;
        opts.Password.RequiredLength = 6;
        opts.Password.RequireNonAlphanumeric = false;
        opts.Password.RequireLowercase = false;
        opts.Password.RequireUppercase = false;
        opts.Password.RequireDigit = false;
    }).AddEntityFrameworkStores<DBContext>().AddDefaultTokenProviders();
}

void ConfigureApp()
{
    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseAuthorization();

    app.MapControllers();
    app.UseCors("MyAllowAllHeadersPolicy");

    Initialise(app);

    app.Run();
}

static void Initialise(IApplicationBuilder app)
{
    var tmpProcService = app.ApplicationServices.CreateScope().ServiceProvider.GetService<ITmpProcService>();
    tmpProcService.Run();
}

