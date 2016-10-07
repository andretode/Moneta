using System.Collections.Generic;
using Moneta.Domain.Entities;
using Moneta.Domain.ValueObjects;

namespace Moneta.Domain.Interfaces.Services
{
    public interface ICategoriaService : IServiceBase<Categoria>
    {
        ValidationResult Adicionar(Categoria conta);
        //int ObterTotalRegistros(string pesquisa);
        //IEnumerable<Conta> ObterContasGrid(int page, string pesquisa);
    }
}
