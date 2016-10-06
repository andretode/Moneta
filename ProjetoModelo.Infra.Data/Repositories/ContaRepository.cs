using System.Data.Entity;
using Moneta.Domain.Entities;
using Moneta.Domain.Interfaces.Repository;
using Moneta.Infra.Data.Context;

namespace Moneta.Infra.Data.Repositories
{
    public class ContaRepository : RepositoryBase<Conta, MonetaContext>, IContaRepository
    {
        public override void Remove(Conta conta)
        {
            var entry = Context.Entry(conta);
            DbSet.Attach(conta);
            entry.State = EntityState.Deleted;
        }
    }
}
