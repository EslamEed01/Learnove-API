using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Authorization;
using Hangfire;
using Hangfire.MySql;
using Learnova.API.Filters;
using Learnova.API.HealthChecks;
using Learnova.Business.DTOs.EmbeddingDTO;
using Learnova.Business.DTOs.GeminiDTO;
using Learnova.Business.DTOs.PineconeDTO;
using Learnova.Business.Identity;
using Learnova.Business.Implementations;
using Learnova.Business.Implementations.PdfProccessingServices;
using Learnova.Business.Mapping;
using Learnova.Business.Services.Interfaces;
using Learnova.Business.Services.Interfaces.PdfProccessingInterfaces;
using Learnova.Business.Services.Interfaces.PdfProccessingServices;
using Learnova.Domain.Entities;
using Learnova.Infrastructure.Authentication;
using Learnova.Infrastructure.Data.Context;
using Learnova.Infrastructure.Data.Seeder;
using Learnova.Infrastructure.Email;
using Learnova.Infrastructure.Filters;
using Learnova.Infrastructure.Identity;
using Learnova.Infrastructure.Implementations;
using Learnova.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json;

namespace Learnova.API
{
    /// <summary>
    /// The main entry point for the Learnova API application.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The main entry point method for the application.
        /// </summary>
        /// <param name="args">Command-line arguments passed to the application.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddOpenApi();
            builder.Services.AddSwaggerGen();

            #region MySqlConnection
            builder.Services.AddDbContext<LearnoveDbContext>(options =>
                    options.UseMySql(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection")),
                    mySqlOptions => mySqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null
                    )
                )
            );
            #endregion

            #region Health Check Services Registration
            builder.Services.AddTransient<PineconeApiHealthCheck>();
            builder.Services.AddTransient<CohereEmbeddingApiHealthCheck>();
            builder.Services.AddTransient<GeminiApiHealthCheck>();
            builder.Services.AddTransient<TextChunkerHealthCheck>();
            builder.Services.AddTransient<PdfProcessingServiceHealthCheck>();
            builder.Services.AddTransient<AiPipelineHealthCheck>();
            #endregion

            #region Health Checks
            builder.Services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy("Application is running"))
                .AddMySql(
                    connectionString: builder.Configuration.GetConnectionString("DefaultConnection")!,
                    name: "mysql-database",
                    failureStatus: HealthStatus.Unhealthy,
                    tags: new[] { "database", "mysql", "infrastructure" },
                    timeout: TimeSpan.FromSeconds(10))

                .AddDbContextCheck<LearnoveDbContext>(
                    customTestQuery: async (context, cancellationToken) =>
                    {
                        var userCount = await context.Users.CountAsync(cancellationToken);
                        var courseCount = await context.Courses.CountAsync(cancellationToken);
                        return userCount >= 0 && courseCount >= 0;
                    })

               .AddCheck<CohereEmbeddingApiHealthCheck>("cohere-embedding-api", tags: new[] { "external-api", "cohere", "embedding" })
               .AddCheck<PineconeApiHealthCheck>("pinecone-api", tags: new[] { "external-api", "pinecone", "vector-store" })
                .AddCheck<GeminiApiHealthCheck>("gemini-api", tags: new[] { "external-api", "gemini", "ai-generation" })
                .AddCheck<TextChunkerHealthCheck>("text-chunker", tags: new[] { "internal-service", "text-processing", "chunking" })
                .AddCheck<PdfProcessingServiceHealthCheck>("pdf-processing", tags: new[] { "internal-service", "pdf-processing", "document-processing" })
                .AddCheck<AiPipelineHealthCheck>("ai-pipeline", tags: new[] { "ai-pipeline", "integration", "end-to-end" });
            #endregion

            //#region Rate Limiting
            //builder.Services.AddRateLimiter(options =>
            //{
            //    options.AddPolicy("CheckoutPolicy", policy =>
            //    {
            //        policy.AddFixedWindowLimiter(policyName: "CheckoutPolicy", options =>
            //        {
            //            options.PermitLimit = 5;
            //            options.Window = TimeSpan.FromMinutes(1);
            //            options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            //            options.QueueLimit = 2;
            //        });
            //    });
            //});
            //#endregion
            var licenseKey = builder.Configuration["AutoMapper:LicenseKey"];

            #region AutomapperConfig
            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.LicenseKey = "";
            }, typeof(MappingProfile), typeof(RoleMappingProfile));
            #endregion

            #region Injections
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<ICourseRepository, CourseRepository>();
            builder.Services.AddScoped<ICourseService, CourseService>();
            builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
            builder.Services.AddScoped<IPdfContentRepository, PdfContentRepository>();
            builder.Services.AddScoped<IVideoRepository, VideoRepository>();
            builder.Services.AddScoped<IAIService, AIService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IRoleService, RoleService>();
            //builder.Services.AddScoped<IVectorStoreService, VectorStoreService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IReviewService, ReviewService>();
            builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
            #endregion

            #region Background Job Services
            builder.Services.AddScoped<IBackgroundJobService, BackgroundJobService>();
            #endregion

            #region Stripe Configuration
            builder.Services.AddScoped<IStripePayment, StripePaymentService>();
            #endregion

            #region register auth
            builder.Services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();
            builder.Services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddSingleton<IJwtProvider, JwtProvider>();

            builder.Services.AddOptions<JwtOptions>()
                        .BindConfiguration(JwtOptions.SectionName)
                        .ValidateDataAnnotations()
                        .ValidateOnStart();

            var jwtSettings = builder.Configuration
                                     .GetSection(JwtOptions.SectionName).Get<JwtOptions>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.SaveToken = true;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings?.Key!)),
                    ValidIssuer = jwtSettings?.Issuer,
                    ValidAudience = jwtSettings?.Audience,
                    RoleClaimType = "role"
                };
            });

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Auth/Login";
                options.AccessDeniedPath = "/Auth/AccessDenied";
            });
            #endregion

            builder.Services
                .AddIdentity<AppUser, ApplicationRole>()
                .AddEntityFrameworkStores<LearnoveDbContext>()
                .AddDefaultTokenProviders();

            #region register aws
            var awsOptions = builder.Configuration.GetSection("AWS");
            builder.Services.AddAWSService<IAmazonS3>(new Amazon.Extensions.NETCore.Setup.AWSOptions
            {
                Region = RegionEndpoint.GetBySystemName(awsOptions["Region"]),
                Credentials = new BasicAWSCredentials(
                    awsOptions["AccessKey"],
                    awsOptions["SecretKey"]
                )
            });
            #endregion

            #region aws S3
            builder.Services.AddScoped<IPdfUploadService, PdfUploadService>();
            builder.Services.AddScoped<IVideoUploadService, VideoUploadService>();
            builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions("AWS"));
            #endregion

            #region Pinecone Configuration
            builder.Services.AddOptions<PineconeOptions>()
                .BindConfiguration(PineconeOptions.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            builder.Services.AddHttpClient<PineconeService>();
            builder.Services.AddScoped<IPineconeService, PineconeService>();
            #endregion

            #region Cohere Embedding Services
            builder.Services.AddOptions<CohereOptions>()
                .BindConfiguration(CohereOptions.SectionName)
                .ValidateDataAnnotations()
                .Validate(options => !string.IsNullOrEmpty(options.ApiKey), "Cohere API key is required")
                .ValidateOnStart();

            builder.Services.AddHttpClient<CohereEmbeddingService>();
            builder.Services.AddScoped<IEmbeddingService, CohereEmbeddingService>();
            builder.Services.AddSingleton<IEmbeddingStatsService, EmbeddingStatsService>();
            builder.Services.AddMemoryCache();
            #endregion

            #region Gemini Configuration
            builder.Services.Configure<GeminiOptions>(
                builder.Configuration.GetSection(GeminiOptions.SectionName));

            builder.Services.AddHttpClient<IGeminiService, GeminiService>();
            builder.Services.AddScoped<IGeminiService, GeminiService>();
            #endregion

            #region Pdf processing
            builder.Services.AddTransient<IEmailSender, EmailService>();
            builder.Services.AddScoped<IPdfProcessingService, PdfProcessingService>();
            builder.Services.AddScoped<TextChunker>();
            #endregion

            #region Swagger configuration
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Learnova API", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.OperationFilter<AuthResponsesOperationFilter>();

                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }
            });
            #endregion


            #region Hangfire config

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            var hangfireConnectionString = connectionString?.Contains("Allow User Variables") == true
                ? connectionString
                : $"{connectionString};Allow User Variables=true";

            builder.Services.AddHangfire(config =>
                config.UseStorage(new MySqlStorage(
                    hangfireConnectionString,
                    new MySqlStorageOptions()
                )));
            builder.Services.AddHangfireServer();

            #endregion

            var app = builder.Build();
            app.UseHangfireDashboard();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            #region Hangfire Dashboard
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireAuthorizationFilter(app.Services) },
                DashboardTitle = "Learnova Background Jobs",
                StatsPollingInterval = 2000,
                DisplayStorageConnectionString = false
            });
            #endregion

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();


            #region Health Check Endpoints
            app.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "application/json";

                    var result = JsonSerializer.Serialize(new
                    {
                        status = report.Status.ToString(),
                        timestamp = DateTime.UtcNow,
                        totalDuration = report.TotalDuration.ToString(),
                        checks = report.Entries.Select(entry => new
                        {
                            name = entry.Key,
                            status = entry.Value.Status.ToString(),
                            description = entry.Value.Description,
                            duration = entry.Value.Duration.ToString(),
                            exception = entry.Value.Exception?.Message,
                            data = entry.Value.Data
                        })
                    }, new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });

                    await context.Response.WriteAsync(result);
                }
            });

            app.MapHealthChecks("/health/live", new HealthCheckOptions
            {
                Predicate = _ => false, // Don't run any checks, just return healthy
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonSerializer.Serialize(new
                    {
                        status = "Healthy",
                        timestamp = DateTime.UtcNow,
                        message = "Application is alive"
                    }));
                }
            });

            app.MapHealthChecks("/health/ready", new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("database"),
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "application/json";

                    var result = JsonSerializer.Serialize(new
                    {
                        status = report.Status.ToString(),
                        timestamp = DateTime.UtcNow,
                        ready = report.Status == HealthStatus.Healthy,
                        checks = report.Entries.Select(entry => new
                        {
                            name = entry.Key,
                            status = entry.Value.Status.ToString(),
                            duration = entry.Value.Duration.ToString()
                        })
                    });

                    await context.Response.WriteAsync(result);
                }
            });

            // Database health check
            app.MapHealthChecks("/health/database", new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("mysql"),
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "application/json";

                    var result = JsonSerializer.Serialize(new
                    {
                        status = report.Status.ToString(),
                        timestamp = DateTime.UtcNow,
                        database = "MySQL",
                        checks = report.Entries.Select(entry => new
                        {
                            name = entry.Key,
                            status = entry.Value.Status.ToString(),
                            description = entry.Value.Description,
                            duration = entry.Value.Duration.ToString(),
                            exception = entry.Value.Exception?.Message
                        })
                    });

                    await context.Response.WriteAsync(result);
                }
            });

            // AI Services health check
            app.MapHealthChecks("/health/ai", new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("external-api") || check.Tags.Contains("ai-pipeline"),
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "application/json";

                    var result = JsonSerializer.Serialize(new
                    {
                        status = report.Status.ToString(),
                        timestamp = DateTime.UtcNow,
                        category = "AI Services",
                        totalDuration = report.TotalDuration.ToString(),
                        checks = report.Entries.Select(entry => new
                        {
                            name = entry.Key,
                            status = entry.Value.Status.ToString(),
                            description = entry.Value.Description,
                            duration = entry.Value.Duration.ToString(),
                            exception = entry.Value.Exception?.Message,
                            tags = entry.Value.Tags
                        })
                    }, new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });

                    await context.Response.WriteAsync(result);
                }
            });

            // Text processing services health check
            app.MapHealthChecks("/health/text-processing", new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("text-processing") || check.Tags.Contains("document-processing"),
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "application/json";

                    var result = JsonSerializer.Serialize(new
                    {
                        status = report.Status.ToString(),
                        timestamp = DateTime.UtcNow,
                        category = "Text Processing Services",
                        totalDuration = report.TotalDuration.ToString(),
                        checks = report.Entries.Select(entry => new
                        {
                            name = entry.Key,
                            status = entry.Value.Status.ToString(),
                            description = entry.Value.Description,
                            duration = entry.Value.Duration.ToString(),
                            exception = entry.Value.Exception?.Message,
                            tags = entry.Value.Tags
                        })
                    }, new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });

                    await context.Response.WriteAsync(result);
                }
            });

            // Pinecone health check
            app.MapHealthChecks("/health/pinecone", new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("pinecone"),
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "application/json";

                    var result = JsonSerializer.Serialize(new
                    {
                        status = report.Status.ToString(),
                        timestamp = DateTime.UtcNow,
                        service = "Pinecone Vector Database",
                        totalDuration = report.TotalDuration.ToString(),
                        checks = report.Entries.Select(entry => new
                        {
                            name = entry.Key,
                            status = entry.Value.Status.ToString(),
                            description = entry.Value.Description,
                            duration = entry.Value.Duration.ToString(),
                            exception = entry.Value.Exception?.Message
                        })
                    }, new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });

                    await context.Response.WriteAsync(result);
                }
            });

            // Cohere health check
            app.MapHealthChecks("/health/cohere", new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("cohere"),
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "application/json";

                    var result = JsonSerializer.Serialize(new
                    {
                        status = report.Status.ToString(),
                        timestamp = DateTime.UtcNow,
                        service = "Cohere Embedding API",
                        totalDuration = report.TotalDuration.ToString(),
                        checks = report.Entries.Select(entry => new
                        {
                            name = entry.Key,
                            status = entry.Value.Status.ToString(),
                            description = entry.Value.Description,
                            duration = entry.Value.Duration.ToString(),
                            exception = entry.Value.Exception?.Message
                        })
                    }, new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });

                    await context.Response.WriteAsync(result);
                }
            });

            // Gemini health check
            app.MapHealthChecks("/health/gemini", new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("gemini"),
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "application/json";

                    var result = JsonSerializer.Serialize(new
                    {
                        status = report.Status.ToString(),
                        timestamp = DateTime.UtcNow,
                        service = "Google Gemini AI API",
                        totalDuration = report.TotalDuration.ToString(),
                        checks = report.Entries.Select(entry => new
                        {
                            name = entry.Key,
                            status = entry.Value.Status.ToString(),
                            description = entry.Value.Description,
                            duration = entry.Value.Duration.ToString(),
                            exception = entry.Value.Exception?.Message
                        })
                    }, new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });

                    await context.Response.WriteAsync(result);
                }
            });
            #endregion

            app.MapControllers();

            #region register Seed data
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<LearnoveDbContext>();
                dbContext.Database.Migrate();
                await SeedData.InitializeAsync(dbContext);
            }
            #endregion

            app.Run();
        }
    }
}
