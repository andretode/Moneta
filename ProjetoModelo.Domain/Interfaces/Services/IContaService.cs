using System.Collections.Generic;
using Moneta.Domain.Entities;
using Moneta.Domain.ValueObjects;

namespace Moneta.Domain.Interfaces.Services
{
    public interface IContaService : IServiceBase<Conta>
    {
        ValidationResult Adicionar(Conta conta);
        int ObterTotalRegistros(string pesquisa);
        IEnumerable<Conta> ObterContasGrid(int page, string pesquisa);
    }
}
