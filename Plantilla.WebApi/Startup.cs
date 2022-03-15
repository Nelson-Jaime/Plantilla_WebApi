using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Plantilla.WebApi.Busines.Contracts.Security;
using Plantilla.WebApi.Busines.Contracts.Service;
using Plantilla.WebApi.Busines.Impl.Extensions;
using Plantilla.WebApi.Busines.Impl.Mapping;
using Plantilla.WebApi.Busines.Impl.Service;
using Plantilla.WebApi.Data.Contracts.Repository;
using Plantilla.WebApi.Data.Contracts.Repository.OAuth;
using Plantilla.WebApi.Extensions;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Security.Claims;
using System.Text;

namespace Plantilla.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var jwtTokenConfig = Configuration.GetSection("Jwt").Get<JwtTokenConfig>();

            services.AddSingleton(jwtTokenConfig);


            services.AddHttpClient();

            services.AddMvcCore();
            services.AddControllers();

            services.AddVersionedApiExplorer(setup =>
            {
                setup.GroupNameFormat = "'v'VVV";
                setup.SubstituteApiVersionInUrl = false;
            });

            services.AddApiVersioning(opt =>
            {
                opt.ReportApiVersions = true;
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.DefaultApiVersion = new ApiVersion(1, 0);
                opt.ApiVersionReader = new UrlSegmentApiVersionReader();
            });

            services.AddBusinessServices(Configuration);
            services.AddRedisClientsFromConfiguration(Configuration);
            services.AddAutoMapper(typeof(UserProfile));


            services.AddTransient<ITokenService>(ts =>
            {
                return new TokenService(jwtTokenConfig,
                                         ts.GetRequiredService<IUserRepository>(),
                                         ts.GetRequiredService<IUserRoleRepository>(),
                                         ts.GetRequiredService<IRoleRepository>(),
                                         ts.GetRequiredService<IRefreshTokenRepository>(),
                                         Configuration,
                                         ts.GetRequiredService<IRefreshTokenFactory>());
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = true;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtTokenConfig.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtTokenConfig.Audience,
                    ValidateIssuerSigningKey = true,
                    RequireExpirationTime = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtTokenConfig.Secret))
                };
            });

            services.AddAuthorization(opts =>
            {
                opts.AddPolicy("WithClaimRole", policy =>
                {
                    policy.RequireClaim(ClaimTypes.Role);
                    policy.RequireAuthenticatedUser();
                });

                opts.AddPolicy("WithTwoRoles", policy =>
                {
                    policy.RequireRole("Role1", "Role2");
                    policy.RequireAuthenticatedUser();
                });

            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Plantilla.WebApi", Version = "v1" });

                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Description = @"Enter 'Bearer' [space] and your token. Example: Bearer 12345abcdef",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.OAuth2,
                    Scheme = "Bearer"
                });

                c.OperationFilter<SecurityRequirementsOperationFilter>();
            });

            //services.AddHttpClient<ITokenService, TokenService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCustomSwagger(provider);

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
