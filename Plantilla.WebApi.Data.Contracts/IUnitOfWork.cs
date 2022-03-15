using System.Threading.Tasks;

namespace Plantilla.WebApi.Data.Contracts
{
    public interface IUnitOfWork
    {
        void SaveChanges();
        Task SaveChangesAsync();
    }
}
