using Plantilla.WebApi.Data.Contracts;
using System.Threading.Tasks;

namespace Plantilla.WebApi.Data.Impl.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly UserDbContext context;

        public UnitOfWork(UserDbContext context)
        {
            this.context = context;
        }

        public void SaveChanges()
        {
            context.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
