using System.Data.Entity;
using Moneta.Domain.Entities;
using Moneta.Domain.Interfaces.Repository;
using Moneta.Infra.Data.Context;

namespace Moneta.Infra.Data.Repositories
{
    public class GrupoLancamentoRepository : RepositoryBase<GrupoLancamento, MonetaContext>, IGrupoLancamentoRepository
    {
        public override void Remove(GrupoLancamento grupoLancamento)
        {
            var entry = Context.Entry(grupoLancamento);
            DbSet.Attach(grupoLancamento);
            entry.State = EntityState.Deleted;
        }
    }
}
