using HealthChecks.UI.Client;
using ManyInOneAPI.Configurations;
using ManyInOneAPI.Data;
using ManyInOneAPI.Infrastructure;
using ManyInOneAPI.Repositories.Payment;
using ManyInOneAPI.Repositories.Quizz;
using ManyInOneAPI.Services.Auth;
using ManyInOneAPI.Services.Clasher;
using ManyInOneAPI.Services.GenAI;
using ManyInOneAPI.Services.Quizz;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    // related to Postgres only
    var pgConnectionString = builder.Configuration.GetSection("Auth:PgConnection").Value!;
    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    // adding healthcheck
    builder.Services.AddHealthChecks()
        //.AddCheck<DatabaseHealthCheck>("DatabasehealthCheck") // for custom check
        //.AddSqlServer(connectionString!)
        .AddNpgSql(pgConnectionString!)
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

    builder.Services.AddDbContext<ManyInOnePgDbContext>(options =>
    {
        options.UseNpgsql(pgConnectionString,
        // for connection failure check if any
        pgSqlOptions =>
        {
            pgSqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorCodesToAdd: null);
        });
    });

    builder.Services.AddTransient<IEmailService, EmailService>();
    builder.Services.AddScoped<IPaymentDetailRepository, PaymentDetailRepository>();
    builder.Services.AddScoped<IQuizRepository, QuizRepository>();
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddHttpContextAccessor();

    builder.Services.AddHttpClient<IGenAIHttpClient, GenAIHttpClient>();
    builder.Services.AddHttpClient<IClashingHttpClient, ClashingHttpClient>();
    builder.Services.AddHttpClient<IQuizService, QuizService>();

    // for user 
    builder.Services.AddDefaultIdentity<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
        options.SignIn.RequireConfirmedPhoneNumber = false;
        options.SignIn.RequireConfirmedEmail = true;

    }).AddEntityFrameworkStores<ManyInOnePgDbContext>(); //.AddEntityFrameworkStores<ManyInOneDbContext>();


    // google authentication and jwt added together
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
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

            // adding this to specify that jwt will be added with cookies when there will be any request , so to get the token only 
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
                 .WithOrigins("http://localhost:4200", "https://localhost:7150", "https://localhost:8081")
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

    // Rate limiting
    builder.Services.AddRateLimiter(rateLimiterOptions =>
    {
        rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        // all 4 types of rate limitting
        // Fixed Window
        rateLimiterOptions.AddFixedWindowLimiter("fixed", options =>
        {
            options.Window = TimeSpan.FromMinutes(5);
            options.PermitLimit = 10;
            options.QueueLimit = 3;
            options.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
        });
        // Sliding Window
        rateLimiterOptions.AddSlidingWindowLimiter("sliding", options =>
        {
            options.Window = TimeSpan.FromSeconds(30);
            options.PermitLimit = 5;
            options.SegmentsPerWindow = 5;
            options.QueueLimit = 3;
            options.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
        });
        // Token Bucket 
        rateLimiterOptions.AddTokenBucketLimiter("bucket", options =>
        {
            options.TokenLimit = 10;
            options.QueueLimit = 3;
            options.ReplenishmentPeriod = TimeSpan.FromSeconds(30);
            options.TokensPerPeriod = 5;
            options.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
        });
        // Concurrency
        rateLimiterOptions.AddConcurrencyLimiter("concurrency", options =>
        {
            options.PermitLimit = 3; // 3 at a time
        });
    });

    // adding global exception handling to dependency
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();
}

var app = builder.Build();
{
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
    // using rate limitting
    app.UseRateLimiter();
    // app to use thta exception handing middleware
    app.UseExceptionHandler();
    app.MapControllers();

    app.Run();
}