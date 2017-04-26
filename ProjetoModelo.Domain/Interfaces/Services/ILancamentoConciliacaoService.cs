using Moneta.Domain.Entities;
using System.Collections.Generic;

namespace Moneta.Domain.Interfaces.Services
{
    public interface ILancamentoConciliacaoService
    {
        IEnumerable<LancamentoAgrupado> GetLancamentosSugeridosParaConciliacao(ExtratoBancario extrato);
    }
}
