﻿using System.Data.Entity;
using Moneta.Domain.Entities;
using Moneta.Domain.Interfaces.Repository;
using Moneta.Infra.Data.Context;
using System.Linq;
using System;

namespace Moneta.Infra.Data.Repositories
{
    public class LancamentoParceladoRepository : RepositoryBase<LancamentoParcelado, MonetaContext>, ILancamentoParceladoRepository
    {
        public override void Remove(LancamentoParcelado lancamento)
        {
            var entry = Context.Entry(lancamento);
            DbSet.Attach(lancamento);
            entry.State = EntityState.Deleted;
        }

        public LancamentoParcelado GetByIdReadOnly(Guid id)
        {
            base.Context.SetProxyCreationEnabledToFalse();
            return DbSet.AsNoTracking().Where(x => x.LancamentoParceladoId == id).First();
        }
    }
}
