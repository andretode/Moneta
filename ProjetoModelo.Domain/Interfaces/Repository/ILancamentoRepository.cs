using System.Collections.Generic;
using Moneta.Domain.Entities;
using System;

namespace Moneta.Domain.Interfaces.Repository
{
    public interface ILancamentoRepository : IRepositoryBase<Lancamento>
    {
        Lancamento GetByIdReadOnly(Guid id);
        
        /// <summary>
        /// Pega todos os lançamentos do BD
        /// </summary>
        /// <param name="somenteOsAtivo">Informe true se quiser que deseja retornado somente os lançamentos ativos do BD. Senão será trago todos, independete se são ou não ativos.</param>
        /// <returns>Retorna todos os lançamentos do BD</returns>
        IEnumerable<Lancamento> GetAll(bool somenteOsAtivo);

        /// <summary>
        /// Realiza a atualização de todos os lançamentos da série parcelada com base nas alterações do lancamento 
        /// passado como parâmetro
        /// </summary>
        /// <param name="lancamento">lancamento base que será utilizado para atualizar toda a série</param>
        void UpdateEmSerie(Lancamento lancamento);
    }
}
