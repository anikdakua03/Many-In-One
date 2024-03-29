using HealthChecks.UI.Client;
using ManyInOneAPI.Configurations;
using ManyInOneAPI.Data;
using ManyInOneAPI.Health;
using ManyInOneAPI.Repositories.Payment;
using ManyInOneAPI.Services.Auth;
using ManyInOneAPI.Services.Clasher;
using ManyInOneAPI.Services.GenAI;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
//var pgConnectionString = builder.Configuration.GetConnectionString("PgConnection");

// adding healthcheck
builder.Services.AddHealthChecks()
    //.AddCheck<DatabaseHealthCheck>("DatabasehealthCheck") // for custom check
    .AddSqlServer(connectionString!)
     //.AddNpgSql(pgConnectionString!)
    ;
// Add services to the container.
builder.Services.AddControllers();


// adding configurations
builder.Services.Configure<AuthConfig>(builder.Configuration.GetSection("Auth"));
builder.Services.Configure<GenAIConfig>(builder.Configuration.GetSection("GenAI"));
builder.Services.Configure<EmailConfig>(builder.Configuration.GetSection("MailConfig"));
builder.Services.Configure<ClasherConfig>(builder.Configuration.GetSection("Clasher"));

builder.Services.AddDbContext<ManyInOneDbContext>(options =>
{
    options.UseSqlServer(connectionString,
    // for connection failure check if any
    sqlServerOptionsAction: sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
    });
});

// builder.Services.AddDbContext<ManyInOnePgDbContext>(options =>
// {
//     options.UseNpgsql(pgConnectionString,
//     // for connection failure check if any
//     pgSqlOptions =>
//     {
//         pgSqlOptions.EnableRetryOnFailure(
//             maxRetryCount: 5,
//             maxRetryDelay: TimeSpan.FromSeconds(30),
//             errorCodesToAdd : null);
//     });
// });

builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddScoped<IPaymentDetailRepository, PaymentDetailRepository>();
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddHttpContextAccessor(); // AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddHttpClient<IGenAIHttpClient, GenAIHttpClient>();
builder.Services.AddHttpClient<IClashingHttpClient, ClashingHttpClient>(client =>
{
    var baseUrl = builder.Configuration.GetSection("Clasher").GetValue<string>("ClasherAPIURL");
    client.BaseAddress = new Uri(baseUrl!);
});

// for user 
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.SignIn.RequireConfirmedPhoneNumber = false;
    options.SignIn.RequireConfirmedEmail = true;

}).AddEntityFrameworkStores<ManyInOneDbContext>(); //.AddEntityFrameworkStores<ManyInOnePgDbContext>(); //.


// google authentication and jwt added together
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    //--------------
    //options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    //options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
    .AddCookie(ck =>
    {
        ck.Cookie.Name = "token";
    })

    .AddJwtBearer(jwt =>
    {
        var key = Encoding.ASCII.GetBytes(builder.Configuration.GetSection("Auth:Secret").Value!);
        jwt.RequireHttpsMetadata = false;
        jwt.SaveToken = true;
        jwt.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration.GetSection("Auth:Issuer").Value!,
            //need to turn true when managing with frontend
            ValidateAudience = true,
            ValidAudience = builder.Configuration.GetSection("Auth:Audience").Value!,
            RequireExpirationTime = false,
            ValidateLifetime = false
        };

        // adding this to specify that jwt will be added with cookies when there will be any reqest , so to get the token only 
        // from the cookie
        jwt.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Cookies["x-access-token"]; // get that only token part
                return Task.CompletedTask;
            }
        };
    })
    .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
    {
        options.ClientId = builder.Configuration.GetSection("Auth:GoogleClientId").Value!;
        options.ClientSecret = builder.Configuration.GetSection("Auth:GoogleClientSecret").Value!;
        options.ClaimActions.MapJsonKey("urn:google:picture", "picturer", "url");
    })
    ;

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policyBuilder =>
    {
        policyBuilder
            .WithOrigins("http://localhost:4200","https://localhost:7150", "https://localhost:8081") // Allow requests from only this origin
            .AllowAnyMethod() 
            .AllowCredentials()
            .AllowAnyHeader(); 
    });
});

// for handling multipart body length
builder.Services.Configure<FormOptions>(options =>
{
    // accepting all max size
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartBodyLengthLimit = int.MaxValue;
    options.MemoryBufferThreshold = int.MaxValue;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Bearer authentication with JWT token",
        Type = SecuritySchemeType.Http
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// for viewing static file within this localhost
app.UseStaticFiles();

app.UseCors("Frontend");

// adding healthchecks , ordering matters
app.MapHealthChecks("/_health", new HealthCheckOptions
{
    // decorating health check responses
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();