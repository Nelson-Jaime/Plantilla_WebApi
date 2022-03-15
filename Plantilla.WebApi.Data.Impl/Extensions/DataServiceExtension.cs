using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Plantilla.WebApi.Data.Contracts;
using Plantilla.WebApi.Data.Contracts.Repository;
using Plantilla.WebApi.Data.Contracts.Repository.OAuth;
using Plantilla.WebApi.Data.Impl.Repository;

namespace Plantilla.WebApi.Data.Impl.Extensions
{
    public static class DataServiceExtension
    {
        public static IServiceCollection AddDataExtension(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<UserDbContext>(builder =>
            {
                builder.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
                builder.UseSqlServer(configuration.GetConnectionString("Db"));
            });

            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IRoleRepository, RoleRepository>();
            services.AddTransient<IUserRoleRepository, UserRoleRepository>();

            services.AddTransient<IRefreshTokenRepository, RefreshTokenRepository>();
            //services.AddTransient<IRefreshTokenRepository, RefreshTokenRedisRepository>();

            services.AddTransient<IRefreshTokenFactory, RefreshTokenFactory>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
