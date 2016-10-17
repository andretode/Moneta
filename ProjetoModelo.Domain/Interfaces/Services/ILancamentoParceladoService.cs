using System.Collections.Generic;
using Moneta.Domain.Entities;
using Moneta.Domain.ValueObjects;
using System;

namespace Moneta.Domain.Interfaces.Services
{
    public interface ILancamentoParceladoService : IServiceBase<LancamentoParcelado>
    {
        ValidationResult Adicionar(LancamentoParcelado lancamento);
        LancamentoParcelado GetByIdReadOnly(Guid id);
    }
}
