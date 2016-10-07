using System.Data.Entity;
using Moneta.Domain.Entities;
using Moneta.Domain.Interfaces.Repository;
using Moneta.Infra.Data.Context;

namespace Moneta.Infra.Data.Repositories
{
    public class LancamentoRepository : RepositoryBase<Lancamento, MonetaContext>, ILancamentoRepository
    {
        public override void Remove(Lancamento lancamento)
        {
            var entry = Context.Entry(lancamento);
            DbSet.Attach(lancamento);
            entry.State = EntityState.Deleted;
        }
    }
}
