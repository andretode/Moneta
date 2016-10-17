using Moneta.Domain.Entities;
using System;

namespace Moneta.Domain.Interfaces.Repository
{
    public interface ILancamentoParceladoRepository : IRepositoryBase<LancamentoParcelado>
    {
        LancamentoParcelado GetByIdReadOnly(Guid id);
    }
}
