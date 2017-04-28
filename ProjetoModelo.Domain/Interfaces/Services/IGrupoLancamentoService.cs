using System.Collections.Generic;
using Moneta.Domain.Entities;
using Moneta.Domain.ValueObjects;
using System;

namespace Moneta.Domain.Interfaces.Services
{
    public interface IGrupoLancamentoService : IServiceBase<GrupoLancamento>
    {
        ValidationResult Adicionar(GrupoLancamento grupoLancamento);
        GrupoLancamento GetByIdReadOnly(Guid id);
        //int ObterTotalRegistros(string pesquisa);
        //IEnumerable<Conta> ObterContasGrid(int page, string pesquisa);
    }
}
