using System.Data.Entity;
using Moneta.Domain.Entities;
using Moneta.Domain.Interfaces.Repository;
using Moneta.Infra.Data.Context;
using System;
using System.Linq;

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

        public GrupoLancamento GetByIdReadOnly(Guid id)
        {
            base.Context.SetProxyCreationEnabledToFalse();

            try
            {
                return DbSet.AsNoTracking().Where(x => x.GrupoLancamentoId == id).Include(x => x.Lancamentos).FirstOrDefault();
            }
            catch { }

            return null;
        }
    }
}
