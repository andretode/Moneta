﻿using System;
using System.Collections.Generic;
using Moneta.Application.Validation;
using Moneta.Application.ViewModels;
using Moneta.Domain.ValueObjects;

namespace Moneta.Application.Interfaces
{
    public interface ILancamentoAppService : IDisposable
    {
        ValidationAppResult Add(LancamentoViewModel LancamentoViewModel);
        //LancamentoViewModel GetById(Guid id);
        //LancamentoViewModel GetByIdReadOnly(Guid id);
        void Update(LancamentoViewModel LancamentoViewModel);
        void Remove(LancamentoViewModel LancamentoViewModel);
        List<Tuple<DateTime, decimal, decimal, decimal>> GetSaldoDoMesPorDia(LancamentosDoMesViewModel lancamentosDoMes, bool resumido);
        LancamentosDoMesViewModel GetLancamentosDoMes(LancamentosDoMesViewModel lancamentosDoMes);
        void AjustarLancamentoParaExibir(LancamentoViewModel lancamentoViewModel);
        void UpdateEmSerie(LancamentoViewModel lancamento);
    }
}
