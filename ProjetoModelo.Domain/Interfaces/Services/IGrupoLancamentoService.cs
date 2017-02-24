using System.Collections.Generic;
using Moneta.Domain.Entities;
using Moneta.Domain.ValueObjects;

namespace Moneta.Domain.Interfaces.Services
{
    public interface IGrupoLancamentoService : IServiceBase<GrupoLancamento>
    {
        ValidationResult Adicionar(GrupoLancamento grupoLancamento);
        //int ObterTotalRegistros(string pesquisa);
        //IEnumerable<Conta> ObterContasGrid(int page, string pesquisa);
    }
}
