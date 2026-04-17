using TaskManager.Context;
using TaskManager.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using TaskManager.Service.Interfaces;
using TaskManager.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using TaskManager.Utils.Authentication;
using TaskManager.Service.Base;
using TaskManager.Utils.i18n;

namespace TaskManager
{
    public class Startup(IConfiguration configuration)
    {
        private IConfiguration Configuration { get; } = configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //I18n
            services.AddScoped<IResourceStringLocalizer, ResourceStringLocalizer>();

            //Context
            services.AddScoped<IRequestContext, RequestContext>();

            //Services
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IMeService, MeService>();
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<IApiKeyService, ApiKeyService>();

            services.AddHttpContextAccessor();
            
            //Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<IApiKeyRepository, ApiKeyRepository>();

            services.AddControllers()
                .AddJsonOptions(opts =>
                {
                    var enumConverter = new JsonStringEnumConverter();
                    opts.JsonSerializerOptions.Converters.Add(enumConverter);
                });

            var connectionString = new StringBuilder();
            connectionString.Append($"User ID={Configuration["DB_USER"]};");
            connectionString.Append($"Password={Configuration["DB_PASS"]};");
            connectionString.Append($"Host={Configuration["DB_HOST"]};");
            connectionString.Append($"Port={Configuration["DB_PORT"] ?? "5432"};");
            connectionString.Append($"Database={Configuration["DB_NAME"]};");
            connectionString.Append("Pooling=true;");
            connectionString.Append("MinPoolSize=0;");
            connectionString.Append("MaxPoolSize=1024;");
            connectionString.Append("ConnectionLifetime=0;");

            services.AddDbContext<TaskManagerContext>(options =>
            {
                options
                    .UseNpgsql(connectionString.ToString())
                    .UseSnakeCaseNamingConvention();
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TaskManager", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Autorização. Ex: Bearer {token}"
                });

                c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
                {
                    Name = "X-Api-Key",
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Description = "Chave de Acesso. Ex: tm_sk_..."
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    },
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "ApiKey"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            services
                .AddAuthentication(x => 
                { 
                    x.DefaultAuthenticateScheme = "JwtOrApiKey";
                    x.DefaultChallengeScheme = "JwtOrApiKey";
                })
                .AddPolicyScheme("JwtOrApiKey", "Jwt or ApiKey", options =>
                {
                    options.ForwardDefaultSelector = context =>
                    {
                        if (context.Request.Headers.ContainsKey("X-Api-Key"))
                        {
                            return ApiKeyAuthenticationOptions.DefaultScheme;
                        }
                        return JwtBearerDefaults.AuthenticationScheme;
                    };
                })
                .AddJwtBearer(options => 
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["Jwt:Issuer"],
                        ValidAudience = Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]!))
                    };
                })
                .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(ApiKeyAuthenticationOptions.DefaultScheme, options => { });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder =>
                    {
                        builder.WithOrigins("https://tasks.alexei.dev.br")
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    });
                    
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment() || env.IsStaging())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TaskManager v1"));
                app.UseCors("AllowAll");
            }
            else
            {
                app.UseCors("AllowSpecificOrigin");
            }


            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
