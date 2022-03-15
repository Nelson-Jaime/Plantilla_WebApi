using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Plantilla.WebApi.Extensions
{
    public class SwaggerVersionDescription
    {
        private readonly Dictionary<string, string> _descriptions;

        /// <summary>
        /// 
        /// </summary>
        public SwaggerVersionDescription()
        {
            _descriptions = new Dictionary<string, string>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="version"></param>
        /// <param name="description"></param>
        public void AddDescription(string version, string description)
        {
            if (!_descriptions.ContainsKey(version))
                _descriptions.Add(version, description);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public string GetDescription(string version)
        {
            return _descriptions.ContainsKey(version) ?
                _descriptions[version] :
                string.Empty;
        }
    }

    public static class OpenApiExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseCustomSwagger(this IApplicationBuilder app, IApiVersionDescriptionProvider provider)
        {

            app.UseSwagger(opts =>
            {
                opts.RouteTemplate = "api-docs/{documentName}/swagger.json";
            });

            app.UseSwaggerUI(options =>
            {
                // build a swagger endpoint for each discovered API version
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"./{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                    options.RoutePrefix = "api-docs";
                }
            });

            return app;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="scheme"></param>
        /// <param name="versionDescription"></param>
        /// <param name="swaggerOptionsAction"></param>
        /// <returns></returns>
        public static IServiceCollection AddCustomSwaggerGen(this IServiceCollection services,
                                                            IConfiguration configuration,
                                                            OpenApiSecurityScheme scheme = null,
                                                            SwaggerVersionDescription versionDescription = null,
                                                            Action<SwaggerGenOptions> swaggerOptionsAction = null)
        {

            services.AddSwaggerGen(opts =>
            {
                var appInfo = PlatformServices.Default.Application;

                //opts.AddSecurityDefinition("oauth2", scheme ??
                //                                        new OpenApiSecurityScheme
                //                                        {
                //                                            Description = @"Enter 'Bearer' [space] and your token. Example: Bearer 12345abcdef",
                //                                            Name = "Authorization",
                //                                            In = ParameterLocation.Header,
                //                                            Type = SecuritySchemeType.ApiKey,
                //                                            Scheme = "Bearer"
                //                                        });

                //opts.OperationFilter<SecurityRequirementsOperationFilter>();

                var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    opts.SwaggerDoc(description.GroupName,
                        CreateInfoForApiVersion(description, appInfo, versionDescription, configuration));
                }

                foreach (var xmlDocumentPath in GetXmlFiles())
                {
                    opts.IncludeXmlComments(xmlDocumentPath);
                }

            });

            if (swaggerOptionsAction != null)
            {
                services.AddSwaggerGen(swaggerOptionsAction);
            }

            return services;
        }

        private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description,
                                                        ApplicationEnvironment appInfo,
                                                        SwaggerVersionDescription versionDescription,
                                                        IConfiguration configuration)
        {
            var info = new OpenApiInfo
            {
                Title = appInfo.ApplicationName,
                Description = versionDescription?.GetDescription(description.ApiVersion.ToString()),
                Version = description.ApiVersion.ToString(),
                Contact = new OpenApiContact
                {
                    Name = configuration.GetValue<string>("ApplicationNamed"),
                    Url = new Uri(configuration.GetValue<string>("CustomerUrl"))
                }
            };

            if (description.IsDeprecated)
            {
                info.Description += " This API version has been deprecated! ";
            }

            return info;
        }

        private static IEnumerable<string> GetXmlFiles()
        {

            var basePath = PlatformServices.Default.Application.ApplicationBasePath;

            return Directory
                .EnumerateFiles(basePath, "*.xml")
                .Where(p => Path.GetFileName(p) != null &&
                            (Path.GetFileName(p).StartsWith("VY.", StringComparison.InvariantCultureIgnoreCase)));
        }
    }
}
