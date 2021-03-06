﻿using System.Collections.Generic;
using Moneta.Domain.Entities;
using System;

namespace Moneta.Domain.Interfaces.Repository
{
    public interface ILancamentoRepository : IRepositoryBase<Lancamento>
    {
        Lancamento GetByIdReadOnly(Guid id);
        void ForceRemove(Guid id);

        /// <summary>
        /// Pega todos os lançamentos do BD
        /// </summary>
        /// <param name="somenteOsAtivo">Informe true se quiser que deseja retornado somente os lançamentos ativos do BD. Senão será trago todos, independete se são ou não ativos.</param>
        /// <param name="asNoTracking">Informe se a busca deve ser asNoTracking</param>
        /// <returns>Retorna todos os lançamentos do BD</returns>
        IEnumerable<Lancamento> GetAll(bool somenteOsAtivo, bool asNoTracking);
    }
}
