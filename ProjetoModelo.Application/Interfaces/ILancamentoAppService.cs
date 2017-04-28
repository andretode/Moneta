using System;
using System.Collections.Generic;
using Moneta.Application.Validation;
using Moneta.Application.ViewModels;
using Moneta.Domain.ValueObjects;

namespace Moneta.Application.Interfaces
{
    public interface ILancamentoAppService : IDisposable
    {
        LancamentoViewModel GetById(Guid id);
        LancamentoViewModel GetByIdReadOnly(Guid id);
        ValidationAppResult Add(LancamentoViewModel LancamentoViewModel);
        ValidationAppResult AddTransferencia(TransferenciaViewModel transferencia);
        //LancamentoViewModel GetById(Guid id);
        //LancamentoViewModel GetByIdReadOnly(Guid id);
        void Update(LancamentoViewModel LancamentoViewModel);
        void Remove(LancamentoViewModel LancamentoViewModel);
        List<Tuple<DateTime, decimal, decimal, decimal>> GetSaldoDoMesPorDia(LancamentosDoMesViewModel lancamentosDoMes, bool resumido);
        LancamentosDoMesViewModel GetLancamentosDoMes(LancamentosDoMesViewModel lancamentosDoMes);
        IEnumerable<LancamentoAgrupadoViewModel> GetLancamentosSugeridosParaConciliacao(ExtratoBancarioViewModel extrato);
        void AjustarLancamentoParaExibir(LancamentoViewModel lancamentoViewModel);
        void UpdateEmSerie(LancamentoViewModel lancamentoViewModel);
        void RemoveEmSerie(LancamentoViewModel lancamentoViewModel);

        void RemoveTransferencia(LancamentoViewModel lancamento);
        void TrocarPago(IEnumerable<LancamentoViewModel> lancamentos);
    }
}
