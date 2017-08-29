using Moneta.Domain.Entities;
using System;
using System.Collections.Generic;

namespace Moneta.Domain.Interfaces.Repository
{
    public interface ILancamentoParceladoRepository : IRepositoryBase<LancamentoParcelado>
    {
        LancamentoParcelado GetByIdReadOnly(Guid id);
        IEnumerable<LancamentoParcelado> GetAll(Guid appUserId);
    }
}
