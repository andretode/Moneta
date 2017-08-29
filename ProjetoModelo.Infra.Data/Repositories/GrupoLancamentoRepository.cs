using System.Data.Entity;
using Moneta.Domain.Entities;
using Moneta.Domain.Interfaces.Repository;
using Moneta.Infra.Data.Context;
using System;
using System.Linq;
using System.Collections.Generic;

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

        public override IEnumerable<GrupoLancamento> GetAll()
        {
            throw new Exception("Esta função está obsoleta, use a GetAll(Guid appUserId)");
        }

        public IEnumerable<GrupoLancamento> GetAll(Guid appUserId)
        {
            return DbSet.Where(o => o.Lancamentos.Any(d => d.AppUserId == appUserId)).ToList();
        }
    }
}
