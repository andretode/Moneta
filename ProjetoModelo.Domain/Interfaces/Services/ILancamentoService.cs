﻿using System.Collections.Generic;
using Moneta.Domain.Entities;
using Moneta.Domain.ValueObjects;
using System;

namespace Moneta.Domain.Interfaces.Services
{
    public interface ILancamentoService
    {
        Lancamento GetById(Guid id);
        Lancamento GetByIdReadOnly(Guid id);
        ValidationResult Adicionar(Lancamento lancamento);
        //Lancamento GetByIdReadOnly(Guid id);
        void Update(Lancamento lancamento);
        void Remove(Lancamento lancamento);
        void Dispose();
        List<Tuple<DateTime, decimal, decimal, decimal>> GetSaldoDoMesPorDia(AgregadoLancamentosDoMes lancamentosDoMes, bool resumido);
        List<SaldoPorCategoria> GetDespesasPorCategoria();
        AgregadoLancamentosDoMes GetLancamentosDoMes(AgregadoLancamentosDoMes lancamentosDoMes);

        /// <summary>
        /// Realiza a atualização de todos os lançamentos da série parcelada com base nas alterações do lancamento 
        /// passado como parâmetro
        /// </summary>
        /// <param name="lancamento">lancamento base que será utilizado para atualizar toda a série</param>
        void UpdateEmSerie(Lancamento lancamento);

        /// <summary>
        /// Remove os lançamentos da série parcelada com base nas alterações do lancamento passado como parâmetro
        /// </summary>
        /// <param name="lancamento">lancamento base que será utilizado para atualizar a série</param>
        void RemoveEmSerie(Lancamento lancamentoEditado);

        void RemoveTransferencia(Lancamento lancamento);
        void TrocarPago(IEnumerable<Lancamento> lancamentos);
    }
}
