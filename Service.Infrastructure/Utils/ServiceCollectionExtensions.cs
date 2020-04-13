using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Service.Infrastructure.Auth;
using Service.Infrastructure.Filters;
using Service.Infrastructure.Models;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Text;

namespace Service.Infrastructure.Utils
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adding service connect related services into service collection
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IServiceCollection AddServiceServices(this IServiceCollection services,
            Action<AppConfigurationOptions> config)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            // getting configuration
            var options = new AppConfigurationOptions();
            config.Invoke(options);

            services.AddSingleton(provider => options);

            ////keeps the casing to that of the model when serializing to json (default is converting to camelCase)
            //services.AddMvc().AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

            // add cors
            //services.AddCors();

            services.AddCors(s =>
            {
                s.AddDefaultPolicy(c =>
                {
                    c.AllowAnyOrigin();
                    c.AllowAnyHeader();
                    c.AllowAnyMethod();
                });
            });

            

            // registers swagger
            services.AddSwagger(config);

            return services;
        }

        private static IServiceCollection AddAdditional(this IServiceCollection services,
            Action<AppConfigurationOptions> config)
        {
            var options = new AppConfigurationOptions();
            config.Invoke(options);

            // registers health check
            services.AddHealthChecks().AddSqlServer(options.DefaultConnection);

            // registers redis
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(options.Redis));

            // configures JwtIssuerOptions
            services.Configure<JwtIssuerOptions>(jwtOptions =>
            {
                jwtOptions.Issuer = options.Issuer;
                //jwtOptions.Audience = options.Audience;
                jwtOptions.SecretKey = options.SecretKey;
                jwtOptions.ValidFor = TimeSpan.FromMinutes(options.JwtTimeout);
            });

            // configures ExceptionEmailOptions
            services.Configure<ExceptionEmailOptions>(eOptions =>
            {
                eOptions.EmailSubject = $"{options.Subject} {options.InstallEnvironment}";
                eOptions.EmailTo = options.ErrorTo;
                eOptions.FromAddress = options.ErrorFrom;
                eOptions.MicroServiceName = options.MicroServiceName;
            });

            // registers EmailHelper
            services.AddTransient<IEmailHelper>(provider =>
            {
                var emailHelper = new EmailHelper(options.SmtpHost);

                return emailHelper;
            });

            // registers jwt libraries
            services.AddJwtAuthLibrary();

            // registers api version
            services.AddApiVersioning(o =>
            {
                o.ReportApiVersions = true;
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);
                o.ApiVersionReader = new HeaderApiVersionReader("api-version");
            });

            // registers authentication
            services.AddServiceAuth(config);

            return services;
        }

        /// <summary>
        /// Injecting JWT Authentication library services
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddJwtAuthLibrary(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddScoped<CustomJwtBearerEvents>();

            services.AddSingleton<IJwtFactory, JwtFactory>();
            services.AddSingleton<IJwtTokenHandler, JwtTokenHandler>();
            services.AddSingleton<IJwtTokenValidator, JwtTokenValidator>();
            services.AddSingleton<ITokenFactory, TokenFactory>();

            return services;
        }

        /// <summary>
        /// Injecting authentication services for service connect
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IServiceCollection AddServiceAuth(this IServiceCollection services,
            Action<AppConfigurationOptions> config)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            // getting configuration
            var options = new AppConfigurationOptions();
            config.Invoke(options);

            // Jwt token parameters
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = options.Issuer,
                ValidateAudience = false,
                //ValidAudience = options.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey =
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SecretKey)),
                RequireExpirationTime = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            // registers jwt authentication
            services.AddAuthentication(authOptions =>
            {
                authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(configureOptions =>
            {
                configureOptions.ClaimsIssuer = options.Issuer;
                configureOptions.TokenValidationParameters = tokenValidationParameters;
                configureOptions.SaveToken = true;

                configureOptions.EventsType = typeof(CustomJwtBearerEvents);
            });

            // registers authorization
            services.AddAuthorizationCore(authOptions =>
            {
                authOptions.AddPolicy(options.Issuer,
                    policy => policy.RequireClaim("rol", "sc_ms_api_access"));
            });

            return services;
        }

        /// <summary>
        /// Adding swagger related services for service connect
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IServiceCollection AddSwagger(this IServiceCollection services,
            Action<AppConfigurationOptions> config)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            // getting configuration
            var options = new AppConfigurationOptions();
            config.Invoke(options);

            // registers swagger ui configuration
            services.AddSwaggerGen(swaggerOptions =>
            {
                swaggerOptions.SwaggerDoc(options.SwaggerVersion, new OpenApiInfo
                {
                    Version = options.SwaggerVersion,
                    Title = options.SwaggerTitle,
                    Description = options.SwaggerDescription,
                    Contact = new OpenApiContact
                    {
                        Name = options.SwaggerTeamName,
                        Email = options.SwaggerTeamContact
                    }
                });

                swaggerOptions.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme."
                });

                swaggerOptions.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                        new string[] { }
                    }
                });

                swaggerOptions.OperationFilter<HeaderParameterOperationFilter>();

                swaggerOptions.ResolveConflictingActions(api => api.First());

                swaggerOptions.IgnoreObsoleteActions();

                swaggerOptions.IgnoreObsoleteProperties();

                //swaggerOptions.IncludeXmlComments(options.XmlCommentsFilePath);
            });

            return services;
        }
    }
}
