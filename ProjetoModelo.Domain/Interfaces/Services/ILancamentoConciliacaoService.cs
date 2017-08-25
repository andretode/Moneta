using Moneta.Domain.Entities;
using System;
using System.Collections.Generic;

namespace Moneta.Domain.Interfaces.Services
{
    public interface ILancamentoConciliacaoService
    {
        IEnumerable<LancamentoAgrupado> GetLancamentosSugeridosParaConciliacao(Guid appUserId, ExtratoBancario extrato);
    }
}
