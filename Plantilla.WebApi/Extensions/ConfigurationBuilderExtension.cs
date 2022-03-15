using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Linq;

namespace Plantilla.WebApi.Extensions
{
    public static class ConfigurationBuilderExtension
    {
        public static IConfigurationBuilder AddJsonFiles(this IConfigurationBuilder builder, WebHostBuilderContext builderContext)
        {

            var configFiles = Directory.GetFiles(Path.Combine(builderContext.HostingEnvironment.ContentRootPath, "Configuration"), "settings.*").ToList();
            var filteredConfigFiles = configFiles.Where(x => !x.Contains(".development.")).ToList();

            if (builderContext.HostingEnvironment.IsDevelopment())
                filteredConfigFiles.AddRange(configFiles.Where(x => x.Contains(".development.")).ToList());


            foreach (var configFile in filteredConfigFiles)
            {
                builder.AddJsonFile(configFile, optional: false, reloadOnChange: true);
            }
            return builder;
        }

        public static IConfigurationBuilder If(this IConfigurationBuilder builder, Func<bool> validate, Action<IConfigurationBuilder> action)
        {

            if (validate.Invoke())
            {
                action.Invoke(builder);
            }

            return builder;
        }
    }
}
