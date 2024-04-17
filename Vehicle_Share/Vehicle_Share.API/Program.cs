using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Globalization;
using System.Reflection;
using System.Text;
using Vehicle_Share.Core.Repository.AuthRepo;
using Vehicle_Share.Core.Repository.GenericRepo;
using Vehicle_Share.Core.Repository.SendOTP;
using Vehicle_Share.Core.Resources;
using Vehicle_Share.EF.Data;
using Vehicle_Share.EF.Helper;
using Vehicle_Share.EF.ImpRepo.AuthRepo;
using Vehicle_Share.EF.ImpRepo.GenericRepo;
using Vehicle_Share.EF.ImpRepo.SendOTPImplement;
using Vehicle_Share.EF.Models;
using Vehicle_Share.Service.CarService;
using Vehicle_Share.Service.LicenseService;
using Vehicle_Share.Service.RequestService;
using Vehicle_Share.Service.TripService;
using Vehicle_Share.Service.UserDataService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<TwilioSettings>(builder.Configuration.GetSection("Twilio"));
builder.Services.AddTransient<ISendOTP, SendOTP>();
//map betwwen jwt member and json file .
builder.Services.Configure<JWT>(builder.Configuration.GetSection(nameof(JWT)));

// add connection to db and inject identity .
builder.Services.AddDbContext<ApplicationDbContext>(
    // option => option.UseSqlServer("Data Source=.;Initial Catalog=VehicleSharing;Integrated Security=True"));
    option => option.UseSqlServer("Server=localhost;Database=VehicleSharing;User Id=sa;Password=Hemakress-123"));
builder.Services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();

// inject Repository
builder.Services.AddScoped<IAuthRepo, AuthRepo>();
builder.Services.AddScoped(typeof(IBaseRepo<>), typeof(BaseRepo<>));
builder.Services.AddScoped<IUserDataServ, UserDataServ>();
builder.Services.AddScoped<ICarServ, CarServ>();
builder.Services.AddScoped<ILicServ, LicServ>();
builder.Services.AddScoped<ITripServ, TripServ>();
builder.Services.AddScoped<IRequestServ, RequestServ>();

builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
// Or you can also register as follows
builder.Services.AddHttpContextAccessor();


// add core .
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});



// Adding Jwt Bearer
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(o =>
    {
        o.RequireHttpsMetadata = false;
        o.SaveToken = false;
        o.TokenValidationParameters = new TokenValidationParameters
        {

            ValidateIssuerSigningKey = true,
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
            ValidAudience = builder.Configuration["JWT:ValidAudience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"])),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

#region LocaLization confgration

// builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// builder.Services.AddMvc()
//     .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
//     .AddDataAnnotationsLocalization();

// builder.Services.AddLocalization(options => options.ResourcesPath = "SharedResources");

// builder.Services.AddMvc()
//         .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
//         .AddDataAnnotationsLocalization(options => {
//             options.DataAnnotationLocalizerProvider = (type, factory) =>
//                 factory.Create(typeof(Vehicle_Share.API.SharedResources.SharedResources));
//         });

// builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// builder.Services.AddMvc()
//     .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
//     .AddDataAnnotationsLocalization();

builder.Services.AddControllersWithViews();
builder.Services.AddLocalization(opt =>
{
    opt.ResourcesPath = "";
});

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    List<CultureInfo> supportedCultures = new List<CultureInfo>
    {
            new CultureInfo("en-US"),
            new CultureInfo("ar-EG")
    };

    options.DefaultRequestCulture = new RequestCulture("ar-EG");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});
builder.Services.AddMvc()
.AddDataAnnotationsLocalization(options =>
{
    options.DataAnnotationLocalizerProvider = (type, factory) =>
        factory.Create(typeof(SharedResources));
});
#endregion

/*
  // Adding Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = false;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,

        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"]))
    };
});
*/

builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MyAPI", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});
var app = builder.Build();

// add role in system 
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    SeedRole.SeedRolesAsync(context).Wait();
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

#region LocaLization Mideware

var options = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(options.Value);

#endregion

app.UseCors(policyName: "CorsPolicy");
app.UseStaticFiles(); // to upload image in wwwroot 
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

