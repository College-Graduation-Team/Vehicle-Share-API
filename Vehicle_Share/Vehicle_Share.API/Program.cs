using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Globalization;
using Vehicle_Share.Service.AuthService;
using System.Text;
using Vehicle_Share.Core.Repository.GenericRepo;
using Vehicle_Share.Core.Resources;
using Vehicle_Share.EF.Data;
using Vehicle_Share.EF.Helper;
using Vehicle_Share.EF.ImpRepo.GenericRepo;
using Vehicle_Share.EF.Models;
using Vehicle_Share.Service.CarService;
using Vehicle_Share.Service.LicenseService;
using Vehicle_Share.Service.RequestService;
using Vehicle_Share.Service.TripService;
using Vehicle_Share.Service.UserDataService;
using Vehicle_Share.Service.IAuthService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.Configure<TwilioSettings>(builder.Configuration.GetSection("Twilio"));
//builder.Services.AddTransient<ISendOTP, SendOTP>();

//map betwwen jwt member and json file .
builder.Services.Configure<JWT>(builder.Configuration.GetSection(nameof(JWT)));

// add connection to db and inject identity .
builder.Services.AddDbContext<ApplicationDbContext>(
     // option => option.UseSqlServer("Data Source=.;Initial Catalog=VehicleSharing;Integrated Security=True"));
      option => option.UseSqlServer("Server=db5051.public.databaseasp.net; Database=db5051; User Id=db5051; Password=6Km-#q7LY%t2; Encrypt=False; MultipleActiveResultSets=True;"));
      //option => option.UseSqlServer("Server=localhost;Database=VehicleSharing;User Id=sa;Password=Hemakress-123"));
     //Server=db4761.databaseasp.net; Database=db4761; User Id=db4761; Password=********; Encrypt=False; MultipleActiveResultSets=True;

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequiredLength = 6;
}).AddEntityFrameworkStores<ApplicationDbContext>()
 .AddDefaultTokenProviders();

// inject Repository
builder.Services.AddScoped<IAuthServ, AuthServ>();
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

// add and admin role in system 
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

