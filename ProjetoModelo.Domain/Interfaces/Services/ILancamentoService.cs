using System.Collections.Generic;
using Moneta.Domain.Entities;
using Moneta.Domain.ValueObjects;
using System;

namespace Moneta.Domain.Interfaces.Services
{
    public interface ILancamentoService : IServiceBase<Lancamento>
    {
        ValidationResult Adicionar(Lancamento lancamento);
        Lancamento GetByIdReadOnly(Guid id);
    }
}
