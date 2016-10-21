using System;
using System.Collections.Generic;
using Moneta.Application.Validation;
using Moneta.Application.ViewModels;
using Moneta.Domain.ValueObjects;

namespace Moneta.Application.Interfaces
{
    public interface ILancamentoAppService : IDisposable
    {
        ValidationAppResult Add(LancamentoViewModel LancamentoViewModel);
        LancamentoViewModel GetById(Guid id);
        LancamentoViewModel GetByIdReadOnly(Guid id);
        IEnumerable<LancamentoViewModel> GetAll();
        IEnumerable<LancamentoViewModel> GetAllReadOnly();
        void Update(LancamentoViewModel LancamentoViewModel);
        void Remove(LancamentoViewModel LancamentoViewModel);
        List<Tuple<DateTime, decimal>> GetSaldoDoMesPorDia(LancamentosDoMesViewModel lancamentosDoMes);
        LancamentosDoMesViewModel GetLancamentosDoMes(LancamentosDoMesViewModel lancamentosDoMes);
        void AjustarLancamentoParaExibir(LancamentoViewModel lancamentoViewModel);
    }
}
