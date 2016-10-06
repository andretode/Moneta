using System;
using System.Collections.Generic;
using Moneta.Domain.Entities;

namespace Moneta.Domain.Interfaces.Repository.ReadOnly
{
    public interface IContaReadOnlyRepository
    {
        Conta GetById(Guid id);
        IEnumerable<Conta> GetAll();
        int ObterTotalRegistros(string pesquisa);
        IEnumerable<Conta> ObterContasGrid(int page, string pesquisa);
    }
}
