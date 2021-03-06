﻿using Contracts;
using Entites;
using LoggerService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repository;
using Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using AutoMapper;
using SushiPizzaServer.ActionFilters;

namespace SushiPizzaServer.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureCORS(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CORS", policy =>
                {
                    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });
        }

        public static void ConfigureLoggerService(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerManager, LoggerManager>();
        }

        public static void ConfigurePostgreSqlContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<RepositoryContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("PostgreSql"),
                    x => x.MigrationsAssembly("PostgresMigrations"));
            });
        }

        public static void ConfigureMsSqlLocalContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<RepositoryContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("MsSqlLocalDb"),
                    x => x.MigrationsAssembly("MSSQLMigrations"));
            });
        }

        public static void ConfigureRepositoryWrapper(this IServiceCollection services)
        {
            services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
        }

        public static void ConfigureAuthenticationWithJwtBearer(this IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.RequireHttpsMetadata = false;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidIssuer = AuthOptions.ISSUER,
                            ValidateAudience = true,
                            ValidAudience = AuthOptions.AUDIENCE,
                            ValidateLifetime = true,
                            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                            ValidateIssuerSigningKey = true
                        };
                    });
        }

        public static void ConfigureAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(Startup));
        }

        public static void ConfigureModelValidationAttribute(this IServiceCollection services)
        {
            services.AddScoped<ModelValidationAttribute>();
        }
    }
}
