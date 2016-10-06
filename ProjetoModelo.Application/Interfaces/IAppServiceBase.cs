using Moneta.Infra.Data.Interfaces;

namespace Moneta.Application.Interfaces
{
    public interface IAppServiceBase<TContext> where TContext : IDbContext
    {
        void BeginTransaction();
        void Commit();
    }
}