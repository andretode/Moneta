using System.Data.Entity;
using Moneta.Domain.Entities;
using Moneta.Domain.Interfaces.Repository;
using Moneta.Infra.Data.Context;
using System.Linq;
using System;

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

        public Lancamento GetByIdReadOnly(Guid id)
        {
            base.Context.SetProxyCreationEnabledToFalse();

            try {
                return DbSet.AsNoTracking().Where(x => x.LancamentoId == id).First();
            }
            catch {}

            return null;
        }
    }
}
