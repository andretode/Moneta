using System.Collections.Generic;
using Moneta.Domain.Entities;
using System;

namespace Moneta.Domain.Interfaces.Repository
{
    public interface IGrupoLancamentoRepository : IRepositoryBase<GrupoLancamento>
    {
        GrupoLancamento GetByIdReadOnly(Guid id);
        IEnumerable<GrupoLancamento> GetAll(Guid appUserId);
    }
}
