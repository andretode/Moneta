using System.Collections.Generic;
using Moneta.Domain.Entities;
using Moneta.Domain.ValueObjects;
using System;

namespace Moneta.Domain.Interfaces.Services
{
    public interface ILancamentoService
    {
        ValidationResult Adicionar(Lancamento lancamento);
        //Lancamento GetByIdReadOnly(Guid id);
        void Update(Lancamento lancamento);
        void Remove(Lancamento lancamento);
        void Dispose();
        List<Tuple<DateTime, decimal, decimal, decimal>> GetSaldoDoMesPorDia(AgregadoLancamentosDoMes lancamentosDoMes, bool resumido);
        AgregadoLancamentosDoMes GetLancamentosDoMes(AgregadoLancamentosDoMes lancamentosDoMes);
    }
}
