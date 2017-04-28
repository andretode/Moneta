using System;
using System.Collections.Generic;
using Moneta.Application.Validation;
using Moneta.Application.ViewModels;

namespace Moneta.Application.Interfaces
{
    public interface IGrupoLancamentoAppService : IDisposable
    {
        ValidationAppResult Add(GrupoLancamentoViewModel grupoLancamentoViewModel);
        GrupoLancamentoViewModel GetById(Guid id);
        GrupoLancamentoViewModel GetByIdReadOnly(Guid id);
        IEnumerable<GrupoLancamentoViewModel> GetAll();
        IEnumerable<GrupoLancamentoViewModel> GetAllReadOnly();
        void Update(GrupoLancamentoViewModel grupoLancamentoViewModel);
        void Remove(GrupoLancamentoViewModel grupoLancamentoViewModel);
        //int ObterTotalRegistros(string pesquisa);
        //IEnumerable<GrupoLancamentoViewModel> ObterGrupoLancamentosGrid(int page, string pesquisa);
    }
}
